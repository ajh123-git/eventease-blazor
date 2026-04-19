using System.ComponentModel.DataAnnotations;

namespace EventEase.Models;

public class EventItem
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title must be 100 characters or fewer.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Location is required.")]
    [StringLength(120, ErrorMessage = "Location must be 120 characters or fewer.")]
    public string Location { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Today;

    [Range(0, int.MaxValue, ErrorMessage = "Attendees cannot be negative.")]
    public int Attendees { get; set; }
}