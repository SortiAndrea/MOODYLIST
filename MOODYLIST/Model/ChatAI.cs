namespace MOODYLIST.Model
{
    public class ChatAI
    {
        public int Id { get; set; }

        public string UserMessage { get; set; }

        public string AiResponse { get; set; }

        public string MoodDetected { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
