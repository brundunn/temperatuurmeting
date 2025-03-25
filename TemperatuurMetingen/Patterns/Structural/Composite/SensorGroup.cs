using System;
using System.Collections.Generic;
using System.Linq;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Patterns.Structural.Composite
{
    public class SensorGroup : ISensorComponent
    {
        private readonly List<ISensorComponent> _components = new List<ISensorComponent>();

        public string Name { get; }
        public string Type { get; }

        public SensorGroup(string name, string type = "Group")
        {
            Name = name;
            Type = type;
        }

        public void AddComponent(ISensorComponent component)
        {
            if (!_components.Contains(component))
            {
                _components.Add(component);
            }
        }

        public void RemoveComponent(ISensorComponent component)
        {
            _components.Remove(component);
        }

        public void AddData(SensorData data)
        {
            // Propagate data to all child components
            foreach (var component in _components)
            {
                component.AddData(data);
            }
        }

        public Dictionary<string, double> GetAggregatedData()
        {
            if (_components.Count == 0)
            {
                return new Dictionary<string, double>
                {
                    { "DataPointCount", 0 },
                    { "Temperature", 0 },
                    { "Humidity", 0 },
                    { "BatteryLevel", 0 }
                };
            }

            var aggregatedStats = new Dictionary<string, double>
            {
                { "DataPointCount", 0 },
                { "Temperature", 0 },
                { "Humidity", 0 },
                { "BatteryLevel", 0 }
            };

            int tempComponentCount = 0;
            int humidityComponentCount = 0;
            int batteryComponentCount = 0;

            // Aggregate data from all components
            foreach (var component in _components)
            {
                var componentStats = component.GetAggregatedData();

                aggregatedStats["DataPointCount"] += componentStats["DataPointCount"];

                if (componentStats["Temperature"] > 0)
                {
                    aggregatedStats["Temperature"] += componentStats["Temperature"];
                    tempComponentCount++;
                }

                if (componentStats["Humidity"] > 0)
                {
                    aggregatedStats["Humidity"] += componentStats["Humidity"];
                    humidityComponentCount++;
                }

                if (componentStats["BatteryLevel"] > 0)
                {
                    aggregatedStats["BatteryLevel"] += componentStats["BatteryLevel"];
                    batteryComponentCount++;
                }
            }

            // Calculate averages
            if (tempComponentCount > 0)
                aggregatedStats["Temperature"] /= tempComponentCount;

            if (humidityComponentCount > 0)
                aggregatedStats["Humidity"] /= humidityComponentCount;

            if (batteryComponentCount > 0)
                aggregatedStats["BatteryLevel"] /= batteryComponentCount;

            return aggregatedStats;
        }

        public void DisplayInfo(int depth = 0)
        {
            string indent = new string(' ', depth * 2);
            Console.WriteLine($"{indent}Group: {Name} (Type: {Type}, Members: {_components.Count})");

            var stats = GetAggregatedData();
            int sensorCount = GetSensorCount();

            Console.WriteLine($"{indent}  Total Sensors: {sensorCount}");
            Console.WriteLine($"{indent}  Total Data Points: {stats["DataPointCount"]}");

            if (stats["Temperature"] > 0)
                Console.WriteLine($"{indent}  Avg Temperature: {stats["Temperature"]:F2}°C");

            if (stats["Humidity"] > 0)
                Console.WriteLine($"{indent}  Avg Humidity: {stats["Humidity"]:F2}%");

            if (stats["BatteryLevel"] > 0)
                Console.WriteLine($"{indent}  Avg Battery: {stats["BatteryLevel"]:F2}%");

            // Display child components with increased indentation
            foreach (var component in _components)
            {
                component.DisplayInfo(depth + 1);
            }
        }

        public int GetSensorCount()
        {
            return _components.Sum(c => c.GetSensorCount());
        }
    }
}