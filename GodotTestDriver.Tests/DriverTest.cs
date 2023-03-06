using System.Threading.Tasks;
using Godot;
using GoDotTest;
using JetBrains.Annotations;

namespace GodotTestDriver.Tests;

[UsedImplicitly]
public abstract class DriverTest : TestClass
{
    protected Fixture Fixture { get; }
    protected  Node RootNode { get; private set; } = null!;

    protected DriverTest(Node testScene) : base(testScene)
    {
        Fixture = new Fixture(testScene.GetTree());
    }
    
    [Setup]
    [UsedImplicitly]
    public async Task Setup()
    {
        RootNode = await Fixture.LoadAndAddScene<Node>($"res://{GetType().Name}.tscn");
    }
    
    
    [Cleanup]
    [UsedImplicitly]
    public async Task Cleanup()
    {
        await Fixture.Cleanup();
    }
    
}