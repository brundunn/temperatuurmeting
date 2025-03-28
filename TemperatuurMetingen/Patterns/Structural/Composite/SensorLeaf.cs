using System;
using System.Collections.Generic;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Patterns.Structural.Composite
{
    /// <summary>
    /// Represents an individual sensor in the Composite design pattern.
    /// Acts as a leaf node that collects and processes data from a specific sensor.
    /// Implements ISensorComponent to maintain hierarchy compatibility with SensorGroup.
    /// </summary>
    public class SensorLeaf : ISensorComponent
    {
        /// <summary>
        /// The unique serial number that identifies this sensor.
        /// </summary>
        private readonly string _serialNumber;

        /// <summary>
        /// Collection of historical data points received from this sensor.
        /// </summary>
        private readonly List<SensorData> _dataPoints = new List<SensorData>();

        /// <summary>
        /// Gets the display name of this sensor.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or privately sets the sensor type classification (e.g., "Temperature", "Humidity").
        /// Initially set to "Unknown" and updated when data is received.
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Initializes a new instance of the SensorLeaf class with the specified serial number and optional name.
        /// </summary>
        /// <param name="serialNumber">The unique identifier for this sensor.</param>
        /// <param name="name">The display name for this sensor. If null, a default name based on the serial number is used.</param>
        public SensorLeaf(string serialNumber, string name = null)
        {
            _serialNumber = serialNumber;
            Name = name ?? $"Sensor-{serialNumber}";
            Type = "Unknown";
        }

        /// <summary>
        /// Adds a new data point to this sensor's history if the serial number matches.
        /// Also updates the sensor type if available in the data.
        /// </summary>
        /// <param name="data">The sensor data to add to this sensor's history.</param>
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

        /// <summary>
        /// Calculates aggregated statistics from all historical data points.
        /// Returns average values for temperature, humidity, and battery level,
        /// as well as the total data point count.
        /// </summary>
        /// <returns>A dictionary containing aggregated statistics for this sensor.</returns>
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

        /// <summary>
        /// Displays information about this sensor to the console.
        /// Shows sensor metadata and aggregated statistics.
        /// </summary>
        /// <param name="depth">The indentation depth for hierarchical display. Defaults to 0.</param>
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

        /// <summary>
        /// Returns the count of sensors represented by this component.
        /// Always returns 1 for leaf nodes in the Composite pattern.
        /// </summary>
        /// <returns>Always returns 1, as this represents a single sensor.</returns>
        public int GetSensorCount()
        {
            return 1;
        }

        /// <summary>
        /// Accepts a visitor to process this sensor node.
        /// Implementation of the Visitor design pattern.
        /// </summary>
        /// <param name="visitor">The visitor object that will process this sensor.</param>
        public void Accept(ISensorVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}