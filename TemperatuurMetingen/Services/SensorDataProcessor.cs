using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Services
{
    public class SensorDataProcessor
    {
        private ISensorDataParser _parser;
        
        public void SetParser(ISensorDataParser parser)
        {
            _parser = parser;
        }
        
        public SensorData ProcessData(string rawData)
        {
            return _parser.Parse(rawData);
        }
    }
}