﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace augalinga.Data.Entities
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }
        public string Project {  get; set; }
        public string Transaction {  get; set; }
        public decimal Amount {  get; set; }
        public DateOnly Date {  get; set; }
        public string Type {  get; set; }
    }
}
