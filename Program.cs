using System.Diagnostics;
using System.IO;
//Testing logic using Console App
Console.WriteLine("Press anything to start.");
Console.ReadKey();

GameScanner scanner = new GameScanner();

             
Console.WriteLine($"The game folder Directory: {scanner.GamesDirectory}");

scanner.DisplayFiles();

Console.WriteLine("Launching game in Game1 folder!");
string Game1Path = Path.Combine(scanner.GamesDirectory, "Game.txt");

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
    Console.WriteLine("Successfully waited for you to come back!");
}





//Baby steps
//Console.WriteLine($"Which one would you like to play?");
//Console.WriteLine($"The game folder Directory: {scanner.GamesDirectory}");

Console.WriteLine("Program ended.");
Console.ReadKey();





//docs/tutorials here: https://learn.microsoft.com/en-us/training/modules/dotnet-files/
//DONE - look for a designated folder and create one if not one - might need to be in the constructor
//DONE - create a folder for games to be placed
//DONE - method for displaying the contents of a folder
//DONE - Fix FolderCheck method and remove flow control from try block
//method to grab the exe for a unity game inside its own folder

class GameScanner
{
    IEnumerable<string> GameFiles { get; set; }

    public string?  documentsPath { get; set;}
    public string? GamesDirectory { get;private set; }



    public GameScanner()
    {
        documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        GamesDirectory = Path.Combine(documentsPath, "StudentGames");

        InitializeFolder();



    }


    public void InitializeFolder()
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


        this.GameFiles = Directory.EnumerateDirectories(GamesDirectory);
    }




    public void CreateFolder(string FolderName)
    {
        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Games"));
    }
    
    public void DisplayFiles()
    {
        foreach (var dir in GameFiles)
        {
            Console.WriteLine(dir);
        }
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
