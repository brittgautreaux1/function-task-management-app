using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using TaskManagementApi.Controllers;
using TaskManagementApi.DTOs;
using TaskManagementApi.Entities;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace TaskManagementApi.Tests.Controllers;

public class AuthControllerTests
{
    // ---- Helpers -------------------------------------------------------

    private static Mock<UserManager<User>> MockUserManager()
    {
        var store = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    private static IConfiguration BuildTestConfiguration()
    {
        var settings = new Dictionary<string, string?>
        {
            { "Jwt:Key", "test-signing-key-that-is-long-enough-for-hmacsha256" }
        };

        return new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
    }

    private static AuthController BuildController(
        Mock<UserManager<User>> userManagerMock,
        ClaimsPrincipal? user = null)
    {
        var controller = new AuthController(userManagerMock.Object, BuildTestConfiguration());

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = user ?? new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        return controller;
    }

    private static ClaimsPrincipal BuildUserPrincipal(string userId)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        }, authenticationType: "TestAuth");

        return new ClaimsPrincipal(identity);
    }

    // ---- Register --------------------------------------------------------

    // Business rule: prevents duplicate accounts on the same email.
    [Fact]
    public async Task Register_ReturnsBadRequest_WhenEmailAlreadyExists()
    {
        var userManagerMock = MockUserManager();
        var dto = new RegisterUserDto
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@example.com",
            Password = "Password123!"
        };

        userManagerMock
            .Setup(m => m.FindByEmailAsync(dto.Email))
            .ReturnsAsync(new User { Email = dto.Email });

        var controller = BuildController(userManagerMock);

        var result = await controller.Register(dto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var message = badRequest.Value!.GetType().GetProperty("message")!.GetValue(badRequest.Value);
        Assert.Equal("Email already exists", message);
        userManagerMock.Verify(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }

    // Verifies the actual contract the frontend depends on: a successful
    // register logs the user in immediately (token + user payload), not
    // just a generic success message.
    [Fact]
    public async Task Register_ReturnsTokenAndUser_WhenRegistrationSucceeds()
    {
        var userManagerMock = MockUserManager();
        var dto = new RegisterUserDto
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@example.com",
            Password = "Password123!"
        };

        userManagerMock.Setup(m => m.FindByEmailAsync(dto.Email)).ReturnsAsync((User?)null);
        userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<User>(), dto.Password))
            .ReturnsAsync(IdentityResult.Success);

        var controller = BuildController(userManagerMock);

        var result = await controller.Register(dto);

        var ok = Assert.IsType<OkObjectResult>(result);

        var token = ok.Value!.GetType().GetProperty("token")!.GetValue(ok.Value) as string;
        Assert.False(string.IsNullOrWhiteSpace(token));

        var userValue = ok.Value!.GetType().GetProperty("user")!.GetValue(ok.Value);
        var email = userValue!.GetType().GetProperty("Email")!.GetValue(userValue);
        Assert.Equal(dto.Email, email);
    }

    // ---- Login ------------------------------------------------------------

    // Security-relevant: wrong password must not leak which part failed —
    // this asserts the specific branch that checks the password itself,
    // not just "user not found".
    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenPasswordIsIncorrect()
    {
        var userManagerMock = MockUserManager();
        var dto = new LoginDto { Email = "jane@example.com", Password = "wrong-password" };
        var user = new User { Id = "user-1", Email = dto.Email };

        userManagerMock.Setup(m => m.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
        userManagerMock.Setup(m => m.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(false);

        var controller = BuildController(userManagerMock);

        var result = await controller.Login(dto);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_ReturnsOkWithToken_WhenCredentialsAreValid()
    {
        var userManagerMock = MockUserManager();
        var dto = new LoginDto { Email = "jane@example.com", Password = "Password123!" };
        var user = new User { Id = "user-1", Email = dto.Email, FirstName = "Jane", LastName = "Doe" };

        userManagerMock.Setup(m => m.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
        userManagerMock.Setup(m => m.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);

        var controller = BuildController(userManagerMock);

        var result = await controller.Login(dto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var token = ok.Value!.GetType().GetProperty("token")!.GetValue(ok.Value) as string;
        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    // ---- GetUserProfile -----------------------------------------------------

    // Edge case: token is still valid but the account was deleted after
    // it was issued. Worth covering since it's easy to miss.
    [Fact]
    public async Task GetUserProfile_ReturnsNotFound_WhenUserNoLongerExists()
    {
        var userManagerMock = MockUserManager();
        var principal = BuildUserPrincipal("user-1");

        userManagerMock.Setup(m => m.FindByIdAsync("user-1")).ReturnsAsync((User?)null);

        var controller = BuildController(userManagerMock, user: principal);

        var result = await controller.GetUserProfile();

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetUserProfile_ReturnsOk_WhenUserExists()
    {
        var userManagerMock = MockUserManager();
        var principal = BuildUserPrincipal("user-1");
        var user = new User { Id = "user-1", Email = "jane@example.com", FirstName = "Jane", LastName = "Doe" };

        userManagerMock.Setup(m => m.FindByIdAsync("user-1")).ReturnsAsync(user);

        var controller = BuildController(userManagerMock, user: principal);

        var result = await controller.GetUserProfile();

        var ok = Assert.IsType<OkObjectResult>(result);
        var email = ok.Value!.GetType().GetProperty("email")!.GetValue(ok.Value);
        Assert.Equal("jane@example.com", email);
    }
}