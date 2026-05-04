namespace MOODYLIST.Model
{
    public class Playlist
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Mood { get; set; }

        public string UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<Canzone> Songs { get; set; } = new();
    }
}

