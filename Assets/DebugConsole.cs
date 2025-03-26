using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DebugConsole : MonoBehaviour
{

    public static DebugConsole Instance;

    [SerializeField] private RectTransform displayrect;
    [SerializeField] private TextMeshProUGUI displaytext;

    private StringBuilder logMessages = new StringBuilder();
    float initialheight;
    private void Awake()
    {
        
        if(Instance != null)
        {
            DestroyImmediate(Instance);
        }
        else
        {
            Instance = this;
        }

        initialheight = displayrect.anchoredPosition.y;

        Application.logMessageReceived += HandleLog;
    }

    public void ChangeDisplayPostion(float newpos)
    {
        displayrect.anchoredPosition = new Vector2(displayrect.anchoredPosition.x, newpos+initialheight);
    }

    private void HandleLog(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error)
        {
            displaytext.text = $"[{type}] {condition}\n{stackTrace}";
        }
        else
        {
            displaytext.text = $"[{type}] {condition}";
        }
    }
}
