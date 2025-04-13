using MemoryGame.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace MemoryGame.Services
{
    public class UserRepository
    {
        private readonly string _usersFilePath;
        private readonly string _savedGamesDirectory;
        private readonly string _statisticsFilePath;

        public UserRepository()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _usersFilePath = Path.Combine(baseDir, "users.json");
            _savedGamesDirectory = Path.Combine(baseDir, "SavedGames");
            _statisticsFilePath = Path.Combine(baseDir, "statistics.json");

            // Ensure SavedGames directory exists
            if (!Directory.Exists(_savedGamesDirectory))
            {
                Directory.CreateDirectory(_savedGamesDirectory);
            }
        }

        public List<User> GetAllUsers()
        {
            if (!File.Exists(_usersFilePath))
                return new List<User>();

            string json = File.ReadAllText(_usersFilePath);
            return string.IsNullOrEmpty(json) ? new List<User>() : JsonSerializer.Deserialize<List<User>>(json);
        }

        public User GetUser(string username)
        {
            var users = GetAllUsers();
            return users.FirstOrDefault(u => u.Username == username);
        }

        public void SaveUsers(List<User> users)
        {
            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_usersFilePath, json);
        }

        public void AddUser(User user)
        {
            var users = GetAllUsers();

            // Check if a user with the same username already exists
            if (users.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("A user with this username already exists.");
            }

            users.Add(user);
            SaveUsers(users);
        }

        public void UpdateUser(User user)
        {
            var users = GetAllUsers();
            var existingUser = users.FirstOrDefault(u => u.Username == user.Username);

            if (existingUser != null)
            { 
                existingUser.ImagePath = user.ImagePath;
                existingUser.GamesPlayed = user.GamesPlayed;
                existingUser.GamesWon = user.GamesWon;

                SaveUsers(users);
            }
            else
            {
                throw new InvalidOperationException("User not found.");
            }
        }

        public void DeleteUser(string username)
        {
            var users = GetAllUsers();
            var userToDelete = users.FirstOrDefault(u => u.Username == username);

            if (userToDelete == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            users.Remove(userToDelete);
            SaveUsers(users);

            try
            {
                string savedGamePath = Path.Combine(_savedGamesDirectory, $"{username}.json");
                if (File.Exists(savedGamePath))
                {
                    File.Delete(savedGamePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting saved game: {ex.Message}");
            }
        }

        public bool UserExists(string username)
        {
            var users = GetAllUsers();
            return users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public List<User> GetTopPlayers(int count = 10)
        {
            var users = GetAllUsers();
            return users
                .OrderByDescending(u => u.GamesWon)
                .ThenByDescending(u => (double)u.GamesWon / (u.GamesPlayed > 0 ? u.GamesPlayed : 1)) 
                .Take(count)
                .ToList();
        }

        public Dictionary<string, double> GetWinRates()
        {
            var users = GetAllUsers();
            return users.ToDictionary(
                u => u.Username,
                u => u.GamesPlayed > 0 ? (double)u.GamesWon / u.GamesPlayed * 100 : 0
            );
        }

        public void IncrementGamesPlayed(string username)
        {
            var user = GetUser(username);
            if (user != null)
            {
                user.GamesPlayed++;
                UpdateUser(user);
            }
        }

        public void IncrementGamesWon(string username)
        {
            var user = GetUser(username);
            if (user != null)
            {
                user.GamesPlayed++;
                user.GamesWon++;
                UpdateUser(user);
            }
        }

        public void ResetStatistics(string username)
        {
            var user = GetUser(username);
            if (user != null)
            {
                user.GamesPlayed = 0;
                user.GamesWon = 0;
                UpdateUser(user);
            }
        }

        public void ResetAllStatistics()
        {
            var users = GetAllUsers();
            foreach (var user in users)
            {
                user.GamesPlayed = 0;
                user.GamesWon = 0;
            }
            SaveUsers(users);
        }
    }
}