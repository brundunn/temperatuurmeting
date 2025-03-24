using System;
using System.IO;
using System.Threading.Tasks;
using TemperatuurMetingen.Patterns.Concurrency.ProducerConsumer;
using TemperatuurMetingen.Patterns.Structural.Facade;

namespace TemperatuurMetingen.Services
{
    public class StreamingDataProcessor
    {
        private readonly SensorSystemFacade _sensorSystem;
        private readonly SensorDataQueue _dataQueue;
        
        public StreamingDataProcessor(SensorSystemFacade sensorSystem)
        {
            _sensorSystem = sensorSystem;
            _dataQueue = new SensorDataQueue();
            
            // Set up event handlers
            _dataQueue.RawDataReceived += (sender, data) => 
            {
                Console.WriteLine($"Data received: {data.Substring(0, Math.Min(20, data.Length))}...");
            };
        }
        
        public void Start()
        {
            // Start the consumer that processes data
            _dataQueue.Start(data => _sensorSystem.ProcessSensorData(data));
            Console.WriteLine("Streaming data processor started.");
        }
        
        public void Stop()
        {
            _dataQueue.Stop();
            Console.WriteLine("Streaming data processor stopped.");
        }
        
        public void AddData(string rawData)
        {
            _dataQueue.Produce(rawData);
        }
        
        public async Task ProcessFileStreamAsync(string filePath, int delayMs = 100)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        // Add the data to the queue
                        _dataQueue.Produce(line);
                        
                        // Simulate real-time data by adding a delay
                        await Task.Delay(delayMs);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
            }
        }
    }
}