using System;
using System.Collections.Generic;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Patterns.Creational.Factory;
using TemperatuurMetingen.Patterns.Behavioral.Observer;
using TemperatuurMetingen.Patterns.Creational.Singleton;
using TemperatuurMetingen.Patterns.Behavioral.Strategy;
using TemperatuurMetingen.Patterns.Structural.Composite;
using TemperatuurMetingen.Patterns.Structural.Bridge;
using TemperatuurMetingen.Patterns.Behavioral.Visitor;
using TemperatuurMetingen.Patterns.Concurrency.Actor;
using TemperatuurMetingen.Services;

namespace TemperatuurMetingen.Patterns.Structural.Facade
{
    /// <summary>
    /// Implements the Facade design pattern to provide a simplified interface to the complex sensor system.
    /// Coordinates interactions between multiple subsystems and design patterns including Observer, 
    /// Singleton, Strategy, Composite, Bridge, Visitor, and Actor model.
    /// </summary>
    public class SensorSystemFacade : IDisposable
    {
        /// <summary>
        /// Processes and validates sensor data.
        /// </summary>
        private readonly SensorDataProcessor _processor;
        
        /// <summary>
        /// Implements the Observer pattern for sensor data notifications.
        /// </summary>
        private readonly SensorDataSubject _subject;
        
        /// <summary>
        /// Singleton instance for managing sensor types.
        /// </summary>
        private readonly SensorTypeManager _typeManager;
        
        /// <summary>
        /// Collection of parsers for different sensor data formats.
        /// </summary>
        private readonly List<ISensorDataParser> _parsers;
        
        /// <summary>
        /// Manages analysis strategies for different sensor types.
        /// </summary>
        private readonly AnalyzerManager _analyzerManager;
        
        /// <summary>
        /// Implements the Composite pattern for hierarchical sensor organization.
        /// </summary>
        private readonly SensorCompositeManager _compositeManager;
        
        /// <summary>
        /// Manages the Actor model for concurrent sensor data processing.
        /// </summary>
        private readonly ActorSystemManager _actorSystem;
        
        /// <summary>
        /// Implements the Bridge pattern for flexible data display options.
        /// </summary>
        private readonly DisplayManager _displayManager;

        /// <summary>
        /// Initializes a new instance of the SensorSystemFacade class.
        /// Sets up all subsystems, registers parsers, observers, and analyzers.
        /// </summary>
        public SensorSystemFacade()
        {
            _processor = new SensorDataProcessor();
            _subject = new SensorDataSubject();
            _typeManager = SensorTypeManager.Instance;
            _analyzerManager = new AnalyzerManager();
            _compositeManager = new SensorCompositeManager();

            // Initialize parsers
            _parsers = new List<ISensorDataParser>
            {
                new StandardFormatParser(),
                new ManufacturerFirstFormatParser()
            };

            // Attach observers
            _subject.Attach(new TemperatureMonitor());
            _subject.Attach(new BatteryMonitor());

            // Initialize analyzers using factories
            _analyzerManager.RegisterAnalyzer("temp", new TemperatureAnalyzerFactory());
            _analyzerManager.RegisterAnalyzer("humidity", new HumidityAnalyzerFactory());
            _analyzerManager.RegisterAnalyzer("battery", new BatteryAnalyzerFactory());

            // Initialize Actor system
            _actorSystem = new ActorSystemManager();

            // Initialize Bridge pattern
            _displayManager = new DisplayManager();

            Console.WriteLine("SensorSystemFacade initialized with all patterns");
        }

