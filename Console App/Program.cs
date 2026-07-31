using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text.Json;




//Testing logic using Console App
Console.WriteLine("Press anything to start.");
Console.ReadKey();

DataBase database = new DataBase();
GameScanner gamescanner = new GameScanner();
UI ui = new UI();
Controller controller = new Controller(gamescanner, database, ui);

controller.RunApp();



Console.WriteLine("Program ended.");
Console.ReadKey();





//docs/tutorials here: https://learn.microsoft.com/en-us/training/modules/dotnet-files/
//DONE - look for a designated folder and create one if not one - might need to be in the constructor
//DONE - create a folder for games to be placed
//DONE - method for displaying the contents of a folder
//DONE - Fix FolderCheck method and remove flow control from try block
//DONE - method to grab the exe for a unity game inside its own folder
//DONE - Add robustness to filescanner to extract Zip Files
//DONE - Incorporate Event Driven UI AFTER decoupling UI from logic classes
//Write Game pathes to JSON file after scanning. Check file before scannining in intialization


public class UI
{
    //List<string> options = new List<string>(); 

    public void OnGameFound( string gameName, string folderPath)
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
        GamesDirectory = Path.Combine(documentsPath, "StudentGames");
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
            string gameName = Path.GetFileName(folder);
            OnGameFound(new GameFoundEvent(gameName, folder));
        }
    }

    protected virtual void OnGameFound(GameFoundEvent e)
    {
        GameFound?.Invoke(this, e);
    }

    public string? FindExecutableInFolder(string folderName)
    {
        string targetFolder = Path.Combine(GamesDirectory, folderName);

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
    public GameFoundEvent(string gameName, string folderPath )
    {
        FolderPath = folderPath;
        GameName = gameName;
    }
}


class Game
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
    public List<string> AuthorNames {  get; set; }
    public string Tagline {  get; set; }
    
    public string ThumbnailPath { get; set; }

    public int SchoolYearCreated { get; set; }

    //Broadcast changes for metadata to UI

    public Game(string title, string ExePath )
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
        string? workingDirectory = Path.GetDirectoryName(ExecutablePath);

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

class Controller
{
    private readonly GameScanner _scannerTool;
    private readonly DataBase _databaseTool;
    private readonly UI _userInterface;
    
    public List<Game> MasterGameList { get; private set;  } = new List<Game>();



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
            _userInterface.DisplayError("Game not found. Check your spelling.");
        }
    }


    public void SaveGames(List<Game> games)
    {
        

        _databaseTool.SaveGames(games);

        
    }


}



class DataBase
{
    //Owns: JSON for config settings
    //writes metadata of games to JSON
    //Checks JOSN file when program starts and returns to the Controller class

    public string configFile {  get; set; }
    private readonly string _configFilePath;


    public DataBase()
    {
        configFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "config.json");
        _configFilePath = configFile;
    }

    public void SaveGames(List<Game> gamesToSave)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };

        string jsonString = JsonSerializer.Serialize(gamesToSave, options);
        
        File.WriteAllText(_configFilePath, jsonString);

    }

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


}