using System;
using System.Collections.Generic;
using Godot;
using JetBrains.Annotations;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// Driver for an ItemList control.
    /// </summary>
    [PublicAPI]
    public class ItemListDriver : ControlDriver<ItemList>
    {
        
        public ItemListDriver(Func<ItemList> producer) : base(producer)
        {
        }

        /// <summary>
        /// Returns a list of all items in the list that are currently selectable.
        /// </summary>
        public List<string> SelectableItems
        {
            get
            {
                var uiControl = Root;
                
                if (uiControl == null)
                {
                    return new List<string>();
                }
                
                var result = new List<string>();
                for (var i = 0; i < uiControl.GetItemCount(); i++)
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
        public bool HasSelection => Root != null && Root.GetSelectedItems().Length > 0;
        
        
        /// <summary>
        /// Selects an item with the given name.
        /// </summary>
        /// <param name="name"></param>
        public void Select(string name)
        {
            var uiControl = VisibleRoot;
            for (var i = 0; i < uiControl.GetItemCount(); i++)
            {
                if (!uiControl.IsItemSelectable(i) || uiControl.GetItemText(i) != name)
                {
                    continue;
                }
                
                uiControl.Select(i);
                // calling this function will not emit the signal so we need to do this ourselves
                uiControl.EmitSignal("item_selected", i);
                return;
            }
            
            throw new InvalidOperationException("List does not contain item with name " + name);
        }
    }
}