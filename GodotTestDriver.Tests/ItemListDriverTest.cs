﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Chickensoft.GoDotTest;
using Godot;
using GodotTestDriver.Drivers;
using JetBrains.Annotations;
using Shouldly;

namespace GodotTestDriver.Tests;

[UsedImplicitly]
public class ItemListDriverTest : DriverTest
{
    private readonly ItemListDriver _itemList;


    public ItemListDriverTest(Node testScene) : base(testScene)
    {
        _itemList = new ItemListDriver(() => RootNode.GetNode<ItemList>("ItemList"));
    }


    [Test]
    public void InspectionWorks()
    {
        // WHEN
        // everything is set up

        // THEN
        // we should have two selectable items
        _itemList.SelectableItems.Count.ShouldBe(2);

        // named "Normal Item 1" and "Normal Item 2"
        _itemList.SelectableItems.First().ShouldBe("Normal Item 1");
        _itemList.SelectableItems.Last().ShouldBe("Normal Item 2");

        // and we should have a total of 4 items
        _itemList.ItemCount.ShouldBe(4);
    }

    [Test]
    public async Task SelectingItemsWorks()
    {
        // WHEN
        // we select the first item
        var signalAwaiter =_itemList.GetSignalAwaiter(ItemList.SignalName.ItemSelected);
        await _itemList.SelectItemWithText("Normal Item 1");

        // THEN
        // the first item is selected
        _itemList.SelectedItems.Count.ShouldBe(1);
        _itemList.SelectedItems.First().ShouldBe("Normal Item 1");

        // and the signal is emitted
        signalAwaiter.IsCompleted.ShouldBeTrue();


        // WHEN
        // we select the second item
        var signalAwaiter2 =_itemList.GetSignalAwaiter(ItemList.SignalName.ItemSelected);
        await _itemList.SelectItemWithText("Normal Item 2");

        // THEN
        // the second item is selected
        _itemList.SelectedItems.Count.ShouldBe(1);
        _itemList.SelectedItems.First().ShouldBe("Normal Item 2");

        // and the signal is emitted
        signalAwaiter2.IsCompleted.ShouldBeTrue();
    }

    [Test]
    public async Task MultiSelectionWorks()
    {
        // WHEN
        // we select the first item
        await _itemList.SelectItemWithText("Normal Item 1");

        // and adding the second item
        await _itemList.SelectItemWithText("Normal Item 2", true);

        // THEN
        // both items are selected
        _itemList.SelectedItems.Count.ShouldBe(2);
        _itemList.SelectedItems.First().ShouldBe("Normal Item 1");
        _itemList.SelectedItems.Last().ShouldBe("Normal Item 2");
    }

    [Test]
    public async Task MassSelectionWorks()
    {
        // WHEN
        // we select all items
        await _itemList.SelectItemsWithText("Normal Item 1", "Normal Item 2");

        // THEN
        // both items are selected
        _itemList.SelectedItems.Count.ShouldBe(2);
        _itemList.SelectedItems.First().ShouldBe("Normal Item 1");
        _itemList.SelectedItems.Last().ShouldBe("Normal Item 2");
    }

    [Test]
    public async Task DeselectionWorks()
    {
        // WHEN
        // we select the first item
        await _itemList.SelectItemWithText("Normal Item 1");

        // and deselect it
        await _itemList.DeselectItemWithText("Normal Item 1");

        // THEN
        // no items are selected
        _itemList.SelectedItems.Count.ShouldBe(0);
    }

    [Test]
    public async Task MassDeselectionWorks()
    {
        // WHEN
        // we select the two items
        await _itemList.SelectItemWithText("Normal Item 1");
        await _itemList.SelectItemWithText("Normal Item 2", true);

        // and deselect all
        await _itemList.DeselectAll();

        // THEN
        // no items are selected
        _itemList.SelectedItems.Count.ShouldBe(0);
    }

    [Test]
    public async Task ActivatingItemsWorks()
    {
        // WHEN
        // we activate the first item
        var signalAwaiter = _itemList.GetSignalAwaiter(ItemList.SignalName.ItemActivated);
        await _itemList.ActivateItemWithText("Normal Item 1");

        // THEN
        // the signal is emitted
        signalAwaiter.IsCompleted.ShouldBeTrue();
    }

    [Test]
    public async Task SelectingDisabledItemsDoesNotWork()
    {
        // WHEN
        // we try to select a disabled item, an InvalidOperationException should be thrown
        await Should.ThrowAsync<InvalidOperationException>(async () => await _itemList.SelectItemWithText("Disabled Item"));
    }

    [Test]
    public async Task SelectingNonExistingItemsDoesNotWork()
    {
        // WHEN
        // we try to select a non-existing item, an InvalidOperationException should be thrown
        await Should.ThrowAsync<InvalidOperationException>(async () => await _itemList.SelectItemWithText("Non-existing Item"));
    }

    [Test]
    public async Task SelectingUnselectableItemsDoesNotWork()
    {
        // WHEN
        // we try to select an unselectable item, an InvalidOperationException should be thrown
        await Should.ThrowAsync<InvalidOperationException>(async () => await _itemList.SelectItemWithText("NonSelectable Item"));
    }
}
