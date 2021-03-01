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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;

namespace Playlistosu
{
    /// <summary>
    /// Interaction logic for ConfirmDialog.xaml
    /// </summary>
    public partial class ConfirmDialog : UserControl
    {
        public ConfirmDialog(string message)
        {
            InitializeComponent();
            MessageTextBlock.Text = message;
        }

        private void ContinueButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(true, ContinueButton);
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(false, CancelButton);
        }
    }
}
