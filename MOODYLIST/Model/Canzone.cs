namespace MOODYLIST.Model
{
    public class Canzone
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }

        public int PlaylistId { get; set; }

        public Playlist Playlist { get; set; }
    }
}
