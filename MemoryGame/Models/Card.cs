using System.ComponentModel;

namespace MemoryGame.Models
{
    public class Card : INotifyPropertyChanged
    {
        #region Variables
        private bool _isFlipped;
        private bool _isActive = true;
        private string _frontImagePath;
        private string _backImagePath;
        private int _id;
        #endregion

        #region Properties
        public int PairId { get; set; }
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string FrontImagePath
        {
            get => _frontImagePath;
            set
            {
                _frontImagePath = value;
                OnPropertyChanged(nameof(FrontImagePath));
            }
        }

        public string BackImagePath
        {
            get => _backImagePath;
            set
            {
                _backImagePath = value;
                OnPropertyChanged(nameof(BackImagePath));
            }
        }

        public bool IsFlipped
        {
            get => _isFlipped;
            set
            {
                _isFlipped = value;
                OnPropertyChanged(nameof(IsFlipped));
            }
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }
        #endregion

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}