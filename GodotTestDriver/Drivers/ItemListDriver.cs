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
                    if (uiControl.IsItemSelectable(i) && !uiControl.IsItemDisabled(i))
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
        /// Returns the number of items in the list.
        /// </summary>
        public int ItemCount => PresentRoot.ItemCount;
        
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
        /// Returns the index of the item with the given text. Throws an exception if the item is not found.
        /// </summary>
        private int IndexOf(string text)
        {
            var uiControl = PresentRoot;
            for (var i = 0; i < uiControl.ItemCount; i++)
            {
                if (uiControl.GetItemText(i) == text)
                {
                    return i;
                }
            }

            throw new InvalidOperationException(ErrorMessage($"List does not contain item with text '{text}'."));
        }
        

        /// <summary>
        /// Selects an item with the given text.
        /// </summary>
        public async Task SelectItemWithText(string text, bool addToSelection = false)
        {
            var uiControl = VisibleRoot;
            
            if (addToSelection && uiControl.SelectMode != ItemList.SelectModeEnum.Multi)
            {
                throw new InvalidOperationException(ErrorMessage("Cannot select multiple items because selection mode is not allowing it."));
            }
            
            await uiControl.GetTree().NextFrame();
            var i = IndexOf(text);
            if (!uiControl.IsItemSelectable(i) || uiControl.IsItemDisabled(i))
            {
                throw new InvalidOperationException(ErrorMessage($"Item with text '{text}' is not selectable."));
            }
            
            // check if the item is already selected
            var wasSelectedBefore = uiControl.IsSelected(i);

            uiControl.Select(i, !addToSelection);
            // calling this function will not emit the signal so we need to do this ourselves
            // if the item was already selected before then only emit the signal if AllowReselect is true
            if (!wasSelectedBefore || uiControl.AllowReselect)
            {
                uiControl.EmitSignal(ItemList.SignalName.ItemSelected, i);
            }
            
            await uiControl.GetTree().WaitForEvents();
        }
        
        /// <summary>
        /// Selects multiple items with the given texts.
        /// </summary>
        public async Task SelectItemsWithText(IEnumerable<string> texts)
        {
            foreach (var text in texts)
            {
                await SelectItemWithText(text, true);
            }
        }
        
        /// <summary>
        /// Selects multiple items with the given texts.
        /// </summary>
        public async Task SelectItemsWithText(params string[] texts)
        {
            await SelectItemsWithText(texts.AsEnumerable());
        }

        
        /// <summary>
        /// Deselects an item with the given text.
        /// </summary>
        public async Task DeselectItemWithText(string text)
        {
            var uiControl = VisibleRoot;
            await uiControl.GetTree().NextFrame();
            
            var i = IndexOf(text);
            if (!uiControl.IsItemSelectable(i) || uiControl.IsItemDisabled(i))
            {
                throw new InvalidOperationException(ErrorMessage($"Item with text '{text}' is not selectable."));
            }

            uiControl.Deselect(i);
            await uiControl.GetTree().WaitForEvents();
        }
        
        /// <summary>
        /// Deselects all items.
        /// </summary>
        public async Task DeselectAll()
        {
            var uiControl = VisibleRoot;
            await uiControl.GetTree().NextFrame();
            uiControl.DeselectAll();
            await uiControl.GetTree().WaitForEvents();
        }
        
        /// <summary>
        /// Activates the item with the given text.
        /// </summary>
        public async Task ActivateItemWithText(string text)
        {
            var uiControl = VisibleRoot;
            await SelectItemWithText(text);
            
            // send the activation signal
            uiControl.EmitSignal(ItemList.SignalName.ItemActivated, IndexOf(text));
            await uiControl.GetTree().WaitForEvents();
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