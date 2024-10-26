using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Color { get; set; }
    [NotMapped]
    public bool IsChecked { get; set; } = true;
}
