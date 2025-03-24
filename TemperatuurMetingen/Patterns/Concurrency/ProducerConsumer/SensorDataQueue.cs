using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Patterns.Concurrency.ProducerConsumer
{
    public class SensorDataQueue
    {
        private readonly BlockingCollection<string> _dataQueue;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _consumerTask;
        
        public event EventHandler<string> RawDataReceived;
        public event EventHandler<SensorData> DataProcessed;
        
        public SensorDataQueue(int boundedCapacity = 100)
        {
            _dataQueue = new BlockingCollection<string>(new ConcurrentQueue<string>(), boundedCapacity);
        }
        
        public void Start(Action<string> processAction)
        {
            if (_consumerTask != null && !_consumerTask.IsCompleted)
            {
                throw new InvalidOperationException("Consumer is already running.");
            }
            
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
            
            // Start the consumer task
            _consumerTask = Task.Run(() => ConsumeData(processAction, token), token);
            
            Console.WriteLine("Producer-Consumer queue started.");
        }
        
        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _dataQueue.CompleteAdding();
            
            try
            {
                _consumerTask?.Wait(TimeSpan.FromSeconds(5));
            }
            catch (AggregateException ex) when (ex.InnerException is TaskCanceledException)
            {
                // Task was canceled, which is expected
            }
            
            Console.WriteLine("Producer-Consumer queue stopped.");
        }
        
        public void Produce(string rawData)
        {
            if (_dataQueue.IsAddingCompleted)
            {
                throw new InvalidOperationException("Cannot add to a completed queue.");
            }
            
            _dataQueue.Add(rawData);
            RawDataReceived?.Invoke(this, rawData);
        }
        
        private void ConsumeData(Action<string> processAction, CancellationToken token)
        {
            try
            {
                // Process items until cancellation is requested or queue is marked as complete
                foreach (var rawData in _dataQueue.GetConsumingEnumerable(token))
                {
                    // Process the data
                    try
                    {
                        processAction(rawData);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing data: {ex.Message}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Cancellation was requested, which is expected
            }
        }
    }
}