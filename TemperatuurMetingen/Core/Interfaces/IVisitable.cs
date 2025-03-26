namespace TemperatuurMetingen.Core.Interfaces;

public interface IVisitable
{
    void Accept(ISensorVisitor visitor);
}