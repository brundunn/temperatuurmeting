using System;
using System.Collections.Generic;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Patterns.Structural.Composite
{

    public class SensorLeaf : ISensorComponent
    {
        private readonly string _serialNumber;
        private readonly List<SensorData> _dataPoints = new List<SensorData>();

        public string Name { get; }
        public string Type { get; private set; }

        public SensorLeaf(string serialNumber, string name = null)
        {
            _serialNumber = serialNumber;
            Name = name ?? $"Sensor-{serialNumber}";
            Type = "Unknown";
        }

        public void AddData(SensorData data)
        {
            if (data.SerialNumber == _serialNumber)
            {
                _dataPoints.Add(data);

                // Update sensor type if available
                if (!string.IsNullOrEmpty(data.Type))
                {
                    Type = data.Type;
                }
            }
        }

        public Dictionary<string, double> GetAggregatedData()
        {
            var result = new Dictionary<string, double>
            {
                { "DataPointCount", _dataPoints.Count },
                { "Temperature", 0 },
                { "Humidity", 0 },
                { "BatteryLevel", 0 }
            };

            if (_dataPoints.Count == 0)
            {
                return result;
            }

            // Calculate averages
            double totalTemp = 0;
            double totalHumidity = 0;
            double totalBattery = 0;
            int tempCount = 0;
            int humidityCount = 0;
            int batteryCount = 0;

            foreach (var data in _dataPoints)
            {
                if (data.Temperature > 0)
                {
                    totalTemp += data.Temperature;
                    tempCount++;
                }

                if (data.Humidity > 0)
                {
                    totalHumidity += data.Humidity;
                    humidityCount++;
                }

                if (data.BatteryLevel > 0 && data.BatteryMax > 0)
                {
                    totalBattery += (data.BatteryLevel / data.BatteryMax) * 100;
                    batteryCount++;
                }
            }

            if (tempCount > 0)
                result["Temperature"] = totalTemp / tempCount;

            if (humidityCount > 0)
                result["Humidity"] = totalHumidity / humidityCount;

            if (batteryCount > 0)
                result["BatteryLevel"] = totalBattery / batteryCount;

            return result;
        }

        public void DisplayInfo(int depth = 0)
        {
            string indent = new string(' ', depth * 2);
            Console.WriteLine($"{indent}Sensor: {Name} (Type: {Type}, Serial: {_serialNumber})");

            var stats = GetAggregatedData();
            Console.WriteLine($"{indent}  Data Points: {stats["DataPointCount"]}");

            if (stats["Temperature"] > 0)
                Console.WriteLine($"{indent}  Avg Temperature: {stats["Temperature"]:F2}°C");

            if (stats["Humidity"] > 0)
                Console.WriteLine($"{indent}  Avg Humidity: {stats["Humidity"]:F2}%");

            if (stats["BatteryLevel"] > 0)
                Console.WriteLine($"{indent}  Avg Battery: {stats["BatteryLevel"]:F2}%");
        }

        public int GetSensorCount()
        {
            return 1;
        }
        
        public void Accept(ISensorVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}