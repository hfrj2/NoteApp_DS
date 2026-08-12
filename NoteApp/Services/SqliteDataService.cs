// Services/SqliteDataService.cs
using NoteApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NoteApp.Services
{
    public class SqliteDataService : IDataService
    {
        private SQLiteAsyncConnection _database;
        private readonly string _dbPath;

        public SqliteDataService()
        {
            _dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "NoteApp.db"
            );
        }

        public async Task<bool> InitializeDatabaseAsync()
        {
            try
            {
                _database = new SQLiteAsyncConnection(_dbPath);

                // 创建表
                await _database.CreateTableAsync<User>();
                await _database.CreateTableAsync<Note>();

                // 检查是否有管理员账户
                var admin = await _database.Table<User>().FirstOrDefaultAsync(u => u.Username == "admin");
                if (admin == null)
                {
                    // 创建默认管理员
                    await _database.InsertAsync(new User
                    {
                        Username = "admin",
                        Password = "admin123",
                        Phone = "13800138000",
                        Address = "System Admin",
                        Role = "Admin",
                        CreateTime = DateTime.Now
                    });
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
                return false;
            }
        }

        #region User Operations

        public async Task<User> GetUserAsync(string username, string password)
        {
            try
            {
                return await _database.Table<User>()
                    .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            try
            {
                return await _database.Table<User>().FirstOrDefaultAsync(u => u.Id == userId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            try
            {
                return await _database.Table<User>().FirstOrDefaultAsync(u => u.Username == username);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> AddUserAsync(User user)
        {
            try
            {
                user.CreateTime = DateTime.Now;
                var result = await _database.InsertAsync(user);
                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddUser error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                var result = await _database.UpdateAsync(user);
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                // 先删除该用户的所有便签
                await _database.ExecuteAsync("DELETE FROM Notes WHERE user_id = ?", userId);

                var result = await _database.DeleteAsync<User>(userId);
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                return await _database.Table<User>().OrderBy(u => u.Username).ToListAsync();
            }
            catch (Exception)
            {
                return new List<User>();
            }
        }

        public async Task<bool> UpdateLastLoginAsync(int userId)
        {
            try
            {
                var user = await GetUserByIdAsync(userId);
                if (user == null) return false;

                user.LastLogin = DateTime.Now;
                return await UpdateUserAsync(user);
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region Note Operations

        public async Task<List<Note>> GetNotesByUserAsync(int userId)
        {
            try
            {
                return await _database.Table<Note>()
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.IsFavorite)
                    .ThenByDescending(n => n.CreateTime)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Note>();
            }
        }

        public async Task<Note> GetNoteByIdAsync(int noteId)
        {
            try
            {
                return await _database.Table<Note>().FirstOrDefaultAsync(n => n.Id == noteId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> AddNoteAsync(Note note)
        {
            try
            {
                note.CreateTime = DateTime.Now;
                var result = await _database.InsertAsync(note);
                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddNote error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateNoteAsync(Note note)
        {
            try
            {
                note.UpdateTime = DateTime.Now;
                var result = await _database.UpdateAsync(note);
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteNoteAsync(int noteId)
        {
            try
            {
                var result = await _database.DeleteAsync<Note>(noteId);
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAllNotesByUserAsync(int userId)
        {
            try
            {
                await _database.ExecuteAsync("DELETE FROM Notes WHERE user_id = ?", userId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Note>> SearchNotesAsync(int userId, string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return await GetNotesByUserAsync(userId);
                }

                return await _database.Table<Note>()
                    .Where(n => n.UserId == userId &&
                                (n.Title.Contains(keyword) || n.Content.Contains(keyword)))
                    .OrderByDescending(n => n.IsFavorite)
                    .ThenByDescending(n => n.CreateTime)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Note>();
            }
        }

        public async Task<bool> ToggleFavoriteAsync(int noteId)
        {
            try
            {
                var note = await GetNoteByIdAsync(noteId);
                if (note == null) return false;

                note.IsFavorite = !note.IsFavorite;
                return await UpdateNoteAsync(note);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Note>> GetFavoriteNotesAsync(int userId)
        {
            try
            {
                return await _database.Table<Note>()
                    .Where(n => n.UserId == userId && n.IsFavorite)
                    .OrderByDescending(n => n.CreateTime)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Note>();
            }
        }

        #endregion
    }
}