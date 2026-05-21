using System.Diagnostics;
using System.IO;




//Testing logic using Console App
Console.WriteLine("Press anything to start.");
Console.ReadKey();

UI UI = new UI();

//Console.WriteLine($"{UI.GrabUnityEXE()}");
UI.Launch();


Console.WriteLine("Program ended.");
Console.ReadKey();





//docs/tutorials here: https://learn.microsoft.com/en-us/training/modules/dotnet-files/
//DONE - look for a designated folder and create one if not one - might need to be in the constructor
//DONE - create a folder for games to be placed
//DONE - method for displaying the contents of a folder
//DONE - Fix FolderCheck method and remove flow control from try block
//DONE - method to grab the exe for a unity game inside its own folder
//Add robustness to filescanner in case there is a folder inside of a folder


public class UI
{
   public GameScanner scanner = new GameScanner();
    //the UI should "own" its own instance of the GameScanner class.

    public void DisplayFiles()
    {
        Console.WriteLine("------Current Game Options------");
        if (!scanner.GameFolders.Any())
        {
            Console.WriteLine($"No items were found inside {scanner.GamesDirectory}. Ensure you have added FOLDERS to this exact path.");
        }

        foreach (var dir in scanner.GameFolders)
        {
            string file = Path.GetFileName(dir);
            Console.WriteLine(file);
        }
    }
    public string GrabUnityEXE()
    {
        Console.WriteLine("Write the name of the game you want to play");
        DisplayFiles(); //Displays directories of game folders
        string response = Console.ReadLine();

        string? exePath = scanner.FindExecutableInFolder(response);

        if (exePath != null)
        {
            return exePath;
        }
        else 
        { 
            Console.WriteLine("No .exe found.");
            return " ";
        }
    }
    public void Launch()
    {
        Console.WriteLine("Which game would you like to start?");
        DisplayFiles();
        string response = Console.ReadLine();

        string Game1Path = scanner.FindExecutableInFolder(response);

        if (!File.Exists(Game1Path))
        {
            Console.WriteLine($"File Not Found at{Game1Path}");
            return;
        }
        else
        {

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = @$"{Game1Path}",
                UseShellExecute = true
            };



            //wrap this in a try catch block for FileNotFound or Win32Execption
            Process process = Process.Start(startInfo);
            //check for null before waiting for it
            process.WaitForExit();

        }
    }

    public void FilePathBad()
    {
        Console.WriteLine($"Warning. File path bad or file not found");
    }
}

public class GameScanner
{
    public IEnumerable<string> GameFolders { get; set; } //list of game directories

    public string?  documentsPath { get; set;} //gets us to MyDocuments
    public string? GamesDirectory { get;private set; } //Path to StudentGamesFolder



    public GameScanner()
    {
        documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        GamesDirectory = Path.Combine(documentsPath, "StudentGames");

        InitializeFolder();



    }


    public void InitializeFolder() //TO-DO: Refactor UI here
    {

        if (!Directory.Exists(GamesDirectory))
        {
            Console.WriteLine($"Folder missing. Creating directory at: {GamesDirectory}");


            Directory.CreateDirectory(GamesDirectory);

            Console.WriteLine("Folder created! Please add student game folders here.");
        }
        else
        {
            Console.WriteLine($"Target folder found: {GamesDirectory}");
        }


        this.GameFolders = Directory.EnumerateDirectories(GamesDirectory);
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


//Game should be able to
/*Contain meta-data for year created, author name(s), Title, ExecutablePath, ThumbnailPath, SchoolYear, 
*Constructor that takes/asks for meta-data and sets a default if data is not given
*Game needs to be able to broadcast updates to its metadata to the UI
 */

/*class Game
{
    List<string> AuthorNames {  get; set; }
    string Tagline {  get; set; }
    string ExecutablePath { get; set; }

    string ThumbnailPath { get; set; }

    int SchoolYear { get; set; }

    //Broadcast changes for metadata to UI
}
*/
