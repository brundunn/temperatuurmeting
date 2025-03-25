using System;
using System.Collections.Generic;
using System.IO;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Patterns.Structural.Bridge.Displays
{
    public class FileDisplay : ISensorDataDisplay
    {
        private readonly ISensorDataFormatter _formatter;
        private readonly string _filePath;
        
        public FileDisplay(ISensorDataFormatter formatter, string filePath)
        {
            _formatter = formatter;
            _filePath = filePath;
            
            // Create or clear the file
            File.WriteAllText(_filePath, $"Sensor Monitoring Log - {DateTime.Now}\n\n");
        }
        
        public void DisplaySensorData(SensorData data)
        {
            AppendToFile(_formatter.FormatSensorData(data));
        }
        
        public void DisplaySensorList(IEnumerable<SensorData> dataList)
        {
            AppendToFile(_formatter.FormatSensorList(dataList));
        }
        
        public void DisplayStatistics(Dictionary<string, double> statistics, string title)
        {
            AppendToFile($"=== {title} ===\n");
            AppendToFile(_formatter.FormatStatistics(statistics));
            AppendToFile("================\n");
        }
        
        public void DisplayAlert(string alertMessage, string severity)
        {
            AppendToFile(_formatter.FormatAlert(alertMessage, severity));
        }
        
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