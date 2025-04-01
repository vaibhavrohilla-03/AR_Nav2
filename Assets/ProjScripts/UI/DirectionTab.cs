using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionTab : MonoBehaviour
{
    private bool closed;
    public Vector2 Openpos;
    public LeanTweenType curve;
    private Vector2 Initialpos;
    private RectTransform rectTransform;
    private void Start()
    {
        closed = true;
        rectTransform = GetComponent<RectTransform>();
        Initialpos = rectTransform.anchoredPosition;
    }
    public void OpenNClose()
    {   
        if(closed)
        {
            LeanTween.move(rectTransform, Openpos, 0.5f).setEase(curve);
            closed = false;

        }
        else
        {
            LeanTween.move(rectTransform, Initialpos, 0.5f).setEase(curve);
            closed= true;
        }
    }



}
