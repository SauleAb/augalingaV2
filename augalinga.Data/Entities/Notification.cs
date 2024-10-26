using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace augalinga.Data.Entities
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        public string PageName { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        [NotMapped]
        public string UserName => User != null ? User.FullName : "Unknown User";
    }
}
