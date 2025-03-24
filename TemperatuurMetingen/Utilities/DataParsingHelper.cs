using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Utilities
{
    public static class DataParsingHelper
    {
        public static Dictionary<string, string> ExtractKeyValuePairs(string rawData)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            
            // This pattern matches key:value pairs without requiring spaces between them
            var pattern = @"([a-zA-Z_]+):([^:]+?)(?=(?:[a-zA-Z_]+:|$))";
            var matches = Regex.Matches(rawData, pattern);
            
            foreach (Match match in matches)
            {
                if (match.Groups.Count == 3)
                {
                    string key = match.Groups[1].Value.Trim().ToLower();
                    string value = match.Groups[2].Value.Trim();
                    
                    // If key already exists, we'll take the first occurrence
                    if (!result.ContainsKey(key))
                    {
                        result.Add(key, value);
                    }
                }
            }
            
            return result;
        }
        
        public static double ParseNumeric(string value)
        {
            if (double.TryParse(value, out double result))
            {
                return result;
            }
            return 0;
        }
        
        // Helper method to standardize the formats
        public static void StandardizeData(SensorData data)
        {
            // Convert temperature from device units to degrees Celsius
            // Based on the sample data, it appears temperature values are in 1/100 degrees
            if (data.Temperature > 100) // Raw format likely
            {
                data.Temperature = Math.Round(data.Temperature / 100.0, 2);
            }
            
            // Convert humidity to percentage (if needed)
            if (data.Humidity > 100) // Raw format likely
            {
                data.Humidity = Math.Round(data.Humidity / 10.0, 2);
            }
            
            // Normalize state values
            if (!string.IsNullOrEmpty(data.State))
            {
                data.State = data.State.ToLower();
            }
            
            // Handle edge cases
            if (string.IsNullOrEmpty(data.SerialNumber) && !string.IsNullOrEmpty(data.Manufacturer))
            {
                data.SerialNumber = "Unknown-" + Guid.NewGuid().ToString().Substring(0, 8);
            }
        }
    }
}