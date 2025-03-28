using System;
using System.Collections.Generic;
using System.IO;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Patterns.Structural.Bridge.Displays
{
    /// <summary>
    /// A concrete implementation of the ISensorDataDisplay interface that renders sensor data to a file.
    /// This class is part of the Bridge pattern, where it acts as a Concrete Implementor.
    /// It uses an injected formatter to determine how the data should be formatted before writing to file.
    /// </summary>
    public class FileDisplay : ISensorDataDisplay
    {
        /// <summary>
        /// The formatter used to format sensor data before writing to file.
        /// </summary>
        private readonly ISensorDataFormatter _formatter;
        
        /// <summary>
        /// The path to the file where sensor data will be written.
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// Initializes a new instance of the FileDisplay class.
        /// Creates or clears the file at the specified path and writes a header.
        /// </summary>
        /// <param name="formatter">The formatter that will be used to format sensor data before display.</param>
        /// <param name="filePath">The path to the file where sensor data will be written.</param>
        public FileDisplay(ISensorDataFormatter formatter, string filePath)
        {
            _formatter = formatter;
            _filePath = filePath;

            // Create or clear the file
            File.WriteAllText(_filePath, $"Sensor Monitoring Log - {DateTime.Now}\n\n");
        }

        /// <summary>
        /// Writes a single sensor data point to the file.
        /// </summary>
        /// <param name="data">The sensor data to display.</param>
        public void DisplaySensorData(SensorData data)
        {
            AppendToFile(_formatter.FormatSensorData(data));
        }

        /// <summary>
        /// Writes a collection of sensor data points to the file.
        /// </summary>
        /// <param name="dataList">The collection of sensor data to display.</param>
        public void DisplaySensorList(IEnumerable<SensorData> dataList)
        {
            AppendToFile(_formatter.FormatSensorList(dataList));
        }

        /// <summary>
        /// Writes statistics with a title to the file.
        /// </summary>
        /// <param name="statistics">A dictionary containing statistic names and their values.</param>
        /// <param name="title">The title to display above the statistics.</param>
        public void DisplayStatistics(Dictionary<string, double> statistics, string title)
        {
            AppendToFile($"=== {title} ===\n");
            AppendToFile(_formatter.FormatStatistics(statistics));
            AppendToFile("================\n");
        }

        /// <summary>
        /// Writes an alert message to the file.
        /// </summary>
        /// <param name="alertMessage">The alert message to display.</param>
        /// <param name="severity">The severity level of the alert.</param>
        public void DisplayAlert(string alertMessage, string severity)
        {
            AppendToFile(_formatter.FormatAlert(alertMessage, severity));
        }

        /// <summary>
        /// Appends the specified content to the file.
        /// If an error occurs during writing, outputs the error message to the console.
        /// </summary>
        /// <param name="content">The content to append to the file.</param>
        private void AppendToFile(string content)
        {
            try
            {
                File.AppendAllText(_filePath, content + "\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to file: {ex.Message}");
            }
        }
    }
}