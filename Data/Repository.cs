using System;
using Microsoft.EntityFrameworkCore;
using SnusSpel.Models;

namespace SnusSpel.Data
{
    // Repository klassen hanterar interaktionen med databasen
    public class Repository
    {
        private readonly Context _context; // Instans av Context klassen för att arbeta med databasen

        // Konstruktor för att skapa en instans av Repository
        public Repository(Context context)
        {
            _context = context; // Tilldelar den medföljande Context instansen till det privata _context
        }

        // Metod för att hämta alla karaktärer från databasen med deras snusinnehav
        public async Task<List<Character>> RetrieveCharacters()
        {
            // Använder LINQ för att utföra en fråga mot databasen och hämta karaktärer med deras snusinnehav
            List<Character> characters = await (
                from c in _context.Characters
                select new Character
                {
                    Name = c.Name,
                    Inventory = (
                        from s in _context.Snuses
                        join i in _context.Inventories on s.Id equals i.SnusId
                        where i.CharacterId == c.Id
                        select new Dosa
                        {
                            Name = s.Name,
                            Brand = s.Brand,
                            Strength = s.Strength,
                            Amount = i.Amount
                        }
                    ).ToList()
                }
            ).ToListAsync() ?? throw new Exception("Characters is null"); // Kastar ett undantag om inga karaktärer hittades

            return characters; // Returnerar listan med karaktärer och deras snusinnehav
        }

        // Metod för att låta en karaktär ta en snus från deras inventering
        public async Task<Status> TakeSnus(Character character, Dosa dosa)
        {
            // Hämtar karaktärens snusinnehav från databasen
            _Inventory? inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.CharacterId == character.Id && i.SnusId == dosa.Id);

            // Om inventeringen är null (karaktären har inte snusen) returneras Status.None
            if (inventory == null)
            {
                return Status.None;
            }

            // Minskar mängden av den valda snusdosan i karaktärens inventering
            inventory.Amount--;
            await _context.SaveChangesAsync(); // Sparar ändringar till databasen

            return Status.Ok; // Returnerar Status.Ok för att indikera att snusen togs framgångsrikt
        }
    }
}