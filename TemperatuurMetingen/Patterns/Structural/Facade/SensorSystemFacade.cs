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
    public class SensorSystemFacade : IDisposable
    {
        private readonly SensorDataProcessor _processor;
        private readonly SensorDataSubject _subject;
        private readonly SensorTypeManager _typeManager;
        private readonly List<ISensorDataParser> _parsers;
        private readonly AnalyzerManager _analyzerManager;
        private readonly SensorCompositeManager _compositeManager;
        private readonly ActorSystemManager _actorSystem;
        private readonly DisplayManager _displayManager;
        
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
        
        public void DisplaySensorHierarchy()
        {
            _compositeManager.DisplayHierarchy();
        }
        public void OrganizeSensorsByManufacturer()
        {
            _compositeManager.OrganizeSensorsByManufacturer();
        }

        public Dictionary<string, double> GetGroupStats(string groupName)
        {
            return _compositeManager.GetGroupStats(groupName);
        }
        public void AddObserver(ISensorDataObserver observer)
        {
            _subject.Attach(observer);
        }
        
        public void RemoveObserver(ISensorDataObserver observer)
        {
            _subject.Detach(observer);
        }
        
        public IReadOnlyDictionary<string, string> GetAllSensorTypes()
        {
            return _typeManager.GetAllSensorTypes();
        }
        
        public Dictionary<string, string> GetAllAnalysisResults()
        {
            return _analyzerManager.GetAllAnalysisResults();
        }
        
        // Visitor pattern methods
        public string ApplyHealthVisitor()
        {
            var visitor = new SensorHealthVisitor();
            return _compositeManager.ApplyVisitor(visitor);
        }
        
        public string ApplyAnomalyDetectionVisitor()
        {
            var visitor = new AnomalyDetectionVisitor();
            return _compositeManager.ApplyVisitor(visitor);
        }
        
        // Actor model methods
        public async Task<Dictionary<string, double>> AnalyzeSensorTypeWithActorsAsync(string sensorType)
        {
            return await _actorSystem.AnalyzeSensorTypeAsync(sensorType);
        }
        
        public async Task<int> GetProcessedDataCountAsync()
        {
            return await _actorSystem.GetProcessedDataCountAsync();
        }
        
        public async Task<string> GetAlertsAsync()
        {
            return await _actorSystem.GetAlertsAsync();
        }
        
        // Bridge pattern methods
        public void DisplayStatistics(Dictionary<string, double> statistics, string title, string displayName = "console")
        {
            _displayManager.DisplayStatistics(statistics, title, displayName);
        }
        
        public void BroadcastAlert(string alertMessage, string severity)
        {
            _displayManager.BroadcastAlert(alertMessage, severity);
        }
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
        public void Dispose()
        {
            _actorSystem.Dispose();
        }

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