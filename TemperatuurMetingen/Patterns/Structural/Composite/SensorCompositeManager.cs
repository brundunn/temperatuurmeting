using System;
using System.Collections.Generic;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Patterns.Creational.Singleton;

namespace TemperatuurMetingen.Patterns.Structural.Composite
{
    /// <summary>
    /// Manages sensors in a hierarchical structure using the Composite design pattern.
    /// Allows organizing sensors into groups, retrieving aggregated data, and applying visitors.
    /// </summary>
    public class SensorCompositeManager
    {
        /// <summary>
        /// Dictionary mapping sensor serial numbers to their component representations.
        /// </summary>
        private readonly Dictionary<string, ISensorComponent> _sensors = new Dictionary<string, ISensorComponent>();
        
        /// <summary>
        /// Dictionary mapping group names to their SensorGroup instances.
        /// </summary>
        private readonly Dictionary<string, SensorGroup> _groups = new Dictionary<string, SensorGroup>();
        
        /// <summary>
        /// Singleton instance of the SensorTypeManager.
        /// </summary>
        private readonly SensorTypeManager _typeManager = SensorTypeManager.Instance;

        /// <summary>
        /// Gets the root group containing all sensors and subgroups.
        /// </summary>
        public SensorGroup RootGroup { get; }

        /// <summary>
        /// Initializes a new instance of the SensorCompositeManager class.
        /// Creates a root group and default sensor type groups.
        /// </summary>
        public SensorCompositeManager()
        {
            RootGroup = new SensorGroup("All Sensors", "Root");

            // Create default groups for sensor types
            CreateSensorTypeGroup("Temperature Sensors", "temp");
            CreateSensorTypeGroup("Humidity Sensors", "humidity");
        }

        /// <summary>
        /// Adds a new sensor data point to the appropriate sensor component.
        /// If the sensor doesn't exist yet, it creates a new SensorLeaf.
        /// </summary>
        /// <param name="data">The sensor data to add.</param>
        public void AddSensorData(SensorData data)
        {
            if (string.IsNullOrEmpty(data.SerialNumber))
                return;

            // Get or create sensor leaf
            if (!_sensors.TryGetValue(data.SerialNumber, out ISensorComponent sensor))
            {
                sensor = new SensorLeaf(data.SerialNumber);
                _sensors.Add(data.SerialNumber, sensor);

                // Add to root group
                RootGroup.AddComponent(sensor);

                // Add to appropriate type group if type is available
                if (!string.IsNullOrEmpty(data.Type))
                {
                    AddSensorToTypeGroup(sensor, data.Type);
                }
            }

            // Add data to the sensor
            sensor.AddData(data);
        }

        /// <summary>
        /// Creates a new sensor group based on a sensor type.
        /// </summary>
        /// <param name="groupName">The display name for the group.</param>
        /// <param name="sensorType">The sensor type this group will contain.</param>
        /// <returns>The created or existing SensorGroup.</returns>
        public SensorGroup CreateSensorTypeGroup(string groupName, string sensorType)
        {
            var key = $"type:{sensorType}";
            if (!_groups.TryGetValue(key, out SensorGroup group))
            {
                group = new SensorGroup(groupName, sensorType);
                _groups.Add(key, group);
                RootGroup.AddComponent(group);
            }

            return group;
        }

        /// <summary>
        /// Creates a custom sensor group with the specified name.
        /// </summary>
        /// <param name="groupName">The name for the custom group.</param>
        /// <returns>The created or existing SensorGroup.</returns>
        public SensorGroup CreateCustomGroup(string groupName)
        {
            if (!_groups.TryGetValue(groupName, out SensorGroup group))
            {
                group = new SensorGroup(groupName, "Custom");
                _groups.Add(groupName, group);
                RootGroup.AddComponent(group);
            }

            return group;
        }

