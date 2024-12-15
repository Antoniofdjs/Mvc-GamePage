namespace MyApp.Data
{

    // Igdb genre data from api /platforms
    public class Platform
    {
        public int ID { get; private set; }
        public string Name { get; private set; }

        public Platform(int Id, string name)
        {
            ID = Id;
            Name = name;
        }
    }
}