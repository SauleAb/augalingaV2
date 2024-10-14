using augalinga.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Meeting
{
    [Key]
    public int Id { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public bool IsAllDay { get; set; }
    public string EventName { get; set; }
    public string? Notes { get; set; }
    public int? UserId { get; set; }
    public string BackgroundColor {  get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }
}
