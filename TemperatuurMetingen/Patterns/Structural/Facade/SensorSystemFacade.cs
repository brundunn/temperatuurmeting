using System;
using System.Collections.Generic;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Patterns.Creational.Factory;
using TemperatuurMetingen.Patterns.Behavioral.Observer;
using TemperatuurMetingen.Patterns.Creational.Singleton;
using TemperatuurMetingen.Patterns.Behavioral.Strategy;
using TemperatuurMetingen.Patterns.Structural.Composite;
using TemperatuurMetingen.Services;

namespace TemperatuurMetingen.Patterns.Structural.Facade
{
    public class SensorSystemFacade
    {
        private readonly SensorDataProcessor _processor;
        private readonly SensorDataSubject _subject;
        private readonly SensorTypeManager _typeManager;
        private readonly List<ISensorDataParser> _parsers;
        private readonly AnalyzerManager _analyzerManager;
        private readonly SensorCompositeManager _compositeManager;
        
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