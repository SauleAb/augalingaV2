using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace augalinga.Data.Entities
{
    public class Meeting
    {
        [Key]

        public int Id { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public bool IsAllDay { get; set; }
        public string EventName { get; set; }
        public string Notes { get; set; }
        public string Employee {  get; set; }
        public string Background { get; set; }

        [NotMapped]
        public Color BrushColor
        {
            get
            {
                Color.TryParse(Background, out Color color);
                return color; 
            }
        }
        [NotMapped]
        public SolidColorBrush Brush => new SolidColorBrush(BrushColor);
    }
}
