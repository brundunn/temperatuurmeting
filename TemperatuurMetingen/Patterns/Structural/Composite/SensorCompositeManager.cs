using System;
using System.Collections.Generic;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Patterns.Creational.Singleton;

namespace TemperatuurMetingen.Patterns.Structural.Composite
{
    public class SensorCompositeManager
    {
        private readonly Dictionary<string, ISensorComponent> _sensors = new Dictionary<string, ISensorComponent>();
        private readonly Dictionary<string, SensorGroup> _groups = new Dictionary<string, SensorGroup>();
        private readonly SensorTypeManager _typeManager = SensorTypeManager.Instance;

        // Root group containing all sensors
        public SensorGroup RootGroup { get; }

        public SensorCompositeManager()
        {
            RootGroup = new SensorGroup("All Sensors", "Root");

            // Create default groups for sensor types
            CreateSensorTypeGroup("Temperature Sensors", "temp");
            CreateSensorTypeGroup("Humidity Sensors", "humidity");
        }

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

        public void AddSensorToGroup(string serialNumber, string groupName)
        {
            if (_sensors.TryGetValue(serialNumber, out ISensorComponent sensor) &&
                _groups.TryGetValue(groupName, out SensorGroup group))
            {
                group.AddComponent(sensor);
            }
        }

        public void AddSensorToTypeGroup(ISensorComponent sensor, string sensorType)
        {
            var key = $"type:{sensorType}";
            if (_groups.TryGetValue(key, out SensorGroup group))
            {
                group.AddComponent(sensor);
            }
        }

        public void DisplayHierarchy()
        {
            Console.WriteLine("\nSensor Hierarchy:");
            RootGroup.DisplayInfo();
        }

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
    }
}