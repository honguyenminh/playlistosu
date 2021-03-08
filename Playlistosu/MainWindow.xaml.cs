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
        #region Class-wide variables
        private readonly string defaultClientPath = 
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            + "\\osu!";
        private string filetype, savePath, customClientPath;
        private bool useCustomClientPath;
        #endregion

        #region UI related methods
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
        #endregion


        #region Main logic
        private void HyperStartButton_Click(object sender, RoutedEventArgs e)
        {
            string clientPath; // Store client path, will be used to store song library path later
            if (useCustomClientPath)
            {
                // Check if user unticked defaultPath checkbox, but didn't select any custom path
                if (customClientPath == null)
                {
                    MessageBox.Show("please select custom osu! client path, or try using" +
                        " default path if you don't know what this is",
                    "oh no no no no no", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                clientPath = customClientPath;
            }
            else clientPath = defaultClientPath;

            // Check if osu! executable exists, not necessary but act as a mere warning for user
            if (File.Exists(clientPath + @"\osu!.exe") == false)
            {
                // TODO: Add MaterialDesign dialog box here
                var result = MessageBox.Show("osu! executable not found, are you sure you want to continue?",
                    "warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No) return;
            }
            // Check if Songs folder exists, if not then ask for song folder
            if (Directory.Exists(clientPath + @"\Songs") == false)
            {
                var result = MessageBox.Show("seems like the song folder is not present, or is not named \"Songs\".\n" +
                    "do you want to specify the song folder?",
                    "songs folder not found", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                // If user click no, just return
                if (result == MessageBoxResult.No) return;

                // Show folder picker
                using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
                {
                    dialog.InitialDirectory = clientPath;
                    dialog.IsFolderPicker = true;
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        clientPath = dialog.FileName;
                    }
                }
            }
            // If Songs folder exists then set clientPath to that folder
            else clientPath = clientPath + @"\Songs";


            List<MapInfo> pendingSongList = new List<MapInfo>();
            // Get all song directory in Songs folder
            string[] subdirectoryEntries = Directory.GetDirectories(clientPath);
            // Loop through all found directory
            // TODO: add logging, and an UI box to show that log
            foreach (string subdirectory in subdirectoryEntries)
            {
                string[] mapFilesEntries = Directory.GetFiles(subdirectory, "*.osu");
                foreach (string mapFile in mapFilesEntries)
                {
                    MapInfo map = new MapInfo();
                    if (ParseMapFile(mapFile, ref map) == Status.Done)
                    {
                        
                    }
                }
            }
        }

        Status ParseMapFile (string mapFile, ref MapInfo map)
        {
            string[] lines = File.ReadAllLines(mapFile);
            // If mapfile version is lower than v14, why bother?
            // TODO: try to support more
            if (lines[0] != "osu file format v14") return Status.NotSupported;

            // You're about to witness the stupidest implementation of a file parser
            ushort currentPos = 1;
            while (currentPos < lines.Length && map.IsMissingMetadata())
            {
                lines[currentPos] = lines[currentPos].Trim();
                // Skip line if it's not a section or line is blank
                if (lines[currentPos] == "")
                {
                    currentPos++; continue;
                }
                if (lines[currentPos][0] != '[')
                {
                    currentPos++; continue;
                }
                // Branch to parse General section
                if (lines[currentPos] == "[General]")
                {
                    currentPos++;
                    // While is still in General block
                    while (lines[currentPos].Trim()[0] != '[')
                    {
                        lines[currentPos] = lines[currentPos].Trim();
                        if (lines[currentPos].StartsWith("AudioFilename: "))
                        {
                            if (lines[currentPos].Length > 15)
                            {
                                map.audioFilename = lines[currentPos].Substring(15);
                                break;
                            }
                            else return Status.NoAudioPath;
                        }
                    }
                }
                // Branch to parse Metadata section
                if (lines[currentPos] == "[Metadata]")
                {
                    currentPos++;
                    // While is still in Metadata block
                    while (lines[currentPos].Trim()[0] != '[' && map.IsMissingMetadata())
                    {
                        lines[currentPos] = lines[currentPos].Trim();
                        if (lines[currentPos].StartsWith("Title:"))
                        {
                            if (lines[currentPos].Length > 6)
                            {
                                map.title = lines[currentPos].Substring(6);
                                currentPos++; continue;
                            }
                            else return Status.MissingMetadata;
                        }
                        if (lines[currentPos].StartsWith("TitleUnicode:"))
                        {
                            if (lines[currentPos].Length > 13)
                            {
                                map.titleUnicode = lines[currentPos].Substring(13);
                            }
                            currentPos++; continue;
                        }
                        if (lines[currentPos].StartsWith("Artist:"))
                        {
                            if (lines[currentPos].Length > 7)
                            {
                                map.artist = lines[currentPos].Substring(7);
                                currentPos++; continue;
                            }
                            else return Status.MissingMetadata;
                        }
                        if (lines[currentPos].StartsWith("ArtistUnicode:"))
                        {
                            if (lines[currentPos].Length > 14)
                            {
                                map.artistUnicode = lines[currentPos].Substring(14);
                            }
                            currentPos++; continue;
                        }
                    }
                }
                currentPos++;
            }
            return Status.Done;
        }
        #endregion
    }
}