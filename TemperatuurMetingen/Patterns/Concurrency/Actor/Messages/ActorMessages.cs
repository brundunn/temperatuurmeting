using System.Collections.Generic;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Patterns.Concurrency.Actor.Messages
{
    /// <summary>
    /// Message containing sensor data to be processed by actors.
    /// </summary>
    public class SensorDataMessage
    {
        /// <summary>
        /// Gets the sensor data payload.
        /// </summary>
        public SensorData Data { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SensorDataMessage"/> class.
        /// </summary>
        /// <param name="data">The sensor data to be processed.</param>
        public SensorDataMessage(SensorData data)
        {
            Data = data;
        }
    }

    /// <summary>
    /// Message requesting analysis on data from a specific sensor type.
    /// </summary>
    public class AnalyzeDataMessage
    {
        /// <summary>
        /// Gets the type of sensor to analyze.
        /// </summary>
        public string SensorType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyzeDataMessage"/> class.
        /// </summary>
        /// <param name="sensorType">The type of sensor to analyze.</param>
        public AnalyzeDataMessage(string sensorType)
        {
            SensorType = sensorType;
        }
    }

    /// <summary>
    /// Message containing the results of a sensor data analysis.
    /// </summary>
    public class DataAnalysisResult
    {
        /// <summary>
        /// Gets the sensor type that was analyzed.
        /// </summary>
        public string SensorType { get; }
        
        /// <summary>
        /// Gets the statistical results of the analysis.
        /// </summary>
        public Dictionary<string, double> Stats { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAnalysisResult"/> class.
        /// </summary>
        /// <param name="sensorType">The type of sensor that was analyzed.</param>
        /// <param name="stats">The dictionary of calculated statistics.</param>
        public DataAnalysisResult(string sensorType, Dictionary<string, double> stats)
        {
            SensorType = sensorType;
            Stats = stats;
        }
    }

    /// <summary>
    /// Message requesting the current status of the system.
    /// </summary>
    public class GetStatusMessage { }

    /// <summary>
    /// Message containing system status information.
    /// </summary>
    public class SystemStatusResult
    {
        /// <summary>
        /// Gets the total number of data points processed by the system.
        /// </summary>
        public int ProcessedDataPoints { get; }
        
        /// <summary>
        /// Gets the number of active sensors in the system.
        /// </summary>
        public int ActiveSensors { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemStatusResult"/> class.
        /// </summary>
        /// <param name="processedDataPoints">The number of data points processed.</param>
        /// <param name="activeSensors">The number of active sensors.</param>
        public SystemStatusResult(int processedDataPoints, int activeSensors)
        {
            ProcessedDataPoints = processedDataPoints;
            ActiveSensors = activeSensors;
        }
    }
}