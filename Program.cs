using System;
using SnusSpel.Models;
using SnusSpel.Data;

namespace SnusSpel
{
    public class Program
    {
        private static readonly Context _context = new Context();
        private static readonly Repository _repository = new Repository(_context);

        private static List<Character> characters = new();

        static async Task Main(string[] args)
        {
            _context.Initialize();

            List<Character> _characters = await _repository.RetrieveCharacters();

            for (int i = 0; i < _characters.Count; i++)
            {
                characters.Add(_characters[i]);
                Console.WriteLine(string.Format("{0}. {1}", i + 1, _characters[i].Name));
            }

            Character player;

            while (true)
            {
                Console.Write("Välj din karaktär: ");
                ConsoleKeyInfo key = Console.ReadKey();
                
                Console.Write("\n");

                if (int.TryParse(key.KeyChar.ToString(), out int choice) || choice < 1 || choice > characters.Count)
                {
                    Character character = characters[choice - 1];
                    player = character;
                    characters.Remove(character);
                    break;
                }
                else
                {
                    Console.WriteLine("Ogiltigt val.");
                }
            }

            while (true)
            {
                Console.Write("\n");
                Console.WriteLine("1. Se vilka andra som har snus.");
                Console.WriteLine("2. Kolla vad du har för snus.");
                Console.WriteLine("3. Snylta en snus.");
                Console.WriteLine("4. Ge bort en snus.");
                Console.WriteLine("5. Avsluta spelet.");
                Console.Write("Vad vill du göra? ");

                ConsoleKeyInfo key = Console.ReadKey();
                
                Console.Write("\n");

                if (int.TryParse(key.KeyChar.ToString(), out int choice))
                {
                    Console.Write("\n");

                    Status status = await RunChoice(choice, player, characters);

                    if (status == Status.Stop)
                    {
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Ogiltigt val.");
                }
            }
        }

        static async Task<Status> RunChoice(int choice, Character player, List<Character> characters)
        {
            switch (choice)
            {
                case 1: {
                    for (int i = 0; i < characters.Count; i++)
                    {
                        Character character = characters[i];
                        Console.WriteLine(string.Format("{0}. {1}", i + 1, character.Name));

                        for (int j = 0; j < character.Inventory.Count; j++)
                        {
                            Dosa dosa = character.Inventory[j];
                            Console.WriteLine(string.Format("    {0}. {1} - {2}", j + 1, dosa.Name, dosa.Brand));
                        }
                    }
                } break;

                case 2: {
                    Console.WriteLine(string.Format("{0} har följande:", player.Name));

                    for (int i = 0; i < player.Inventory.Count; i++)
                    {
                        Dosa dosa = player.Inventory[i];
                        Console.WriteLine(string.Format("    {0}. {1} - {2}", i + 1, dosa.Name, dosa.Brand));
                        Console.WriteLine(string.Format("        {0}st", dosa.Amount));
                    }
                } break;

                case 3: {
                    for (int i = 0; i < characters.Count; i++)
                    {
                        Console.WriteLine(string.Format("{0}. {1}", i + 1, characters[i].Name));
                    }

                    while (true)
                    {
                        Console.Write("Vem vill du snylta av: ");
                        ConsoleKeyInfo characterKey = Console.ReadKey();

                        Console.Write("\n");

                        if (int.TryParse(characterKey.KeyChar.ToString(), out int characterIndex) || characterIndex < 1 || characterIndex > characters.Count)
                        {
                            Character character = characters[characterIndex - 1];
                            
                            for (int i = 0; i < character.Inventory.Count; i++)
                            {
                                Dosa dosa = character.Inventory[i];
                                Console.WriteLine(string.Format("{0}. {1} - {2}", i + 1, dosa.Name, dosa.Brand));
                                Console.WriteLine(string.Format("    {0}st", dosa.Amount));
                            }

                            while (true)
                            {
                                Console.Write("Vilken dosa vill du snylta från: ");
                                ConsoleKeyInfo dosaKey = Console.ReadKey();
                                
                                Console.Write("\n");

                                if (int.TryParse(dosaKey.KeyChar.ToString(), out int dosaIndex) || dosaIndex < 1 || dosaIndex > characters.Count)
                                {
                                    Dosa dosa = character.Inventory[dosaIndex];

                                    if (dosa.Amount > 0)
                                    {
                                        Status status = await _repository.TakeSnus(character, dosa);
                                        
                                        if (status == Status.Ok)
                                        {
                                            dosa.TakeSnus();
                                            Console.WriteLine(string.Format("Nu har {0} bara {1}st snus kvar i sin {2} dosa", character.Name, dosa.Amount, dosa.Name));
                                        }
                                        break;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Ogiltigt val.");
                                }
                            }
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Ogiltigt val.");
                        }
                    }
                } break;

                case 4: {
                    Console.WriteLine("Choice 4");
                } break;

                case 5: {
                    return Status.Stop;
                }

                default: {
                    Console.WriteLine("Ogiltigt val.");
                } break;
            }

            return Status.Ok;
        }
    }
}