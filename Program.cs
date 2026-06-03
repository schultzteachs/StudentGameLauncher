using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
//Incorporate Event Driven UI AFTER decoupling UI from logic classes

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
    public IEnumerable<string> GameFolders { get; set; } //list of game directories. Basically my list of games

    public string?  documentsPath { get; set;} //gets us to MyDocuments
    public string? GamesDirectory { get;private set; } //Path to StudentGamesFolder
    public string[]? zipFiles { get; set; } // place for zipfiles before extraction

    public event EventHandler<GameFoundEvent>? GameFound;


    public GameScanner()
    {
        documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //gets us to MyDocuments folder
        GamesDirectory = Path.Combine(documentsPath, "StudentGames"); //gets us to the student game folder

    }
     
    public void Scan()
    {

        InitializeFolder(); //checks if game folder exists and creates one and sets the property to the right path

        DealWithZipFiles();
    }
    public void DealWithZipFiles()
    {
        zipFiles = Directory.GetFiles(GamesDirectory, "*.zip");
        if (zipFiles.Length > 0)
        {
             foreach (string file in zipFiles)
            {

                Console.WriteLine($"{file}");
            }
           
            foreach (string file in zipFiles)
            {
                try { ZipFile.ExtractToDirectory(file, GamesDirectory);

                    File.Delete(file);
                }
                catch (Exception)
                {
                    
                    continue;
                }

            }
        }
    }
    public void InitializeFolder() 
    {

        if (!Directory.Exists(GamesDirectory))
        {
            
            //raise event that folder is missing and we are creating one

            Directory.CreateDirectory(GamesDirectory);

            //raise event to tell user to add files to the new folder in MyDocuments
        }
        else
        {
             //raise event to tell user the folder was found

        }


        this.GameFolders = Directory.EnumerateDirectories(GamesDirectory);
        foreach (string folder in this.GameFolders)
        {

            string gameName = Path.GetFileName(folder);
            //raise event that a new game is found! 
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
//Game should be able to
/*Contain meta-data for year created, author name(s), Title, ExecutablePath, ThumbnailPath, SchoolYear, 
*Constructor that takes/asks for meta-data and sets a default if data is not given
*Game needs to be able to broadcast updates to its metadata to the UI
*/

class Game
{
    //the good stuff for now
    public string Title { get; set; }
    public string ExecutablePath { get; set; }

    //stuff to add later
    List<string> AuthorNames {  get; set; }
    string Tagline {  get; set; }
    
    string ThumbnailPath { get; set; }

    int SchoolYearCreated { get; set; }

    //Broadcast changes for metadata to UI

    public Game(string title, string ExePath )
    {
        ExecutablePath = ExePath;
        Title = title;
    }

    public void Launch()
    {
        //game launches itself
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = @$"{ExecutablePath}",
            UseShellExecute = true
        };
        try
        {
            //wrap this in a try catch block for FileNotFound or Win32Execption
            Process process = Process.Start(startInfo);
            //check for null before waiting for it
            process.WaitForExit();
            Console.Clear();
        }
        catch (FileNotFoundException) { return; }
        catch (Win32Exception) { return; }
        catch (IOException) { return; }
        
    }




    /* string Game1Path = scanner.FindExecutableInFolder(response);

        if (!File.Exists(Game1Path))
        {
            Console.WriteLine($"File Not Found at{Game1Path}");
            return;
        }
        else
        {
            Console.Write($"Launching {response}...");
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = @$"{Game1Path}",
                UseShellExecute = true
            };



            //wrap this in a try catch block for FileNotFound or Win32Execption
            Process process = Process.Start(startInfo);
            //check for null before waiting for it
            process.WaitForExit();
            Console.Clear();
        }

    */


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

        _scannerTool.GameFound += OnGameFound;
    }

    public void RunApp()
    {
        StartScanning();

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

}



class DataBase
{
    //Owns: JSON for config settings
    //writes metadata of games to JSON
    //Checks JOSN file when program starts and returns to the Controller class
}