using System;
using System.Threading.Tasks;
using Godot;
using GoDotTest;
using GodotTestDriver.Drivers;
using GodotTestDriver.Tests;
using Shouldly;

public partial class LabelDriverTest : DriverTest
{
    private readonly LabelDriver _label;

    public LabelDriverTest(Node testScene) : base(testScene)
    {
        _label = new LabelDriver(() => RootNode.GetNode<Label>("Label"));
    }

    [Test]
    public void InspectionWorks()
    {
        // WHEN
        // everything is set up
        
        // THEN
        // the label text is "Hello World!"
        _label.Text.ShouldBe("Hello World!");
    }
}