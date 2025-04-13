using MemoryGame.Models;
using System;
using System.IO;
using System.Text.Json;

namespace MemoryGame.Services
{
    public class GameRepository
    {
        private readonly string _savedGamesDirectory;

        public GameRepository()
        {
            _savedGamesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SavedGames");
            if (!Directory.Exists(_savedGamesDirectory))
            {
                Directory.CreateDirectory(_savedGamesDirectory);
            }
        }

        public void SaveGame(GameState gameState)
        {
            try
            {
                string filePath = Path.Combine(_savedGamesDirectory, $"{gameState.Username}.json");
                string json = JsonSerializer.Serialize(gameState, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save game: {ex.Message}", ex);
            }
        }

        public GameState LoadGame(string username)
        {
            try
            {
                string filePath = Path.Combine(_savedGamesDirectory, $"{username}.json");

                if (!File.Exists(filePath))
                    return null;

                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<GameState>(json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load game: {ex.Message}", ex);
            }
        }

        public void DeleteSavedGame(string username)
        {
            try
            {
                string filePath = Path.Combine(_savedGamesDirectory, $"{username}.json");

                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete saved game: {ex.Message}", ex);
            }
        }
    }
}