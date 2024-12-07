using MyApp.Models;
using System.Collections.Generic;

namespace MyApp.Data
{
    public class LocalData
    {
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
    }
}
