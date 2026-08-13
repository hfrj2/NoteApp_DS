using System;
using System.ComponentModel.DataAnnotations;

namespace NoteApp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string AccountName { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}