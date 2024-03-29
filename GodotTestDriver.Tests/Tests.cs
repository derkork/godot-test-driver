using Chickensoft.GoDotTest;
using System.Reflection;
using Godot;

namespace GodotTestDriver.Tests;


public partial class Tests : Node2D {
  /// <summary>
  /// Called when the node enters the scene tree for the first time.
  /// </summary>
  public override void _Ready()
  {
    CallDeferred("Go");
  }

  private void Go()
  {
    GoTest.RunTests(Assembly.GetExecutingAssembly(), this);

  }
}
