using System;
using System.IO;
using System.Threading.Tasks;
using TemperatuurMetingen.Patterns.Structural.Facade;

namespace TemperatuurMetingen.Services
{
    /// <summary>
    /// Provides functionality to read and process sensor data from files.
    /// Works with the SensorSystemFacade to handle the processing of each data line.
    /// </summary>
    public class FileDataReader
    {
        /// <summary>
        /// The facade that handles sensor data processing and system coordination.
        /// </summary>
        private readonly SensorSystemFacade _facade;

        /// <summary>
        /// Initializes a new instance of the FileDataReader class.
        /// </summary>
        /// <param name="facade">The sensor system facade used to process sensor data.</param>
        public FileDataReader(SensorSystemFacade facade)
        {
            _facade = facade;
        }

        /// <summary>
        /// Asynchronously reads sensor data from a specified file and processes each line through the facade.
        /// Each line is expected to be a single sensor data entry in a format the system can parse.
        /// </summary>
        /// <param name="filePath">The full path to the file containing sensor data.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
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