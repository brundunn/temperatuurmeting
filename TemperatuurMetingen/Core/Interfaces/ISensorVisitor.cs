using TemperatuurMetingen.Core.Models;
using TemperatuurMetingen.Patterns.Structural.Composite;

namespace TemperatuurMetingen.Core.Interfaces
{
    public interface ISensorVisitor
    {
        void Visit(SensorLeaf sensor);
        void Visit(SensorGroup group);
        void Reset();
        string GetResult();
    }
}