using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] int buttonIndex;
    public TabGroup tabGroup;
    private Image background;

    public UnityEvent onTabSelected;
    public UnityEvent onTabDeselected;

    public Image Background { get => background; set => background = value; }

    private void Awake()
    {
        background = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    void Start()
    {
        Background = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }

    public void Select()
    {
        if (onTabSelected != null)
        {
            PlayerManager.instance.Index = buttonIndex;
            onTabSelected.Invoke();
        }
    }

    public void Deselect()
    {
        if (onTabDeselected != null)
        {
            onTabDeselected.Invoke();
        }
    }
}
