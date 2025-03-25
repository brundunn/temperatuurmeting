using System;
using System.IO;
using System.Threading.Tasks;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Patterns.Structural.Facade;
using TemperatuurMetingen.Patterns.Creational.Singleton;
using TemperatuurMetingen.Services;

namespace TemperatuurMetingen
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Temperature Sensor Monitoring System");
            Console.WriteLine("=====================================");
            Console.WriteLine("Demonstration of Multiple Design Patterns");
            Console.WriteLine();

            // Create the facade
            using (var sensorSystem = new SensorSystemFacade())
            {
                // Create a custom observer for statistics
                var statisticsObserver = new StatisticsObserver();
                sensorSystem.AddObserver(statisticsObserver);

                // Get file path from user or use default
                string filePath = "sensor_data.txt";
                if (args.Length > 0)
                {
                    filePath = args[0];
                }
                else
                {
                    Console.WriteLine(
                        "Enter the path to the sensor data file (or press Enter to use default 'sensor_data.txt'):");
                    string input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        filePath = input;
                    }
                }

                Console.WriteLine($"Reading sensor data from: {filePath}");
                Console.WriteLine();

                // Create file reader
                var fileReader = new FileDataReader(sensorSystem);

                // Process data from file
                await fileReader.ReadDataFromFile(filePath);

                // Display statistics
                statisticsObserver.DisplayStatistics();

                // Display all sensor types (Singleton pattern)
                Console.WriteLine("\nDetected Sensor Types (Singleton Pattern):");
                var sensorTypes = sensorSystem.GetAllSensorTypes();
                foreach (var sensor in sensorTypes)
                {
                    Console.WriteLine($"Serial: {sensor.Key}, Type: {sensor.Value}");
                }

                // Display analysis results (Factory Method pattern)
                Console.WriteLine("\nAnalysis Results (Factory Method Pattern):");
                var analysisResults = sensorSystem.GetAllAnalysisResults();
                foreach (var result in analysisResults)
                {
                    Console.WriteLine($"\n{result.Key}:");
                    Console.WriteLine(result.Value);
                }

                // Organize sensors by manufacturer (Composite pattern)
                sensorSystem.OrganizeSensorsByManufacturer();

                // Display sensor hierarchy (Composite pattern)
                Console.WriteLine("\nSensor Hierarchy (Composite Pattern):");
                sensorSystem.DisplaySensorHierarchy();

                // Apply visitor pattern
                Console.WriteLine("\nSensor Health Report (Visitor Pattern):");
                string healthReport = sensorSystem.ApplyHealthVisitor();
                Console.WriteLine(healthReport);

                Console.WriteLine("\nAnomaly Detection Report (Visitor Pattern):");
                string anomalyReport = sensorSystem.ApplyAnomalyDetectionVisitor();
                Console.WriteLine(anomalyReport);

                // Actor model pattern
                Console.WriteLine("\nActor Model Pattern Results:");
                var tempStats = await sensorSystem.AnalyzeSensorTypeWithActorsAsync("temp");
                var humStats = await sensorSystem.AnalyzeSensorTypeWithActorsAsync("humidity");

                Console.WriteLine($"Processed data points: {await sensorSystem.GetProcessedDataCountAsync()}");
                Console.WriteLine("\nTemperature sensor stats from Actor system:");
                foreach (var stat in tempStats)
                {
                    Console.WriteLine($"  {stat.Key}: {stat.Value:F2}");
                }

                Console.WriteLine("\nHumidity sensor stats from Actor system:");
                foreach (var stat in humStats)
                {
                    Console.WriteLine($"  {stat.Key}: {stat.Value:F2}");
                }

                // Display alerts from Actor system
                Console.WriteLine("\nAlerts from Actor system:");
                string alerts = await sensorSystem.GetAlertsAsync();
                if (string.IsNullOrEmpty(alerts))
                {
                    Console.WriteLine("No alerts generated.");
                }
                else
                {
                    Console.WriteLine(alerts);
                }

                // Bridge pattern
                Console.WriteLine("\nBridge Pattern Results:");
                Console.WriteLine("Displaying statistics with different formats...");

                // Use Bridge pattern to display statistics
                sensorSystem.DisplayStatistics(tempStats, "Temperature Statistics", "console");
                sensorSystem.DisplayStatistics(tempStats, "Temperature Statistics (JSON)", "json-console");

                // Broadcast an alert through all displays
                sensorSystem.BroadcastAlert("System monitoring complete", "info");

                Console.WriteLine("\nA log file has been generated using the Bridge pattern.");
                Console.WriteLine("Check 'sensor_log.txt' and 'sensor_log.json' for details.");

                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }

        class StatisticsObserver : ISensorDataObserver
        {
            private int _totalReadings = 0;
            private double _totalTemperature = 0;
            private double _maxTemperature = double.MinValue;
            private double _minTemperature = double.MaxValue;
            private int _tempReadings = 0;

            private double _totalHumidity = 0;
            private double _maxHumidity = double.MinValue;
            private double _minHumidity = double.MaxValue;
            private int _humReadings = 0;

            public void Update(SensorData data)
            {
                _totalReadings++;

                if (data.Type == "temp" && data.Temperature > 0)
                {
                    _totalTemperature += data.Temperature;
                    _maxTemperature = Math.Max(_maxTemperature, data.Temperature);
                    _minTemperature = Math.Min(_minTemperature, data.Temperature);
                    _tempReadings++;
                }

                if (data.Humidity > 0)
                {
                    _totalHumidity += data.Humidity;
                    _maxHumidity = Math.Max(_maxHumidity, data.Humidity);
                    _minHumidity = Math.Min(_minHumidity, data.Humidity);
                    _humReadings++;
                }
            }

            public void DisplayStatistics()
            {
                Console.WriteLine("\nSensor Statistics:");
                Console.WriteLine($"Total readings processed: {_totalReadings}");

                if (_tempReadings > 0)
                {
                    Console.WriteLine("\nTemperature Statistics:");
                    Console.WriteLine($"Average temperature: {_totalTemperature / _tempReadings:F2}°C");
                    Console.WriteLine($"Maximum temperature: {_maxTemperature:F2}°C");
                    Console.WriteLine($"Minimum temperature: {_minTemperature:F2}°C");
                }

                if (_humReadings > 0)
                {
                    Console.WriteLine("\nHumidity Statistics:");
                    Console.WriteLine($"Average humidity: {_totalHumidity / _humReadings:F2}%");
                    Console.WriteLine($"Maximum humidity: {_maxHumidity:F2}%");
                    Console.WriteLine($"Minimum humidity: {_minHumidity:F2}%");
                }
            }
        }
    }
}