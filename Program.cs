//Testing logic using Console App
Console.WriteLine("Press anything to start.");
Console.ReadKey();

GameScanner scanner = new GameScanner();

Console.WriteLine($"Current Directory: {scanner.currentDirectory}");
Console.WriteLine($"The game folder Directory: {scanner.GamesDirectory}");

scanner.DisplayFiles();






//docs/tutorials here: https://learn.microsoft.com/en-us/training/modules/dotnet-files/
//DONE - look for a designated folder and create one if not one - might need to be in the constructor
//DONE - create a folder for games to be placed
//DONE - method for displaying the contents of a folder
//Fix FolderCheck method and remove flow control from try block
//method to grab the exe for a unity game inside its own folder

class GameScanner
{
    IEnumerable<string> GameFiles { get; set; }

    public string? currentDirectory { get; set;}
    public string? GamesDirectory { get; set; }
    public GameScanner()
    {
        try
        {

             currentDirectory = Directory.GetCurrentDirectory();

             GamesDirectory = Path.Combine(currentDirectory, "Games");


            bool FolderExists = Directory.Exists(GamesDirectory);
            Console.WriteLine($"Folder exists: {FolderExists}");
            this.GameFiles = Directory.EnumerateDirectories("Games");
        }
        catch (System.IO.DirectoryNotFoundException)
        {
            

            bool FolderExists = Directory.Exists(GamesDirectory);
            Console.WriteLine($"Folder exists: {FolderExists}");
            Console.WriteLine("Creating Folder! Add games to the directory above.");
            CreateFolder("Games");
            this.GameFiles = Directory.EnumerateDirectories("Games");
        }



    }

    public bool FolderCheck()//fix this - remove flow from try statement above and use selection statements here.
    {
        
        bool FolderExists = Directory.Exists(currentDirectory);
        Console.WriteLine($"Folder exists: {FolderExists}");

        return FolderExists;
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
