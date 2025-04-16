using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class DrawPlacer : MonoBehaviour
{

    [SerializeField] private GameObject PlacerPrefab;
    private GameObject placer;
    private ARcontroller controller;
    private Vector3 hitposition;
    private Quaternion hitrotation;
    //public static DrawPlacer Instance { get; private set; }
    List<ARRaycastHit> hitList;

    private ARPlane plane;
    private void Start()
    {
        if (controller == null)
            controller = ARcontroller.Instance;

        if (controller == null)
        {
            Debug.LogWarning("ARcontroller.Instance is null in DrawPlacer Start()");
            return;
        }



        hitList = new List<ARRaycastHit>();
        placer = Instantiate(PlacerPrefab);
        hitposition = placer.transform.position + (placer.transform.forward * 2f) ;
        hitrotation = placer.transform.rotation ;


    }

    private void Update()
    {
        if (controller == null)
        {
            controller = ARcontroller.Instance;
            if (controller == null)
                return;
        }

        if (castray(out hitposition, out hitrotation))
        {
            placer.transform.position = Vector3.Lerp(placer.transform.position, hitposition , Time.deltaTime * 10f);
            placer.transform.rotation = Quaternion.Lerp(placer.transform.rotation, hitrotation, Time.deltaTime * 10f);
        }

    }


    private bool castray( out Vector3 hitPosition, out Quaternion hitRotation)
    {   
        Ray ray = new Ray(transform.position, transform.forward);
        if (controller.RaycastManager.Raycast(ray, hitList, TrackableType.PlaneWithinPolygon))
        {
            TrackableId id = hitList[0].trackableId;

            plane = controller.PlaneManager.GetPlane(id);

            if (plane != null)
            {
                if (plane.alignment == PlaneAlignment.HorizontalUp)
                {   
                    hitRotation = Quaternion.AngleAxis(90.0f,Vector3.right);
                    hitPosition = hitList[0].pose.position +new Vector3(0,0.15f,0);

                }
                else if (plane.alignment == PlaneAlignment.Vertical)
                {
                    hitRotation = Quaternion.LookRotation(plane.normal * 90.0f);
                    hitPosition = hitList[0].pose.position +  (plane.normal* 0.015f);
                }
                else
                {
                    hitRotation = Quaternion.identity; 
                    hitPosition = Vector3.zero;
                }
            }
            else
            {
                hitRotation = Quaternion.identity;
                hitPosition = Vector3.zero;

            }
            return true;
        }
        else
        {
            hitRotation = Quaternion.identity;
            hitPosition = Vector3.zero;
            return false;
        }

    }

    public Vector3 getplacerpos()
    {
        return hitposition;
    }

    public ARPlane getplane()
    {
        if(plane != null)
            return plane;
        else
            Debug.Log("NULL PLANE");
            return null;
    }

    public Pose getpose()
    {   if (hitList.Count > 0)
        {
            return hitList[0].pose;
        }
        else
            return Pose.identity;
    }

    
}
