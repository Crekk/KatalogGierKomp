namespace KatalogGierKomp
{
    public class Game
    {
        public int Id { get; set; }
        public byte[]? Image { get; set; }
        public string Title { get; set; } = "";
        public int? Score { get; set; }
        public string Review { get; set; } = "";
        public int Completion { get; set; }

        public string ScoreText
        {
            get
            {
                if (Score == null)
                {
                    return "No score";
                }

                return Score + "/10";
            }
        }

        public string CompletionStatus
        {
            get
            {
                if (Completion == 0) return "Plan to play";
                if (Completion == 1) return "Playing";
                if (Completion == 2) return "Completed";
                if (Completion == 3) return "Dropped";

                return "Unknown";
            }
        }
    }
}
