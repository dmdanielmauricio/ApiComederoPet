﻿using System.ComponentModel.DataAnnotations;

namespace ApiComederoPet.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; }

        [Required, MaxLength(100)] 
        public string Password { get; set; }
    }
}



