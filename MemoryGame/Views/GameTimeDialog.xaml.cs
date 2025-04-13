using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MemoryGame.Views
{
    /// <summary>
    /// Interaction logic for GameTimeDialog.xaml
    /// </summary>
    public partial class GameTimeDialog : Window
    {
        public TimeSpan SelectedTime { get; private set; }

        public GameTimeDialog()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Get selected values from combo boxes
            if (MinutesComboBox.SelectedItem is ComboBoxItem minutesItem &&
                SecondsComboBox.SelectedItem is ComboBoxItem secondsItem)
            {
                int minutes = int.Parse(minutesItem.Content.ToString());
                int seconds = int.Parse(secondsItem.Content.ToString());

                SelectedTime = new TimeSpan(0, minutes, seconds);
                DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}