using MOODYLIST.Model;

namespace MOODYLIST.Services
{
    public class GeneratorePlaylist
    {
        public List<Canzone> GenerateSongs(string mood)
        {
            var songs = new List<Canzone>();

            if (mood.Contains("happy"))
            {
                songs.Add(new Canzone { Title = "Happy", Artist = "Pharrell Williams" });
                songs.Add(new Canzone { Title = "Can't Stop the Feeling", Artist = "Justin Timberlake" });
            }
            else if (mood.Contains("sad"))
            {
                songs.Add(new Canzone { Title = "Someone Like You", Artist = "Adele" });
                songs.Add(new Canzone { Title = "Fix You", Artist = "Coldplay" });
            }
            else
            {
                songs.Add(new Canzone { Title = "Blinding Lights", Artist = "The Weeknd" });
            }

            return songs;
        }
    }
}
