using System;
using System.Collections.Generic;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Core.Interfaces;

namespace TemperatuurMetingen.Patterns.Creational.Factory;

    public class AnalyzerManager
    {
        private readonly Dictionary<string, ISensorDataAnalyzer> _analyzers = new Dictionary<string, ISensorDataAnalyzer>();
            
        public void RegisterAnalyzer(string sensorType, ISensorDataAnalyzer analyzer)
        {
            if (!_analyzers.ContainsKey(sensorType))
            {
                _analyzers.Add(sensorType, analyzer);
            }
        }
            
        public void RegisterAnalyzer(string sensorType, AnalyzerFactory factory)
        {
            if (!_analyzers.ContainsKey(sensorType))
            {
                _analyzers.Add(sensorType, factory.CreateAnalyzer());
            }
        }
            
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