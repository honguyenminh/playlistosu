using System;
using System.IO;
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
//using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Playlistosu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string defaultClientPath = 
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            + "\\osu!";
        private string filetype, savePath, customClientPath;
        private bool useCustomClientPath;

        public MainWindow()
        {
            InitializeComponent();
            ClientPathTextBlock.Text = "Client path: " + defaultClientPath;
            LoadFileTypeComboBox();
            FileTypeComboBox.SelectionChanged += FileType_SelectionChanged;
            FileTypeComboBox.SelectedItem = ".vlc";
        }

        // Add supported file types to the combobox
        private void LoadFileTypeComboBox()
        {
            FileTypeComboBox.Items.Add(".vlc");
            FileTypeComboBox.Items.Add(".m3u");
            FileTypeComboBox.Items.Add(".pls");
        }
        // Event handler when the selection of filetype combobox changes
        private void FileType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filetype = (string)FileTypeComboBox.SelectedItem;
        }

        // Change theme button, but who uses light theme anyway eh
        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            if (theme.Paper == Theme.Dark.MaterialDesignPaper)
            {
                theme.SetBaseTheme(Theme.Light);
            }
            else
            {
                theme.SetBaseTheme(Theme.Dark);
            }

            paletteHelper.SetTheme(theme);
        }

        private void DefaultPathCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            useCustomClientPath = true;
            DefaultPathCheckBox.BorderThickness = new Thickness(2, 2, 2, 2);
            ClientFolderChooseButton.Visibility = Visibility.Visible;
            ClientPathTextBlock.Text = "Client path: " + customClientPath;
        }
        private void DefaultPathCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            useCustomClientPath = false;
            DefaultPathCheckBox.BorderThickness = new Thickness(1, 1, 1, 1);
            if (ClientPathTextBlock != null)
            {
                ClientFolderChooseButton.Visibility = Visibility.Collapsed;
                ClientPathTextBlock.Text = "Client path: " + defaultClientPath;
            }
        }

        private void ClientFolderChooseButton_Click(object sender, RoutedEventArgs e)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.InitialDirectory = "C:\\Users";
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    // This sets property to dynamic resource, which creates overhead
                    // We dont need that, so changed to static res below
                    // ClientFolderChooseButton.SetResourceReference(BackgroundProperty, "PrimaryHueDarkBrush");
                    ClientFolderChooseButton.Background = (Brush)Application.Current.Resources["PrimaryHueDarkBrush"];
                    ClientFolderChooseButton.BorderThickness = new Thickness(0, 0, 0, 0);
                    customClientPath = dialog.FileName;
                    if (Path.GetDirectoryName(dialog.FileName) != "osu!")
                    {
                        if (Directory.Exists(dialog.FileName + "\\osu!"))
                        {
                            customClientPath += "\\osu!";
                        }
                    }

                    ClientPathTextBlock.Text = "Client path: " + customClientPath;
                }
            }
        }

        private void DestinationChooseChip_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.SaveFileDialog())
            {
                dialog.Filter = "Choosen file type|*" + filetype;
                dialog.FileName = "osu! playlist" + filetype;
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    DestinationChooseChip.SetResourceReference(BackgroundProperty, "PrimaryHueDarkBrush");
                    savePath = dialog.FileName;
                    
                    SavePathTextBlock.Text = "Path: " + savePath;
                }
            }
        }


        // Main logic goes here
        private void HyperStartButton_Click(object sender, RoutedEventArgs e)
        {
            string clientPath;
            if (useCustomClientPath) clientPath = customClientPath;
            else clientPath = defaultClientPath;

            // Check if osu! executable and Songs folder exist
            if (File.Exists(clientPath + "\\osu!.exe") == false)
            {
                // TODO: Add MaterialDesign dialog box here
                var view = new ConfirmDialog("osu! executable not found, are you sure you want to continue?");
                var result = DialogHost.Show(view);
            }
        }
    }
}
