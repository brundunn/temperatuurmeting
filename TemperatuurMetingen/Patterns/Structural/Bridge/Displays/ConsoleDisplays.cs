using System;
using System.Collections.Generic;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Patterns.Structural.Bridge.Displays
{
    /// <summary>
    /// A concrete implementation of the ISensorDataDisplay interface that renders sensor data to the console.
    /// This class is part of the Bridge pattern, where it acts as a Concrete Implementor.
    /// It uses an injected formatter to determine how the data should be formatted before display.
    /// </summary>
    public class ConsoleDisplay : ISensorDataDisplay
    {
        private readonly ISensorDataFormatter _formatter;

        /// <summary>
        /// Initializes a new instance of the ConsoleDisplay class.
        /// </summary>
        /// <param name="formatter">The formatter that will be used to format sensor data before display.</param>
        public ConsoleDisplay(ISensorDataFormatter formatter)
        {
            _formatter = formatter;
        }

        /// <summary>
        /// Displays a single sensor data point to the console.
        /// </summary>
        /// <param name="data">The sensor data to display.</param>
        public void DisplaySensorData(SensorData data)
        {
            Console.WriteLine(_formatter.FormatSensorData(data));
        }

        /// <summary>
        /// Displays a collection of sensor data points to the console.
        /// </summary>
        /// <param name="dataList">The collection of sensor data to display.</param>
        public void DisplaySensorList(IEnumerable<SensorData> dataList)
        {
            Console.WriteLine(_formatter.FormatSensorList(dataList));
        }

        /// <summary>
        /// Displays statistics with a title to the console.
        /// </summary>
        /// <param name="statistics">A dictionary containing statistic names and their values.</param>
        /// <param name="title">The title to display above the statistics.</param>
        public void DisplayStatistics(Dictionary<string, double> statistics, string title)
        {
            Console.WriteLine($"=== {title} ===");
            Console.WriteLine(_formatter.FormatStatistics(statistics));
            Console.WriteLine("================");
        }

        /// <summary>
        /// Displays an alert message to the console with color-coded severity.
        /// </summary>
        /// <param name="alertMessage">The alert message to display.</param>
        /// <param name="severity">The severity level which determines the text color.
        /// Supported values: "critical" (red), "warning" (yellow), "info" (cyan).</param>
        public void DisplayAlert(string alertMessage, string severity)
        {
            var originalColor = Console.ForegroundColor;

            switch (severity.ToLower())
            {
                case "critical":
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case "warning":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case "info":
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
            }

            Console.WriteLine(_formatter.FormatAlert(alertMessage, severity));
            Console.ForegroundColor = originalColor;
        }
    }
}