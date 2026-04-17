using System;
using System.Collections.Generic;
using System.Text;

namespace KatalogGierKomp
{
    public class Game
    {
        public int Id { get; set; }
        public string Image { get; set; } = "";     //bo stringi nie są nullable
        public string Title { get; set; } = ""; 
        public int Score { get; set; }
        public string Review { get; set; } = "";
        public int Completion { get; set; }
    }
}
