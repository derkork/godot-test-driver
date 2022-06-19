using System;
using System.Threading.Tasks;
using Godot;
using JetBrains.Annotations;

namespace GodotTestDriver.Util
{
    [PublicAPI]
    public static class WaitExtensions
    {
        /// <summary>
        /// Waits for the given amount of time that the given action performs without any exception. If the action
        /// throws an exception it will be repeated until it no longer throws an exception or the timeout is reached.
        /// When the timeout is reached the last exception will be thrown.
        ///
        /// This can be used to wait for an assertion to become true within a certain period of time.
        /// </summary>
        public static async Task WithinSeconds(this SceneTree tree, float seconds, Action action)
        {
            var timeout = new Timeout(seconds);
            do
            {
                await tree.ProcessFrame();
                try
                {
                    action();
                    return;
                }
                catch (Exception)
                {
                    if (timeout.IsReached)
                    {
                        throw;
                    }
                }
            } while (true);
        }

        /// <summary>
        /// Waits for the given amount of time that the given action performs without any exception. If the action
        /// throws an exception it will be repeated until it no longer throws an exception or the timeout is reached.
        /// When the timeout is reached the last exception will be thrown.
        ///
        /// This can be used to wait for an assertion to become true within a certain period of time.
        /// </summary>
        public static async Task WithinSeconds(this Node node, float seconds, Action action)
        {
            await node.GetTree().WithinSeconds(seconds, action);
        }


        /// <summary>
        /// Waits for the given amount of seconds for the given condition to be true. If the condition is not true
        /// after the timeout is reached an exception will be thrown.
        /// </summary>
        public static async Task WithinSeconds(this SceneTree tree, float seconds, Func<bool> condition)
        {
            var timeout = new Timeout(seconds);
            do
            {
                await tree.ProcessFrame();
                if (condition())
                {
                    return;
                }

            } while (!timeout.IsReached);
            
            throw new TimeoutException("Condition was not true within the given time.");
        }
        
        /// <summary>
        /// Waits for the given amount of seconds for the given condition to be true. If the condition is not true
        /// after the timeout is reached an exception will be thrown.
        /// </summary>
        public static async Task WithinSeconds(this Node node, float seconds, Func<bool> condition)
        {
            await node.GetTree().WithinSeconds(seconds, condition);
        }
        
        /// <summary>
        /// Runs the given action repeatedly every frame for the given amount of seconds. If the action throws an
        /// exception, the exception will be thrown and the loop will be stopped. This can be used to check an
        /// assertion to be true for a certain period of time.
        /// </summary>
        public static async Task DuringSeconds(this SceneTree tree, float seconds, Action action)
        {
            var timeout = new Timeout(seconds);
            do
            {
                action();
                await tree.ProcessFrame();
            } while (!timeout.IsReached);
        }
        
        /// <summary>
        /// Runs the given action repeatedly every frame for the given amount of seconds. If the action throws an
        /// exception, the exception will be thrown and the loop will be stopped. This can be used to check an
        /// assertion to be true for a certain period of time.
        /// </summary>
        public static async Task DuringSeconds(this Node node, float seconds, Action action)
        {
            await node.GetTree().DuringSeconds(seconds, action);
        }

        /// <summary>
        /// Waits for the events triggered by the most recent action to be processed.
        /// </summary>
        public static async Task WaitForEvents(this SceneTree tree)
        {
            await tree.ProcessFrame(2);
        }

        /// <summary>
        /// Waits until the given amount of frames have passed. Returns in the context of the `Process` method
        /// on the main thread.
        /// </summary>
        public static async Task ProcessFrame(this SceneTree tree, int frames = 1)
        {
            // add a temporary node to the tree and wait until it's process method is called.
            var processWaiter = new ProcessWaiter();
            processWaiter.CountDown = frames;
            tree.Root.AddChild(processWaiter);

            // node will destroy itself when it's process method is called.
            await processWaiter.ToSignal(processWaiter, nameof(ProcessWaiter.OnProcess));
        }

        /// <summary>
        /// Waits until the given amount of physics frames have passed. Returns in the context of the `PhysicsProcess`
        /// method on the main thread.
        /// </summary>
        public static async Task PhysicsProcessFrame(this SceneTree tree, int frames = 1)
        {
            // add a temporary node to the tree and wait until it's process method is called.
            var processWaiter = new ProcessWaiter();
            processWaiter.CountDown = frames;
            tree.Root.AddChild(processWaiter);
            
            // node will destroy itself when it's PhysicsProcess method is called.
            await processWaiter.ToSignal(processWaiter, nameof(ProcessWaiter.OnPhysicsProcess));
        }
    }
}