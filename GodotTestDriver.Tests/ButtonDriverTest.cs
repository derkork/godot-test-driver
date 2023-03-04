using System;
using System.Threading.Tasks;
using GodotTestDriver.Drivers;
using JetBrains.Annotations;

namespace GodotTestDriver.Tests;

using Godot;
using GoDotTest;
using Shouldly;

[UsedImplicitly]
public class ButtonDriverTest : DriverTest
{
    
    private readonly ButtonDriver _buttonDriver;
    private readonly LabelDriver _labelDriver;
    private readonly ControlDriver<Panel> _panelDriver;

    public ButtonDriverTest(Node testScene) : base(testScene)
    {
        _buttonDriver = new ButtonDriver(() => RootNode.GetNode<Button>("Button"));
        _labelDriver = new LabelDriver(() => RootNode.GetNode<Label>("Label"));
        _panelDriver = new ControlDriver<Panel>(() => RootNode.GetNode<Panel>("Panel"));
    }


    [Test]
    public async Task ClickingWorks()
    {
        // WHEN
        // i click the button
        await _buttonDriver.ClickCenter();
        // the label text changes.
        _labelDriver.Text.ShouldBe("did work");
        // and the panel disappears
        _panelDriver.IsVisible.ShouldBeFalse();
    }
    
    [Test]
    public async Task ClickingDisabledButtonThrowsException()
    {
        // SETUP
        _buttonDriver.PresentRoot.Disabled = true;
        // WHEN
        // i click the button then an exception is thrown
        await Should.ThrowAsync<InvalidOperationException>(async () => await _buttonDriver.ClickCenter());
    }
    
    [Test]
    public async Task ClickingHiddenButtonThrowsException()
    {
        // SETUP
        _buttonDriver.PresentRoot.Visible = false;
        // WHEN
        // i click the button then an exception is thrown
        await Should.ThrowAsync<InvalidOperationException>(async () => await _buttonDriver.ClickCenter());
    }
}