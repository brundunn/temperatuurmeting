using System.Collections.Generic;

namespace TemperatuurMetingen.Patterns.Creational.Singleton;

    public class SensorTypeManager
    {
        private static SensorTypeManager _instance;
        private static readonly object _lock = new object();
        private readonly Dictionary<string, string> _sensorTypes = new Dictionary<string, string>();
            
        private SensorTypeManager() { }
            
        public static SensorTypeManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SensorTypeManager();
                    }
                    return _instance;
                }
            }
        }
            
        public void RegisterSensorType(string serialNumber, string type)
        {
            lock (_lock)
            {
                if (!_sensorTypes.ContainsKey(serialNumber))
                {
                    _sensorTypes.Add(serialNumber, type);
                }
                else if (_sensorTypes[serialNumber] != type)
                {
                    // Update type if it has changed
                    _sensorTypes[serialNumber] = type;
                }
            }
        }
            
        public string GetSensorType(string serialNumber)
        {
            lock (_lock)
            {
                if (_sensorTypes.ContainsKey(serialNumber))
                {
                    return _sensorTypes[serialNumber];
                }
                return "unknown";
            }
        }
            
        public IReadOnlyDictionary<string, string> GetAllSensorTypes()
        {
            lock (_lock)
            {
                return new Dictionary<string, string>(_sensorTypes);
            }
        }
            
        public int GetSensorCount()
        {
            lock (_lock)
            {
                return _sensorTypes.Count;
            }
        }
    }