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
    [HideInInspector] public string userInput = "";
    private bool isEditorTesting = false;

    public void Start()
    {
        // Check if we're in the editor
#if UNITY_EDITOR
        isEditorTesting = true;
#endif

        Inputfield.onSelect.AddListener(delegate { OpenKeyboard(); });
        Inputfield.onEndEdit.AddListener(OnInputFieldEndEdit);
    }
    public void OpenKeyboard()
    {
        if (!isEditorTesting)
        {
            if (keyboard == null || !keyboard.active)
            {
                keyboard = TouchScreenKeyboard.Open(Inputfield.text, TouchScreenKeyboardType.Default);
            }
        }
    }
    public void popinputField()
    {
        inputReceived = false;
        userInput = "";
        Inputfield.text = "";
        Inputfield.gameObject.SetActive(true);
        Inputfield.Select();
        Inputfield.ActivateInputField();
        StartCoroutine(MonitorInput());
    }
    public void closeinputField()
    {
        Inputfield.gameObject.SetActive(false);
        Inputfield.DeactivateInputField();
        Inputfield.text = "";
        if (!isEditorTesting && keyboard != null && keyboard.active)
        {
            keyboard.active = false;
        }
    }
    private void OnInputFieldEndEdit(string input)
    {
        // This catches both hardware keyboard Enter presses and mouse clicks away from the field
        if (!string.IsNullOrEmpty(input) && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            userInput = input;
            inputReceived = true;
            Debug.Log("Input received via Enter key: " + userInput);
        }
    }
    private IEnumerator MonitorInput()
    {
        // In Editor: primarily rely on Enter key and OnInputFieldEndEdit
        // On Device: monitor TouchScreenKeyboard status

        while (!inputReceived && Inputfield.gameObject.activeInHierarchy)
        {
            // For Editor testing - check for Enter key continuously
            if (isEditorTesting)
            {
                if (Inputfield.isFocused && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
                {
                    if (!string.IsNullOrEmpty(Inputfield.text))
                    {
                        userInput = Inputfield.text;
                        inputReceived = true;
                        Debug.Log("Input received in Editor via Enter key: " + userInput);
                    }
                }
            }
            // For Android device - check TouchScreenKeyboard status
            else if (keyboard != null)
            {
                if (keyboard.status == TouchScreenKeyboard.Status.Done)
                {
                    if (!string.IsNullOrEmpty(Inputfield.text))
                    {
                        userInput = Inputfield.text;
                        inputReceived = true;
                        Debug.Log("Input received from touch keyboard: " + userInput);
                    }
                }
                else if (keyboard.status == TouchScreenKeyboard.Status.Canceled)
                {
                    // User canceled input
                    Debug.Log("Input canceled");
                    break;
                }
            }

            yield return null;
        }
    }
    public bool Isreceived()
    {
        return inputReceived && !string.IsNullOrEmpty(userInput);
    }
    // Helper method for testing in editor - can be triggered via Inspector button
    public void EditorSubmitCurrentText()
    {
        if (!string.IsNullOrEmpty(Inputfield.text))
        {
            userInput = Inputfield.text;
            inputReceived = true;
            Debug.Log("Input submitted manually in Editor: " + userInput);
        }
    }
}