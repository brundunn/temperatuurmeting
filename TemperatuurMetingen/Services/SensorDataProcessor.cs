using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Services
{
    /// <summary>
    /// Processes raw sensor data using configurable parser implementations.
    /// Serves as a simple implementation of the Strategy pattern for data parsing.
    /// </summary>
    public class SensorDataProcessor
    {
        /// <summary>
        /// The parser strategy used to interpret raw sensor data.
        /// </summary>
        private ISensorDataParser _parser;

        /// <summary>
        /// Sets the parser strategy to use for processing sensor data.
        /// </summary>
        /// <param name="parser">The parser implementation to use.</param>
        public void SetParser(ISensorDataParser parser)
        {
            _parser = parser;
        }

        /// <summary>
        /// Processes raw sensor data using the currently set parser.
        /// </summary>
        /// <param name="rawData">The raw string data from a sensor to process.</param>
        /// <returns>A structured SensorData object containing the parsed information.</returns>
        /// <exception cref="System.NullReferenceException">Thrown when no parser has been set.</exception>
        public SensorData ProcessData(string rawData)
        {
            return _parser.Parse(rawData);
        }
    }
}