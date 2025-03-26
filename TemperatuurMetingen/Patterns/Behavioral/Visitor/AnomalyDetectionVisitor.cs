using System;
using System.Collections.Generic;
using System.Text;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Patterns.Structural.Composite;

namespace TemperatuurMetingen.Patterns.Behavioral.Visitor
{
    public class AnomalyDetectionVisitor : ISensorVisitor
    {
        private readonly double _tempThresholdHigh;
        private readonly double _tempThresholdLow;
        private readonly double _humidityThresholdHigh;
        private readonly double _humidityThresholdLow;
        
        private List<string> _anomalies = new List<string>();
        
        public AnomalyDetectionVisitor(
            double tempThresholdLow = 15.0,
            double tempThresholdHigh = 30.0,
            double humidityThresholdLow = 30.0,
            double humidityThresholdHigh = 70.0)
        {
            _tempThresholdLow = tempThresholdLow;
            _tempThresholdHigh = tempThresholdHigh;
            _humidityThresholdLow = humidityThresholdLow;
            _humidityThresholdHigh = humidityThresholdHigh;
        }
        
        public void Visit(SensorLeaf sensor)
        {
            var data = sensor.GetAggregatedData();
            if (data["DataPointCount"] == 0)
                return;
                
            // Check for temperature anomalies
            if (data["Temperature"] > 0)
            {
                if (data["Temperature"] > _tempThresholdHigh)
                {
                    _anomalies.Add($"High temperature detected on {sensor.Name}: {data["Temperature"]:F1}°C");
                }
                else if (data["Temperature"] < _tempThresholdLow)
                {
                    _anomalies.Add($"Low temperature detected on {sensor.Name}: {data["Temperature"]:F1}°C");
                }
            }
            
            // Check for humidity anomalies
            if (data["Humidity"] > 0)
            {
                if (data["Humidity"] > _humidityThresholdHigh)
                {
                    _anomalies.Add($"High humidity detected on {sensor.Name}: {data["Humidity"]:F1}%");
                }
                else if (data["Humidity"] < _humidityThresholdLow)
                {
                    _anomalies.Add($"Low humidity detected on {sensor.Name}: {data["Humidity"]:F1}%");
                }
            }
        }
        
        public void Visit(SensorGroup group)
        {
            //Just checking leaf nodes, no special handling for groups
        }
        
        public void Reset()
        {
            _anomalies.Clear();
        }
        
        public string GetResult()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("Anomaly Detection Report:");
            
            if (_anomalies.Count == 0)
            {
                result.AppendLine("No anomalies detected.");
            }
            else
            {
                result.AppendLine($"Found {_anomalies.Count} anomalies:");
                foreach (var anomaly in _anomalies)
                {
                    result.AppendLine($"- {anomaly}");
                }
            }
            
            return result.ToString();
        }
    }
}