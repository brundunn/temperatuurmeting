using System;
using System.Collections.Generic;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Core.Interfaces;

namespace TemperatuurMetingen.Patterns.Creational.Factory;

    /// <summary>
    /// Manages sensor data analyzers and coordinates data analysis across multiple analyzer types.
    /// Acts as a client for the Factory pattern by working with analyzer factories.
    /// </summary>
    public class AnalyzerManager
    {
        /// <summary>
        /// Dictionary of analyzers indexed by sensor type.
        /// </summary>
        private readonly Dictionary<string, ISensorDataAnalyzer> _analyzers = new Dictionary<string, ISensorDataAnalyzer>();

        /// <summary>
        /// Registers a pre-configured analyzer for a specific sensor type.
        /// </summary>
        /// <param name="sensorType">The type of sensor this analyzer will process.</param>
        /// <param name="analyzer">The analyzer instance to register.</param>
        public void RegisterAnalyzer(string sensorType, ISensorDataAnalyzer analyzer)
        {
            if (!_analyzers.ContainsKey(sensorType))
            {
                _analyzers.Add(sensorType, analyzer);
            }
        }

        /// <summary>
        /// Registers an analyzer for a sensor type by creating it through the provided factory.
        /// Demonstrates the Factory Method pattern usage.
        /// </summary>
        /// <param name="sensorType">The type of sensor this analyzer will process.</param>
        /// <param name="factory">The factory that will create the appropriate analyzer.</param>
        public void RegisterAnalyzer(string sensorType, AnalyzerFactory factory)
        {
            if (!_analyzers.ContainsKey(sensorType))
            {
                _analyzers.Add(sensorType, factory.CreateAnalyzer());
            }
        }

        /// <summary>
        /// Analyzes sensor data using the appropriate registered analyzer.
        /// Also applies battery analysis if a battery analyzer is registered.
        /// </summary>
        /// <param name="data">The sensor data to analyze.</param>
        public void AnalyzeData(SensorData data)
        {
            if (string.IsNullOrEmpty(data.Type))
            {
                return;
            }

            if (_analyzers.ContainsKey(data.Type))
            {
                _analyzers[data.Type].Analyze(data);
            }

            // Also analyze with general analyzers like battery
            if (_analyzers.ContainsKey("battery"))
            {
                _analyzers["battery"].Analyze(data);
            }
        }

        /// <summary>
        /// Retrieves analysis results from all registered analyzers.
        /// </summary>
        /// <returns>A dictionary mapping analyzer identifiers to their analysis results.</returns>
        public Dictionary<string, string> GetAllAnalysisResults()
        {
            var results = new Dictionary<string, string>();
            foreach (var analyzer in _analyzers)
            {
                results.Add($"{analyzer.Key} ({analyzer.Value.AnalyzerType})", analyzer.Value.GetAnalysisResult());
            }
            return results;
        }
    }