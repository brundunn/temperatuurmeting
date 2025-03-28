using System;
using System.Collections.Generic;
using System.Text;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Patterns.Structural.Bridge.Formatters
{
    /// <summary>
    /// A concrete implementation of the ISensorDataFormatter interface that formats data into human-readable text.
    /// This class is part of the Bridge pattern, where it acts as a Concrete Implementor.
    /// </summary>
    public class TextFormatter : ISensorDataFormatter
    {
        /// <summary>
        /// Formats a single sensor data object into a human-readable string.
        /// </summary>
        /// <param name="data">The sensor data to format.</param>
        /// <returns>A formatted string representation of the sensor data. Returns "No data" if data is null.</returns>
        public string FormatSensorData(SensorData data)
        {
            if (data == null)
                return "No data";

            return $"Sensor: {data.SerialNumber} | Type: {data.Type} | " +
                   $"Temp: {data.Temperature:F1}°C | Humidity: {data.Humidity:F1}% | " +
                   $"Battery: {data.BatteryLevel}/{data.BatteryMax} | State: {data.State}";
        }

        /// <summary>
        /// Formats a collection of sensor data objects into a human-readable multiline string.
        /// </summary>
        /// <param name="dataList">The collection of sensor data to format.</param>
        /// <returns>A multiline string representation of the sensor data collection with header, footer, and count.</returns>
        public string FormatSensorList(IEnumerable<SensorData> dataList)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Sensor Data List:");
            builder.AppendLine("----------------");

            int count = 0;
            foreach (var data in dataList)
            {
                builder.AppendLine(FormatSensorData(data));
                count++;
            }

            builder.AppendLine("----------------");
            builder.AppendLine($"Total sensors: {count}");

            return builder.ToString();
        }

        /// <summary>
        /// Formats statistics (key-value pairs) into a human-readable multiline string.
        /// Automatically appends the appropriate unit suffix based on the statistic name.
        /// </summary>
        /// <param name="statistics">A dictionary containing statistic names and their values.</param>
        /// <returns>A multiline string representation of the statistics with appropriate units.</returns>
        public string FormatStatistics(Dictionary<string, double> statistics)
        {
            var builder = new StringBuilder();

            foreach (var stat in statistics)
            {
                string formattedValue = stat.Key.Contains("Temperature") ? $"{stat.Value:F1}°C" :
                                      stat.Key.Contains("Humidity") ? $"{stat.Value:F1}%" :
                                      stat.Key.Contains("Battery") ? $"{stat.Value:F1}%" :
                                      $"{stat.Value:F1}";

                builder.AppendLine($"{stat.Key}: {formattedValue}");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Formats an alert message with severity into a simple text format.
        /// </summary>
        /// <param name="alertMessage">The alert message to format.</param>
        /// <param name="severity">The severity level of the alert.</param>
        /// <returns>A string representing the alert in the format "[severity] message".</returns>
        public string FormatAlert(string alertMessage, string severity)
        {
            return $"[{severity}] {alertMessage}";
        }
    }
}