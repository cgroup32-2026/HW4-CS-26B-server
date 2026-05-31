namespace tar1.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SteamUrl { get; set; }
        public string Image { get; set; }
        public string ReleaseDate { get; set; }
        public string ReviewSummary { get; set; }
        public int Price { get; set; }
        public List<string> Tags { get; set; }
        public bool Windows { get; set; }
        public bool Mac { get; set; }
        public bool Linux { get; set; }

        public static List<Game> GamesList = new List<Game>();

        public bool Insert()
        {
            if (GamesList.Any(g => g.Id == this.Id || g.Name == this.Name))
            {
                return false;
            }
            GamesList.Add(this);
            return true;
        }

        public List<Game> Read()
        {
            return GamesList;
        }

        public List<Game> GetByName(string name)
        {
            return GamesList.Where(g => g.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public bool Update(int id, Game updatedGame)
        {
            var index = GamesList.FindIndex(g => g.Id == id);
            if (index == -1) return false;

            updatedGame.Id = id; 
            GamesList[index] = updatedGame;
            return true;
        }
    }
}
