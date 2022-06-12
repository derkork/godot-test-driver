# Godot Test Driver

This library provides an API that simplifies writing integration tests for Godot projects. It is agnostic of the test framework that you use, so you can use it with any C# test framework that works with Godot (or no framework at all if you prefer).

**Note: This is currently in development. The API is not stable.** 

## Usage overview
### Fixture

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
    
    // This is a cleanup method. Like the setup method, the exact
    // way of how stuff is cleaned up differs from framework to
    // framework, but most have a cleanup method.
    async Task TearDown() {
        // dispose of anything we created during the test.
        await Fixture.Cleanup();
    }
}
```

## Test drivers
### Introduction

Test drivers serve as an abstraction layer between your test code and your game code. They are a high level interface through which the tests can "see" the game and interact with it. With a test driver, your game tests do not need to know how the game works under the hood. This makes your tests a lot more robust to change.





## FAQ
### Why is everything `async`?

Integration tests in games usually trigger some operation and then need to wait for the operation to have effect. This waiting can last several frames. Using `async` / `await` makes it much easier to write such tests.

## Driver API design guidelines
- Calls that read state will always succeed.
- All other calls will succeed if the controlled object is in a suitable state to perform the requested operation. Otherwise these calls will throw an `InvalidOperationException`.

