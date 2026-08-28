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

        /// <summary>是否为管理员（决定能否进入用户管理）</summary>
        public bool IsAdmin { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}