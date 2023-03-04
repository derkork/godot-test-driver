using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using GodotTestDriver.Util;
using JetBrains.Annotations;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// Driver for a popup menu.
    /// </summary>
    [PublicAPI]
    public partial class PopupMenuDriver<T> : WindowDriver<T> where T:PopupMenu
    {
        public PopupMenuDriver(Func<T> producer, string description = "") : base(producer, description)
        {
        }

        /// <summary>
        /// Returns the amount of items in the popup menu.
        /// </summary>
        public int ItemCount => PresentRoot.ItemCount;
        
        /// <summary>
        /// Returns the text of the items in the popup menu.
        /// </summary>
        public IEnumerable<string> MenuItems
        {
            get
            {
                for(var i = 0; i < ItemCount; i++)
                {
                    yield return PresentRoot.GetItemText(i);
                }
            }
        }
        
        /// <summary>
        /// Returns the text of the items in the popup menu which are currently selectable (e.g. not disabled and no separator).
        /// </summary>
        public IEnumerable<string> SelectableMenuItems
        {
            get
            {
                for(var i = 0; i < ItemCount; i++)
                {
                    if(PresentRoot.IsItemDisabled(i) || PresentRoot.IsItemSeparator(i))
                    {
                        continue;
                    }
                    yield return PresentRoot.GetItemText(i);
                }
            }
        }

        /// <summary>
        /// Returns whether the item at the given index is checked.
        /// </summary>
        public bool IsItemChecked(int index)
        {
            VerifyIndex(index);
            return PresentRoot.IsItemChecked(index);
        }
        
        /// <summary>
        /// Returns whether the item at the given index is disabled.
        /// </summary>
        public bool IsItemDisabled(int index)
        {
            VerifyIndex(index);
            return PresentRoot.IsItemDisabled(index);
        }

        /// <summary>
        /// Returns whether the item at the given index is a separator.
        /// </summary>
        public bool IsItemSeparator(int index)
        {
            VerifyIndex(index);
            return PresentRoot.IsItemSeparator(index);
        }
        
        /// <summary>
        /// Returns the ID of the item at the given index.
        /// </summary>
        public int GetItemId(int index)
        {
            VerifyIndex(index);
            return PresentRoot.GetItemId(index);
        }
        
        /// <summary>
        /// Selects the item at the given index.
        /// </summary>
        public async Task SelectItemAtIndex(int index)
        {
            var popup = VisibleRoot;
            VerifyIndex(index);

            // verify that item is not disabled
            if (popup.IsItemDisabled(index))
            {
                throw new InvalidOperationException(
                    $"Item at index {index} is disabled and cannot be selected.");
            }
            
            // verify that item is not a separator
            if (popup.IsItemSeparator(index))
            {
                throw new InvalidOperationException(
                    $"Item at index {index} is a separator and cannot be selected.");
            }
            
            await popup.GetTree().NextFrame();
            
            // select item
            // ideally we would use a mouse click here but since the API does not provide the position of
            // each entry, we have to fake it.
            popup.EmitSignal(PopupMenu.SignalName.IndexPressed, index);
            popup.Hide();
            await popup.GetTree().WaitForEvents();
        }

        /// <summary>
        /// Verifies that the given index is valid.
        /// </summary>
        private void VerifyIndex(int index)
        {
            // verify index is in range
            if (index < 0 || index >= ItemCount)
            {
                throw new IndexOutOfRangeException(
                    $"Index {index} is out of range for popup menu with {ItemCount} items.");
            }
        }

        /// <summary>
        /// Selects the item with the given ID.
        /// </summary>
        public async Task SelectItemWithId(int id)
        {
            var popup = PresentRoot;
            for(var i = 0; i < ItemCount; i++)
            {
                if (popup.GetItemId(i) != id)
                {
                    continue;
                }
                await SelectItemAtIndex(i);
                return;
            }
            
            throw new InvalidOperationException(
                $"No item with id {id} found in popup menu.");
        }
        
        /// <summary>
        /// Selects the item with the given text. If multiple items have the same text, the first one is selected.
        /// </summary>
        public async Task SelectItemWithText(string text)
        {
            var popup = PresentRoot;
            for(var i = 0; i < ItemCount; i++)
            {
                if (popup.GetItemText(i) != text)
                {
                    continue;
                }
                await SelectItemAtIndex(i);
                return;
            }
            
            throw new InvalidOperationException(
                $"No item with text {text} found in popup menu.");
        }
    }

    
    /// <summary>
    /// Driver for a popup menu.
    /// </summary>
    [PublicAPI]
    public sealed class PopupMenuDriver : PopupMenuDriver<PopupMenu>
    {
        public PopupMenuDriver(Func<PopupMenu> producer, string description = "") : base(producer, description)
        {
        }
    }
    
}