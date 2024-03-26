using System;
using SnusSpel.Models;
using SnusSpel.Data;

namespace SnusSpel
{
    public class Program
    {
        // Skapar en instans av Context och Repository klasserna för att hantera databasen
        private static readonly Context _context = new Context();
        private static readonly Repository _repository = new Repository(_context);

        // En global lista som håller alla karaktärer i spelet
        private static List<Character> characters = new(); // Global lista med alla karaktärer

        // Huvudmetoden som körs när programmet startar
        static async Task Main(string[] args)
        {
            // Skapar databasen och lägger till fördefinierade data
            _context.Initialize();

            // Hämtar karaktärer från databasen och lägger till dem i den globala listan
            List<Character> _characters = await _repository.RetrieveCharacters();
            for (int i = 0; i < _characters.Count; i++)
            {
                characters.Add(_characters[i]);
                Console.WriteLine(string.Format("{0}. {1}", i + 1, _characters[i].Name));
            }

            // Variabel för spelarens valda karaktär
            Character player;

            // Loop för att låta spelaren välja sin karaktär
            while (true)
            {
                Console.Write("Välj din karaktär: ");
                ConsoleKeyInfo key = Console.ReadKey();
                
                Console.Write("\n");

                if (int.TryParse(key.KeyChar.ToString(), out int choice) && choice > 0 && choice <= characters.Count)
                {
                    Character character = characters[choice - 1]; // Se vilken karaktär spelaren valde utifrån vilken siffra
                    player = character;
                    characters.Remove(character); // ta bort den valda karaktären från den globala listan
                    break;
                }
                else
                {
                    Console.WriteLine("Ogiltigt val.");
                }
            }

            // Huvudloop för att hantera olika spelalternativ tills spelet avslutas
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

                    // Kör det valda alternativet i metoden nedanför
                    Status status = await RunChoice(choice, player, characters);

                    //kontrollera om spelet ska avslutas
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

        // Metod för att köra det valda spel alternativet
        static async Task<Status> RunChoice(int choice, Character player, List<Character> characters)
        {
            switch (choice)
            {
                // Alternativ 1: Visa andra spelares snusinnehav
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

                // Alternativ 2: Visa spelarens snusinnehav
                case 2: {
                    Console.WriteLine(string.Format("{0} har följande:", player.Name));

                    for (int i = 0; i < player.Inventory.Count; i++)
                    {
                        Dosa dosa = player.Inventory[i];
                        Console.WriteLine(string.Format("    {0}. {1} - {2}", i + 1, dosa.Name, dosa.Brand));
                        Console.WriteLine(string.Format("        {0}st", dosa.Amount));
                    }
                } break;

                // Alternativ 3: Snylta en snus från en annan spelare
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

                        if (int.TryParse(characterKey.KeyChar.ToString(), out int characterIndex) && characterIndex > 0 && characterIndex <= characters.Count)
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

                                if (int.TryParse(dosaKey.KeyChar.ToString(), out int dosaIndex) && dosaIndex > 0 && dosaIndex <= character.Inventory.Count)
                                {
                                    Dosa dosa = character.Inventory[dosaIndex - 1];

                                    if (dosa.Amount > 0)
                                    {
                                        Status status = await _repository.TakeSnus(character, dosa);
                                        
                                        if (status == Status.Ok)
                                        {
                                            dosa.TakeSnus();
                                            Console.WriteLine(string.Format("Nu har {0} bara {1}st snus kvar i sin {2} dosa", character.Name, dosa.Amount, dosa.Name));
                                        }
                                    }
                                    
                                    break;
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

                // Alternativ 4: Ge bort en snus (ej implementerad)
                case 4: {
                    Console.WriteLine("Choice 4");
                } break;

                // Alternativ 5: Avsluta spelet
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