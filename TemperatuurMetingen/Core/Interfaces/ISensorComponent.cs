using System.Collections.Generic;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Core.Interfaces
{
    public interface ISensorComponent : IVisitable
    {
        string Name { get; }
        string Type { get; }
        void AddData(SensorData data);
        Dictionary<string, double> GetAggregatedData();
        void DisplayInfo(int depth = 0);
        int GetSensorCount();
    }
}