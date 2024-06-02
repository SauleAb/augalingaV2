using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace augalinga.Data.Entities
{
    public class Document
    {
        [Key]
        public int Id { get; set; }
        public string Project {  get; set; }
        public string Link {  get; set; }
        public string Name {  get; set; }
    }
}
