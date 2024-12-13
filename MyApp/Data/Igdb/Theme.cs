namespace MyApp.Data
{

    //Igdb theme data from api
    public class Theme
    {
        public int ID { get; private set; }
        public string Name { get; private set; }

        public Theme(int Id, string name)
        {
            ID = Id;
            Name = name;
        }
    }
}