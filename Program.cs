using System.IO;


//Testing logic using Console App

GameScanner scanner = new GameScanner();
scanner.DisplayFiles();




//Game should be able to
/*Contain meta-data for year created, author name(s), Title, ExecutablePath, ThumbnailPath, SchoolYear, 
*Constructor that takes/asks for meta-data and sets a default if data is not given
*Game needs to be able to broadcast updates to its metadata to the UI
 */
class Game
{
    List<string> AuthorNames {  get; set; }
    string Tagline {  get; set; }
    string ExecutablePath { get; set; }

    string ThumbnailPath { get; set; }

    int SchoolYear { get; set; }

    //Broadcast changes for metadata to UI
}


//docs/tutorials here: https://learn.microsoft.com/en-us/training/modules/dotnet-files/
class GameScanner
{
    IEnumerable<string> GameFiles = Directory.EnumerateDirectories("Games");




    //look for a designated folder and create one if not one - might need to be in the constructor

    //create a folder for games to be placed
    public void CreateFolder()
    {
        Directory.CreateDirectory
    }
    //method for displaying the contents of a folder
    public void DisplayFiles()
    {
        foreach (var dir in GameFiles) {
        
            Console.WriteLine(dir);

        }
    }
    //method to grab the exe for a unity game inside its own folder
}