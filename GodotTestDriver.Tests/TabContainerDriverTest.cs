﻿using System.Threading.Tasks;
using Chickensoft.GoDotTest;
using Godot;
using GodotTestDriver.Drivers;
using JetBrains.Annotations;
using Shouldly;

namespace GodotTestDriver.Tests;

[UsedImplicitly]
public class TabContainerDriverTest : DriverTest
{
    private readonly TabContainerDriver _tabContainer;
    private readonly ControlDriver<Control> _firstTabContent;
    private readonly ControlDriver<Control> _secondTabContent;

    public TabContainerDriverTest(Node testScene) : base(testScene)
    {
        _tabContainer = new TabContainerDriver(() => RootNode.GetNode<TabContainer>("TabContainer"));
        _firstTabContent = new ControlDriver<Control>(() => _tabContainer.Root?.FindChild("First Tab Content") as Control);
        _secondTabContent = new ControlDriver<Control>(() => (_tabContainer.Root?.FindChild("Second Tab Content") as Control));
    }

    [Test]
    public void InspectionWorks()
    {
        // WHEN
        // everything is set up

        // THEN
        // the tab container has two tabs
        _tabContainer.TabCount.ShouldBe(2);
        // the first tab is selected
        _tabContainer.SelectedTabIndex.ShouldBe(0);
        // the first tab has the correct title
        _tabContainer.SelectedTabTitle.ShouldBe("First Tab");
        // the first tab has the correct content
        _firstTabContent.IsVisible.ShouldBeTrue();
        _secondTabContent.IsVisible.ShouldBeFalse();
    }

    // changing a tab works
    [Test]
    public async Task ChangingTabsWorks()
    {
        // WHEN
        // the second tab is selected
        await _tabContainer.SelectTabWithTitle("Second Tab");

        // THEN
        // the second tab is selected
        _tabContainer.SelectedTabIndex.ShouldBe(1);
        // the second tab has the correct title
        _tabContainer.SelectedTabTitle.ShouldBe("Second Tab");
        // the second tab has the correct content
        _firstTabContent.IsVisible.ShouldBeFalse();
        _secondTabContent.IsVisible.ShouldBeTrue();
    }
}
