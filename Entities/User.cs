﻿using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(30)]
        public string? FullName{ get; set; }

        [Required]
        [StringLength(30)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }
        public bool Locked{ get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "user";
    }
}