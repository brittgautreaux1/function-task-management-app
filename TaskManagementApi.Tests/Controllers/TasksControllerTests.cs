using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Controllers;
using TaskManagementApi.Data;
using TaskManagementApi.DTOs;
using Xunit;
using TaskEntity = TaskManagementApi.Entities.Task;

namespace TaskManagementApi.Tests.Controllers;

public class TasksControllerUnitTests
{
    // ---- Helpers -------------------------------------------------------

    private static AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static ClaimsPrincipal BuildUserPrincipal(string userId)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        }, authenticationType: "TestAuth");

        return new ClaimsPrincipal(identity);
    }

    private static TasksController BuildController(AppDbContext context, string userId)
    {
        var controller = new TasksController(context);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = BuildUserPrincipal(userId) }
        };

        return controller;
    }

    // ---- GetTasks -----------------------------------------------------

    // Core data-isolation guarantee: users must never see each other's tasks.
    [Fact]
    public async Task GetTasks_ReturnsOnlyTasksBelongingToCurrentUser()
    {
        using var context = CreateInMemoryContext();

        context.Tasks.AddRange(
            new TaskEntity { Title = "User 1 Task", UserId = "user-1", CreatedAt = DateTime.UtcNow },
            new TaskEntity { Title = "User 2 Task", UserId = "user-2", CreatedAt = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var controller = BuildController(context, userId: "user-1");

        var result = await controller.GetTasks();

        var tasks = Assert.IsAssignableFrom<IEnumerable<TaskDto>>(result.Value);
        var taskList = tasks.ToList();

        Assert.Single(taskList);
        Assert.Equal("User 1 Task", taskList[0].Title);
    }

    // ---- Ownership authorization ---------------------------------------
    // These three guard against a real bug we caught: the original
    // implementation looked tasks up by id alone, letting any
    // authenticated user modify or delete someone else's task.

    [Fact]
    public async Task UpdateTask_ReturnsNotFound_WhenTaskBelongsToAnotherUser()
    {
        using var context = CreateInMemoryContext();
        var task = new TaskEntity { Title = "Belongs to user-1", UserId = "user-1", CreatedAt = DateTime.UtcNow };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var controller = BuildController(context, userId: "user-2");
        var dto = new UpdateTaskDto { Title = "Attempted hijack", Description = "Should not apply" };

        var result = await controller.UpdateTask(task.Id, dto);

        Assert.IsType<NotFoundResult>(result);

        var unchanged = await context.Tasks.FindAsync(task.Id);
        Assert.Equal("Belongs to user-1", unchanged!.Title);
    }

    [Fact]
    public async Task DeleteTask_ReturnsNotFound_WhenTaskBelongsToAnotherUser()
    {
        using var context = CreateInMemoryContext();
        var task = new TaskEntity { Title = "Belongs to user-1", UserId = "user-1", CreatedAt = DateTime.UtcNow };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var controller = BuildController(context, userId: "user-2");

        var result = await controller.DeleteTask(task.Id);

        Assert.IsType<NotFoundResult>(result);
        Assert.Single(context.Tasks);
    }

    [Fact]
    public async Task ToggleComplete_ReturnsNotFound_WhenTaskBelongsToAnotherUser()
    {
        using var context = CreateInMemoryContext();
        var task = new TaskEntity { Title = "Belongs to user-1", UserId = "user-1", IsCompleted = false, CreatedAt = DateTime.UtcNow };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var controller = BuildController(context, userId: "user-2");

        var result = await controller.ToggleComplete(task.Id);

        Assert.IsType<NotFoundResult>(result);

        var unchanged = await context.Tasks.FindAsync(task.Id);
        Assert.False(unchanged!.IsCompleted);
    }

    // ---- ToggleComplete state logic --------------------------------------
    // Not a plain CRUD write — the result depends on the task's current
    // state, so it's worth asserting the flip actually happens.
    [Fact]
    public async Task ToggleComplete_FlipsIsCompleted_FromFalseToTrue()
    {
        using var context = CreateInMemoryContext();
        var task = new TaskEntity { Title = "Toggle me", UserId = "user-1", IsCompleted = false, CreatedAt = DateTime.UtcNow };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var controller = BuildController(context, userId: "user-1");
        var before = DateTime.UtcNow;

        var result = await controller.ToggleComplete(task.Id);

        var ok = Assert.IsType<OkObjectResult>(result);
        var toggled = Assert.IsType<TaskEntity>(ok.Value);

        Assert.True(toggled.IsCompleted);
        Assert.True(toggled.UpdatedAt >= before);
    }
}