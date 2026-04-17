namespace KatalogGierKomp
{
    public class Game
    {
        public int Id { get; set; }
        public byte[]? Image { get; set; }
        public string Title { get; set; } = "";
        public int Score { get; set; }
        public string Review { get; set; } = "";
        public int Completion { get; set; }

        public string CompletionStatus => Completion switch
        {
            0 => "Plan to play",
            1 => "Playing",
            2 => "Completed",
            3 => "Dropped",
            _ => "Unknown"
        };
    }
}
