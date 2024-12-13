using MyApp.Models;
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
            new Theme(41, "4X (explore, expand, exploit, and exterminate)"),
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
    }
}
