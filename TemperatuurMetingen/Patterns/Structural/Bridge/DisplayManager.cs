using System;
using System.Collections.Generic;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Patterns.Structural.Bridge.Displays;
using TemperatuurMetingen.Patterns.Structural.Bridge.Formatters;

namespace TemperatuurMetingen.Patterns.Structural.Bridge
{
    /// <summary>
    /// Manages multiple display implementations as part of the Bridge design pattern.
    /// Acts as the Abstraction in the Bridge pattern, delegating display operations to the appropriate implementors.
    /// Allows clients to work with different combinations of displays and formatters.
    /// </summary>
    public class DisplayManager
    {
        /// <summary>
        /// A collection of named display instances that can be used to output sensor data.
        /// Keys represent display names and values are the display implementations.
        /// </summary>
        private readonly Dictionary<string, ISensorDataDisplay>
            _displays = new Dictionary<string, ISensorDataDisplay>();

        /// <summary>
        /// Initializes a new instance of the DisplayManager class with default displays:
        /// - console: Text formatted console display
        /// - json-console: JSON formatted console display
        /// - text-file: Text formatted file display
        /// - json-file: JSON formatted file display
        /// </summary>
        public DisplayManager()
        {
            // Create default displays with different formatters
            var textFormatter = new TextFormatter();
            var jsonFormatter = new JsonFormatter();

            _displays["console"] = new ConsoleDisplay(textFormatter);
            _displays["json-console"] = new ConsoleDisplay(jsonFormatter);
            _displays["text-file"] = new FileDisplay(textFormatter, "sensor_log.txt");
            _displays["json-file"] = new FileDisplay(jsonFormatter, "sensor_log.json");
        }

        /// <summary>
        /// Adds a new display to the manager with the specified name.
        /// If a display with the name already exists, the operation is ignored.
        /// </summary>
        /// <param name="name">The name to identify the display.</param>
        /// <param name="display">The display implementation to add.</param>
        public void AddDisplay(string name, ISensorDataDisplay display)
        {
            if (!_displays.ContainsKey(name))
            {
                _displays[name] = display;
            }
        }

        /// <summary>
        /// Removes a display with the specified name from the manager.
        /// If no display with the name exists, the operation is ignored.
        /// </summary>
        /// <param name="name">The name of the display to remove.</param>
        public void RemoveDisplay(string name)
        {
            if (_displays.ContainsKey(name))
            {
                _displays.Remove(name);
            }
        }

        /// <summary>
        /// Displays a single sensor data object using the specified display.
        /// If the specified display doesn't exist, the operation is ignored.
        /// </summary>
        /// <param name="data">The sensor data to display.</param>
        /// <param name="displayName">The name of the display to use. Defaults to "console".</param>
        public void DisplaySensorData(SensorData data, string displayName = "console")
        {
            if (_displays.TryGetValue(displayName, out var display))
            {
                display.DisplaySensorData(data);
            }
        }

        /// <summary>
        /// Displays a collection of sensor data objects using the specified display.
        /// If the specified display doesn't exist, the operation is ignored.
        /// </summary>
        /// <param name="dataList">The collection of sensor data to display.</param>
        /// <param name="displayName">The name of the display to use. Defaults to "console".</param>
        public void DisplaySensorList(IEnumerable<SensorData> dataList, string displayName = "console")
        {
            if (_displays.TryGetValue(displayName, out var display))
            {
                display.DisplaySensorList(dataList);
            }
        }

        /// <summary>
        /// Displays statistics with a title using the specified display.
        /// If the specified display doesn't exist, the operation is ignored.
        /// </summary>
        /// <param name="statistics">A dictionary containing statistic names and their values.</param>
        /// <param name="title">The title to display above the statistics.</param>
        /// <param name="displayName">The name of the display to use. Defaults to "console".</param>
        public void DisplayStatistics(Dictionary<string, double> statistics, string title,
            string displayName = "console")
        {
            if (_displays.TryGetValue(displayName, out var display))
            {
                display.DisplayStatistics(statistics, title);
            }
        }

        /// <summary>
        /// Displays an alert message with severity using the specified display.
        /// If the specified display doesn't exist, the operation is ignored.
        /// </summary>
        /// <param name="alertMessage">The alert message to display.</param>
        /// <param name="severity">The severity level of the alert.</param>
        /// <param name="displayName">The name of the display to use. Defaults to "console".</param>
        public void DisplayAlert(string alertMessage, string severity, string displayName = "console")
        {
            if (_displays.TryGetValue(displayName, out var display))
            {
                display.DisplayAlert(alertMessage, severity);
            }
        }

        /// <summary>
        /// Broadcasts statistics to all registered displays.
        /// </summary>
        /// <param name="statistics">A dictionary containing statistic names and their values.</param>
        /// <param name="title">The title to display above the statistics.</param>
        public void BroadcastStatistics(Dictionary<string, double> statistics, string title)
        {
            foreach (var display in _displays.Values)
            {
                display.DisplayStatistics(statistics, title);
            }
        }

        /// <summary>
        /// Broadcasts an alert message to all registered displays.
        /// </summary>
        /// <param name="alertMessage">The alert message to broadcast.</param>
        /// <param name="severity">The severity level of the alert.</param>
        public void BroadcastAlert(string alertMessage, string severity)
        {
            foreach (var display in _displays.Values)
            {
                display.DisplayAlert(alertMessage, severity);
            }
        }
    }
}