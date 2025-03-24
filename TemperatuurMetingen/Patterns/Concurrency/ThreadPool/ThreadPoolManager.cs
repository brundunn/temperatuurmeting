using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TemperatuurMetingen.Patterns.Concurrency.ThreadPool
{
    public class ThreadPoolManager
    {
        private static readonly Lazy<ThreadPoolManager> _instance = 
            new Lazy<ThreadPoolManager>(() => new ThreadPoolManager());
        
        public static ThreadPoolManager Instance => _instance.Value;
        
        private readonly int _maxDegreeOfParallelism;
        private readonly SemaphoreSlim _semaphore;
        
        private ThreadPoolManager()
        {
            // Use number of processor cores as default parallelism
            _maxDegreeOfParallelism = Environment.ProcessorCount;
            _semaphore = new SemaphoreSlim(_maxDegreeOfParallelism, _maxDegreeOfParallelism);
            
            Console.WriteLine($"Thread Pool initialized with {_maxDegreeOfParallelism} worker threads.");
        }
        
        public async Task<TResult> QueueTaskAsync<TResult>(Func<TResult> function)
        {
            try
            {
                // Wait until a thread is available
                await _semaphore.WaitAsync();
                
                // Execute the task using the thread pool
                return await Task.Run(function);
            }
            finally
            {
                // Release the thread back to the pool
                _semaphore.Release();
            }
        }
        
        public async Task QueueTaskAsync(Action action)
        {
            try
            {
                // Wait until a thread is available
                await _semaphore.WaitAsync();
                
                // Execute the task using the thread pool
                await Task.Run(action);
            }
            finally
            {
                // Release the thread back to the pool
                _semaphore.Release();
            }
        }
        
        public async Task ProcessBatchAsync<T>(IEnumerable<T> items, Func<T, Task> processFunction)
        {
            var tasks = new List<Task>();
            
            foreach (var item in items)
            {
                tasks.Add(QueueTaskAsync(async () => await processFunction(item)));
            }
            
            await Task.WhenAll(tasks);
        }
    }
}