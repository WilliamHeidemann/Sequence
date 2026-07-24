using Cloud_Code_Module_Reference;
using Game.Domain.Models;

namespace TestProject;

public class Tests
{
    [Test]
    public void TestHelloWorld()
    {
        var module = new MyModule(null, null);
        var helloAnon = module.Hello("Anon");
        Assert.That(helloAnon, Is.Not.Null);
        Assert.That(helloAnon, Is.EqualTo("Hello, Anon!"));
    }

    [Test]
    public void TestPosition()
    {
        Position position = new Position(Row.One, Column.One);
        
        Assert.That(position.Row, Is.EqualTo(Row.One));
        Assert.That(position.Column, Is.EqualTo(Column.One));
    }
}
