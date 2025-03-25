using System;
using System.Collections.Generic;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Patterns.Structural.Bridge.Displays;
using TemperatuurMetingen.Patterns.Structural.Bridge.Formatters;

namespace TemperatuurMetingen.Patterns.Structural.Bridge
{
    public class DisplayManager
    {
        private readonly Dictionary<string, ISensorDataDisplay>
            _displays = new Dictionary<string, ISensorDataDisplay>();

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

        public void AddDisplay(string name, ISensorDataDisplay display)
        {
            if (!_displays.ContainsKey(name))
            {
                _displays[name] = display;
            }
        }

        public void RemoveDisplay(string name)
        {
            if (_displays.ContainsKey(name))
            {
                _displays.Remove(name);
            }
        }

        public void DisplaySensorData(SensorData data, string displayName = "console")
        {
            if (_displays.TryGetValue(displayName, out var display))
            {
                display.DisplaySensorData(data);
            }
        }

        public void DisplaySensorList(IEnumerable<SensorData> dataList, string displayName = "console")
        {
            if (_displays.TryGetValue(displayName, out var display))
            {
                display.DisplaySensorList(dataList);
            }
        }

        public void DisplayStatistics(Dictionary<string, double> statistics, string title,
            string displayName = "console")
        {
            if (_displays.TryGetValue(displayName, out var display))
            {
                display.DisplayStatistics(statistics, title);
            }
        }

        public void DisplayAlert(string alertMessage, string severity, string displayName = "console")
        {
            if (_displays.TryGetValue(displayName, out var display))
            {
                display.DisplayAlert(alertMessage, severity);
            }
        }

        public void BroadcastStatistics(Dictionary<string, double> statistics, string title)
        {
            foreach (var display in _displays.Values)
            {
                display.DisplayStatistics(statistics, title);
            }
        }

        public void BroadcastAlert(string alertMessage, string severity)
        {
            foreach (var display in _displays.Values)
            {
                display.DisplayAlert(alertMessage, severity);
            }
        }
    }
}