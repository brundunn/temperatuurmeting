using System.Collections.Generic;
using TemperatuurMetingen.Core.Interfaces;
using TemperatuurMetingen.Core.Models;

namespace TemperatuurMetingen.Patterns.Behavioral.Observer;

public class SensorDataSubject
{
    /// <summary>
    /// List of observers subscribed to sensor data updates.
    /// </summary>
    private readonly List<ISensorDataObserver> _observers = new List<ISensorDataObserver>();

    /// <summary>
    /// Attaches an observer to the subject.
    /// </summary>
    /// <param name="observer">The observer to attach.</param>
    public void Attach(ISensorDataObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    /// <summary>
    /// Detaches an observer from the subject.
    /// </summary>
    /// <param name="observer">The observer to detach.</param>
    public void Detach(ISensorDataObserver observer)
    {
        _observers.Remove(observer);
    }

    /// <summary>
    /// Notifies all attached observers with the provided sensor data.
    /// </summary>
    /// <param name="data">The sensor data to notify observers with.</param>
    public void Notify(SensorData data)
    {
        foreach (var observer in _observers)
        {
            observer.Update(data);
        }
    }
}