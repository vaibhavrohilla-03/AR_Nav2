using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class SimpleObjectPlace : MonoBehaviour
{
    [SerializeField] private GameObject ObjectToPlace;
    [SerializeField] private InputActionReference touchpos_refrence;
    [SerializeField] private InputActionReference touchpress_refrence;
    [SerializeField] private ARRaycastManager raycastmanager;

    private GameObject spawnedObject;

    private void Start()
    {
        touchpos_refrence.action.Enable();
        touchpress_refrence.action.Enable();
        touchpress_refrence.action.performed += OnPressed;
        Debug.Log("scriptenabled");


    }

    //private void Update()
    //{
    //    if (touchpress_refrence.action.IsPressed())
    //    {
    //        Vector2 touchPosition = touchpos_refrence.action.ReadValue<Vector2>();
    //        Debug.Log("Touch input detected at: " + touchPosition);
    //    }
    //    else
    //    {
    //        Debug.Log("No input detected.");
    //    }
    //}

    private bool CastRay(Vector2 touchposition)
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastmanager.Raycast(touchposition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {

            Pose hitPose = hits[0].pose;
            PlaceObject(hitPose);
            return true;

        }

        return false;
    }

    private void PlaceObject(Pose hitPose)
    {

        if (ObjectToPlace != null)
        {
            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(ObjectToPlace, hitPose.position, hitPose.rotation);

            }
            else
            {
                spawnedObject.transform.position = hitPose.position;
                spawnedObject.transform.rotation = hitPose.rotation;

            }
        }

    }

    public  GameObject GetObject()
    {
        if(ObjectToPlace != null)
            return ObjectToPlace;

        else
        {
            Debug.Log("Object to place isn't set ");
            return null;

        }
    }





    private void OnPressed(InputAction.CallbackContext context)
    {
        Vector2 touchPosition = touchpos_refrence.action.ReadValue<Vector2>();

        CastRay(touchPosition);
        Debug.Log("pressed");

    }


    private void OnDestroy()
    {
        touchpress_refrence.action.performed -= OnPressed;
    }
}
