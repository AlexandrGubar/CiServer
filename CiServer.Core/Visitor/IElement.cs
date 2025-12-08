namespace CiServer.Core.Visitor;

public interface IElement
{
    void Accept(IVisitor visitor);
}