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
        }
        else
        {
            Destroy(gameObject); 
        }
        ;
        //Application.targetFrameRate = 60;
    }

    private void InitializeManagers()
    {
        // Look for managers in the active scene
        Session = FindObjectOfType<ARSession>();
        PlaneManager = FindObjectOfType<ARPlaneManager>();
        RaycastManager = FindObjectOfType<ARRaycastManager>();
        AnchorManager = FindObjectOfType<ARAnchorManager>();
        PointCloudManager = FindObjectOfType<ARPointCloudManager>();

        if (Session == null) Debug.LogWarning("ARSession not found in scene.");
        if (PlaneManager == null) Debug.LogWarning("ARPlaneManager not found in scene.");
        if (RaycastManager == null) Debug.LogWarning("ARRaycastManager not found in scene.");
        if (AnchorManager == null) Debug.LogWarning("ARAnchorManager not found in scene.");
        if (PointCloudManager == null) Debug.LogWarning("ARPointCloudManager not found in scene.");
    }

}
