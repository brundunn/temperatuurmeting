using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Patterns.Concurrency.Actor.Messages
{
    public class SensorDataMessage
    {
        public SensorData Data { get; }
        
        public SensorDataMessage(SensorData data)
        {
            Data = data;
        }
    }
    
    public class AnalyzeDataMessage
    {
        public string SensorType { get; }
        
        public AnalyzeDataMessage(string sensorType)
        {
            SensorType = sensorType;
        }
    }
    
    public class DataAnalysisResult
    {
        public string SensorType { get; }
        public Dictionary<string, double> Stats { get; }
        
        public DataAnalysisResult(string sensorType, Dictionary<string, double> stats)
        {
            SensorType = sensorType;
            Stats = stats;
        }
    }
    
    public class GetStatusMessage { }
    
    public class SystemStatusResult
    {
        public int ProcessedDataPoints { get; }
        public int ActiveSensors { get; }
        
        public SystemStatusResult(int processedDataPoints, int activeSensors)
        {
            ProcessedDataPoints = processedDataPoints;
            ActiveSensors = activeSensors;
        }
    }
}