using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

 

public class ARcontroller : MonoBehaviour
{

    public static ARcontroller Instance { get; private set; }

    // AR Foundation Managers
    public ARSession Session;
    public GameObject ARsetup;
    public ARPlaneManager PlaneManager;
    public ARRaycastManager RaycastManager;
    public ARPointCloudManager PointCloudManager;
    public ARAnchorManager AnchorManager;
    //public AnchorPlacer placer;
    

   //[HideInInspector]
    //public ApplicationState StateofApplication;


    //public enum ApplicationState
    //{   Initial,
    //    ARsession,
    //    Scanning,
    //    placingArrows,
    //    Hosting,
    //    Hosted,
    //    Resolving,
    //}

    private void Awake()
    {
       
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
        //Application.targetFrameRate = 60;
    }

  
}
