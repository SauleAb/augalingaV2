using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Color { get; set; }
    public string Token { get; set; }
    public List<Meeting> Meetings {  get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? TokenExpiry { get; set; }
    [NotMapped]
    public bool IsChecked { get; set; } = true;
}
