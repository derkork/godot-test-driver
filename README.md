# Godot Test Driver

### What is it?
This library provides an API that simplifies writing integration tests for Godot projects. It provides:
- A very simple and minimal framework for interacting with Godot nodes from integration tests. With it, you can effectively decouple your integration tests from the implementation details of your Godot project.
- Working implementations for sending commands, mouse clicks and keystrokes which you will need in every integration test.
- Drivers for many of Godot's built-in nodes which you can use as building blocks for your integration tests.
- A fixture implementation for setting up test fixtures and destroying them properly after the test.

### What is it not?
GodotTestDriver is not a test framework. There are already a lot of godot test frameworks out there (e.g GodotXUnit, WAT, GDUnit3) so there is no need add another one to the list. Pick one and use GodotTestDriver together with it.

GodotTestDriver is also not an assertions library. Most test frameworks come with built-in assertions, so you can just use these together 

## How to use GodotTestDriver
### Installation

GodotTestDriver is published on [NuGet](https://www.nuget.org/packages/GodotTestDriver). To add it use this command line command (or the NuGet facilities of your IDE):

```bash
dotnet add package GodotTestDriver --version 0.0.30
```

### Fixtures

This library provides a `Fixture` class which you can use to create and automatically dispose of Godot nodes and scenes. The fixture ensures that all tree modifications run on the main thread. 

```csharp
using GodotTestDriver;

class MyTest {
     // You will need get hold of a SceneTree instance. The way you get
     // hold of it will depend on the testing framework you use.
    SceneTree tree = ...;
    Fixture fixture;
    Player player;
    Arena arena;
    
    // This is a setup method. The exact way of how stuff is set up
    // differs from framework to framework, but most have a setup
    // method.
    async Task Setup() {
        // Create a new Fixture instance.
        fixture = new Fixture(tree);
        
        // load the arena scene. It will be automatically
        // disposed of when the fixture is disposed.
        arena = await fixture.LoadAndAddScene<Arena>("res://arena.tscn");
        
        // load the player. it also will be automatically disposed.
        player = await fixture.LoadScene<Player>("res://player.tscn");
        
        // add the player to the arena.
        arena.AddChild(player);
    }
    
 
    async Task TestBattle() {
        // load a monster. again, it will be automatically disposed.
        var monster = fixture.LoadScene<Monster>("res://monster.tscn");
        
        // add the monster to the arena
        arena.AddChild(monster);
        
        // create a weapon on the fly without loading a scene.
        // We call fixture.AutoFree to schedule this object for
        // deletion when the fixture is cleaned up.
        var weapon = fixture.AutoFree(new Weapon());
        
        // add the weapon to the player.
        arena.AddChild(weapon);
        
        
        // run the actual tests.
        ....
    }
    
    // You can also add custom cleanup steps to the fixture while
    // the test is running. These will be performed after the
    // test is done. This is very useful for cleaning up stuff
    // that is created during the tests.
    async Task TestSaving() {
        ... 
        // save the game
        await GameDialog.SaveButton.Click();
        
        // instruct the fixture to delete our savegame in the
        // cleanup phase.
        fixture.AddCleanupStep(() => File.Delete("user://savegame.dat"));
                
        // assert that the game was saved
        Assert.That(File.Exists("user://savegame.dat"));

        ....
        // when the test is done, the fixture will run your custom
        // cleanup step (e.g. delete the save game in this case)
    }
    
    
    // This is a cleanup method. Like the setup method, the exact
    // way of how stuff is cleaned up differs from framework to
    // framework, but most have a cleanup method.
    async Task TearDown() {
        // dispose of anything we created during the test.
        // this will also run all custom cleanup steps.
        await Fixture.Cleanup();
    }
}
```

## Test drivers
### Introduction

Test drivers serve as an abstraction layer between your test code and your game code. They are a high level interface through which the tests can "see" the game and interact with it. With a test driver, your game tests do not need to know how the game works under the hood. This makes your tests a lot more robust to change.

### Producing nodes for the test driver to work on
Test drivers work on a part of the node tree. Each test driver takes a _producer_ as argument, which is a function that is supposed to produce a node from the current tree that the driver will work on. E.g. the `ButtonDriver` takes a function that produces a button node.

How exactly this node is produced depends on your game and test setup. Lets say you would use a classic test framework that has some kind of `Setup` method:

```csharp
class MyTest {

    ButtonDriver buttonDriver;
    
    async Task Setup() {
        buttonDriver = new ButtonDriver(() => GetTree().GetNodeOrNull<Button>("UI/MyButton"));
        
        // ... more setup here
    }
}
```

In this example, the `ButtonDriver` would try to get the node it should work on using the `GetNodeOrNull`  function. When the driver is constructed, it will not check whether the node is actually present. This only happens when the driver is used. This way you can set up a driver without having a matching node structure in place. This is very useful as node structures can dynamically change while your tests are running (e.g. a dialog can be added to the scene or removed from it, same with monsters or players). 

### Using the test driver
After you have created the test driver you can use it in your tests:

```csharp

async Task TestButtonDisappearsWhenClicked() {
    // when
    // will click the button in its center. This will actually
    // move the mouse set a click and trigger all the events of a
    // proper button click.
    await buttonDriver.ClickCenter();
    
    // then
    // the button should be present but invisible.
    Assert.That(button.Visible).IsFalse();
}

```

Note how your tests now interface with the driver, rather than the underlying node structure. When the `ClickCenter` method is called and the button is not actually present and visible, the method will throw an exception explaining why you cannot click the button right now. This way you will get proper error messages when you are testing your game and not just `NullReferenceException`s which greatly helps in debugging tests.

### Composition of test drivers
Using a test driver by its own is nice, but it is only enough for very simple cases. Most of the time you will have complex nested node structures that make up your game entities and the UI. You can therefore compose test drivers into tree-like structures to represent these entities. Let's say you have a dialog popping up asking the player whether they want to save the game before quitting. It consists of three buttons and a label.

You can write a custom driver that represents this dialog to your tests:

```csharp

// the root of the dialog would be a panel container.
class ConfirmationDialogDriver : ControlDriver<PanelContainer> {

    // we have a label and three buttons 
    public LabelDriver Label { get; }
    public ButtonDriver YesButton { get; }
    public ButtonDriver NoButton { get; }
    public ButtonDriver CancelButton { get; }
    
    public ConfirmationDialogDriver(Func<PanelContainer> producer) : base(producer) {
        // for each of the elements we create a new driver, that
        // uses a producer fetching the respective node from below
        // our own root node. 

        // Root is a built-in property of the driver base class,
        // which will run the producer function to get the root node.
        Label = new LabelDriver(() => Root?.GetNodeOrNull<Label>("VBox/Label"));
        YesButton = new ButtonDriver(() => Root?.GetNodeOrNull<Button>("VBox/HBox/YesButton"));
        NoButton = new ButtonDriver(() => Root?.GetNodeOrNull<Button>("VBox/HBox/NoButton"));
        CancelButton = new ButtonDriver(() => Root?.GetNodeOrNull<Button>("VBox/HBox/CancelButton"));
    }
}
```

Now we can use this driver in our tests to test the dialog:

```csharp
ConfirmationDialogDriver dialogDriver;

async Task Setup() {
    // prepare the driver
    dialogDriver = new ConfirmationDialogDriver(() => GetTree().GetNodeOrNull<PanelContainer>("UI/ConfirmationDialog"));
}


async Task ClickingYesClosesTheDialog() {
    // when
    // we click the yes button.
    await dialogDriver.YesButton.ClickCenter();
    
    // then
    // the dialog should be gone.
    Assert.That(dialogDriver.Visible).IsFalse();
}
```

Note that because of the way drivers are implemented `dialogDriver.YesButton` will never throw a `NullReferenceException` even if the button is currently not present in the tree. This greatly simplifies your testing code. 

### Built-in drivers
 
- [BaseButtonDriver](Drivers/BaseButtonDriver.cs) - a driver base class for button-like UI elements
- [ButtonDriver](Drivers/ButtonDriver.cs) - a driver for buttons
- [Camera2DDriver](Drivers/Camera2DDriver.cs) - a driver for 2D cameras
- [CanvasItemDriver](Drivers/CanvasItemDriver.cs) - a driver for canvas items
- [CheckBoxDriver](Drivers/CheckBoxDriver.cs) - a driver for check boxes
- [ControlDriver](Drivers/ControlDriver.cs) - the root driver class for drivers working on controls, can be used for any control
- [GraphEditDriver](Drivers/GraphEditDriver.cs) - a driver for graph editors
- [GraphNodeDriver](Drivers/GraphNodeDriver.cs) - a driver for graph nodes
- [ItemListDriver](Drivers/ItemListDriver.cs) - a driver for item lists
- [LabelDriver](Drivers/LabelDriver.cs) - a driver for labels
- [LineEditDriver](Drivers/LineEditDriver.cs) - a driver for line edits
- [Node2DDriver](Drivers/Node2DDriver.cs) - a driver for 2D nodes
- [NodeDriver](Drivers/NodeDriver.cs) - the root driver class.
- [OptionButtonDriver](Drivers/OptionButtonDriver.cs) - a driver for option buttons
- [PopupMenuDriver](Drivers/PopupMenuDriver.cs) - a driver for popup menus
- [RichTextLabelDriver](Drivers/RichTextLabelDriver.cs) - a driver for rich text labels
- [TextEditDriver](Drivers/TextEditDriver.cs) - a driver for text edits
- [TweenDriver](Drivers/TweenDriver.cs) - a driver for tweens

## Input
### Simulating mouse input
GodotTest provides a number of extension functions on `Viewport` that allow you to simulate mouse input in a viewport. 

```csharp

// you can move the mouse to a certain position (e.g. for simulating a hover)
await viewport.MoveMouseTo(new Vector2(100, 100));

// you can click at a certain position (default is left mouse button)
await viewport.ClickMouseAt(new Vector2(100, 100));

// you can give a ButtonList argument to click with a different mouse button
await viewport.ClickMouseAt(new Vector2(100, 100), ButtonList.Right);

// you can also send single mouse presses and releases
await viewport.PressMouse();
await viewport.ReleaseMouse();

// there is also built-in support for mouse dragging
// this will press the mouse at the first point, then move it to the 
// second point and release it there.
await viewport.DragMouse(new Vector2(100, 100), new Vector2(400, 400));

// again you can give a ButtonList argument to drag with a different mouse button
await viewport.DragMouse(new Vector2(100, 100), new Vector2(400, 400), ButtonList.Right);
```

All functions will wait until the events have been properly processed.

### Simulating keyboard input
GodotTest provides a number of extension functions on `SceneTree`/`Node` that allow you to simulate keyboard input.

    
```csharp

// you can press down a key
await node.PressKey(KeyList.A);
// you can also specify modifiers (e.g. shift+F1)
await node.PressKey(KeyList.F1, shift: true);
// you can also specify multiple modifiers (e.g. ctrl+shift+F1)
await node.PressKey(KeyList.F1, control: true, shift: true);

// you can release a key
await node.ReleaseKey(KeyList.A);

// you can also combine pressing and releasing a key
await node.TypeKey(KeyList.A);
```

All functions will wait until the events have been properly processed.

## Waiting extensions

GodotTestDriver provides a number of extension functions on `SceneTree` which allow you to wait for certain events to happen. This is a common requirement in integration tests, where you will click or send some key strokes and then some action happens that takes a while to process.

```csharp

Fixture fixture;
// this is a custom driver for the game under test
ArenaDriver arena;

public async Task Setup() {
    fixture = new Fixture(GetTree());
    // add the arena to the scene
    var arenaInstance = fixture.LoadAndAddScene("res://arena.tscn");
    arena = new ArenaDriver(() => arenaInstance);

    // load a monster and put it into the arena
    var monster = fixture.LoadScene<Monster>("res://monster.tscn");
    arena.AddMonster(monster);

    // load a player and put it into the arena
    var player = fixture.LoadScene<Player>("res://player.tscn");
    arena.AddPlayer(player);
}

// you can wait for a certain amount of time for a condition
// to become true
public async Task TestCombat() {
    // when
    // i open the arena gates
    arena.OpenGates();

    // then
    // within 5 seconds the player should be dead because
    // the monster will attack the player.
    await GetTree().WithinSeconds(5, () => {
        // this assertion will be repeatedly run every frame
        // until it either succeeds or the 5 seconds have elapsed  
        Assert.True(arena.Player.IsDead);
    });
}

// you can also check for a condition to stay true for a 
// certain amount of time
public async Task TestGodMode() {
    // setup
    // give god mode to the player
    arena.Player.EnableGodMode();    

    // when
    // i open the arena gates
    arena.OpenGates();

    // then
    // the player will not lose any health within the next 5 seconds 
    await GetTree().DuringSeconds(5, () => {
        // this assertion will be repeatedly run every frame
        // until it either fails or the 5 seconds have elapsed  
        Assert.Equal(arenaDriver.Player.MaxHealth, arenaDriver.Player.Health);
    });
}

```

## FAQ
### Why is everything `async`?

Integration tests in games usually trigger some operation and then need to wait for the operation to have effect. This waiting can last several frames. Using `async` / `await` makes it much easier to write such tests.

### What should I consider when writing my own drivers?
- All calls should succeed if the controlled object is in a suitable state to perform the requested operation. Otherwise these calls should throw an `InvalidOperationException`. For example if you use a `ButtonDriver` and the button is not currently visible when you try to click it, the driver will throw an `InvalidOperationException`.
- All calls that potentially modify state should always be executed in the `Process` phase. You can use the `await GetTree().ProcessFrame()` extension function that is provided by this library to wait for the process phase.
- All calls that raise events should wait for at least two process frames before they return. This is to ensure that the event has been properly processed before the call returns. This way you don't need to litter your tests with code that waits for a few frames. You can use the `await GetTree().WaitForEvents()` extension function that is provided by this library to wait for the events to be processed.
- Producer functions should never throw an exception. If they cannot find the node, they should just return `null`.

