using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace InventoryRecordsSystem
{
    // b. Marker interface for logging
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    // a. Immutable inventory record (implements IInventoryEntity)
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    // c. Generic Inventory Logger
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private List<T> _log = new List<T>();
        private string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item)
        {
            _log.Add(item);
        }

        public List<T> GetAll()
        {
            return _log;
        }

        public void SaveToFile()
        {
            try
            {
                using (var writer = new StreamWriter(_filePath))
                {
                    string json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
                    writer.Write(json);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied while saving file: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                using (var reader = new StreamReader(_filePath))
                {
                    string json = reader.ReadToEnd();
                    var items = JsonSerializer.Deserialize<List<T>>(json);
                    _log = items ?? new List<T>();
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing file contents: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
            }
        }
    }

    // f. InventoryApp integration layer
    public class InventoryApp
    {
        private InventoryLogger<InventoryItem> _logger;

        public InventoryApp(string filePath)
        {
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "Office Chair", 15, DateTime.Now));
            _logger.Add(new InventoryItem(2, "Desk Lamp", 30, DateTime.Now));
            _logger.Add(new InventoryItem(3, "Whiteboard", 8, DateTime.Now));
            _logger.Add(new InventoryItem(4, "Stapler", 50, DateTime.Now));
            _logger.Add(new InventoryItem(5, "Printer Paper (Ream)", 100, DateTime.Now));
        }

        public void SaveData()
        {
            _logger.SaveToFile();
        }

        public void LoadData()
        {
            _logger.LoadFromFile();
        }

        public void PrintAllItems()
        {
            foreach (var item in _logger.GetAll())
            {
                Console.WriteLine($"  ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded:g}");
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            string filePath = "inventory_log.json";

            // First "session": seed and persist data
            var app = new InventoryApp(filePath);
            app.SeedSampleData();
            app.SaveData();
            Console.WriteLine("Data seeded and saved to disk.");

            // Simulate a new session by creating a fresh InventoryApp instance
            Console.WriteLine("\n--- Simulating new session ---");
            var newSessionApp = new InventoryApp(filePath);
            newSessionApp.LoadData();
            Console.WriteLine("Data loaded from disk:");
            newSessionApp.PrintAllItems();
        }
    }
}
