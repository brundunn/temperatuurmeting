using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Utilities
{
    /// <summary>
    /// Provides utility methods for parsing and standardizing sensor data.
    /// Contains functions to extract data from raw inputs and convert values to standard formats.
    /// </summary>
    public static class DataParsingHelper
    {
        /// <summary>
        /// Extracts key-value pairs from a raw data string using regular expressions.
        /// </summary>
        /// <param name="rawData">The raw data string containing key:value pairs.</param>
        /// <returns>A dictionary containing the extracted key-value pairs with case-insensitive keys.</returns>
        /// <remarks>
        /// The method expects data in format "key1:value1key2:value2" without requiring spaces between pairs.
        /// If duplicate keys are found, only the first occurrence is preserved.
        /// </remarks>
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

        /// <summary>
        /// Parses a string value to a double.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed double value, or 0 if parsing fails.</returns>
        public static double ParseNumeric(string value)
        {
            if (double.TryParse(value, out double result))
            {
                return result;
            }
            return 0;
        }

        /// <summary>
        /// Standardizes sensor data by converting values to consistent units and formats.
        /// </summary>
        /// <param name="data">The sensor data object to standardize.</param>
        /// <remarks>
        /// Performs the following standardizations:
        /// - Converts temperature from 1/100 degrees to degrees Celsius
        /// - Converts humidity to percentage format
        /// - Normalizes state values to lowercase
        /// - Generates a placeholder serial number if missing but manufacturer is available
        /// </remarks>
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