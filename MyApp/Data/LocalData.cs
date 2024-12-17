using System.Collections.Generic;

namespace MyApp.Data
{
    public class LocalData
    {
        // Cheap Shark Stores Id and names, Active stores only here
        public List<Store> Stores = new List<Store>
        {
            new Store(1,"Steam"),
            new Store(2,"Gamers Gate"),
            new Store(3,"Green Man Gaming"),
            new Store(7,"GOG"),
            new Store(8,"Origin"),
            new Store(11,"Humble Store"),
            new Store(13,"Uplay"),
            new Store(15,"Fanatical"),
            new Store(21,"WinGameStore"),
            new Store(23,"GameBillet"),
            new Store(24,"Voidu"),
            new Store(25,"Epic Games Store"),
            new Store(27,"GamesPlanet"),
            new Store(28,"GamesLoad"),
            new Store(29,"2Game"),
            new Store(30,"IndieGala"),
            new Store(31,"Blizzard Shop"),
            new Store(33,"DLGamer"),
            new Store(34,"Noctre"),
            new Store(35,"DreamGame"),
        };

        // Igdb Themes api values
        public List<Theme> Themes = new List<Theme>{
            new Theme(1, "Action"),
            new Theme(17, "Fantasy"),
            new Theme(18, "Science fiction"),
            new Theme(19, "Horror"),
            new Theme(20, "Thriller"),
            new Theme(21, "Survival"),
            new Theme(22, "Historical"),
            new Theme(23, "Stealth"),
            new Theme(27, "Comedy"),
            new Theme(28, "Business"),
            new Theme(31, "Drama"),
            new Theme(32, "Non-fiction"),
            new Theme(33, "Sandbox"),
            new Theme(34, "Educational"),
            new Theme(35, "Kids"),
            new Theme(38, "Open world"),
            new Theme(39, "Warfare"),
            new Theme(40, "Party"),
            new Theme(41, "4X"),
            new Theme(42, "Erotic"),
            new Theme(43, "Mystery"),
            new Theme(44, "Romance"),
        };

        public List<Genre> Genres = new List<Genre>{
            new Genre(2, "Point-and-click"),
            new Genre(4, "Fighting"),
            new Genre(5, "Shooter"),
            new Genre(7, "Music"),
            new Genre(8, "Platform"),
            new Genre(9, "Puzzle"),
            new Genre(10, "Racing"),
            new Genre(11, "Real Time Strategy (RTS)"),
            new Genre(12, "Role-playing (RPG)"),
            new Genre(13, "Simulator"),
            new Genre(14, "Sport"),
            new Genre(15, "Strategy"),
            new Genre(16, "Turn-based strategy (TBS)"),
            new Genre(24, "Tactical"),
            new Genre(25, "Hack and slash/Beat \u0027em up"),
            new Genre(26, "Quiz/Trivia"),
            new Genre(30, "Pinball"),
            new Genre(31, "Adventure"),
            new Genre(32, "Indie"),
            new Genre(33, "Arcade"),
            new Genre(34, "Visual Novel"),
            new Genre(35, "Card \u0026 Board Game"),
            new Genre(36, "MOBA"),
        };

        public List<Platform> Platforms = new List<Platform>{ // Id and abrbreviation
            //Pc and Vr and mobile
            new Platform(3,"Linux"),
            new Platform(6,"Pc(Windows)"),
            new Platform(14,"Mac"),
            new Platform(34,"Android"),
            new Platform(39,"iOS"),
            new Platform(163,"Steam VR"),
            new Platform(387,"Oculus Go"),
            new Platform(384,"Oculus Quest"),
            new Platform(385,"Oculus Rift"),
            new Platform(386,"Meta Quest 2"),
            new Platform(405,"Windows Mobile"),
            new Platform(471,"Meta Quest 3"),



            
            // Sony (family id:1)
            new Platform(7,"PS1"),
            new Platform(8,"PS2"),
            new Platform(9,"PS3"),
            new Platform(38,"PSP"),
            new Platform(46,"PS Vita"),
            new Platform(48,"PS4"),
            new Platform(165,"PSVR"),
            new Platform(167,"PS5"),
            new Platform(390,"PSVR2"),

            //MicroSoft (family id:2)
            new Platform(11,"Xbox"),
            new Platform(12,"Xbox 360"),
            new Platform(49,"Xbox One"),
            new Platform(169,"Xbox X/S"),

            //Nintendo (family id:5)
            new Platform(20,"Nintendo DS"),
            new Platform(21,"Game Cube"),
            new Platform(22,"GB(Color)"),
            new Platform(24,"GB(Advanced)"),
            new Platform(41,"Wii U"),
            new Platform(130,"Switch"),

            new Platform(23,"Dreamcast"),

        };
    }
}
