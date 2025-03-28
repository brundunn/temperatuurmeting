using System.Collections.Generic;

namespace TemperatuurMetingen.Patterns.Creational.Singleton;

    /// <summary>
    /// Manages sensor type information using the Singleton pattern to ensure a single instance throughout the application.
    /// Provides thread-safe access to sensor type registration and retrieval operations.
    /// </summary>
    public class SensorTypeManager
    {
        /// <summary>
        /// The single instance of the SensorTypeManager class.
        /// </summary>
        private static SensorTypeManager _instance;
        
        /// <summary>
        /// Lock object for thread synchronization.
        /// </summary>
        private static readonly object _lock = new object();
        
        /// <summary>
        /// Dictionary storing sensor serial numbers mapped to their corresponding types.
        /// </summary>
        private readonly Dictionary<string, string> _sensorTypes = new Dictionary<string, string>();

        /// <summary>
        /// Private constructor to prevent external instantiation.
        /// </summary>
        private SensorTypeManager() { }

        /// <summary>
        /// Gets the singleton instance of the SensorTypeManager.
        /// Creates the instance if it doesn't exist yet.
        /// </summary>
        /// <remarks>
        /// Thread-safe implementation of the Singleton pattern using double-check locking.
        /// </remarks>
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

        /// <summary>
        /// Registers or updates a sensor's type based on its serial number.
        /// </summary>
        /// <param name="serialNumber">The unique serial number of the sensor.</param>
        /// <param name="type">The type of the sensor.</param>
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

        /// <summary>
        /// Retrieves the type of a sensor based on its serial number.
        /// </summary>
        /// <param name="serialNumber">The unique serial number of the sensor.</param>
        /// <returns>The type of the sensor, or "unknown" if the serial number is not registered.</returns>
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

        /// <summary>
        /// Gets a read-only copy of all registered sensor types.
        /// </summary>
        /// <returns>A dictionary containing all sensor serial numbers and their types.</returns>
        public IReadOnlyDictionary<string, string> GetAllSensorTypes()
        {
            lock (_lock)
            {
                return new Dictionary<string, string>(_sensorTypes);
            }
        }

        /// <summary>
        /// Gets the total number of registered sensors.
        /// </summary>
        /// <returns>The count of registered sensors.</returns>
        public int GetSensorCount()
        {
            lock (_lock)
            {
                return _sensorTypes.Count;
            }
        }
    }