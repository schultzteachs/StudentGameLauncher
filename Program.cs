using System.IO;
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
    IEnumerable<string> files = Directory.EnumerateDirectories("GameFiles");


    public void DisplayFiles()
    {
        foreach (var file in files) { }
    }
}