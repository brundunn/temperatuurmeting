using System;
using System.Collections.Generic;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Patterns.Structural.Bridge.Displays
{
    public class ConsoleDisplay : ISensorDataDisplay
    {
        private readonly ISensorDataFormatter _formatter;
        
        public ConsoleDisplay(ISensorDataFormatter formatter)
        {
            _formatter = formatter;
        }
        
        public void DisplaySensorData(SensorData data)
        {
            Console.WriteLine(_formatter.FormatSensorData(data));
        }
        
        public void DisplaySensorList(IEnumerable<SensorData> dataList)
        {
            Console.WriteLine(_formatter.FormatSensorList(dataList));
        }
        
        public void DisplayStatistics(Dictionary<string, double> statistics, string title)
        {
            Console.WriteLine($"=== {title} ===");
            Console.WriteLine(_formatter.FormatStatistics(statistics));
            Console.WriteLine("================");
        }
        
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