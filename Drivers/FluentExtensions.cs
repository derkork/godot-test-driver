using System;
using Godot;
using JetBrains.Annotations;
using YoHDot.Tests.TestDrivers.BuiltInNodes;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// Fluent extensions which make it easier to create test drivers.s
    /// </summary>
    [PublicAPI]
    public static class FluentExtensions
    {
        public static Func<T> Producer<T>(this NodeDriver driver, NodePath path) where T : Node
            => driver.RootNode.Producer<T>(path);

        public static Func<T> Producer<T>([CanBeNull] this Node node, NodePath path) where T:Node 
            => () => node?.GetNode<T>(path);

        // Button
        public static ButtonDriver ButtonDriver(this NodeDriver driver, NodePath path) 
            => new ButtonDriver(driver.Producer<Button>(path));

        public static ButtonDriver ButtonDriver(this Node node, NodePath path)
            => new ButtonDriver(node.Producer<Button>(path));
        
        public static ButtonDriver ButtonDriver(this Button button)
            => new ButtonDriver(() => button);

        // Camera2D
        public static Camera2DDriver Camera2DDriver(this NodeDriver driver, NodePath path) 
            => new Camera2DDriver(driver.Producer<Camera2D>(path));
        
        public static Camera2DDriver Camera2DDriver(this Node node, NodePath path)
            => new Camera2DDriver(node.Producer<Camera2D>(path));
        
        public static Camera2DDriver Camera2DDriver(this Camera2D camera)
            => new Camera2DDriver(() => camera);
        
        // ItemList
        public static ItemListDriver ItemListDriver(this NodeDriver driver, NodePath path) 
            => new ItemListDriver(driver.Producer<ItemList>(path));
        
        public static ItemListDriver ItemListDriver(this Node node, NodePath path)
            => new ItemListDriver(node.Producer<ItemList>(path));
        
        public static ItemListDriver ItemListDriver(this ItemList itemList)
            => new ItemListDriver(() => itemList);
        
        // Label
        public static LabelDriver LabelDriver(this NodeDriver driver, NodePath path) 
            => new LabelDriver(driver.Producer<Label>(path));
        
        public static LabelDriver LabelDriver(this Node node, NodePath path)
            => new LabelDriver(node.Producer<Label>(path));
        
        public static LabelDriver LabelDriver(this Label label)
            => new LabelDriver(() => label);
        
        // LineEdit
        public static LineEditDriver LineEditDriver(this NodeDriver driver, NodePath path) 
            => new LineEditDriver(driver.Producer<LineEdit>(path));
        
        public static LineEditDriver LineEditDriver(this Node node, NodePath path)
            => new LineEditDriver(node.Producer<LineEdit>(path));
        
        public static LineEditDriver LineEditDriver(this LineEdit lineEdit)
            => new LineEditDriver(() => lineEdit);
        
        // RichTextLabel
        public static RichTextLabelDriver RichTextLabelDriver(this NodeDriver driver, NodePath path) 
            => new RichTextLabelDriver(driver.Producer<RichTextLabel>(path));
        
        public static RichTextLabelDriver RichTextLabelDriver(this Node node, NodePath path)
            => new RichTextLabelDriver(node.Producer<RichTextLabel>(path));
        
        public static RichTextLabelDriver RichTextLabelDriver(this RichTextLabel richTextLabel)
            => new RichTextLabelDriver(() => richTextLabel);
        
        // Tween
        public static TweenDriver TweenDriver(this NodeDriver driver, NodePath path) 
            => new TweenDriver(driver.Producer<Tween>(path));
        
        public static TweenDriver TweenDriver(this Node node, NodePath path)
            => new TweenDriver(node.Producer<Tween>(path));
        
        public static TweenDriver TweenDriver(this Tween tween)
            => new TweenDriver(() => tween);
        
    }
}