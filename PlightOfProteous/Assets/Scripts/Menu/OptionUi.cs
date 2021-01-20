using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUi : MonoBehaviour
{
    public Button soundButton;
    public Button optionButton;

    public void Start()
    {
        soundButton.onClick.AddListener(ToggleSound);
        optionButton.onClick.AddListener(OptionOpen);
    }

    void OptionOpen()
    {

    }

    void ToggleSound()
    {

    }
}
