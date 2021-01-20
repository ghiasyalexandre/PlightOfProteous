using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;
    public TabButton selectedTab;
    public Color tabIdle;
    public Color tabHover;
    public Color tabActive;

    public void Subscribe(TabButton button)
    {
        if (tabButtons == null)
            tabButtons = new List<TabButton>();
    }

    public void OnTabExit(TabButton button)
    {
        ResetTabs();
        button.Background.color = tabIdle;
    }

    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
            button.Background.color = tabHover;
    }

    public void OnTabSelected(TabButton button)
    {
        if (selectedTab != null)
            selectedTab.Deselect();
        selectedTab = button;
        selectedTab.Select();

        ResetTabs();
        button.Background.color = tabActive;
    }

    public void ResetTabs()
    {
        foreach(TabButton button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab)
                continue;
            button.Background.color = tabIdle;
        }
    }
}
