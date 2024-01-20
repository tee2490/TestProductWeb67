﻿using System.ComponentModel.DataAnnotations;

namespace ProductWeb.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public DateTime CreateDateTime { get; set; } = DateTime.Now;
    }
}
