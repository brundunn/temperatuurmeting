# Temperature Monitoring System Documentation

For launch instructions and dependencies, please see Requirements-Dependencies.md

## Table of Contents
1. [Project Overview](#project-overview)
2. [Design Patterns Implemented](#design-patterns-implemented)
3. [Pattern Implementation Details](#pattern-implementation-details)
   - [Singleton Pattern](#1-singleton-pattern)
   - [Factory Method Pattern](#2-factory-method-pattern)
   - [Strategy Pattern](#3-strategy-pattern)
   - [Observer Pattern](#4-observer-pattern)
   - [Composite Pattern](#5-composite-pattern)
   - [Facade Pattern](#6-facade-pattern)
   - [Visitor Pattern](#7-visitor-pattern)
   - [Thread Pool Pattern](#8-thread-pool-pattern)
   - [Producer-Consumer Pattern](#9-producer-consumer-pattern)
   - [Actor Model Pattern](#10-actor-model-pattern)
   - [Bridge Pattern](#11-bridge-pattern)
4. [System Architecture](#system-architecture)
5. [Code Examples](#code-examples)
6. [Testing and Validation](#testing-and-validation)
7. [Extensibility and Maintenance](#extensibility-and-maintenance)
8. [Reflection on Alternatives](#reflection-on-alternatives)
9. [Conclusion](#conclusion)

## Project Overview

The Temperature Monitoring System is designed to process, analyze, and visualize sensor data from various temperature and humidity sensors. The system reads sensor data from text files (simulating real-time feeds from a Raspberry Pi Pico), processes the data through various components, and provides analysis, visualization, and alerts based on sensor readings.

The system showcases multiple design patterns to create a flexible, maintainable, and scalable architecture that can handle different sensor types, data formats, and output methods.

## Design Patterns Implemented

This project implements a comprehensive set of design patterns across the following categories:

### Creational Patterns
1. **Singleton Pattern** - Ensures a single instance of the SensorTypeManager
2. **Factory Method Pattern** - Creates different analyzer types for sensor data

### Structural Patterns
1. **Composite Pattern** - Organizes sensors into hierarchical groups
2. **Facade Pattern** - Simplifies the complex system through a unified interface
3. **Bridge Pattern** - Separates data formatting from display mechanisms

### Behavioral Patterns
1. **Strategy Pattern** - Provides different algorithms for parsing sensor data
2. **Observer Pattern** - Notifies components when new sensor data is received
3. **Visitor Pattern** - Adds operations to sensor components without modifying them

### Concurrency Patterns
1. **Thread Pool Pattern** - Manages worker threads for processing sensor data
2. **Producer-Consumer Pattern** - Separates data acquisition from processing
3. **Actor Model Pattern** - Isolates components that process messages independently

## Pattern Implementation Details

### 1. Singleton Pattern

#### Context and Problem
In our sensor monitoring system, we need to track sensor types across different components. Without a centralized registry, each component would need to maintain its own list of sensor types, leading to data inconsistency and redundancy.

#### Implementation
```csharp
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
}
```

#### Benefits
- Ensures only one instance of the sensor type registry exists
- Provides global access to sensor type information
- Thread-safe implementation with double-check locking
- Centralizes the management of sensor type information

#### Potential Drawbacks
- Global state can make testing more difficult
- Introduces a dependency to all classes that use it

### 2. Factory Method Pattern

#### Context and Problem
Our system needs to analyze different types of sensor data (temperature, humidity, battery) with specialized algorithms. We need a way to create different analyzer objects without specifying their concrete classes, allowing for future expansion to new sensor types.

#### Implementation
```csharp
// Abstract product
public interface ISensorDataAnalyzer
{
    string AnalyzerType { get; }
    void Analyze(SensorData data);
    string GetAnalysisResult();
}

// Concrete products
public class TemperatureAnalyzer : ISensorDataAnalyzer
{
    private readonly List<double> _temperatures = new List<double>();
    private readonly double _warningThreshold;
    private readonly double _criticalThreshold;
    
    public string AnalyzerType => "Temperature";
    
    public TemperatureAnalyzer(double warningThreshold = 25.0, double criticalThreshold = 30.0)
    {
        _warningThreshold = warningThreshold;
        _criticalThreshold = criticalThreshold;
    }
    
    public void Analyze(SensorData data)
    {
        if (data.Temperature > 0)
        {
            _temperatures.Add(data.Temperature);
        }
    }
    
    public string GetAnalysisResult()
    {
        // Analysis logic here
    }
}

// Abstract factory
public abstract class AnalyzerFactory
{
    public abstract ISensorDataAnalyzer CreateAnalyzer();
}

// Concrete factories
public class TemperatureAnalyzerFactory : AnalyzerFactory
{
    private readonly double _warningThreshold;
    private readonly double _criticalThreshold;
    
    public TemperatureAnalyzerFactory(double warningThreshold = 25.0, double criticalThreshold = 30.0)
    {
        _warningThreshold = warningThreshold;
        _criticalThreshold = criticalThreshold;
    }
    
    public override ISensorDataAnalyzer CreateAnalyzer()
    {
        return new TemperatureAnalyzer(_warningThreshold, _criticalThreshold);
    }
}
```

#### Benefits
- Allows creation of different analyzer types without specifying concrete classes
- Encapsulates object creation logic
- Enables configuration of analyzers through factory parameters
- Easy to add new analyzer types by creating new factories

#### Potential Drawbacks
- Introduces additional classes for each product type
- Can add complexity for simple object creation

### 3. Strategy Pattern

#### Context and Problem
The sensor data comes in different formats, with varying field names and structures. We need a flexible way to parse different data formats without creating a complex conditional parsing logic that would be difficult to maintain and extend.

#### Implementation
```csharp
// Strategy interface
public interface ISensorDataParser
{
    bool CanParse(string rawData);
    SensorData Parse(string rawData);
}

// Concrete strategies
public class StandardFormatParser : ISensorDataParser
{
    public bool CanParse(string rawData)
    {
        return rawData.StartsWith("serial:");
    }

    public SensorData Parse(string rawData)
    {
        var data = new SensorData();
        
        // Parse standard format data
        var pairs = DataParsingHelper.ExtractKeyValuePairs(rawData);
        
        // Parsing logic for standard format
        foreach (var pair in pairs)
        {
            // Map fields to SensorData properties
        }
        
        // Standardize the data
        DataParsingHelper.StandardizeData(data);
        
        return data;
    }
}

public class ManufacturerFirstFormatParser : ISensorDataParser
{
    public bool CanParse(string rawData)
    {
        return rawData.StartsWith("manufac:") || rawData.StartsWith("manu:");
    }

    public SensorData Parse(string rawData)
    {
        // Different parsing logic here
    }
}

// Context class
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
```

#### Benefits
- Encapsulates different parsing algorithms
- Makes it easy to add new parsing strategies
- Eliminates complex conditional logic
- Allows runtime selection of parsing strategies based on data format

#### Potential Drawbacks
- Increases the number of classes in the system
- Clients need to know which strategy to select

### 4. Observer Pattern

#### Context and Problem
When new sensor data is received, multiple components (monitoring displays, alert systems, data loggers) need to be notified and take appropriate actions. We need a way to notify these components without creating tight coupling between them and the data source.

#### Implementation
```csharp
// Observer interface
public interface ISensorDataObserver
{
    void Update(SensorData data);
}

// Subject class
public class SensorDataSubject
{
    private readonly List<ISensorDataObserver> _observers = new List<ISensorDataObserver>();
    
    public void Attach(ISensorDataObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }
    
    public void Detach(ISensorDataObserver observer)
    {
        _observers.Remove(observer);
    }
    
    public void Notify(SensorData data)
    {
        foreach (var observer in _observers)
        {
            observer.Update(data);
        }
    }
}

// Concrete observers
public class TemperatureMonitor : ISensorDataObserver
{
    private readonly double _warningThreshold;
    private readonly double _criticalThreshold;
    
    public TemperatureMonitor(double warningThreshold = 25.0, double criticalThreshold = 30.0)
    {
        _warningThreshold = warningThreshold;
        _criticalThreshold = criticalThreshold;
    }
    
    public void Update(SensorData data)
    {
        if (data.Type != "temp")
        {
            return; // Only process temperature sensors
        }
        
        Console.WriteLine($"Temperature reading: {data.Temperature}°C from sensor {data.SerialNumber}");
        
        if (data.Temperature > _criticalThreshold)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"CRITICAL: Temperature exceeds {_criticalThreshold}°C!");
            Console.ResetColor();
        }
        else if (data.Temperature > _warningThreshold)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"WARNING: Temperature exceeds {_warningThreshold}°C!");
            Console.ResetColor();
        }
    }
}

public class BatteryMonitor : ISensorDataObserver
{
    // Similar implementation for battery monitoring
}
```

#### Benefits
- Loose coupling between data source and listeners
- Support for broadcast communication
- Easy to add or remove observers at runtime
- Components can be developed and tested independently

#### Potential Drawbacks
- No guarantee of the order in which observers are notified
- Potential for memory leaks if observers aren't properly detached
- Observers might receive updates they don't need

### 5. Composite Pattern

#### Context and Problem
Sensors need to be organized in hierarchical groups (by type, manufacturer, location) while allowing operations to be performed on both individual sensors and groups. We need a way to treat individual sensors and groups of sensors uniformly.

#### Implementation
```csharp
// Component interface
public interface ISensorComponent
{
    string Name { get; }
    string Type { get; }
    void AddData(SensorData data);
    Dictionary<string, double> GetAggregatedData();
    void DisplayInfo(int depth = 0);
    int GetSensorCount();
}

// Leaf class
public class SensorLeaf : ISensorComponent
{
    private readonly string _serialNumber;
    private readonly List<SensorData> _dataPoints = new List<SensorData>();
    
    public string Name { get; }
    public string Type { get; private set; }
    
    public SensorLeaf(string serialNumber, string name = null)
    {
        _serialNumber = serialNumber;
        Name = name ?? $"Sensor-{serialNumber}";
        Type = "Unknown";
    }
    
    public void AddData(SensorData data)
    {
        if (data.SerialNumber == _serialNumber)
        {
            _dataPoints.Add(data);
            
            // Update sensor type if available
            if (!string.IsNullOrEmpty(data.Type))
            {
                Type = data.Type;
            }
        }
    }
    
    public Dictionary<string, double> GetAggregatedData()
    {
        // Calculate aggregated statistics
    }
    
    public void DisplayInfo(int depth = 0)
    {
        // Display sensor information
    }
    
    public int GetSensorCount()
    {
        return 1;
    }
}

// Composite class
public class SensorGroup : ISensorComponent
{
    private readonly List<ISensorComponent> _components = new List<ISensorComponent>();
    
    public string Name { get; }
    public string Type { get; }
    
    public SensorGroup(string name, string type = "Group")
    {
        Name = name;
        Type = type;
    }
    
    public void AddComponent(ISensorComponent component)
    {
        if (!_components.Contains(component))
        {
            _components.Add(component);
        }
    }
    
    public void RemoveComponent(ISensorComponent component)
    {
        _components.Remove(component);
    }
    
    public void AddData(SensorData data)
    {
        // Forward data to all children
        foreach (var component in _components)
        {
            component.AddData(data);
        }
    }
    
    public Dictionary<string, double> GetAggregatedData()
    {
        // Aggregate data from all children
    }
    
    public void DisplayInfo(int depth = 0)
    {
        // Display group and child information
    }
    
    public int GetSensorCount()
    {
        return _components.Sum(c => c.GetSensorCount());
    }
}
```

#### Benefits
- Treats individual objects and compositions uniformly
- Makes it easy to add new components types
- Simplifies client code by allowing it to treat individual and composite objects the same way
- Creates hierarchical structures that reflect real-world sensor organization

#### Potential Drawbacks
- Can make design overly general
- Might be difficult to restrict what can be added to composites
- Operations might not make sense for all components

### 6. Facade Pattern

#### Context and Problem
The temperature monitoring system consists of many subsystems (parsers, analyzers, observers, data storage). Client code needs to interact with these subsystems, but doing so directly would create complex, tightly coupled code. We need a simplified interface to the complex system.

#### Implementation
```csharp
public class SensorSystemFacade : IDisposable
{
    private readonly SensorDataProcessor _processor;
    private readonly SensorDataSubject _subject;
    private readonly SensorTypeManager _typeManager;
    private readonly List<ISensorDataParser> _parsers;
    private readonly AnalyzerManager _analyzerManager;
    private readonly SensorCompositeManager _compositeManager;
    private readonly ActorSystemManager _actorSystem;
    private readonly DisplayManager _displayManager;
    
    public SensorSystemFacade()
    {
        // Initialize all subsystems
        _processor = new SensorDataProcessor();
        _subject = new SensorDataSubject();
        _typeManager = SensorTypeManager.Instance;
        _analyzerManager = new AnalyzerManager();
        _compositeManager = new SensorCompositeManager();
        
        // Initialize parsers
        _parsers = new List<ISensorDataParser>
        {
            new StandardFormatParser(),
            new ManufacturerFirstFormatParser()
        };
        
        // Attach observers
        _subject.Attach(new TemperatureMonitor());
        _subject.Attach(new BatteryMonitor());
        
        // Initialize analyzers
        _analyzerManager.RegisterAnalyzer("temp", new TemperatureAnalyzerFactory());
        _analyzerManager.RegisterAnalyzer("humidity", new HumidityAnalyzerFactory());
        _analyzerManager.RegisterAnalyzer("battery", new BatteryAnalyzerFactory());
        
        // Initialize Actor system
        _actorSystem = new ActorSystemManager();
        
        // Initialize Bridge pattern
        _displayManager = new DisplayManager();
    }
    
    public void ProcessSensorData(string rawData)
    {
        try
        {
            // Determine the correct parser
            ISensorDataParser parser = DetermineParser(rawData);
            if (parser == null)
            {
                Console.WriteLine($"No suitable parser found for data: {rawData}");
                return;
            }
            
            // Process the data
            SensorData data = parser.Parse(rawData);
            
            // Register the sensor type
            if (!string.IsNullOrEmpty(data.SerialNumber) && !string.IsNullOrEmpty(data.Type))
            {
                _typeManager.RegisterSensorType(data.SerialNumber, data.Type);
            }
            
            // Add data to the composite manager
            _compositeManager.AddSensorData(data);
            
            // Analyze the data
            _analyzerManager.AnalyzeData(data);
            
            // Send to Actor system
            _actorSystem.ProcessSensorData(data);
            
            // Display using Bridge pattern
            _displayManager.DisplaySensorData(data);
            
            // Notify observers
            _subject.Notify(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing sensor data: {ex.Message}");
        }
    }
    
    // Additional methods to access different subsystems
    public void AddObserver(ISensorDataObserver observer) { /* ... */ }
    public IReadOnlyDictionary<string, string> GetAllSensorTypes() { /* ... */ }
    public Dictionary<string, string> GetAllAnalysisResults() { /* ... */ }
    public void DisplaySensorHierarchy() { /* ... */ }
    // etc.
    
    private ISensorDataParser DetermineParser(string rawData)
    {
        foreach (var parser in _parsers)
        {
            if (parser.CanParse(rawData))
            {
                return parser;
            }
        }
        return null;
    }
    
    public void Dispose()
    {
        _actorSystem?.Dispose();
    }
}
```

#### Benefits
- Simplifies the interface to a complex subsystem
- Decouples client code from subsystem components
- Promotes loose coupling between clients and subsystems
- Shields clients from subsystem implementation details

#### Potential Drawbacks
- Can become a "god object" with too many responsibilities
- Might hide important functionality that advanced clients need
- May introduce an additional layer that impacts performance

### 7. Visitor Pattern

#### Context and Problem
We need to perform various operations (health checks, anomaly detection) on our sensor hierarchy without modifying the sensor classes. Each operation needs to behave differently depending on whether it's operating on an individual sensor or a group.

#### Implementation
```csharp
// Visitor interface
public interface ISensorVisitor
{
    void Visit(SensorLeaf sensor);
    void Visit(SensorGroup group);
    void Reset();
    string GetResult();
}

// Updated component interface
public interface ISensorComponent
{
    // Existing methods
    string Name { get; }
    string Type { get; }
    void AddData(SensorData data);
    Dictionary<string, double> GetAggregatedData();
    void DisplayInfo(int depth = 0);
    int GetSensorCount();
    
    // New method for visitor
    void Accept(ISensorVisitor visitor);
}

// Implementation in concrete classes
public class SensorLeaf : ISensorComponent
{
    // Existing implementation
    
    public void Accept(ISensorVisitor visitor)
    {
        visitor.Visit(this);
    }
}

public class SensorGroup : ISensorComponent
{
    // Existing implementation
    
    public void Accept(ISensorVisitor visitor)
    {
        visitor.Visit(this);
        
        // Visit all children
        foreach (var component in _components)
        {
            component.Accept(visitor);
        }
    }
}

// Concrete visitor
public class SensorHealthVisitor : ISensorVisitor
{
    private StringBuilder _report = new StringBuilder();
    private int _healthySensors = 0;
    private int _warningSensors = 0;
    private int _criticalSensors = 0;
    
    public void Visit(SensorLeaf sensor)
    {
        var data = sensor.GetAggregatedData();
        if (data["DataPointCount"] == 0)
            return;
            
        // Determine sensor health based on battery level
        if (data["BatteryLevel"] < 30)
        {
            _criticalSensors++;
            _report.AppendLine($"CRITICAL: Sensor {sensor.Name} has low battery ({data["BatteryLevel"]:F1}%)");
        }
        else if (data["BatteryLevel"] < 50)
        {
            _warningSensors++;
            _report.AppendLine($"WARNING: Sensor {sensor.Name} has moderate battery ({data["BatteryLevel"]:F1}%)");
        }
        else
        {
            _healthySensors++;
        }
    }
    
    public void Visit(SensorGroup group)
    {
        // Handle groups differently
        if (group.Type != "Root") // Skip root group
        {
            var data = group.GetAggregatedData();
            _report.AppendLine($"Group: {group.Name} - {group.GetSensorCount()} sensors, Avg Battery: {data["BatteryLevel"]:F1}%");
        }
    }
    
    public void Reset()
    {
        _report.Clear();
        _healthySensors = 0;
        _warningSensors = 0;
        _criticalSensors = 0;
    }
    
    public string GetResult()
    {
        // Generate and return the report
    }
}

// Another concrete visitor
public class AnomalyDetectionVisitor : ISensorVisitor
{
    // Implementation for anomaly detection
}
```

#### Benefits
- Allows adding new operations without modifying existing classes
- Groups related operations in a single visitor class
- Collects state across the entire object structure
- Separates algorithms from the objects they operate on

#### Potential Drawbacks
- Breaks encapsulation by exposing component details to visitors
- All concrete components must be known to the visitor
- Adding new component classes requires updating all visitors

### 8. Thread Pool Pattern

#### Context and Problem
Processing sensor data can be CPU-intensive, especially with many sensors sending data simultaneously. We need to efficiently utilize system resources by parallelizing data processing while avoiding the overhead of creating and destroying threads for each operation.

#### Implementation
```csharp
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

// Usage in a service class
public class ThreadPoolDataProcessor
{
    private readonly SensorSystemFacade _sensorSystem;
    private readonly ThreadPoolManager _threadPool;
    
    public ThreadPoolDataProcessor(SensorSystemFacade sensorSystem)
    {
        _sensorSystem = sensorSystem;
        _threadPool = ThreadPoolManager.Instance;
    }
    
    public async Task ProcessDataLinesAsync(IEnumerable<string> dataLines)
    {
        Console.WriteLine("Starting parallel processing of sensor data...");
        
        await _threadPool.ProcessBatchAsync(dataLines, async (line) => 
        {
            // Simulate some processing time
            await Task.Delay(50);
            
            // Process the data
            _sensorSystem.ProcessSensorData(line);
        });
        
        Console.WriteLine("Parallel processing complete.");
    }
}
```

#### Benefits
- Efficient resource utilization
- Reduced overhead compared to creating new threads
- Controlled parallelism to avoid overwhelming the system
- Simplified asynchronous programming model

#### Potential Drawbacks
- Thread pool starvation if tasks block for too long
- Difficulty in prioritizing tasks
- Potential for deadlocks if not carefully designed
- May not be suitable for very long-running operations

### 9. Producer-Consumer Pattern

#### Context and Problem
In a real sensor system, data arrives continuously and at varying rates. We need to decouple the data collection (producer) from data processing (consumer) to handle varying rates and ensure stable operation even when processing takes longer than data acquisition.

#### Implementation
```csharp
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

// Usage in streaming service
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
    }
    
    public void Stop()
    {
        _dataQueue.Stop();
    }
    
    public void AddData(string rawData)
    {
        _dataQueue.Produce(rawData);
    }
    
    public async Task ProcessFileStreamAsync(string filePath, int delayMs = 100)
    {
        // Read file and process as a stream
    }
}
```

#### Benefits
- Decouples data production from consumption
- Handles varying rates of data production and consumption
- Improves responsiveness and resource utilization
- Provides natural load balancing and throttling

#### Potential Drawbacks
- Additional complexity compared to direct processing
- Potential for memory issues with unbounded queues
- Adds latency to data processing
- Requires careful design to avoid deadlocks

### 10. Actor Model Pattern

#### Context and Problem
Our sensor system has multiple independent components that need to process data concurrently while maintaining isolation and fault tolerance. Traditional concurrency models with shared state can lead to race conditions, deadlocks, and complexity. The Actor Model pattern provides a solution by treating each component as an isolated actor that communicates through messages.

#### Implementation
```csharp
// Actor messages
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

// Actor implementations
public class SensorDataActor : ReceiveActor
{
    private readonly Dictionary<string, List<SensorData>> _sensorDataStore = new Dictionary<string, List<SensorData>>();
    private int _totalProcessedData = 0;
    
    public SensorDataActor()
    {
        // Handle incoming sensor data
        Receive<SensorDataMessage>(message => {
            ProcessSensorData(message.Data);
        });
        
        // Handle analysis requests
        Receive<AnalyzeDataMessage>(message => {
            var result = AnalyzeSensorData(message.SensorType);
            Sender.Tell(new DataAnalysisResult(message.SensorType, result));
        });
    }
    
    private void ProcessSensorData(SensorData data)
    {
        if (string.IsNullOrEmpty(data.SerialNumber))
            return;
            
        if (!_sensorDataStore.TryGetValue(data.SerialNumber, out var dataList))
        {
            dataList = new List<SensorData>();
            _sensorDataStore[data.SerialNumber] = dataList;
        }
        
        dataList.Add(data);
        _totalProcessedData++;
    }
    
    private Dictionary<string, double> AnalyzeSensorData(string sensorType)
    {
        // Analyze data for the specified sensor type
        // Return statistics
    }
}

// Actor system manager
public class ActorSystemManager : IDisposable
{
    private readonly ActorSystem _actorSystem;
    private readonly IActorRef _sensorDataActor;
    private readonly IActorRef _alertActor;
    
    public ActorSystemManager()
    {
        // Create the actor system
        _actorSystem = ActorSystem.Create("SensorMonitoringSystem");
        
        // Create the actors
        _sensorDataActor = _actorSystem.ActorOf(Props.Create(() => new SensorDataActor()), "sensorData");
        _alertActor = _actorSystem.ActorOf(Props.Create(() => new AlertActor()), "alerts");
    }
    
    public void ProcessSensorData(SensorData data)
    {
        var message = new SensorDataMessage(data);
        
        // Send to both actors
        _sensorDataActor.Tell(message);
        _alertActor.Tell(message);
    }
    
    public async Task<Dictionary<string, double>> AnalyzeSensorTypeAsync(string sensorType)
    {
        var result = await _sensorDataActor.Ask<DataAnalysisResult>(
            new AnalyzeDataMessage(sensorType), 
            TimeSpan.FromSeconds(5)
        );
        
        return result.Stats;
    }
    
    public void Dispose()
    {
        _actorSystem?.Terminate();
        _actorSystem?.Dispose();
    }
}
```

#### Benefits
- Isolation between components reduces shared state issues
- Natural concurrency model with message passing
- Improved fault tolerance with supervision hierarchies
- Scalability across cores and even distributed systems
- Simplified concurrent programming by avoiding locks

#### Potential Drawbacks
- Increased complexity for simpler scenarios
- Message-passing overhead
- Potential for message loss if not properly designed
- Debugging can be challenging due to asynchronous nature

### 11. Bridge Pattern

#### Context and Problem
Our sensor monitoring system needs to display data in various formats (text, JSON) through different output mechanisms (console, file logging). Traditional inheritance would lead to an explosion of classes (TextConsoleDisplay, JsonConsoleDisplay, TextFileDisplay, etc.). We need a way to separate data formatting from display mechanisms.

#### Implementation
```csharp
// Implementation interface
public interface ISensorDataFormatter
{
    string FormatSensorData(SensorData data);
    string FormatSensorList(IEnumerable<SensorData> dataList);
    string FormatStatistics(Dictionary<string, double> statistics);
    string FormatAlert(string alertMessage, string severity);
}

// Abstraction interface
public interface ISensorDataDisplay
{
    void DisplaySensorData(SensorData data);
    void DisplaySensorList(IEnumerable<SensorData> dataList);
    void DisplayStatistics(Dictionary<string, double> statistics, string title);
    void DisplayAlert(string alertMessage, string severity);
}

// Concrete formatter implementations
public class TextFormatter : ISensorDataFormatter
{
    public string FormatSensorData(SensorData data)
    {
        if (data == null)
            return "No data";
            
        return $"Sensor: {data.SerialNumber} | Type: {data.Type} | " +
               $"Temp: {data.Temperature:F1}°C | Humidity: {data.Humidity:F1}% | " +
               $"Battery: {data.BatteryLevel}/{data.BatteryMax} | State: {data.State}";
    }
    
    public string FormatSensorList(IEnumerable<SensorData> dataList)
    {
        // Format a list of sensor data as text
    }
    
    public string FormatStatistics(Dictionary<string, double> statistics)
    {
        // Format statistics as text
    }
    
    public string FormatAlert(string alertMessage, string severity)
    {
        return $"[{severity}] {alertMessage}";
    }
}

public class JsonFormatter : ISensorDataFormatter
{
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        WriteIndented = true
    };
    
    public string FormatSensorData(SensorData data)
    {
        // Format sensor data as JSON
    }
    
    // Other formatting methods
}

// Concrete display implementations
public class ConsoleDisplay : ISensorDataDisplay
{
    private readonly ISensorDataFormatter _formatter;
    
    public ConsoleDisplay(ISensorDataFormatter formatter)
    {
        _formatter = formatter;
    }
    
    public void DisplaySensorData(SensorData data)
    {
        Console.WriteLine(_formatter.FormatSensorData(data));
    }
    
    // Other display methods
}

public class FileDisplay : ISensorDataDisplay
{
    private readonly ISensorDataFormatter _formatter;
    private readonly string _filePath;
    
    public FileDisplay(ISensorDataFormatter formatter, string filePath)
    {
        _formatter = formatter;
        _filePath = filePath;
        
        // Create or clear the file
        File.WriteAllText(_filePath, $"Sensor Monitoring Log - {DateTime.Now}\n\n");
    }
    
    public void DisplaySensorData(SensorData data)
    {
        AppendToFile(_formatter.FormatSensorData(data));
    }
    
    // Other display methods
    
    private void AppendToFile(string content)
    {
        try
        {
            File.AppendAllText(_filePath, content + "\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to file: {ex.Message}");
        }
    }
}

// Display manager
public class DisplayManager
{
    private readonly Dictionary<string, ISensorDataDisplay> _displays = new Dictionary<string, ISensorDataDisplay>();
    
    public DisplayManager()
    {
        // Create default displays with different formatters
        var textFormatter = new TextFormatter();
        var jsonFormatter = new JsonFormatter();
        
        _displays["console"] = new ConsoleDisplay(textFormatter);
        _displays["json-console"] = new ConsoleDisplay(jsonFormatter);
        _displays["text-file"] = new FileDisplay(textFormatter, "sensor_log.txt");
        _displays["json-file"] = new FileDisplay(jsonFormatter, "sensor_log.json");
    }
    
    // Methods to use different displays
    public void DisplaySensorData(SensorData data, string displayName = "console")
    {
        if (_displays.TryGetValue(displayName, out var display))
        {
            display.DisplaySensorData(data);
        }
    }
    
    // Other methods
}
```

#### Benefits
- Separates abstraction (display) from implementation (formatting)
- Both display mechanisms and formatting strategies can vary independently
- New display types or formatters can be added without affecting existing code
- Avoids a combinatorial explosion of classes
- Follows Open/Closed Principle and Single Responsibility Principle

#### Potential Drawbacks
- Adds complexity with additional interfaces and classes
- May be overkill for simple display requirements
- Requires careful design of the abstraction and implementation interfaces

## System Architecture

The Temperature Monitoring System is structured using a layered architecture with clear separation of concerns. Here's an overview of the key architectural components:

### Core Layer
- **Models**: Contains the `SensorData` class which represents the core data structure
- **Interfaces**: Defines contracts for components like `ISensorDataParser`, `ISensorDataObserver`, etc.
- **Enums**: Contains enumeration types like `SensorState` for representing sensor states

### Patterns Layer
- **Strategy**: Contains parser implementations for different data formats
- **Observer**: Implements the observer pattern for real-time monitoring
- **Singleton**: Provides centralized sensor type management
- **Factory**: Creates appropriate analyzers for different sensor types
- **Composite**: Organizes sensors in hierarchical groups
- **Visitor**: Implements operations on the sensor hierarchy
- **Bridge**: Separates data formatting from display mechanisms
- **Concurrency**: Contains thread pool, producer-consumer, and actor implementations

### Services Layer
- **FileDataReader**: Reads sensor data from files
- **SensorDataProcessor**: Processes raw sensor data
- **ThreadPoolDataProcessor**: Processes data using parallel execution
- **StreamingDataProcessor**: Handles continuous data streams

### Facade Layer
- **SensorSystemFacade**: Provides a unified interface to all subsystems

### Flow of Data

1. Raw sensor data is read from a file using `FileDataReader` (simulating real-time data from sensors)
2. The data is passed to the `SensorSystemFacade` which acts as the entry point to the system
3. The facade determines the appropriate parser (Strategy Pattern) and parses the raw data
4. Parsed sensor data is:
   - Registered with the `SensorTypeManager` (Singleton Pattern)
   - Added to the sensor hierarchy (Composite Pattern)
   - Analyzed by appropriate analyzers (Factory Method Pattern)
   - Sent to the Actor System for concurrent processing (Actor Model Pattern)
   - Displayed using the Bridge Pattern
   - Broadcast to observers (Observer Pattern)
5. The `SensorCompositeManager` maintains a hierarchical organization of sensors
6. Visitors can traverse the sensor hierarchy to perform operations (Visitor Pattern)
7. Concurrency patterns manage efficient processing of data

### Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                      Client Application                      │
└───────────────────────────┬─────────────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────────────┐
│                     SensorSystemFacade                       │
└───┬───────┬────────┬─────────┬──────────┬────────┬──────────┘
    │       │        │         │          │        │
┌───▼───┐ ┌─▼────┐ ┌─▼────┐ ┌──▼─────┐ ┌──▼────┐ ┌─▼────────┐
│Parsers│ │Sensor│ │Sensor│ │Analysis│ │Display│ │Concurrency│
│       │ │Type  │ │Comp- │ │Engines │ │Manager│ │Managers   │
│       │ │Manager│ │osite │ │        │ │       │ │          │
└───────┘ └──────┘ └──────┘ └────────┘ └───────┘ └──────────┘
```

## Code Examples

### Using the Facade Pattern

The Facade pattern provides a simplified interface to the complex system. Here's an example of using the facade to process sensor data:

```csharp
// Create the facade
using (var sensorSystem = new SensorSystemFacade())
{
    // Add custom observer
    var statisticsObserver = new StatisticsObserver();
    sensorSystem.AddObserver(statisticsObserver);
    
    // Process data from file
    await fileReader.ReadDataFromFile("sensor_data.txt");
    
    // Get sensor types using singleton
    var sensorTypes = sensorSystem.GetAllSensorTypes();
    
    // Display sensor hierarchy (composite pattern)
    sensorSystem.DisplaySensorHierarchy();
    
    // Use visitor pattern
    string healthReport = sensorSystem.ApplyHealthVisitor();
    Console.WriteLine(healthReport);
    
    // Use bridge pattern to display statistics
    var tempStats = await sensorSystem.AnalyzeSensorTypeWithActorsAsync("temp");
    sensorSystem.DisplayStatistics(tempStats, "Temperature Statistics", "console");
}
```

### Using the Strategy Pattern

The Strategy pattern allows selecting a parsing algorithm at runtime. Here's an example of how parsers are selected and used:

```csharp
// Determine the correct parser based on data format
private ISensorDataParser DetermineParser(string rawData)
{
    foreach (var parser in _parsers)
    {
        if (parser.CanParse(rawData))
        {
            return parser;
        }
    }
    return null;
}

// Process the data using the selected parser
public SensorData ProcessData(string rawData)
{
    ISensorDataParser parser = DetermineParser(rawData);
    if (parser == null)
    {
        throw new InvalidOperationException("No suitable parser found");
    }
    
    _processor.SetParser(parser);
    return _processor.ProcessData(rawData);
}
```

### Using the Thread Pool Pattern

The Thread Pool pattern manages worker threads efficiently. Here's an example of processing data in parallel:

```csharp
// Process multiple sensor readings in parallel
public async Task ProcessDataLinesAsync(IEnumerable<string> dataLines)
{
    var threadPool = ThreadPoolManager.Instance;
    
    await threadPool.ProcessBatchAsync(dataLines, async (line) => 
    {
        // Simulate some real-world processing time
        await Task.Delay(50);
        
        // Process the data
        _sensorSystem.ProcessSensorData(line);
    });
}
```

### Using the Actor Model Pattern

The Actor Model pattern provides isolation and concurrency. Here's an example of sending messages to actors:

```csharp
// Actor handling messages
public class SensorDataActor : ReceiveActor
{
    private readonly Dictionary<string, List<SensorData>> _sensorDataStore = 
        new Dictionary<string, List<SensorData>>();
    
    public SensorDataActor()
    {
        // Define message handlers
        Receive<SensorDataMessage>(message => {
            ProcessSensorData(message.Data);
        });
        
        Receive<AnalyzeDataMessage>(message => {
            var result = AnalyzeSensorData(message.SensorType);
            Sender.Tell(new DataAnalysisResult(message.SensorType, result));
        });
    }
    
    private void ProcessSensorData(SensorData data)
    {
        // Store and process the sensor data
    }
}

// Sending a message to an actor
public void ProcessSensorData(SensorData data)
{
    var message = new SensorDataMessage(data);
    _sensorDataActor.Tell(message);
}

// Asking an actor for a response
public async Task<Dictionary<string, double>> AnalyzeSensorTypeAsync(string sensorType)
{
    var result = await _sensorDataActor.Ask<DataAnalysisResult>(
        new AnalyzeDataMessage(sensorType), 
        TimeSpan.FromSeconds(5)
    );
    
    return result.Stats;
}
```

## Testing and Validation

The Temperature Monitoring System includes comprehensive unit tests to validate the functionality of each design pattern. The following testing approach was used:

### Test Strategy

1. **Unit Tests**: Small, focused tests for individual components and patterns
2. **Integration Tests**: Tests that verify multiple components working together
3. **System Tests**: Tests that validate the entire system behavior

### Test Framework

The system uses MSTest for unit testing, with tests organized by pattern:

```csharp
[TestClass]
public class SingletonPatternTests
{
    [TestMethod]
    public void SingletonInstance_ShouldReturnSameInstance()
    {
        // Arrange & Act
        var instance1 = SensorTypeManager.Instance;
        var instance2 = SensorTypeManager.Instance;
        
        // Assert
        Assert.IsNotNull(instance1);
        Assert.IsNotNull(instance2);
        Assert.AreSame(instance1, instance2, "Singleton should return the same instance");
    }
    
    // Additional tests...
}
```

### Testing Concurrent Patterns

Special attention was given to testing concurrent patterns like Thread Pool and Actor Model:

```csharp
[TestMethod]
public async Task ThreadPool_ShouldExecuteInParallel()
{
    // Arrange
    var threadPool = ThreadPoolManager.Instance;
    int concurrentExecutions = 0;
    int maxConcurrentExecutions = 0;
    object lockObj = new object();
    
    using var executionBarrier = new ManualResetEvent(false);
    
    // Act - Create multiple tasks that will all try to run at once
    var tasks = new List<Task>();
    
    for (int i = 0; i < 5; i++)
    {
        tasks.Add(Task.Run(async () => {
            await threadPool.QueueTaskAsync(() => {
                // Track concurrent execution
                lock (lockObj)
                {
                    concurrentExecutions++;
                    maxConcurrentExecutions = Math.Max(maxConcurrentExecutions, concurrentExecutions);
                }
                
                // Wait at barrier to ensure concurrent execution
                executionBarrier.WaitOne();
                
                lock (lockObj)
                {
                    concurrentExecutions--;
                }
            });
        }));
    }
    
    // Give tasks time to start
    await Task.Delay(1000);
    
    // Release barrier and complete tasks
    executionBarrier.Set();
    await Task.WhenAll(tasks);
    
    // Assert
    Assert.IsTrue(maxConcurrentExecutions > 1, 
        "ThreadPool should execute tasks in parallel");
}
```

### Testing the Visitor Pattern

Tests for the Visitor pattern validate that visitors can traverse the composite structure correctly:

```csharp
[TestMethod]
public void SensorGroup_ShouldPropagateCVisitorToChildren()
{
    // Arrange
    var rootGroup = new SensorGroup("Root");
    var childGroup = new SensorGroup("Child");
    var sensor1 = new SensorLeaf("1", "Sensor1");
    var sensor2 = new SensorLeaf("2", "Sensor2");
    
    // Build the hierarchy
    rootGroup.AddComponent(childGroup);
    childGroup.AddComponent(sensor1);
    rootGroup.AddComponent(sensor2);
    
    var visitor = new MockVisitor();
    
    // Act
    rootGroup.Accept(visitor);
    
    // Assert
    Assert.AreEqual(2, visitor.LeafVisitCount, "Visitor should visit both leaf sensors");
    Assert.AreEqual(2, visitor.GroupVisitCount, "Visitor should visit both groups");
}
```

### Testing Coverage

The test suite aims for high code coverage, particularly focusing on:

1. **Core Logic**: Ensuring all business logic works correctly
2. **Edge Cases**: Testing boundary conditions and error handling
3. **Concurrency**: Verifying thread safety and parallel execution
4. **Integration**: Checking that patterns work together correctly

## Extensibility and Maintenance

The Temperature Monitoring System is designed with extensibility and maintainability as key priorities. The use of design patterns creates a flexible architecture that can be easily extended and maintained.

### Extensibility Points

1. **New Sensor Types**
   - Simply add a new sensor type identifier and use the existing infrastructure
   - The Composite pattern organizes sensors by type automatically

2. **New Data Formats**
   - Implement a new concrete strategy class that implements `ISensorDataParser`
   - Add it to the parser list in the facade

3. **New Analysis Algorithms**
   - Create a new analyzer class implementing `ISensorDataAnalyzer`
   - Create a corresponding factory class
   - Register it with the analyzer manager

4. **New Display Formats**
   - Add a new formatter implementing `ISensorDataFormatter`
   - Create displays using the new formatter

5. **New Operations on Sensors**
   - Create a new visitor class implementing `ISensorVisitor`
   - Apply it to the sensor hierarchy

### Maintenance Considerations

1. **Loose Coupling**
   - Components interact through interfaces
   - Changes to one component are unlikely to affect others

2. **Single Responsibility Principle**
   - Each class has a specific responsibility
   - Changes to one aspect of the system are localized

3. **Open/Closed Principle**
   - The system is open for extension but closed for modification
   - New functionality can be added without changing existing code

4. **Dependency Inversion**
   - High-level modules depend on abstractions, not concrete implementations
   - Facilitates unit testing and component replacement

5. **Documentation**
   - Each pattern's purpose and implementation is documented
   - Code includes meaningful comments and follows consistent naming conventions

### Example: Adding a New Parser

To add support for a new data format:

```csharp
public class JSONFormatParser : ISensorDataParser
{
    public bool CanParse(string rawData)
    {
        return rawData.TrimStart().StartsWith("{");
    }

    public SensorData Parse(string rawData)
    {
        // Parse JSON data and convert to SensorData object
        var data = JsonSerializer.Deserialize<SensorData>(rawData);
        
        // Standardize data
        DataParsingHelper.StandardizeData(data);
        
        return data;
    }
}

// Then in SensorSystemFacade constructor:
_parsers.Add(new JSONFormatParser());
```

## Reflection on Alternatives

During the development of the Temperature Monitoring System, several alternative approaches were considered for each design pattern. This section reflects on those alternatives and the rationale for the chosen implementations.

### Singleton vs. Dependency Injection

For the `SensorTypeManager`, a singleton pattern was chosen to ensure a single consistent registry of sensor types. An alternative approach would be to use dependency injection:

```csharp
// Alternative using dependency injection
public class SensorTypeManager : ISensorTypeManager
{
    private readonly Dictionary<string, string> _sensorTypes = new Dictionary<string, string>();
    
    // Methods implementation
}

// Register as singleton in DI container
services.AddSingleton<ISensorTypeManager, SensorTypeManager>();
```

The singleton pattern was chosen for simplicity and to avoid the overhead of setting up a dependency injection container. However, in a larger production system, dependency injection would offer better testability and flexibility.

### Strategy vs. Switch Statement

For parsing different data formats, the Strategy pattern was chosen over a switch statement approach:

```csharp
// Alternative using switch statement
public SensorData Parse(string rawData)
{
    if (rawData.StartsWith("serial:"))
    {
        // Parse standard format
    }
    else if (rawData.StartsWith("manufac:") || rawData.StartsWith("manu:"))
    {
        // Parse manufacturer format
    }
    else
    {
        throw new FormatException("Unknown data format");
    }
}
```

The Strategy pattern was chosen because:
- It follows the Open/Closed Principle
- Adding new formats doesn't require modifying existing code
- Each parser is isolated and can be tested independently
- The pattern provides better separation of concerns

### Observer vs. Event-based Communication

For notifying components about new sensor data, the Observer pattern was chosen over C# events:

```csharp
// Alternative using C# events
public class SensorDataProvider
{
    public event EventHandler<SensorDataEventArgs> DataReceived;
    
    protected virtual void OnDataReceived(SensorData data)
    {
        DataReceived?.Invoke(this, new SensorDataEventArgs(data));
    }
    
    public void ProcessData(string rawData)
    {
        // Process data
        var data = Parse(rawData);
        
        // Notify subscribers
        OnDataReceived(data);
    }
}
```

The Observer pattern was chosen because:
- It provides more control over the subscription process
- It allows implementing custom notification logic
- It's more aligned with the design pattern curriculum requirements
- It's more explicit in showing the pattern structure

### Thread Pool vs. Parallel.ForEach

For parallel processing, a custom Thread Pool implementation was chosen over using `Parallel.ForEach`:

```csharp
// Alternative using Parallel.ForEach
public void ProcessDataLines(IEnumerable<string> dataLines)
{
    Parallel.ForEach(
        dataLines,
        new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
        line => {
            _sensorSystem.ProcessSensorData(line);
        }
    );
}
```

The custom Thread Pool was chosen because:
- It provides more control over thread management
- It allows for custom throttling and prioritization
- It can be extended to support more advanced features
- It demonstrates the pattern implementation more explicitly

## Conclusion

The Temperature Monitoring System successfully implements a comprehensive set of design patterns to create a flexible, maintainable, and scalable architecture for processing sensor data. Key achievements include:

### Design Pattern Implementation
- **Creational Patterns**: Singleton and Factory Method patterns provide centralized management and flexible object creation
- **Structural Patterns**: Composite, Facade, and Bridge patterns create a well-organized and decoupled structure
- **Behavioral Patterns**: Strategy, Observer, and Visitor patterns enable flexible behaviors and operations
- **Concurrency Patterns**: Thread Pool, Producer-Consumer, and Actor Model patterns enable efficient parallel processing

### Architecture Benefits
- **Separation of Concerns**: Each component has a clear, single responsibility
- **Loose Coupling**: Components interact through interfaces, minimizing dependencies
- **Extensibility**: New features can be added without modifying existing code
- **Testability**: Components can be tested in isolation with well-defined interfaces
- **Maintainability**: Clean architecture makes the system easy to understand and modify

### Learning Outcomes
- Deep understanding of design pattern implementations and their interactions
- Experience with concurrent programming patterns
- Practical application of software engineering principles
- Creating a flexible architecture that can evolve with changing requirements

This project demonstrates how design patterns can be combined to create a robust solution for real-world problems like sensor data monitoring. The patterns work together synergistically, each addressing specific aspects of the system design, resulting in a well-structured application that is both powerful and maintainable.

The Temperature Monitoring System serves as a practical example of how to apply design patterns in C# development, showcasing the benefits of a pattern-oriented architecture in creating flexible, maintainable software systems.
