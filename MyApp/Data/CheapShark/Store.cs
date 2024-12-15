namespace MyApp.Data
{

    public class Store
    {

        public int Id { get; set; } // steam 1, Amazon 4, Gamestop 5, Origin 8, Epic Games Store 25
        public string Name { get; set; }

        public Store(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}