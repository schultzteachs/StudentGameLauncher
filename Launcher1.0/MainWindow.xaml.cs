using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
//using Windows.Gaming.Input;

namespace Launcher1._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DataBase _database = new DataBase();
        private readonly GameScanner _gamescanner = new GameScanner();

        private string _schoolName = "Default School";

        public event PropertyChangedEventHandler PropertyChanged;
        public string SchoolName
        {
            get => _schoolName;
            set
            {
                if (_schoolName != value)
                {
                    _schoolName = value;
                    OnPropertyChanged(); // Tells the UI to update
                }
            }
        }
        private string _appBackgroundHex = "#121212";
        private string _cardBackgroundHex = "#800080";

        // WPF Window Background binds directly to this Brush
        public Brush AppBackground
        {
            get
            {
                Brush brush;
                try { brush = (Brush)new BrushConverter().ConvertFromString(_appBackgroundHex); }
                catch { brush = new SolidColorBrush(Color.FromRgb(18, 18, 18)); } // Fallback

                // Ensure the global application resource is updated so XAML elements find it
                Application.Current.Resources["AppBackgroundBrush"] = brush;
                return brush;
            }
        }

        // WPF Resource Dictionary CardColor binds directly to this Color struct
        public Color CardColor
        {
            get
            {
                Color color;
                try { color = (Color)ColorConverter.ConvertFromString(_cardBackgroundHex); }
                catch { color = Color.FromRgb(128, 0, 128); } // Fallback Purple

                // Ensure the global application resource is updated for your cards
                Application.Current.Resources["CardBackgroundBrush"] = new SolidColorBrush(color);
                return color;
            }
        }

        public ObservableCollection<Game> MasterGameList { get; set; } = new ObservableCollection<Game>();


        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            InitializeLauncher();



            Loaded += (s, e) => MenuBox.Focus();

        }
        private void InitializeLauncher()
        {
            var saveFile = _database.LoadApp();
            this._schoolName = saveFile.SchoolName;

            this._appBackgroundHex = saveFile.AppBackgroundHex ?? "#121212";
            this._cardBackgroundHex = saveFile.CardBackgroundHex ?? "#800080";

            var primaryBg = this.AppBackground;
            var cardBg = this.CardColor;

            var GameList = saveFile.Games;
            foreach (var game in GameList)
            {
                MasterGameList.Add(game);
            }


            _gamescanner.GameFound += OnGameFound;
            _gamescanner.Scan();
            _database.SaveGames(MasterGameList.ToList());


            MenuBox.SelectedIndex = 0;
        }

        private void SetupGamepadPolling()
        {
            //for joystick controller/generic USB device
        }


        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        //MENU INDEXES!!!!

        private void ExecuteOptionAction()
        {
            if (Options.SelectedIndex == 0)
            {
                RescanButton_Click(this, null);
            }
            else if (Options.SelectedIndex == 1)
            {
                EditMetaData();
            }
            else if (Options.SelectedIndex == 2)
            {
                OpenSettingsWindow();
            }
        }

        private void EditMetaData()
        {
            //mention need for keyboard to edit meta-data. Warn that removing games from Folder will require setting data again
            MessageBox.Show("Opening meta-data window. You will need a keyboard. Any games removed from the directory StudentGames under MyDocuments will need meta-data reentered!");
            //open meta-data window

            MetaDataWindow metadataPopup = new MetaDataWindow(this.MasterGameList);
            metadataPopup.Owner = this;
            bool? dialogResult = metadataPopup.ShowDialog();

            if (dialogResult == true)
            {
                this.DataContext = null;
                this.DataContext = this;
                _database.SaveGames(MasterGameList.ToList());

                MessageBox.Show("Settings Saved!");



                //window should have editable fields for each game and the corresponding meta-data
                //When the user presses save, the meta-data for each specific game should be saved with it

                //save the gamelist and app settings to the json file
            }
        }

        private void OpenSettingsWindow()
        {
            MessageBox.Show("Settings opening! Use a keyboard to make changes.");
            SettingsWindow settingsPopup = new SettingsWindow(this.SchoolName);
            settingsPopup.Owner = this;
            bool? dialogResult = settingsPopup.ShowDialog();

            if (dialogResult == true)
            {
                this._schoolName = settingsPopup.UpdatedSchoolName;
                this._appBackgroundHex = settingsPopup.UpdatedAppHex;
                this._cardBackgroundHex = settingsPopup.UpdatedCardHex;
                this.DataContext = null;
                this.DataContext = this;
                _database.SaveApp(new LauncherSettings
                {
                    SchoolName = this.SchoolName,
                    Games = MasterGameList.ToList(),
                    AppBackgroundHex = this._appBackgroundHex,
                    CardBackgroundHex = this._cardBackgroundHex
                });

                MessageBox.Show("Settings Saved!");

            }
        }

        private void RescanButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Rescanning for games!");
            MasterGameList.Clear();
            _gamescanner.Scan();
            _database.SaveGames(MasterGameList.ToList());
        }


        private void OnGameFound(object? sender, GameFoundEvent e)
        {
            if (!MasterGameList.Any(g => g.Title.Equals(e.GameName, StringComparison.OrdinalIgnoreCase)))
            {
                string? exePath = _gamescanner.FindExecutableInFolder(e.GameName);
                if (exePath != null)
                {

                    Game newGame = new Game(e.GameName, exePath);

                    // Crucial: UI additions must run via the Dispatcher if called from backend threads
                    Dispatcher.Invoke(() => MasterGameList.Add(newGame));
                }
            }
        }


        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // CASE 1: Focus is currently up top on Rescan / Settings options
            if (Options.IsKeyboardFocusWithin)
            {
                if (e.Key == Key.Down)
                {
                    // If we have scanned games available, drop focus down
                    if (MenuBox.Items.Count > 0)
                    {
                        Options.SelectedIndex = -1; // Uncheck top items visually
                        MenuBox.SelectedIndex = 0;  // Target the first game card

                        // Force focus shift to the dynamic item card container
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            var container = MenuBox.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
                            container?.Focus();
                        }), System.Windows.Threading.DispatcherPriority.Input);
                    }
                    e.Handled = true; // Input handled, stop event cascading
                }
            }
            // CASE 2: Focus is currently traveling through the horizontal game card row
            else if (MenuBox.IsKeyboardFocusWithin)
            {
                if (e.Key == Key.Up)
                {
                    MenuBox.SelectedIndex = -1; // Uncheck game list visually
                    Options.SelectedIndex = 0;  // Target 'Rescan' button

                    // Force focus shift back to the utility menu row container
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var container = Options.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
                        container?.Focus();
                    }), System.Windows.Threading.DispatcherPriority.Input);

                    e.Handled = true;
                }
            }
        }

        // --- UTILITY MENU SELECTION EXECUTION HOOKS ---
        private void Options_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape)
            {
                ExecuteOptionAction();
                e.Handled = true;
            }
        }

        private void Options_MouseClick(object sender, MouseButtonEventArgs e)
        {
            ExecuteOptionAction();
        }






        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            // WPF extracts the precise object bound to the specific button instance clicked
            if (sender is FrameworkElement element && element.DataContext is Game selectedGame)
            {
                // Trigger the launching process loop written inside your Game class
                selectedGame.Launch();
            }
        }

        private void RemoveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Game selectedGame)
            {

                MasterGameList?.Remove(selectedGame);
            }
        }

        private void EditButton_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Game selectedGame)
            {
                // Trigger the launching process loop written inside your Game class
                //MasterGameList?.Remove(selectedGame);
            }
        }
    }


    /*
public class UI
    {
        //List<string> options = new List<string>(); 

        public void OnGameFound(string gameName, string folderPath)
        {
            Console.WriteLine($"Game found at {folderPath} and named {gameName}!!!");
        }



        public string? GetInput()
        {
            return Console.ReadLine();
        }

        public void DisplayFiles(List<string> gamenames)
        {
            Console.WriteLine("------Current Game Options------");

            if (gamenames.Count == 0)
            {
                Console.WriteLine("No games available. Drop games into StudentGames Folder under MyDocuments and retry!");
                return;
            }

            foreach (string name in gamenames)
            {
                Console.WriteLine($"- {name}");
            }
        }

        public string? GetGameSelection()
        {
            Console.WriteLine("\nWhich game would you like to play? (Type the exact name)");
            return Console.ReadLine();
        }

        public void DisplayError(string message)
        {
            Console.WriteLine($"[ERROR] {message}");
        }

        public void FilePathBad()
        {
            DisplayError("File path bad or not found!");
        }
    }
    */
    public class GameScanner
    {
        public IEnumerable<string> GameFolders { get; set; }
        public string? documentsPath { get; set; }
        public string? GamesDirectory { get; private set; }
        public string[]? zipFiles { get; set; }

        public event EventHandler<GameFoundEvent>? GameFound;

        public GameScanner()
        {
            documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            GamesDirectory = System.IO.Path.Combine(documentsPath, "StudentGames");
        }

        public void Scan()
        {
            // Step 1: Secure the base directory structure first
            EnsureDirectoryExists();

            // Step 2: Unpack any zip packages so they are fully extracted directories
            DealWithZipFiles();

            // Step 3: Now that disk state is settled, discover all game folders
            DiscoverGameFolders();
        }

        private void EnsureDirectoryExists()
        {
            if (!Directory.Exists(GamesDirectory))
            {
                Directory.CreateDirectory(GamesDirectory);
                // Optional: Raise event to tell user to drop games here
            }
        }

        public void DealWithZipFiles()
        {
            zipFiles = Directory.GetFiles(GamesDirectory, "*.zip");
            if (zipFiles.Length > 0)
            {
                foreach (string file in zipFiles)
                {
                    try
                    {
                        ZipFile.ExtractToDirectory(file, GamesDirectory);
                        File.Delete(file);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
        }

        private void DiscoverGameFolders()
        {
            this.GameFolders = Directory.EnumerateDirectories(GamesDirectory);
            foreach (string folder in this.GameFolders)
            {
                string gameName = System.IO.Path.GetFileName(folder);
                OnGameFound(new GameFoundEvent(gameName, folder));
            }
        }

        protected virtual void OnGameFound(GameFoundEvent e)
        {
            GameFound?.Invoke(this, e);
        }

        public string? FindExecutableInFolder(string folderName)
        {
            string targetFolder = System.IO.Path.Combine(GamesDirectory, folderName);

            if (!Directory.Exists(targetFolder))
            {
                return null;
            }
            return Directory.GetFiles(targetFolder, "*.exe").FirstOrDefault();
        }
    }

    public class GameFoundEvent : EventArgs
    {
        public string FolderPath = "";
        public string GameName = "";
        public GameFoundEvent(string gameName, string folderPath)
        {
            FolderPath = folderPath;
            GameName = gameName;

        }
    }


    public class Game
    {

        //Game should be able to
        /*Contain meta-data for year created, author name(s), Title, ExecutablePath, ThumbnailPath, SchoolYear, 
        *Constructor that takes/asks for meta-data and sets a default if data is not given
        *Game needs to be able to broadcast updates to its metadata to the UI
        */


        //the good stuff for now
        public string Title { get; set; }
        public string ExecutablePath { get; set; }

        //optional stuff to add later
        public string? AuthorNames { get; set; }
        public string? Tagline { get; set; }

        public string? ThumbnailPath { get; set; }

        public int? SchoolYearCreated { get; set; }

        //Broadcast changes for metadata to UI

        public Game(string title, string ExePath)
        {
            ExecutablePath = ExePath;
            Title = title;

        }

        public Game()
        {

        }
        public void Launch()
        {
            //game launches itself
            string? workingDirectory = System.IO.Path.GetDirectoryName(ExecutablePath);

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = ExecutablePath,
                WorkingDirectory = workingDirectory, // Forces the OS to run the game from its own folder
                UseShellExecute = true
            };

            try
            {
                Process? process = Process.Start(startInfo);
                if (process != null)
                {
                    process.WaitForExit();
                }
                Console.Clear();
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is Win32Exception || ex is IOException)
            {
                return; // Cleanly catch execution errors without crashing the launcher
            }
        }





    }


    /*
    class Controller
    {
        private readonly GameScanner _scannerTool;
        private readonly DataBase _databaseTool;
        private readonly UI _userInterface;

        public List<Game> MasterGameList { get; private set; } = new List<Game>();



        //this class owns the Scanner/Game/Database classes.... wait no. Use Dependency Injection instead.
        //takes in UI commands and acts on them
        //tells UI what to print
        //saves new games paths to the database


        public Controller(GameScanner scannerTool, DataBase databaseTool, UI userInterface)
        {
            _scannerTool = scannerTool;
            _databaseTool = databaseTool;
            _userInterface = userInterface;
            List<Game> _internalGameList = _databaseTool.LoadGames();
            MasterGameList = _internalGameList;

            _scannerTool.GameFound += OnGameFound;
        }

        public void RunApp()
        {
            StartScanning();
            SaveGames(MasterGameList);
            List<string> gameTitles = new List<string>();
            foreach (Game game in MasterGameList)
            {
                gameTitles.Add(game.Title);
            }

            _userInterface.DisplayFiles(gameTitles);

            string? userChoice = _userInterface.GetGameSelection();

            LaunchSelectedGame(userChoice);

        }
        private void OnGameFound(object? sender, GameFoundEvent e)
        {
            if (!MasterGameList.Any(g => (g.Title).Equals(e.GameName, StringComparison.OrdinalIgnoreCase)))
            {
                string? exePath = _scannerTool.FindExecutableInFolder(e.GameName);

                if (exePath != null)
                {

                    Game newGame = new Game(e.GameName, exePath);
                    MasterGameList.Add(newGame);



                    _userInterface.OnGameFound(newGame.Title, e.FolderPath);
                }
                else
                {

                    _userInterface.DisplayError($"No .exe found in folder: {e.GameName}. Skipping.");
                }
            }
        }


        
        public void StartScanning()
        {
            _scannerTool.Scan();
        }

        private void LaunchSelectedGame(string? selectedName)
        {
            if (string.IsNullOrWhiteSpace(selectedName)) return;

            // LINQ to search our list for a game matching the user's input + a spicy lambda to ignore case
            Game? gameToLaunch = MasterGameList.FirstOrDefault(g => g.Title.Equals(selectedName, StringComparison.OrdinalIgnoreCase));

            //FirstorDefault loops thru and returns the first instance of something that matched the search criteria. default is returned if not found. 

            if (gameToLaunch != null)
            {
                Console.WriteLine($"\nBooting up {gameToLaunch.Title}...");
                gameToLaunch.Launch();
            }
            else
            {
               // _userInterface.DisplayError("Game not found. Check your spelling.");
            }
        }


        public void SaveGames(List<Game> games)
        {


            _database.SaveGames(games);


        }


    }

    */

    class DataBase
    {
        //Owns: JSON for config settings
        //writes metadata of games to JSON
        //Checks JOSN file when program starts and returns to the Controller class

        public string configFile { get; set; }
        private readonly string _configFilePath;
        public LauncherSettings _laucherSettings { get; set; }

        public DataBase()
        {
            configFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "StudentGames\\config.json");
            _configFilePath = configFile;
            _laucherSettings = LoadApp();
        }


        public void SaveGames(List<Game> gamesToSave)
        {
            _laucherSettings.Games = gamesToSave;
            SaveApp(_laucherSettings);
        }

        /*
        public List<Game> LoadGames()
        {

            if (File.Exists(_configFilePath))
            {
                string fileContents = File.ReadAllText(_configFilePath);
                try
                {
                    return JsonSerializer.Deserialize<List<Game>>(fileContents);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Message: {ex.Message}");
                    return new List<Game>();
                }
            }
            else
            {

                return new List<Game>();

            }
        }
        */
        public void SaveApp(LauncherSettings settings)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };

            string jsonString = JsonSerializer.Serialize(settings, options);

            File.WriteAllText(_configFilePath, jsonString);
        }

        public LauncherSettings LoadApp()
        {
            if (File.Exists(_configFilePath))
            {
                string fileContents = File.ReadAllText(_configFilePath);
                try
                {
                    return JsonSerializer.Deserialize<LauncherSettings>(fileContents);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Message: settings file not found or there was an issue. Creating a new one.");
                    return new LauncherSettings();
                }
            }
            else
            {

                return new LauncherSettings();

            }
        }
    }

    public class LauncherSettings
    {
        public string SchoolName { get; set; } = "Default School";
        public List<Game> Games { get; set; } = new List<Game>();

        public string AppBackgroundHex { get; set; } = "#121212";
        public string CardBackgroundHex { get; set; } = "#800080";




    }


}