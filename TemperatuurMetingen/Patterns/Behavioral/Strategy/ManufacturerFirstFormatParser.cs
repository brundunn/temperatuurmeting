using System;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Utilities;

namespace TemperatuurMetingen.Patterns.Behavioral.Strategy
{
    /// <summary>
    /// Parses sensor data in the manufacturer-first format.
    /// </summary>
    public class ManufacturerFirstFormatParser : ISensorDataParser
    {
        /// <summary>
        /// Determines if the parser can handle the given raw data.
        /// </summary>
        /// <param name="rawData">The raw sensor data string.</param>
        /// <returns>True if the data starts with "manufac:" or "manu:", otherwise false.</returns>
        public bool CanParse(string rawData)
        {
            return rawData.StartsWith("manufac:") || rawData.StartsWith("manu:");
        }

        /// <summary>
        /// Parses the raw sensor data into a <see cref="SensorData"/> object.
        /// </summary>
        /// <param name="rawData">The raw sensor data string.</param>
        /// <returns>A <see cref="SensorData"/> object containing the parsed data.</returns>
        public SensorData Parse(string rawData)
        {
            var data = new SensorData();

            // Split the raw data into key-value pairs
            var pairs = DataParsingHelper.ExtractKeyValuePairs(rawData);

            // Parse each key-value pair - use same logic as StandardFormatParser
            foreach (var pair in pairs)
            {
                switch (pair.Key.ToLower())
                {
                    case "manufac":
                    case "manu":
                    case "manufacturer":
                        data.Manufacturer = pair.Value;
                        break;
                    case "serialnumber":
                    case "serial":
                        data.SerialNumber = pair.Value;
                        break;
                    case "temp":
                        data.Temperature = DataParsingHelper.ParseNumeric(pair.Value);
                        break;
                    case "hum":
                        data.Humidity = DataParsingHelper.ParseNumeric(pair.Value);
                        break;
                    case "batlevel":
                    case "batterylevel":
                    case "bat":
                        data.BatteryLevel = DataParsingHelper.ParseNumeric(pair.Value);
                        break;
                    case "batmax":
                        data.BatteryMax = DataParsingHelper.ParseNumeric(pair.Value);
                        break;
                    case "batmin":
                        data.BatteryMin = DataParsingHelper.ParseNumeric(pair.Value);
                        break;
                    case "state":
                        data.State = pair.Value;
                        break;
                    case "type":
                        data.Type = pair.Value;
                        break;
                    case "error":
                        data.Error = pair.Value;
                        break;
                    case "v":
                    case "v2":
                    case "v3":
                        data.Voltage = DataParsingHelper.ParseNumeric(pair.Value);
                        break;
                }
            }

            // Standardize the data
            DataParsingHelper.StandardizeData(data);

            return data;
        }
    }
}