        /// <summary>
        /// Adds a sensor to a specific group by their identifiers.
        /// </summary>
        /// <param name="serialNumber">The serial number of the sensor to add.</param>
        /// <param name="groupName">The name of the group to add the sensor to.</param>
        public void AddSensorToGroup(string serialNumber, string groupName)
        {
            if (_sensors.TryGetValue(serialNumber, out ISensorComponent sensor) &&
                _groups.TryGetValue(groupName, out SensorGroup group))
            {
                group.AddComponent(sensor);
            }
        }

        /// <summary>
        /// Adds a sensor to its type-specific group.
        /// </summary>
        /// <param name="sensor">The sensor component to add.</param>
        /// <param name="sensorType">The type of the sensor.</param>
        public void AddSensorToTypeGroup(ISensorComponent sensor, string sensorType)
        {
            var key = $"type:{sensorType}";
            if (_groups.TryGetValue(key, out SensorGroup group))
            {
                group.AddComponent(sensor);
            }
        }

        /// <summary>
        /// Displays the entire sensor hierarchy to the console.
        /// </summary>
        public void DisplayHierarchy()
        {
            Console.WriteLine("\nSensor Hierarchy:");
            RootGroup.DisplayInfo();
        }

        /// <summary>
        /// Gets aggregated statistics for a specific group.
        /// </summary>
        /// <param name="groupName">The name of the group to get statistics for, or "root" for the root group.</param>
        /// <returns>A dictionary containing the aggregated statistics.</returns>
        public Dictionary<string, double> GetGroupStats(string groupName)
        {
            if (_groups.TryGetValue(groupName, out SensorGroup group))
            {
                return group.GetAggregatedData();
            }

            if (groupName == "root")
            {
                return RootGroup.GetAggregatedData();
            }

            return new Dictionary<string, double>();
        }

        /// <summary>
        /// Organizes sensors into groups based on their manufacturers.
        /// Creates a group for each manufacturer and assigns sensors accordingly.
        /// </summary>
        public void OrganizeSensorsByManufacturer()
        {
            // Group sensors by manufacturer
            var manufacturers = new HashSet<string>();

            foreach (var sensor in _sensors.Values)
            {
                if (sensor is SensorLeaf leaf)
                {
                    // Get the manufacturer from the latest data
                    var data = leaf.GetAggregatedData();
                    if (data["DataPointCount"] > 0)
                    {
                        var manufacturer = GetManufacturer(leaf.Name);
                        if (!string.IsNullOrEmpty(manufacturer))
                        {
                            manufacturers.Add(manufacturer);
                        }
                    }
                }
            }

            // Create manufacturer groups
            foreach (var manufacturer in manufacturers)
            {
                var groupName = $"Manufacturer: {manufacturer}";
                var group = CreateCustomGroup(groupName);

                // Add sensors to manufacturer groups
                foreach (var kvp in _sensors)
                {
                    if (GetManufacturer(kvp.Key) == manufacturer)
                    {
                        group.AddComponent(kvp.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Determines the manufacturer of a sensor based on its serial number.
        /// This is a placeholder implementation that returns mock values.
        /// </summary>
        /// <param name="serialNumber">The serial number of the sensor.</param>
        /// <returns>The name of the manufacturer.</returns>
        private string GetManufacturer(string serialNumber)
        {
            // This is a placeholder - in a real implementation,
            // you would retrieve the manufacturer from the sensor data
            // For now, we'll just return a mock value
            if (serialNumber.StartsWith("1"))
                return "Qualcomm";
            else if (serialNumber.StartsWith("2"))
                return "Texas Instruments";
            else if (serialNumber.StartsWith("3"))
                return "NXP";
            else if (serialNumber.StartsWith("9"))
                return "Infineon";
            else
                return "Unknown";
        }

        /// <summary>
        /// Applies a visitor to the entire sensor hierarchy and returns the result.
        /// Uses the Visitor design pattern to traverse the composite structure.
        /// </summary>
        /// <param name="visitor">The sensor visitor to apply.</param>
        /// <returns>The result string from the visitor after traversal.</returns>
        public string ApplyVisitor(ISensorVisitor visitor)
        {
            visitor.Reset();
            RootGroup.Accept(visitor);
            return visitor.GetResult();
        }
    }
}