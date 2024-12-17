namespace MyApp.Data
{

    // Igdb genre data from api /platforms
    public class GameMode
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Slug { get; private set; }

        public GameMode(int Id, string name, string slug)
        {
            ID = Id;
            Name = name;
            Slug = slug;
        }
    }
}