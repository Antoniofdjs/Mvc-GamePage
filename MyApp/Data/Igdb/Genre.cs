namespace MyApp.Data
{

    // Igdb genre data from api
    public class Genre
    {
        public int ID { get; private set; }
        public string Name { get; private set; }

        public Genre(int Id, string name)
        {
            ID = Id;
            Name = name;
        }
    }
}