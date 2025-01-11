using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace augalinga.Data.Entities
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }
        public int? ProjectId {  get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string? Address { get; set; }
        public string? Notes {  get; set; }
        public string? Category { get; set; }
    }
}
