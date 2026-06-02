using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;    




//Testing logic using Console App
Console.WriteLine("Press anything to start.");
Console.ReadKey();

DataBase database = new DataBase();
GameScanner gamescanner = new GameScanner();
Controller controller = new Controller(gamescanner, database);





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

    public void DisplayOptions(IEnumerable<string> options)
    {
        foreach (string option in options)
        {
            Console.WriteLine(option);
        }

    }

    public string GetInput()
    {
        string response = Console.ReadLine();
        return response;
    }

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
        //add launch code for games found
        
        string response = Console.ReadLine();

       
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
    public string[]? zipFiles { get; set; } // place for zipfiles before extraction


    public GameScanner()
    {
        documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        GamesDirectory = Path.Combine(documentsPath, "StudentGames");

        InitializeFolder();

        DealWithZipFiles();
    }

    public void DealWithZipFiles()
    {
        zipFiles = Directory.GetFiles(GamesDirectory, "*.zip");
        if (zipFiles.Length > 0)
        {
            Console.WriteLine("The following zipfiles were found:");
            foreach (string file in zipFiles)
            {

                Console.WriteLine($"{file}");
            }
            Console.WriteLine("The .zip files will be extracted and added to your list of games.");

            foreach (string file in zipFiles)
            {
                try { ZipFile.ExtractToDirectory(file, GamesDirectory);

                    File.Delete(file);
                }
                catch (Exception)
                {
                    Console.WriteLine("Skipping over duplicates...");
                    
                    continue;
                }

            }
        }
    }
    public void InitializeFolder() //TO-DO: Refactor UI here
    {

        if (!Directory.Exists(GamesDirectory))
        {
            Console.WriteLine($"Folder missing. Creating directory at: {GamesDirectory}");


            Directory.CreateDirectory(GamesDirectory);

            Console.WriteLine("Folder created! Exit now and add games to the file name StudentGames under MyDocuments. Please add student game folders here.");

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

class Game
{
    List<string> AuthorNames {  get; set; }
    string Tagline {  get; set; }
    public string ExecutablePath { get; set; }

    public string Title { get; set; }
    string ThumbnailPath { get; set; }

    int SchoolYearCreated { get; set; }

    //Broadcast changes for metadata to UI

    public Game(string ExePath, string title)
    {
        ExecutablePath = ExePath;
        Title = title;
    }

    public void Launch()
    {
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
        catch (FileNotFoundException) { }
        catch (Win32Exception) { }
        catch (IOException) { }
        //launches itself
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
    
    //this class owns the Scanner/Game/Database classes.... wait no. Use Dependency Injection instead.
    //takes in UI commands and acts on them
    //tells UI what to print
    //saves new games paths to the database


    public Controller(GameScanner scannerTool, DataBase databaseTool)
    {
        _scannerTool = scannerTool;
        _databaseTool = databaseTool;
    }

}



class DataBase
{
    //Owns: JSON for config settings
    //writes metadata of games to JSON
    //Checks JOSN file when program starts and returns to the Controller class
}