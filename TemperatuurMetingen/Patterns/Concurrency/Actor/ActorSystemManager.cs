using System;
using System.Threading.Tasks;
using Akka.Actor;
using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Patterns.Concurrency.Actor.Actors;
using TemperatuurMetingen.Patterns.Concurrency.Actor.Messages;

namespace TemperatuurMetingen.Patterns.Concurrency.Actor
{
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
            
            Console.WriteLine("Actor system initialized with SensorDataActor and AlertActor");
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
        
        public async Task<int> GetProcessedDataCountAsync()
        {
            var result = await _sensorDataActor.Ask<SystemStatusResult>(
                new GetStatusMessage(), 
                TimeSpan.FromSeconds(5)
            );
            
            return result.ProcessedDataPoints;
        }
        
        public async Task<string> GetAlertsAsync()
        {
            var result = await _alertActor.Ask<string>(
                new GetStatusMessage(), 
                TimeSpan.FromSeconds(5)
            );
            
            return result;
        }
        
        public void Dispose()
        {
            _actorSystem?.Terminate();
            _actorSystem?.Dispose();
        }
    }
}
