using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopInput : MonoBehaviour
{
    public TMP_InputField Inputfield;
    private TouchScreenKeyboard keyboard;
    private bool inputReceived = false;
    public string userInput = "";

    public void Start()
    {
        Inputfield.onSelect.AddListener(delegate { OpenKeyboard(); });
        Inputfield.onEndEdit.AddListener(OnInputReceived);
    }
    public void OpenKeyboard()
    {
        if (keyboard == null || !keyboard.active)
        {
            keyboard = TouchScreenKeyboard.Open(Inputfield.text, TouchScreenKeyboardType.Default);
        }

    }
    public void popinputField()
    {   
        inputReceived = false;
        Inputfield.gameObject.SetActive(true);
        Inputfield.Select();
        Inputfield.ActivateInputField();
    }

    public void closeinputField()
    {
        Inputfield.gameObject.SetActive(false);
        Inputfield.DeactivateInputField();
        Inputfield.text = "";
        if (keyboard != null && keyboard.active)
        {
            keyboard.active = false;
        }
        inputReceived = false;
    }

    private void OnInputReceived(string input)
    {
        userInput = input;
        inputReceived = true;
    }

    public bool Isreceived()
    {
        return inputReceived;
    }
}
