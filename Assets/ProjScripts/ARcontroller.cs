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
    

   //[HideInInspector]
    //public ApplicationState StateofApplication;


    public AnchorPlacer placer;
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

    //private void Start()
    //{
    //    StateofApplication = ApplicationState.Initial;
    //    UpdateStates();
    //}
    //public void UpdateStates()
    //{
    //    switch (StateofApplication)
    //    {
    //        case ApplicationState.Initial:

               
    //            ARsetup.SetActive(false);
                
    //            break;

    //        case ApplicationState.ARsession:

    //            ARsetup.SetActive(true);
                
    //            break;

    //        case ApplicationState.Scanning:
    //            Debug.Log("logging featurepoints");
    //            break;

    //        case ApplicationState.Hosting:

    //            break;
    //        case ApplicationState.Hosted:
                
    //            break;
    //        case ApplicationState.Resolving:
    //            break;
    //    }
    //}
}
