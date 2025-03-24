using System;
using System.IO;
using System.Threading.Tasks;
using TemperatuurMetingen.Patterns.Structural.Facade;

namespace TemperatuurMetingen.Services
{
    public class FileDataReader
    {
        private readonly SensorSystemFacade _facade;
        
        public FileDataReader(SensorSystemFacade facade)
        {
            _facade = facade;
        }
        
        public async Task ReadDataFromFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"File not found: {filePath}");
                    return;
                }
                
                string[] lines = await File.ReadAllLinesAsync(filePath);
                
                Console.WriteLine($"Reading {lines.Length} sensor data entries from file...");
                
                foreach (string line in lines)
                {
                    // Process each line as sensor data
                    _facade.ProcessSensorData(line);
                    
                    // Add a delay to simulate real-time data
                    await Task.Delay(500);
                }
                
                Console.WriteLine("File processing complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
            }
        }
    }
    
}
