using System;
using System.Collections.Generic;
using System.Text.Json;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Patterns.Structural.Bridge.Formatters
{
    /// <summary>
    /// A concrete implementation of the ISensorDataFormatter interface that formats data into JSON format.
    /// This class is part of the Bridge pattern, where it acts as a Concrete Implementor.
    /// </summary>
    public class JsonFormatter : ISensorDataFormatter
    {
        /// <summary>
        /// JSON serialization options used by all formatter methods.
        /// Configured with indented writing for better readability.
        /// </summary>
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        /// <summary>
        /// Formats a single sensor data object into a JSON string.
        /// </summary>
        /// <param name="data">The sensor data to format.</param>
        /// <returns>A JSON string representation of the sensor data. Returns "{}" if data is null.</returns>
        public string FormatSensorData(SensorData data)
        {
            if (data == null)
                return "{}";

            return JsonSerializer.Serialize(data, _options);
        }

        /// <summary>
        /// Formats a collection of sensor data objects into a JSON array string.
        /// </summary>
        /// <param name="dataList">The collection of sensor data to format.</param>
        /// <returns>A JSON array string representation of the sensor data collection.</returns>
        public string FormatSensorList(IEnumerable<SensorData> dataList)
        {
            return JsonSerializer.Serialize(dataList, _options);
        }

        /// <summary>
        /// Formats statistics (key-value pairs) into a JSON object string.
        /// </summary>
        /// <param name="statistics">A dictionary containing statistic names and their values.</param>
        /// <returns>A JSON object string representation of the statistics.</returns>
        public string FormatStatistics(Dictionary<string, double> statistics)
        {
            return JsonSerializer.Serialize(statistics, _options);
        }

        /// <summary>
        /// Formats an alert message with severity into a JSON object string.
        /// </summary>
        /// <param name="alertMessage">The alert message to format.</param>
        /// <param name="severity">The severity level of the alert.</param>
        /// <returns>A JSON object string containing the alert message, severity, and timestamp.</returns>
        public string FormatAlert(string alertMessage, string severity)
        {
            var alert = new
            {
                Severity = severity,
                Message = alertMessage,
                Timestamp = DateTime.Now
            };

            return JsonSerializer.Serialize(alert, _options);
        }
    }
}