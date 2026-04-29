namespace MOODYLIST.Model
{
    public class Preferiti
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int PlaylistId { get; set; }

        public Playlist Playlist { get; set; }
    }
}
