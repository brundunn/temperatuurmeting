﻿using System;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Utilities;

namespace TemperatuurMetingen.Patterns.Behavioral.Strategy
{
    /// <summary>
    /// Parses sensor data in the standard format.
    /// </summary>
    public class StandardFormatParser : ISensorDataParser
    {
        /// <summary>
        /// Determines if the parser can handle the given raw data.
        /// </summary>
        /// <param name="rawData">The raw sensor data string.</param>
        /// <returns>True if the data starts with "serial:", otherwise false.</returns>
        public bool CanParse(string rawData)
        {
            return rawData.StartsWith("serial:");
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

            // Parse each key-value pair
            foreach (var pair in pairs)
            {
                switch (pair.Key.ToLower())
                {
                    case "serial":
                    case "serialnumber":
                        data.SerialNumber = pair.Value;
                        break;
                    case "temp":
                        data.Temperature = DataParsingHelper.ParseNumeric(pair.Value);
                        break;
                    case "hum":
                        data.Humidity = DataParsingHelper.ParseNumeric(pair.Value);
                        break;
                    case "bat":
                    case "batlevel":
                    case "batterylevel":
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
                    case "manu":
                    case "manufac":
                    case "manufacturer":
                        data.Manufacturer = pair.Value;
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