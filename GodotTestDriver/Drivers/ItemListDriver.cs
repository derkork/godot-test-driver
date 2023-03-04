using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using GodotTestDriver.Util;
using JetBrains.Annotations;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// Driver for an ItemList control.
    /// </summary>
    [PublicAPI]
    public partial class ItemListDriver<T> : ControlDriver<T> where T:ItemList
    {
        
        public ItemListDriver(Func<T> producer, string description = "") : base(producer, description)
        {
        }

        /// <summary>
        /// Returns a list of all items in the list that are currently selectable.
        /// </summary>
        public List<string> SelectableItems
        {
            get
            {
                var uiControl = PresentRoot;
                
                var result = new List<string>();
                for (var i = 0; i < uiControl.ItemCount; i++)
                {
                    if (uiControl.IsItemSelectable(i))
                    {
                        result.Add(uiControl.GetItemText(i));
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Returns true if something is selected.
        /// </summary>
        public bool HasSelection => PresentRoot.GetSelectedItems().Length > 0;
        
        
        /// <summary>
        /// Returns the currently selected items.
        /// </summary>
        public List<string> SelectedItems
        {
            get
            {
                var uiControl = PresentRoot;
                return uiControl.GetSelectedItems()
                    .Select(i => uiControl.GetItemText(i)).ToList();
            }
        }
        
        
        /// <summary>
        /// Returns the indices of the currently selected items.
        /// </summary>
        public List<int> SelectedIndices
        {
            get
            {
                var uiControl = PresentRoot;
                return uiControl.GetSelectedItems().ToList();
            }
        }
        

        /// <summary>
        /// Selects an item with the given text.
        /// </summary>
        public async Task SelectItemWithText(string text)
        {
            var uiControl = VisibleRoot;
            await uiControl.GetTree().NextFrame();

            for (var i = 0; i < uiControl.ItemCount; i++)
            {
                if (uiControl.GetItemText(i) != text)
                {
                    continue;
                }
                
                if (!uiControl.IsItemSelectable(i))
                {
                    throw new InvalidOperationException(ErrorMessage($"Item with text '{text}' is not selectable."));
                }
                
                uiControl.Select(i);
                // calling this function will not emit the signal so we need to do this ourselves
                uiControl.EmitSignal(ItemList.SignalName.ItemSelected, i);
                await uiControl.GetTree().WaitForEvents();
                return;
            }
            
            throw new InvalidOperationException(ErrorMessage($"List does not contain item with text '{text}'."));
        }
    }
    
    /// <summary>
    /// Driver for an ItemList control.
    /// </summary>
    [PublicAPI]
    public sealed class ItemListDriver : ItemListDriver<ItemList>
    {
        public ItemListDriver(Func<ItemList> producer, string description = "") : base(producer, description)
        {
        }
    }
}