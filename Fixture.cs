using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using GodotTestDriver.Util;
using JetBrains.Annotations;
using Object = Godot.Object;

namespace GodotTestDriver
{
    /// <summary>
    /// This is a test fixture that can be use to manage nodes in the test scene. It ensures that modifications to the scene are
    /// always executed in the main thread. It also provides a simple means of cleaning up the scene after each test.
    /// </summary>
    [PublicAPI]
    public class Fixture
    {
        private List<Func<Task>> _cleanupSteps = new List<Func<Task>>();
        
        /// <summary>
        /// The <see cref="SceneTree"/> that this fixture is managing.
        /// </summary>
        public SceneTree Tree { get; }

        public Fixture(SceneTree tree)
        {
            Tree = tree;
        }

        /// <summary>
        /// Adds a node to the root of the tree. When this call returns the node will be in the scene and its
        /// <see cref="Node._Ready"/> function will have been called by Godot. 
        /// </summary>
        public async Task<T> AddToRoot<T>(T node) where T :Node
        {
            await Tree.ProcessFrame();
            
            Tree.Root.AddChild(node);
            
            await Tree.WaitForEvents(); // make sure _Ready is called.
            return node;
        }

        /// <summary>
        /// Loads a scene from the given path and adds it to the root of the tree. The scene will also
        /// be registered as the <see cref="SceneTree.CurrentScene"/>. Finally the scene will be scheduled
        /// for automatic cleanup when this fixture is cleaned up.
        /// </summary>
        public async Task<T> LoadAndAddScene<T>(string path) where T : Node
        {
            var instance = await LoadScene<T>(path);
            var result =  await AddToRoot(instance);
            Tree.CurrentScene = instance;
            return result;
        }

        
        /// <summary>
        /// Loads a scene from the given path and instantiates it. The instance will be scheduled for automatic cleanup
        /// when this fixture is cleaned up.
        /// </summary>
        public async Task<T> LoadScene<T>(string path) where T : Node
        {
            // make sure we run in the main thread
            await Tree.ProcessFrame();
            return AutoFree(GD.Load<PackedScene>(path).Instance<T>());
        }
        
        
        /// <summary>
        /// Schedules the given object to be automatically freed when this fixture is cleaned up. Returns the object that was
        /// given to it so this call can be nicely chained.
        /// </summary>
        public T AutoFree<T>(T toFree) where T : Object
        {
            // References are automatically freed, so calling AutoFree on references is wrong.
            if (toFree is Reference)
            {
                Log.Error("Trying to auto-free a reference type. I will ignore this but you should probably not do this.");
                return toFree;
            }
            
            AddCleanupStep(() =>
            {
                // it can happen that a node is already freed when the auto-free is running because
                // the test freed it, or a parent node was freed.
                if (Object.IsInstanceValid(toFree))
                {
                    toFree.Free();
                }
            });
            return toFree;
        }

        /// <summary>
        /// Runs all accumulated cleanup steps. Cleanup steps are run in reverse order in which they were added (e.g.
        /// last added is run first).
        /// </summary>
        public async Task Cleanup()
        {
            // ensure we run on the main thread.
            await Tree.ProcessFrame();
            
            if (_cleanupSteps.Count > 0)
            {
                // run cleanup in reverse order, so inner cleanup is done first.
                _cleanupSteps.Reverse();
                
                Log.Info($"Running {_cleanupSteps.Count} cleanup actions.");
                // run all cleanup actions.
                foreach (var action in _cleanupSteps)
                {
                    await action();
                }
                
                _cleanupSteps.Clear();
            }
        }
        
        /// <summary>
        /// Allows to register an asynchronous cleanup action that runs when <see cref="Cleanup"/> is called.
        /// </summary>
        /// <param name="action"></param>
        protected void AddCleanupStep(Func<Task> action)
        {
            _cleanupSteps.Add(action);  
        }
        
        /// <summary>
        /// Allows to register a synchronous cleanup action that runs when <see cref="Cleanup"/> is called
        /// </summary>
        /// <param name="action"></param>
        public void AddCleanupStep(Action action)
        {
#pragma warning disable 1998
            _cleanupSteps.Add(async () => action());
#pragma warning restore 1998
        }

    }
}