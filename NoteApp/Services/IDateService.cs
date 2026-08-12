// Services/IDataService.cs
using NoteApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoteApp.Services
{
    public interface IDataService
    {
        Task<bool> InitializeDatabaseAsync();

        // User operations
        Task<User> GetUserAsync(string username, string password);
        Task<User> GetUserByIdAsync(int userId);
        Task<User> GetUserByUsernameAsync(string username);
        Task<bool> AddUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int userId);
        Task<List<User>> GetAllUsersAsync();
        Task<bool> UpdateLastLoginAsync(int userId);

        // Note operations
        Task<List<Note>> GetNotesByUserAsync(int userId);
        Task<Note> GetNoteByIdAsync(int noteId);
        Task<bool> AddNoteAsync(Note note);
        Task<bool> UpdateNoteAsync(Note note);
        Task<bool> DeleteNoteAsync(int noteId);
        Task<List<Note>> SearchNotesAsync(int userId, string keyword);
        Task<bool> ToggleFavoriteAsync(int noteId);
        Task<List<Note>> GetFavoriteNotesAsync(int userId);
        Task<bool> DeleteAllNotesByUserAsync(int userId);
    }
}