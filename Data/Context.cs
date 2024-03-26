using System;
using Microsoft.EntityFrameworkCore;
using SnusSpel.Models;

namespace SnusSpel.Data
{
    // Context klassen hanterar databasanslutning och datahantering
    public class Context : DbContext
    {
        // DbSets för att representera tabeller i databasen
        public DbSet<_Character> Characters { get; set; }
        public DbSet<_Inventory> Inventories { get; set; }
        public DbSet<_Snus> Snuses { get; set; }

        // Metod för att initialisera databasen med fördefinierade data
        public void Initialize()
        {
            // Skapar databasen om den inte redan existerar
            Database.EnsureCreated();

            // Lägger till karaktär data i databasen om det inte redan finns
            if (!Characters.Any())
            {
                SeedCharacterData();
            }

            // Lägger till inventerings data i databasen om det inte redan finns
            if (!Inventories.Any())
            {
                SeedInventoryData();
            }

            // Lägger till snus data i databasen om det inte redan finns
            if (!Snuses.Any())
            {
                SeedSnusData();
            }
        }

        // Konfigurerar databasanlutningen till en SQLite-databas med filnamnet "SnusSpel.db"
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source=SnusSpel.db");

        // Metod för att lägga till fördefinierade karaktärer till databasen
        private void SeedCharacterData()
        {
            Characters.AddRange(
                new _Character { Name = "Mert" },
                new _Character { Name = "Wincent" },
                new _Character { Name = "Pouyan" },
                new _Character { Name = "Kevan" },
                new _Character { Name = "Jonte" }
            );

            SaveChanges(); // Sparar ändringar till databasen
        }

        // Metod för att lägga till fördefinierade inventeringsposter till databasen
        private void SeedInventoryData()
        {
            Inventories.AddRange(
                // Mert
                new _Inventory { CharacterId = 1, SnusId = 1, Amount = 17 },
                new _Inventory { CharacterId = 1, SnusId = 4, Amount = 11 },
                // Wincent
                new _Inventory { CharacterId = 2, SnusId = 2, Amount = 2 },
                new _Inventory { CharacterId = 2, SnusId = 6, Amount = 8 },
                new _Inventory { CharacterId = 2, SnusId = 8, Amount = 20 },
                // Pouyan
                new _Inventory { CharacterId = 3, SnusId = 7, Amount = 12 },
                // Kevan
                new _Inventory { CharacterId = 4, SnusId = 1, Amount = 4 },
                new _Inventory { CharacterId = 4, SnusId = 2, Amount = 20 },
                new _Inventory { CharacterId = 4, SnusId = 5, Amount = 20 },
                new _Inventory { CharacterId = 4, SnusId = 6, Amount = 20 },
                // Jonte
                new _Inventory { CharacterId = 5, SnusId = 8, Amount = 20 }
            );

            SaveChanges(); // Sparar ändringar till databasen
        }

        // Metod för att lägga till fördefinierade snusprodukter till databasen
        private void SeedSnusData()
        {
            Snuses.AddRange(
                new _Snus { Name = "Ice cool", Brand = "Velo", Strength = Strength.Strong },
                new _Snus { Name = "Freeze", Brand = "Velo", Strength = Strength.ExtraStrong },
                new _Snus { Name = "Royal purple", Brand = "Velo", Strength = Strength.Strong },
                new _Snus { Name = "Orange spark", Brand = "Velo", Strength = Strength.ExtraStrong },
                new _Snus { Name = "Jalapeno lime", Brand = "Loop", Strength = Strength.Normal },
                new _Snus { Name = "Mango mint", Brand = "Loop", Strength = Strength.Normal },
                new _Snus { Name = "Spearmint", Brand = "Zyn", Strength = Strength.Strong },
                new _Snus { Name = "Black cherry", Brand = "Zyn", Strength = Strength.NikotinFri }
            );

            SaveChanges(); // Sparar ändringar till databasen
        }
    }
}