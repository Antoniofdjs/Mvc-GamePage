using MyApp.Models;
using System.Collections.Generic;

namespace MyApp.Data
{
    public class LocalData
    {
        public List<Store> Stores = new List<Store>
        {
            new Store(1,"Steam"),
            new Store(4,"Amazon"),
            new Store(5,"Gamestop"),
            new Store(8,"Origin"),
            new Store(25,"Epic Games Store"),

        };
    }
}
