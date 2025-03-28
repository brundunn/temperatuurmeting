using System;
using System.Collections.Generic;
using System.Linq;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Patterns.Structural.Composite
{
    /// <summary>
    /// Represents a group of sensors in the Composite design pattern.
    /// Acts as a composite object that can contain both individual sensors (leaves)
    /// and other sensor groups, allowing for a hierarchical structure.
    /// </summary>
    public class SensorGroup : ISensorComponent
    {
        /// <summary>
        /// The collection of child components (sensors or other groups) in this group.
        /// </summary>
        private readonly List<ISensorComponent> _components = new List<ISensorComponent>();

        /// <summary>
        /// Gets the display name of this sensor group.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the type classification of this sensor group (e.g., "Group", "Temperature", "Custom").
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Initializes a new instance of the SensorGroup class with the specified name and type.
        /// </summary>
        /// <param name="name">The display name for this sensor group.</param>
        /// <param name="type">The type classification of this group. Defaults to "Group".</param>
        public SensorGroup(string name, string type = "Group")
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        /// Adds a sensor component to this group if it's not already present.
        /// </summary>
        /// <param name="component">The sensor component (leaf or group) to add.</param>
        public void AddComponent(ISensorComponent component)
        {
            if (!_components.Contains(component))
            {
                _components.Add(component);
            }
        }

        /// <summary>
        /// Removes a sensor component from this group.
        /// </summary>
        /// <param name="component">The sensor component to remove.</param>
        public void RemoveComponent(ISensorComponent component)
        {
            _components.Remove(component);
        }

        /// <summary>
        /// Propagates sensor data to all child components in this group.
        /// Part of the ISensorComponent interface implementation.
        /// </summary>
        /// <param name="data">The sensor data to propagate to children.</param>
        public void AddData(SensorData data)
        {
            // Propagate data to all child components
            foreach (var component in _components)
            {
                component.AddData(data);
            }
        }

        /// <summary>
        /// Calculates aggregated statistics from all child components.
        /// Returns average values for temperature, humidity, and battery level,
        /// as well as the total data point count.
        /// </summary>
        /// <returns>A dictionary containing aggregated statistics for this group and all its children.</returns>
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

        /// <summary>
        /// Displays information about this group and its child components to the console.
        /// Shows group metadata, aggregated statistics, and recursively displays child components.
        /// </summary>
        /// <param name="depth">The indentation depth for hierarchical display. Defaults to 0.</param>
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

        /// <summary>
        /// Returns the total number of individual sensors (leaves) in this group and all subgroups.
        /// </summary>
        /// <returns>The total count of sensors contained in this hierarchy.</returns>
        public int GetSensorCount()
        {
            return _components.Sum(c => c.GetSensorCount());
        }
        
        /// <summary>
        /// Accepts a visitor to traverse this component and its children.
        /// Implementation of the Visitor design pattern to perform operations on the composite structure.
        /// </summary>
        /// <param name="visitor">The visitor object that will process this group.</param>
        public void Accept(ISensorVisitor visitor)
        {
            visitor.Visit(this);

            // Visit all children
            foreach (var component in _components)
            {
                component.Accept(visitor);
            }
        }
    }
}