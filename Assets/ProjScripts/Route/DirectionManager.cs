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

    [HideInInspector] public UnityEvent OnAddingDirection;
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
            Router.InitializeRoute("test Route");
            Router.AddRouteDirections(selecteddirection);
            OnAddingDirection.Invoke();
        }
        else
        {
           GameObject directionmodel =  selecteddirection.AddintoScene(directionpresets, position,Quaternion.identity);
           Debug.Log("direction placed of type" + selecteddirection.DirectionType);
            Router.AddRouteDirections(selecteddirection);
           OnAddingDirection.Invoke();
        }
    }

}
