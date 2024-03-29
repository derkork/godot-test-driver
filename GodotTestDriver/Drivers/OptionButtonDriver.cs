using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using GodotTestDriver.Util;
using JetBrains.Annotations;

namespace GodotTestDriver.Drivers
{
    /// <summary>
    /// Driver for <see cref="OptionButton"/> controls.
    /// </summary>
    [PublicAPI]
    public partial class OptionButtonDriver<T> : BaseButtonDriver<T> where T : OptionButton
    {
        public OptionButtonDriver(Func<T> producer, string description = "") : base(producer, description)
        {
        }


        /// <summary>
        /// Returns a list of all items in the option button.
        /// </summary>
        public IEnumerable<string> Items
        {
            get
            {
                var uiControl = PresentRoot;
                for (var i = 0; i < uiControl.ItemCount; i++)
                {
                    yield return uiControl.GetItemText(i);
                }
            }
        }

        /// <summary>
        /// Returns a list of all items in the option button that are currently selectable (e.g. not disabled).
        /// </summary>
        public IEnumerable<string> SelectableItems
        {
            get
            {
                var uiControl = PresentRoot;

                for (var i = 0; i < uiControl.ItemCount; i++)
                {
                    if (!uiControl.IsItemDisabled(i) && !uiControl.IsItemSeparator(i))
                    {
                        yield return uiControl.GetItemText(i);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the currently selected item, or null if no item is selected.
        /// </summary>
        public string SelectedItem
        {
            get
            {
                var uiControl = PresentRoot;
                return uiControl.Selected < 0 ? null : uiControl.GetItemText(uiControl.Selected);
            }
        }

        /// <summary>
        /// Returns the currently selected item index, or -1 if no item is selected.
        /// </summary>
        public int SelectedIndex => PresentRoot.Selected;

        /// <summary>
        /// Returns the currently selected item ID, or -1 if no item is selected.
        /// </summary>
        public int SelectedId => PresentRoot.Selected < 0 ? -1 : PresentRoot.GetItemId(PresentRoot.Selected);


        /// <summary>
        /// Returns the index of the item with the given text. Throws an exception if the item is not found or not selectable.
        /// </summary>
        private int IndexOf(string text)
        {
            var uiControl = VisibleRoot;
            for (var i = 0; i < uiControl.ItemCount; i++)
            {
                if (uiControl.GetItemText(i) == text)
                {
                    if (uiControl.IsItemDisabled(i) || uiControl.IsItemSeparator(i))
                    {
                        throw new InvalidOperationException(
                            ErrorMessage($"Item with text '{text}' is not selectable."));
                    }

                    return i;
                }
            }

            throw new InvalidOperationException(
                ErrorMessage($"Option button does not contain item with name '{text}'."));
        }


        /// <summary>
        /// Selects an item with the given text.
        /// </summary>
        public async Task SelectItemWithText(string text)
        {
            var uiControl = VisibleRoot;
            await uiControl.GetTree().NextFrame();
            var index = IndexOf(text);
            uiControl.Select(index);
            // calling this function will not emit the signal so we need to do this ourselves
            uiControl.EmitSignal(OptionButton.SignalName.ItemSelected, index);
            await uiControl.GetTree().WaitForEvents();
        }

        /// <summary>
        /// Selects an item with the given ID.
        /// </summary>
        public async Task SelectItemWithId(int id)
        {
            var uiControl = VisibleRoot;
            await uiControl.GetTree().NextFrame();

            for (var i = 0; i < uiControl.ItemCount; i++)
            {
                if (uiControl.GetItemId(i) != id)
                {
                    continue;
                }

                if (uiControl.IsItemDisabled(i))
                {
                    throw new InvalidOperationException(ErrorMessage($"Item with ID '{id}' is not selectable."));
                }

                uiControl.Select(i);
                // calling this function will not emit the signal so we need to do this ourselves
                uiControl.EmitSignal(OptionButton.SignalName.ItemSelected, i);
                await uiControl.GetTree().WaitForEvents();
                return;
            }

            throw new InvalidOperationException(ErrorMessage($"Option button does not contain item with ID '{id}'."));
        }
    }


    [PublicAPI]
    public sealed class OptionButtonDriver : OptionButtonDriver<OptionButton>
    {
        public OptionButtonDriver(Func<OptionButton> producer, string description = "") : base(producer, description)
        {
        }
    }
}