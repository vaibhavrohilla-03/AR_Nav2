using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static UIController Instance;
    public TMP_InputField InputField;
    [HideInInspector] public string UserInput;
    public GameObject MakeStartButton;
    public DirectionTab directionpanel;
    public GameObject AddDirectionButton;
    public GameObject HostStartButton;
    public GameObject MakeNewButton;
    private bool inputreceived = false;

    private TouchScreenKeyboard keyboard;
    private float pointerDownTime;
    private float tapThreshold = 0.3f;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        InputField.onSelect.AddListener(delegate { OpenKeyboard(); });
        InputField.onEndEdit.AddListener(OnInputFieldEndEdit);
    }

    public void OpenKeyboard()
    {
        if (keyboard == null || !keyboard.active)
        {
            keyboard = TouchScreenKeyboard.Open(InputField.text, TouchScreenKeyboardType.Default);
        }

        InputField.Select();
        InputField.ActivateInputField();
    }
    public void OpeninputField()
    {
        
        StartCoroutine(OpenInputFieldDelayed());
    }
    private IEnumerator OpenInputFieldDelayed()
    {
        InputField.text = "";
        InputField.gameObject.SetActive(true);
        yield return null; 
        InputField.Select();
        InputField.ActivateInputField();
        OpenKeyboard();

        inputreceived = false;
    }
    public void CloseinputField(bool resetinputfield = true)
    {
        InputField.gameObject.SetActive(false);
        InputField.DeactivateInputField();
        if (keyboard != null && !keyboard.active)
        {
            keyboard.active = false;
        }
        keyboard = null;

        if(resetinputfield)
        {
            inputreceived = false;
            InputField.text = "";
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        float heldTime = Time.time - pointerDownTime;
        if (heldTime < tapThreshold)
        {
            OpenKeyboard();
        }
       
    }

    public  void OnInputFieldEndEdit(string input)
    {
        
        if (!string.IsNullOrEmpty(input))
        {
            UserInput = input;
            inputreceived = true;
            CloseinputField(false);
        }
    }

    public bool Isreceived()
    {
        return inputreceived;
    }
    public void DisableButton(GameObject Button)
    {
        Button.SetActive(false);
    }

    public void EnableButton(GameObject Button)
    {
        Button.SetActive(true);
    }
    public void OpenandcloseDirectionPanel()
    {
        directionpanel.OpenNClose();
    }
    public IEnumerator WaitUntillInput(Action<string> returntext,Action<bool> OnSuccess)
    {
        Debug.Log("Waiting for input...");
        yield return new WaitUntil(() => Isreceived());
        Debug.Log("Input received: " + InputField.text);

        if (!String.IsNullOrEmpty(InputField.text)) 
        {

            Debug.Log("Calling OnSuccess with true");
            OnSuccess?.Invoke(true);
            returntext?.Invoke(InputField.text);
        }
        else
        {
            Debug.Log("Calling OnSuccess with false");
            OnSuccess?.Invoke(false);
        }
    }
}
