using System.ComponentModel.DataAnnotations;

namespace TaskManagementApi.DTOs
{
    public class UpdateTaskDto
    {
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string? Title { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        public bool? IsCompleted { get; set; }
        
        public DateTime? DueDate { get; set; }
    }
}