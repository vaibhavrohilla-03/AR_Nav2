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
    private Direction selecteddirection;
    private ARcontroller controller;
    [HideInInspector]public ARAnchor anchor;

    
    private void Start()
    {
        controller = ARcontroller.Instance;
    }
    public void selectDirection(int typeindex)
    {
        DirectionType type = (DirectionType)typeindex;
        selecteddirection = new Direction(type);
        Debug.Log("direction Selected of type " +  type);
    }
    public void Adddirection()
    {
        Vector3 position = placer.getplacerpos();
        if(selecteddirection != null && selecteddirection.DirectionType == DirectionType.Start)
        {
            GameObject directionmodel =  selecteddirection.AddintoScene(directionpresets, position,Quaternion.identity);
            anchor =  directionmodel.AddComponent<ARAnchor>();
            Debug.Log("anchor placed with anchor id" + anchor.trackableId);
            Router.InitializeRoute("TestNamewill replace with user input");
            Router.AddRouteDirections(selecteddirection,position);
        }
        else
        {
            if (Router.GetCurrentRoute() == null || Router.GetCurrentRoute().directions.Count == 0)
            {
                Debug.LogError("Start direction must be placed first.");
                return;
            }

            Direction startDirection = Router.GetCurrentRoute().directions[0];
            GameObject startObject = GameObject.FindWithTag("Start"); 
            if (startObject == null)
            {
                Debug.LogError("Start object not found in scene.");
                return;
            }

            GameObject directionmodel = selecteddirection.AddintoSceneasChild(directionpresets, position, Quaternion.identity, startObject);
            Vector3 relativePosition = startObject.transform.InverseTransformPoint(position);

            Debug.Log("Direction placed relative to start: " + relativePosition);
            Router.AddRouteDirections(selecteddirection, relativePosition);
        }
    }

}
