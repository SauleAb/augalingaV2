using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace augalinga.Data.Entities
{
    public class Project
    {
        [Key]

        public int Id { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public Project(string name)
        {
            Name = name;
        }

        public Project() { }
    }
}
