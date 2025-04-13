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
    /// Interaction logic for BoardSizeDialog.xaml
    /// </summary>
    public partial class BoardSizeDialog : Window
    {
        public int SelectedRows { get; private set; }
        public int SelectedColumns { get; private set; }

        public BoardSizeDialog()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Get selected values from combo boxes
            if (RowsComboBox.SelectedItem is ComboBoxItem rowsItem &&
                ColumnsComboBox.SelectedItem is ComboBoxItem columnsItem)
            {
                SelectedRows = int.Parse(rowsItem.Content.ToString());
                SelectedColumns = int.Parse(columnsItem.Content.ToString());

                // Check if total cards is even
                if ((SelectedRows * SelectedColumns) % 2 != 0)
                {
                    MessageBox.Show("The total number of cards (rows × columns) must be even.",
                                   "Invalid Size",
                                   MessageBoxButton.OK,
                                   MessageBoxImage.Warning);
                    return;
                }

                DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
