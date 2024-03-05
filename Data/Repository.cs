using System;
using Microsoft.EntityFrameworkCore;
using SnusSpel.Models;

namespace SnusSpel.Data
{
    public class Repository
    {
        private readonly Context _context;

        public Repository(Context context)
        {
            _context = context;
        }

        public async Task<List<Character>> RetrieveCharacters()
        {
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
            ).ToListAsync() ?? throw new Exception("Characters is null");

            return characters;
        }

        public async Task<Status> TakeSnus(Character character, Dosa dosa)
        {
            _Inventory? inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.CharacterId == character.Id && i.SnusId == dosa.Id);

            if (inventory == null)
            {
                return Status.None;
            }

            inventory.Amount--;
            await _context.SaveChangesAsync();

            return Status.Ok;
        }
    }
}