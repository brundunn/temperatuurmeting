using System;
using System.Collections.Generic;
using System.Text;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Patterns.Structural.Composite;

namespace TemperatuurMetingen.Patterns.Behavioral.Visitor
{
    /// <summary>
    /// Implements the Visitor pattern to assess sensor health based on battery levels.
    /// Categorizes sensors into healthy, warning, critical, and offline states.
    /// </summary>
    public class SensorHealthVisitor : ISensorVisitor
    {
        /// <summary>
        /// Stores the detailed report of sensor health information.
        /// </summary>
        private StringBuilder _report = new StringBuilder();

        /// <summary>
        /// Number of sensors with good battery levels.
        /// </summary>
        private int _healthySensors = 0;

        /// <summary>
        /// Number of sensors with moderate battery levels requiring attention.
        /// </summary>
        private int _warningSensors = 0;

        /// <summary>
        /// Number of sensors with critically low battery levels needing immediate attention.
        /// </summary>
        private int _criticalSensors = 0;

        /// <summary>
        /// Number of sensors that are not transmitting data.
        /// </summary>
        private int _offlineSensors = 0;

        /// <summary>
        /// Visits a sensor leaf node to assess its health based on battery level.
        /// </summary>
        /// <param name="sensor">The sensor leaf node to visit.</param>
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

        /// <summary>
        /// Visits a sensor group node and records group information.
        /// </summary>
        /// <param name="group">The sensor group to visit.</param>
        public void Visit(SensorGroup group)
        {
            // Handle groups differently, just log group info
            if (group.Type != "Root") // Skip root group
            {
                var data = group.GetAggregatedData();
                _report.AppendLine($"Group: {group.Name} - {group.GetSensorCount()} sensors, Avg Battery: {data["BatteryLevel"]:F1}%");
            }
        }

        /// <summary>
        /// Clears all counters and the report to start a fresh assessment.
        /// </summary>
        public void Reset()
        {
            _report.Clear();
            _healthySensors = 0;
            _warningSensors = 0;
            _criticalSensors = 0;
            _offlineSensors = 0;
        }

        /// <summary>
        /// Generates a comprehensive health report of all visited sensors.
        /// </summary>
        /// <returns>A formatted string containing the sensor health report with summary counts and detailed issues.</returns>
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