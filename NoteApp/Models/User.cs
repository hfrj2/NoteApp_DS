// Models/User.cs
using SQLite;
using System;

namespace NoteApp.Models
{
    [Table("Users")]
    public class User
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int Id { get; set; }

        [Column("username"), Unique, NotNull]
        public string Username { get; set; }

        [Column("password"), NotNull]
        public string Password { get; set; }

        [Column("phone")]
        public string Phone { get; set; }

        [Column("address")]
        public string Address { get; set; }

        [Column("role")]
        public string Role { get; set; } = "User";

        [Column("create_time")]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [Column("last_login")]
        public DateTime? LastLogin { get; set; }
    }
}