using System;
using System.Collections.Generic;
using System.Text;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Patterns.Structural.Composite;

namespace TemperatuurMetingen.Patterns.Behavioral.Visitor
{
    public class SensorHealthVisitor : ISensorVisitor
    {
        private StringBuilder _report = new StringBuilder();
        private int _healthySensors = 0;
        private int _warningSensors = 0;
        private int _criticalSensors = 0;
        private int _offlineSensors = 0;
        
        public void Visit(SensorLeaf sensor)
        {
            var data = sensor.GetAggregatedData();
            if (data["DataPointCount"] == 0)
                return;
                
            // Determine sensor health based on battery level
            if (data["BatteryLevel"] < 30)
            {
                _criticalSensors++;
                _report.AppendLine($"CRITICAL: Sensor {sensor.Name} has low battery ({data["BatteryLevel"]:F1}%)");
            }
            else if (data["BatteryLevel"] < 50)
            {
                _warningSensors++;
                _report.AppendLine($"WARNING: Sensor {sensor.Name} has moderate battery ({data["BatteryLevel"]:F1}%)");
            }
            else
            {
                _healthySensors++;
            }
        }
        
        public void Visit(SensorGroup group)
        {
            // Handle groups differently, just log group info
            if (group.Type != "Root") // Skip root group
            {
                var data = group.GetAggregatedData();
                _report.AppendLine($"Group: {group.Name} - {group.GetSensorCount()} sensors, Avg Battery: {data["BatteryLevel"]:F1}%");
            }
        }
        
        public void Reset()
        {
            _report.Clear();
            _healthySensors = 0;
            _warningSensors = 0;
            _criticalSensors = 0;
            _offlineSensors = 0;
        }
        
        public string GetResult()
        {
            StringBuilder summary = new StringBuilder();
            summary.AppendLine("Sensor Health Report:");
            summary.AppendLine($"- Healthy Sensors: {_healthySensors}");
            summary.AppendLine($"- Warning Sensors: {_warningSensors}");
            summary.AppendLine($"- Critical Sensors: {_criticalSensors}");
            summary.AppendLine($"- Offline Sensors: {_offlineSensors}");
            summary.AppendLine();
            summary.AppendLine("Detailed Issues:");
            summary.Append(_report.ToString());
            
            return summary.ToString();
        }
    }
}