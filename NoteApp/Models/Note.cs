// Models/Note.cs
using SQLite;
using System;

namespace NoteApp.Models
{
    [Table("Notes")]
    public class Note
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int Id { get; set; }

        [Column("user_id"), NotNull]
        public int UserId { get; set; }

        [Column("title"), NotNull]
        public string Title { get; set; }

        [Column("content")]
        public string Content { get; set; }

        [Column("create_time"), NotNull]
        public DateTime CreateTime { get; set; }

        [Column("update_time")]
        public DateTime? UpdateTime { get; set; }

        [Column("is_favorite")]
        public bool IsFavorite { get; set; } = false;

        [Column("color")]
        public string Color { get; set; } = "#FFFFFF";

        [Column("category")]
        public string Category { get; set; } = "默认";
    }
}