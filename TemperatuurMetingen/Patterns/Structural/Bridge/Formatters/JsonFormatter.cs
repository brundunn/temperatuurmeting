using System;
using System.Collections.Generic;
using System.Text.Json;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Patterns.Structural.Bridge.Formatters
{
    public class JsonFormatter : ISensorDataFormatter
    {
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        
        public string FormatSensorData(SensorData data)
        {
            if (data == null)
                return "{}";
                
            return JsonSerializer.Serialize(data, _options);
        }
        
        public string FormatSensorList(IEnumerable<SensorData> dataList)
        {
            return JsonSerializer.Serialize(dataList, _options);
        }
        
        public string FormatStatistics(Dictionary<string, double> statistics)
        {
            return JsonSerializer.Serialize(statistics, _options);
        }
        
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