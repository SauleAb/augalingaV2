using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace augalinga.Data.Entities
{
    public class Photo
    {
        [Key]
        public int Id { get; set; }
        public int ProjectId {  get; set; }
        public string Title { get; set; }
        public string Category {  get; set; }
        public byte[] Bytes { get; set; }
        public string Link {  get; set; }
        public string Name {  get; set; }
    }
}
