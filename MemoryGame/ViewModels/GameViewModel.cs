using MemoryGame.Models;
using MemoryGame.Services;
using MemoryGame.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Collections.Generic;

namespace MemoryGame.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        #region Variables
        private readonly UserRepository _userRepository;
        private readonly GameRepository _gameRepository;
        private User _currentUser;
        private string _currentCategory = "ItalianBrainrot"; // Default category
        private string _timeLeft = "00:00";
        private int _rows = 4;
        private int _columns = 4;
        private ObservableCollection<Card> _cards;
        private string _statusMessage = "New game ready to start.";
        private DispatcherTimer _gameTimer;
        private TimeSpan _remainingTime;
        private Card _firstSelectedCard;
        private Card _secondSelectedCard;
        private bool _isProcessingTurn = false;
        private string _boardSizeMode = "Standard";
        private int _customRows = 4;
        private int _customColumns = 4;
        #endregion

        #region Commands
        public ICommand SetCategoryCommand { get; }
        public ICommand NewGameCommand { get; }
        public ICommand OpenGameCommand { get; }
        public ICommand SaveGameCommand { get; }
        public ICommand StatisticsCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand SetBoardSizeCommand { get; }
        public ICommand FlipCardCommand { get; }
        public ICommand AboutCommand { get; }
        #endregion

        #region Properties
        public Window WindowInstance { get; set; }

        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
            }
        }

        public string CurrentCategory
        {
            get => _currentCategory;
            set
            {
                _currentCategory = value;
                OnPropertyChanged(nameof(CurrentCategory));
            }
        }

        public string TimeLeft
        {
            get => _timeLeft;
            set
            {
                _timeLeft = value;
                OnPropertyChanged(nameof(TimeLeft));
            }
        }

        public int Rows
        {
            get => _rows;
            set
            {
                _rows = value;
                OnPropertyChanged(nameof(Rows));
            }
        }

        public int Columns
        {
            get => _columns;
            set
            {
                _columns = value;
                OnPropertyChanged(nameof(Columns));
            }
        }

        public ObservableCollection<Card> Cards
        {
            get => _cards;
            set
            {
                _cards = value;
                OnPropertyChanged(nameof(Cards));
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }
        #endregion

        #region Constructor
        public GameViewModel()
        {
            _userRepository = new UserRepository();
            _gameRepository = new GameRepository();
            Cards = new ObservableCollection<Card>();

            // Initialize commands
            SetCategoryCommand = new RelayCommand<string>(SetCategory);
            NewGameCommand = new RelayCommand(NewGame);
            OpenGameCommand = new RelayCommand(OpenGame);
            SaveGameCommand = new RelayCommand(SaveGame, CanSaveGame);
            StatisticsCommand = new RelayCommand(ShowStatistics);
            ExitCommand = new RelayCommand(Exit);
            SetBoardSizeCommand = new RelayCommand<string>(SetBoardSize);
            FlipCardCommand = new RelayCommand<Card>(FlipCard, CanFlipCard);
            AboutCommand = new RelayCommand(ShowAbout);

            // Initialize the game timer
            _gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _gameTimer.Tick += GameTimer_Tick;
        }
        #endregion

        #region Methods
        private void SetCategory(string category)
        {
            CurrentCategory = category;
            StatusMessage = $"Category set to {category}";
        }

        private void NewGame()
        {
            if (_boardSizeMode == "Standard")
            {
                Rows = 4;
                Columns = 4;
            }
            else
            {
                Rows = _customRows;
                Columns = _customColumns;
            }

            TimeSpan gameTime = AskForGameTime();
            if (gameTime == TimeSpan.Zero)
                return;

            _remainingTime = gameTime;
            TimeLeft = $"{_remainingTime.Minutes:00}:{_remainingTime.Seconds:00}";

            CreateCardDeck();

            _gameTimer.Start();

            StatusMessage = "Game started! Find all matching pairs.";
        }

        private TimeSpan AskForGameTime()
        {
            var dialog = new GameTimeDialog();
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                return dialog.SelectedTime;
            }

            return TimeSpan.Zero;
        }

        private void CreateCardDeck()
        {
            int pairsCount = (Rows * Columns) / 2;
            Cards.Clear();

            // Get images for the selected category
            List<string> categoryImages = GetCategoryImages(_currentCategory, pairsCount);
            if (categoryImages.Count < pairsCount)
            {
                MessageBox.Show($"Not enough images in the {_currentCategory} category. Please select another category or add more images.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Create pairs of cards
            List<Card> cardDeck = new List<Card>();
            string backImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "CardBack.png");

            for (int i = 0; i < pairsCount; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Card card = new Card
                    {
                        Id = i * 2 + j,
                        PairId = i,
                        FrontImagePath = categoryImages[i],
                        BackImagePath = backImagePath,
                        IsFlipped = false,
                        IsActive = true
                    };
                    cardDeck.Add(card);
                }
            }

            ShuffleCards(cardDeck);

            foreach (Card card in cardDeck)
            {
                Cards.Add(card);
            }
        }

        private List<string> GetCategoryImages(string category, int count)
        {
            string categoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", category);

            if (!Directory.Exists(categoryPath))
            {
                Directory.CreateDirectory(categoryPath);
            }

            List<string> imageFiles = Directory.GetFiles(categoryPath, "*.jpg")
                .Concat(Directory.GetFiles(categoryPath, "*.jpeg"))
                .Concat(Directory.GetFiles(categoryPath, "*.png"))
                .Concat(Directory.GetFiles(categoryPath, "*.gif"))
                .ToList();

            if (imageFiles.Count < count)
                return imageFiles;

            Random random = new Random();
            return imageFiles.OrderBy(x => random.Next()).Take(count).ToList();
        }

        private void ShuffleCards(List<Card> cards)
        {
            Random random = new Random();
            int n = cards.Count;

            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                Card temp = cards[k];
                cards[k] = cards[n];
                cards[n] = temp;
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            _remainingTime = _remainingTime.Subtract(TimeSpan.FromSeconds(1));

            if (_remainingTime.TotalSeconds <= 0)
            {
                _gameTimer.Stop();
                TimeLeft = "00:00";
                GameOver(false);
            }
            else
            {
                TimeLeft = $"{_remainingTime.Minutes:00}:{_remainingTime.Seconds:00}";
            }
        }

        private void GameOver(bool won)
        {
            _gameTimer.Stop();
            CurrentUser.GamesPlayed++;
            if (won)
            {
                CurrentUser.GamesWon++;
                MessageBox.Show("Congratulations! You've found all pairs!", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
                StatusMessage = "Game Over - You Win!";
            }
            else
            {
                MessageBox.Show("Time's up! Better luck next time.", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
                StatusMessage = "Game Over - Time's Up!";
            }

            _userRepository.UpdateUser(CurrentUser);
        }

        private bool CanFlipCard(Card card)
        {
            return card != null && card.IsActive && !card.IsFlipped && !_isProcessingTurn;
        }

        private void FlipCard(Card card)
        {
            if (!CanFlipCard(card))
                return;

            card.IsFlipped = true;

            if (_firstSelectedCard == null)
            {
                _firstSelectedCard = card;
            }
            else
            {
                _secondSelectedCard = card;
                _isProcessingTurn = true;

                CheckForMatch();
            }
        }

        private async void CheckForMatch()
        {
            if (_firstSelectedCard.PairId == _secondSelectedCard.PairId)
            {
                await Task.Delay(500);

                _firstSelectedCard.IsActive = false;
                _secondSelectedCard.IsActive = false;

                StatusMessage = "Match found!";

                if (Cards.All(c => !c.IsActive))
                {
                    GameOver(true);
                }
            }
            else
            {
                await Task.Delay(1000);

                _firstSelectedCard.IsFlipped = false;
                _secondSelectedCard.IsFlipped = false;

                StatusMessage = "Not a match. Try again.";
            }

            _firstSelectedCard = null;
            _secondSelectedCard = null;
            _isProcessingTurn = false;
        }

        private void OpenGame()
        {
            GameState savedGame = _gameRepository.LoadGame(CurrentUser.Username);
            if (savedGame == null)
            {
                MessageBox.Show("No saved game found for this user.", "Open Game", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            CurrentCategory = savedGame.Category;
            Rows = savedGame.Rows;
            Columns = savedGame.Columns;
            _remainingTime = savedGame.RemainingTime;
            TimeLeft = $"{_remainingTime.Minutes:00}:{_remainingTime.Seconds:00}";

            Cards.Clear();
            foreach (Card card in savedGame.Cards)
            {
                Cards.Add(card);
            }

            _gameTimer.Start();

            StatusMessage = "Saved game loaded successfully.";
        }

        private bool CanSaveGame()
        {
            return _gameTimer != null && _gameTimer.IsEnabled;
        }

        private void SaveGame()
        {
            if (!CanSaveGame())
                return;

            _gameTimer.Stop();

            GameState gameState = new GameState
            {
                Username = CurrentUser.Username,
                Category = CurrentCategory,
                Rows = Rows,
                Columns = Columns,
                RemainingTime = _remainingTime,
                Cards = Cards.ToList()
            };

            _gameRepository.SaveGame(gameState);

            StatusMessage = "Game saved successfully.";
        }

        private void ShowStatistics()
        {
            var statisticsWindow = new StatisticsWindow();
            statisticsWindow.ShowDialog();
        }

        private void Exit()
        {
            if (_gameTimer.IsEnabled)
            {
                _gameTimer.Stop();
            }

            var loginWindow = new LoginView();
            loginWindow.Show();
            WindowInstance?.Close();

            Application.Current.MainWindow = loginWindow;
        }

        private void SetBoardSize(string mode)
        {
            _boardSizeMode = mode;

            if (mode == "Custom")
            {
                var sizeDialog = new BoardSizeDialog();
                bool? result = sizeDialog.ShowDialog();

                if (result == true)
                {
                    _customRows = sizeDialog.SelectedRows;
                    _customColumns = sizeDialog.SelectedColumns;
                    StatusMessage = $"Custom board size set to {_customRows}x{_customColumns}";
                }
                else
                {
                    _boardSizeMode = "Standard";
                }
            }
            else
            {
                StatusMessage = "Standard board size (4x4) selected";
            }
        }

        private void ShowAbout()
        {
            MessageBox.Show(
                "Memory Game\n\n" +
                "Created by: Petre Flaviu-Mihai\n" +
                "Email: flaviu.petre@student.unitbv.ro\n" +
                "Group: 10LF332\n" +
                "Specialization: IA",
                "About Memory Game",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
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

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => _execute();

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}