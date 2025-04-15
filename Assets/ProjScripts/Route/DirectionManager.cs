using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

public class DirectionManager : MonoBehaviour
{
    public Directions directionpresets;
    public DrawPlacer placer;
    public MakeRoute Router;
    public FirebaseManager firebaseManager;
    public HostAnchor hostedanchor;

    private Direction selecteddirection;
    private ARcontroller controller;
    private UIController UIcontroller;
    [HideInInspector]public ARAnchor anchor;
    private List<Direction> Directionslist = new List<Direction>();
    private List<GameObject> placedDirectionModels = new List<GameObject>();



    private void Start()
    {
        controller = ARcontroller.Instance;
        UIcontroller = UIController.Instance;
    }
    public void selectDirection(int typeindex)
    {
        DirectionType type = (DirectionType)typeindex;
        selecteddirection = new Direction(type);
        Debug.Log("direction Selected of type " +  type);
    }

    public void makestart()
    {
        Vector3 position = placer.getplacerpos();
        selectDirection(0);
        GameObject startmodel = selecteddirection.AddintoScene(directionpresets,position,Quaternion.identity);
        anchor = controller.AnchorManager.AttachAnchor(placer.getplane(),placer.getpose());
        Debug.Log("anchor placed with anchor id" + anchor.trackableId);
        UIcontroller.DisableButton(UIcontroller.MakeStartButton);
        UIcontroller.EnableButton(UIcontroller.HostStartButton);

    }

    public void Adddirection()
    {
        Vector3 position = placer.getplacerpos();
        if (selecteddirection != null && selecteddirection.DirectionType == DirectionType.End)  
        {
            UIcontroller.OpeninputField();
            StartCoroutine(UIController.Instance.WaitUntillInput(
                    inputtext =>
                    {
                        if(!String.IsNullOrEmpty(UIcontroller.UserInput))
                        {
                            Directionslist.Add(selecteddirection);
                            Router.InitializeRoute(UIcontroller.UserInput,Directionslist);
                            firebaseManager.SaveRoute(hostedanchor.startpointName, UIcontroller.UserInput, hostedanchor.cloudanchorID);
                            UIcontroller.EnableButton(UIcontroller.MakeNewButton);
                        }
                    },
                    onComplete =>
                    {
                        if (onComplete)
                        {
                            Debug.Log("route made successfully with route name " + UIcontroller.InputField.text);
                        }
                        else
                            Debug.Log("input was cancelled or empty");
                    }
                ));
        }
        else
        {
            GameObject directionmodel = selecteddirection.AddintoSceneasChild(directionpresets, position, Quaternion.identity, anchor.transform.gameObject);
            Debug.Log("Direction placed relative to start: " + directionmodel.transform.localPosition);
            Directionslist.Add(selecteddirection);
            placedDirectionModels.Add(directionmodel);
        }
    }
    public void ClearPreviousRoute()
    {
        foreach (var model in placedDirectionModels)
        {
            Destroy(model);
        }

        placedDirectionModels.Clear();
        Router.ClearCurrentRoute();
    }
}
