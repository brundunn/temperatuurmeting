using System.Collections.Generic;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Patterns.Behavioral.Observer;

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