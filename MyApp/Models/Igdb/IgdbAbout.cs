using Newtonsoft.Json.Linq;

namespace MyApp.Models.Igdb
{

    // Data from the igdb /games api for the "_About" partial view.
    public class IgdbAbout
    {
        public string StoryLine { get; set; } = "";
        public string Summary { get; set; } = "";
        public IgdbAbout() { }
    }
}