using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Core.Interfaces;

public interface ISensorDataAnalyzer
{
    string AnalyzerType { get; }
    void Analyze(SensorData data);
    string GetAnalysisResult();
}