        /// <summary>
        /// Processes raw sensor data through the entire system pipeline.
        /// Parses data, registers sensor types, analyzes results, updates observers,
        /// and displays the data through the appropriate channels.
        /// </summary>
        /// <param name="rawData">The raw string data from a sensor.</param>
        public void ProcessSensorData(string rawData)
        {
            try
            {
                // Determine the correct parser based on the data format
                ISensorDataParser parser = DetermineParser(rawData);
                if (parser == null)
                {
                    Console.WriteLine($"No suitable parser found for data: {rawData}");
                    return;
                }

                // Process the data
                SensorData data = parser.Parse(rawData);

                // Add data to the composite manager
                _compositeManager.AddSensorData(data);

                // Register the sensor type
                if (!string.IsNullOrEmpty(data.SerialNumber) && !string.IsNullOrEmpty(data.Type))
                {
                    _typeManager.RegisterSensorType(data.SerialNumber, data.Type);
                }

                // Analyze the data
                _analyzerManager.AnalyzeData(data);

                // Send to Actor system
                _actorSystem.ProcessSensorData(data);

                // Display using Bridge pattern
                _displayManager.DisplaySensorData(data);

                // Notify observers
                _subject.Notify(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing sensor data: {ex.Message}");
            }
        }

        /// <summary>
        /// Displays the hierarchical organization of sensors to the console.
        /// </summary>
        public void DisplaySensorHierarchy()
        {
            _compositeManager.DisplayHierarchy();
        }

        /// <summary>
        /// Organizes sensors into groups based on their manufacturers.
        /// </summary>
        public void OrganizeSensorsByManufacturer()
        {
            _compositeManager.OrganizeSensorsByManufacturer();
        }

        /// <summary>
        /// Gets statistics for a specific group of sensors.
        /// </summary>
        /// <param name="groupName">The name of the sensor group to retrieve statistics for.</param>
        /// <returns>A dictionary containing aggregated statistics for the specified group.</returns>
        public Dictionary<string, double> GetGroupStats(string groupName)
        {
            return _compositeManager.GetGroupStats(groupName);
        }

        /// <summary>
        /// Adds an observer to receive notifications about sensor data.
        /// </summary>
        /// <param name="observer">The observer to add.</param>
        public void AddObserver(ISensorDataObserver observer)
        {
            _subject.Attach(observer);
        }

        /// <summary>
        /// Removes an observer from receiving notifications.
        /// </summary>
        /// <param name="observer">The observer to remove.</param>
        public void RemoveObserver(ISensorDataObserver observer)
        {
            _subject.Detach(observer);
        }

        /// <summary>
        /// Gets a read-only dictionary of all registered sensors and their types.
        /// </summary>
        /// <returns>A dictionary mapping sensor serial numbers to their type.</returns>
        public IReadOnlyDictionary<string, string> GetAllSensorTypes()
        {
            return _typeManager.GetAllSensorTypes();
        }

        /// <summary>
        /// Gets all analysis results from the analyzer manager.
        /// </summary>
        /// <returns>A dictionary containing analysis results.</returns>
        public Dictionary<string, string> GetAllAnalysisResults()
        {
            return _analyzerManager.GetAllAnalysisResults();
        }

        /// <summary>
        /// Applies the health visitor to evaluate the health of all sensors.
        /// Implements the Visitor pattern.
        /// </summary>
        /// <returns>A string containing the health analysis results.</returns>
        public string ApplyHealthVisitor()
        {
            var visitor = new SensorHealthVisitor();
            return _compositeManager.ApplyVisitor(visitor);
        }

        /// <summary>
        /// Applies the anomaly detection visitor to identify anomalies in sensor data.
        /// Implements the Visitor pattern.
        /// </summary>
        /// <returns>A string containing the anomaly detection results.</returns>
        public string ApplyAnomalyDetectionVisitor()
        {
            var visitor = new AnomalyDetectionVisitor();
            return _compositeManager.ApplyVisitor(visitor);
        }

        /// <summary>
        /// Analyzes sensors of a specific type using the Actor model for concurrent processing.
        /// </summary>
        /// <param name="sensorType">The type of sensors to analyze.</param>
        /// <returns>A task that resolves to a dictionary containing analysis results.</returns>
        public async Task<Dictionary<string, double>> AnalyzeSensorTypeWithActorsAsync(string sensorType)
        {
            return await _actorSystem.AnalyzeSensorTypeAsync(sensorType);
        }

        /// <summary>
        /// Gets the count of processed sensor data points from the Actor system.
        /// </summary>
        /// <returns>A task that resolves to the count of processed data points.</returns>
        public async Task<int> GetProcessedDataCountAsync()
        {
            return await _actorSystem.GetProcessedDataCountAsync();
        }

        /// <summary>
        /// Gets alert messages from the Actor system.
        /// </summary>
        /// <returns>A task that resolves to a string containing alert messages.</returns>
        public async Task<string> GetAlertsAsync()
        {
            return await _actorSystem.GetAlertsAsync();
        }

        /// <summary>
        /// Displays statistics using the specified display method.
        /// Implements the Bridge pattern.
        /// </summary>
        /// <param name="statistics">The statistics to display.</param>
        /// <param name="title">The title for the statistics display.</param>
        /// <param name="displayName">The name of the display method to use. Defaults to "console".</param>
        public void DisplayStatistics(Dictionary<string, double> statistics, string title, string displayName = "console")
        {
            _displayManager.DisplayStatistics(statistics, title, displayName);
        }

        /// <summary>
        /// Broadcasts an alert to all registered displays.
        /// Implements the Bridge pattern.
        /// </summary>
        /// <param name="alertMessage">The alert message to broadcast.</param>
        /// <param name="severity">The severity level of the alert.</param>
        public void BroadcastAlert(string alertMessage, string severity)
        {
            _displayManager.BroadcastAlert(alertMessage, severity);
        }

        /// <summary>
        /// Determines the appropriate parser for the given raw data.
        /// </summary>
        /// <param name="rawData">The raw sensor data string to parse.</param>
        /// <returns>The appropriate parser for the data, or null if no suitable parser is found.</returns>
        private ISensorDataParser DetermineParser(string rawData)
        {
            foreach (var parser in _parsers)
            {
                if (parser.CanParse(rawData))
                {
                    return parser;
                }
            }
            return null;
        }

        /// <summary>
        /// Releases resources used by the Actor system.
        /// </summary>
        public void Dispose()
        {
            _actorSystem.Dispose();
        }

        /// <summary>
        /// Processes raw sensor data and returns the parsed SensorData object.
        /// Similar to ProcessSensorData but returns the processed object instead of void.
        /// </summary>
        /// <param name="rawData">The raw string data from a sensor.</param>
        /// <returns>The processed SensorData object, or null if processing failed.</returns>
        public SensorData ProcessSensorDataAndReturn(string rawData)
        {
            try
            {
                // Determine the correct parser based on the data format
                ISensorDataParser parser = DetermineParser(rawData);
                if (parser == null)
                {
                    Console.WriteLine($"No suitable parser found for data: {rawData}");
                    return null;
                }

                // Process the data
                SensorData data = parser.Parse(rawData);

                // Register the sensor type
                if (!string.IsNullOrEmpty(data.SerialNumber) && !string.IsNullOrEmpty(data.Type))
                {
                    _typeManager.RegisterSensorType(data.SerialNumber, data.Type);
                }

                // Analyze the data
                _analyzerManager.AnalyzeData(data);

                // Send to Actor system
                _actorSystem.ProcessSensorData(data);

                // Notify observers
                _subject.Notify(data);

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing sensor data: {ex.Message}");
                return null;
            }
        }
    }
}