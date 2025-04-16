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

    private GameObject previewDirectionModel;

    private GameObject directionmodel;

    private Quaternion currentRotation = Quaternion.identity;

    [SerializeField] private Material transparentMaterial;

    private void Start()
    {
        controller = ARcontroller.Instance;
        UIcontroller = UIController.Instance;
    }
    private void Update()
    {
        HandleRotationGesture();
    }

    public void selectDirection(int typeindex)
    {
        DirectionType type = (DirectionType)typeindex;
        selecteddirection = new Direction(type);
        Debug.Log("direction Selected of type " +  type);

        if(type == 0)
        {
            return;
        }

        if (previewDirectionModel != null)
            Destroy(previewDirectionModel);

        Vector3 pos = placer.getplacerpos();
        previewDirectionModel = selecteddirection.AddintoScene(directionpresets, pos, currentRotation);
        MaterialHelper.ApplyTransparentMaterial(previewDirectionModel,transparentMaterial);
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

                            

                            Debug.Log("Direction placed relative to start: " + directionmodel.transform.localPosition);
                            
                            placedDirectionModels.Add(directionmodel);
                        }
                    },
                    onComplete =>
                    {
                        if (onComplete)
                        {
                            directionmodel = selecteddirection.AddintoSceneasChild(
                            directionpresets,
                            previewDirectionModel.transform.position,
                            previewDirectionModel.transform.rotation,
                            anchor.transform.gameObject);

                            Destroy(previewDirectionModel);
                            previewDirectionModel = null;

                            Debug.Log("route made successfully with route name " + UIcontroller.InputField.text);
                        }
                        else
                            Debug.Log("input was cancelled or empty");
                    }
                ));
        }
        else
        {
            directionmodel = selecteddirection.AddintoSceneasChild(
                directionpresets,
                previewDirectionModel.transform.position,
                previewDirectionModel.transform.rotation,
                anchor.transform.gameObject);

            Destroy(previewDirectionModel);
            previewDirectionModel = null;

            Debug.Log("Direction placed relative to start: " + directionmodel.transform.localPosition);
            Directionslist.Add(selecteddirection);
            placedDirectionModels.Add(directionmodel);
        }
        if(selecteddirection == null)
        {
            Debug.Log("no selected direction");
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


    private void HandleRotationGesture()
    {
        if (previewDirectionModel == null)
            return;

        
        previewDirectionModel.transform.position = placer.getplacerpos();

        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 prevTouch0 = touch0.position - touch0.deltaPosition;
            Vector2 prevTouch1 = touch1.position - touch1.deltaPosition;

            float prevAngle = Mathf.Atan2(prevTouch1.y - prevTouch0.y, prevTouch1.x - prevTouch0.x) * Mathf.Rad2Deg;
            float currentAngle = Mathf.Atan2(touch1.position.y - touch0.position.y, touch1.position.x - touch0.position.x) * Mathf.Rad2Deg;

            float angleDelta = currentAngle - prevAngle;

           

            currentRotation *= Quaternion.Euler(0, angleDelta, 0);
            Debug.Log($"Updated Current Rotation: {currentRotation.eulerAngles}");

            previewDirectionModel.transform.rotation = currentRotation;
        }
    }
}
