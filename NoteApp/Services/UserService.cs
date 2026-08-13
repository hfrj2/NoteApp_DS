using System;
using System.Collections.Generic;
using NoteApp.Helpers;
using NoteApp.Models;
using NoteApp.Repositories;

namespace NoteApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User Login(string accountName, string password)
        {
            var user = _userRepository.GetByAccountName(accountName);
            if (user == null)
                return null;

            if (PasswordHasher.VerifyPassword(password, user.PasswordHash))
                return user;

            return null;
        }

        public bool Register(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(user.AccountName) || string.IsNullOrWhiteSpace(password))
                return false;

            if (_userRepository.AccountExists(user.AccountName))
                return false;

            user.PasswordHash = PasswordHasher.HashPassword(password);
            user.CreatedAt = DateTime.Now;
            _userRepository.Add(user);
            return true;
        }

        public List<User> GetAllUsers()
        {
            return _userRepository.GetAll();
        }

        public User GetUserById(int id)
        {
            return _userRepository.GetById(id);
        }

        public void UpdateUser(User user, string newPassword = null)
        {
            if (user == null) return;

            // 如果提供了新密码，则更新哈希
            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                user.PasswordHash = PasswordHasher.HashPassword(newPassword);
            }

            _userRepository.Update(user);
        }

        public void DeleteUser(int userId)
        {
            _userRepository.Delete(userId);
        }

        public bool AccountExists(string accountName, int? excludeId = null)
        {
            return _userRepository.AccountExists(accountName, excludeId);
        }
    }
}