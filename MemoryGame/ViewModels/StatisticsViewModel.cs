using MemoryGame.Models;
using MemoryGame.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace MemoryGame.ViewModels
{
    public class StatisticsViewModel : INotifyPropertyChanged
    {
        #region Variables
        private readonly UserRepository _userRepository;
        private ObservableCollection<PlayerStatistics> _playerStats;
        #endregion

        #region Properties
        public ObservableCollection<PlayerStatistics> PlayerStats
        {
            get => _playerStats;
            set
            {
                _playerStats = value;
                OnPropertyChanged(nameof(PlayerStats));
            }
        }

        public StatisticsViewModel()
        {
            _userRepository = new UserRepository();
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            var users = _userRepository.GetAllUsers();

            var stats = users.Select(u => new PlayerStatistics
            {
                Username = u.Username,
                GamesPlayed = u.GamesPlayed,
                GamesWon = u.GamesWon,
                WinRate = u.GamesPlayed > 0 ? (decimal)u.GamesWon / u.GamesPlayed * 100 : 0
            }).ToList();

            PlayerStats = new ObservableCollection<PlayerStatistics>(stats);
        }
        #endregion

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public class PlayerStatistics
    {
        public string Username { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public decimal WinRate { get; set; }
    }
}