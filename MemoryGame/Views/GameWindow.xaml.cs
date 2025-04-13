using MemoryGame.Models;
using MemoryGame.ViewModels;
using System.Windows;

namespace MemoryGame.Views
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        public GameWindow(User user)
        {
            InitializeComponent();

            // Set the user in the ViewModel
            if (DataContext is GameViewModel viewModel)
            {
                viewModel.CurrentUser = user;
                viewModel.WindowInstance = this;
            }
        }
    }
}