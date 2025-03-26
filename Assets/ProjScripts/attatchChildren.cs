using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class AttatchDirections : MonoBehaviour
{
    private DrawPlacer placer;

    public GameObject ChildPrefab;

    private GameObject ChildObject;

    private ARcontroller controller;

    private void Start()
    {
            controller = ARcontroller.Instance;
    }

    public void InstantiateChild()
    {
        Vector3 WorldPos = controller.placer.placerUI.getplacerpos();
        Vector3 localposition = controller.placer.placedAnchor.transform.InverseTransformPoint(WorldPos);
        Transform placedanchor = controller.placer.placedAnchor.transform;
        ChildObject = Instantiate(ChildPrefab, placedanchor);
        ChildObject.transform.localPosition = localposition;    
        
    }

}
