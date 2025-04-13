using MemoryGame.Models;
using MemoryGame.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;

namespace MemoryGame.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        #region Variables

        private readonly UserRepository _userRepository;
        private string _newUsername;
        private string _selectedImagePath;
        private User _selectedUser;
        private ObservableCollection<User> _users;
        private List<string> _availableImages;
        private int _currentImageIndex;
        private string _currentDisplayImage;
        #endregion

        #region Commands

        public ICommand CreateUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand NextImageCommand { get; }
        public ICommand PreviousImageCommand { get; }
        public ICommand SelectCurrentImageCommand { get; }
        #endregion

        #region Properties

        public LoginViewModel()
        {
            _userRepository = new UserRepository();
            LoadUsers();
            LoadAvailableImages();

            CreateUserCommand = new RelayCommand(CreateUser, CanCreateUser);
            DeleteUserCommand = new RelayCommand(DeleteUser, () => SelectedUser != null);
            PlayCommand = new RelayCommand(Play, () => SelectedUser != null);
            NextImageCommand = new RelayCommand(NextImage, CanNavigateNext);
            PreviousImageCommand = new RelayCommand(PreviousImage, CanNavigatePrevious);
            SelectCurrentImageCommand = new RelayCommand(SelectCurrentImage, () => !string.IsNullOrEmpty(CurrentDisplayImage));
        }

        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string NewUsername
        {
            get => _newUsername;
            set
            {
                _newUsername = value;
                OnPropertyChanged(nameof(NewUsername));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string SelectedImagePath
        {
            get => _selectedImagePath;
            set
            {
                _selectedImagePath = value;
                OnPropertyChanged(nameof(SelectedImagePath));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string CurrentDisplayImage
        {
            get => _currentDisplayImage;
            set
            {
                _currentDisplayImage = value;
                OnPropertyChanged(nameof(CurrentDisplayImage));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        #endregion

        #region Methods
        private void LoadUsers()
        {
            var userList = _userRepository.GetAllUsers();
            Users = new ObservableCollection<User>(userList);
        }

        private void LoadAvailableImages()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string avatarsDir = Path.Combine(baseDir, "Avatars");

            if (!Directory.Exists(avatarsDir))
            {
                Directory.CreateDirectory(avatarsDir);
            }

            _availableImages = Directory.GetFiles(avatarsDir, "*.jpg")
                .Concat(Directory.GetFiles(avatarsDir, "*.jpeg"))
                .Concat(Directory.GetFiles(avatarsDir, "*.png"))
                .Concat(Directory.GetFiles(avatarsDir, "*.gif"))
                .ToList();

            if (_availableImages.Count > 0)
            {
                _currentImageIndex = 0;
                CurrentDisplayImage = _availableImages[_currentImageIndex];
            }
            else
            {
                CurrentDisplayImage = null;
            }
        }

        private void NextImage()
        {
            if (_currentImageIndex < _availableImages.Count - 1)
            {
                _currentImageIndex++;
                CurrentDisplayImage = _availableImages[_currentImageIndex];
            }
        }

        private bool CanNavigateNext()
        {
            return _availableImages != null && _currentImageIndex < _availableImages.Count - 1;
        }

        private bool CanNavigatePrevious()
        {
            return _availableImages != null && _currentImageIndex > 0;
        }

        private void PreviousImage()
        {
            if (_currentImageIndex > 0)
            {
                _currentImageIndex--;
                CurrentDisplayImage = _availableImages[_currentImageIndex];
            }
        }

        private void SelectCurrentImage()
        {
            if (!string.IsNullOrEmpty(CurrentDisplayImage))
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                if (CurrentDisplayImage.StartsWith(baseDir))
                {
                    SelectedImagePath = CurrentDisplayImage.Substring(baseDir.Length);
                }
                else
                {
                    SelectedImagePath = CurrentDisplayImage;
                }
            }
        }

        private void CreateUser()
        {
            var newUser = new User
            {
                Username = NewUsername,
                ImagePath = SelectedImagePath,
                GamesPlayed = 0,
                GamesWon = 0
            };

            _userRepository.AddUser(newUser);
            Users.Add(newUser);

            NewUsername = string.Empty;
            SelectedImagePath = string.Empty;
        }

        private bool CanCreateUser()
        {
            return !string.IsNullOrWhiteSpace(NewUsername) &&
                   !string.IsNullOrWhiteSpace(SelectedImagePath) &&
                   !Users.Any(u => u.Username == NewUsername);
        }

        private void DeleteUser()
        {
            if (SelectedUser != null)
            {
                _userRepository.DeleteUser(SelectedUser.Username);
                Users.Remove(SelectedUser);
                SelectedUser = null;
            }
        }

        private void Play()
        {
            try
            {
                // Navigate to the game window
                var gameWindow = new Views.GameWindow(SelectedUser);
                gameWindow.Show();

                // Close the login window
                if (Application.Current.MainWindow is Window loginWindow)
                {
                    Application.Current.MainWindow = gameWindow;
                    loginWindow.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
}