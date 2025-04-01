using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Google.XR.ARCoreExtensions;

public class HostAnchor : MonoBehaviour
{
    private HostCloudAnchorResult hostingresult;
    private HostCloudAnchorPromise hostAnchorPromise;
    private ARcontroller controller;
    private ARAnchor placedanchor;
    [HideInInspector]public static string cloudanchorID;
    private FeatureMapQuality mapQuality;

    public QRGenerator QRgenerator;
    public DirectionManager directionManager;
    private void Start()
    {
        if (ARcontroller.Instance == null)
        {
            Debug.LogError("ARcontroller instance is null! Ensure ARcontroller is initialized before calling Hostanchor().");
            return;
        }
        controller = ARcontroller.Instance;
        mapQuality = FeatureMapQuality.Insufficient;
        if(QRgenerator == null)
        {
            Debug.Log("QRgenerator null");
        }
    }

    public void Hostanchor()
    {

        StartCoroutine(tryhostinganchor((bool success) => {
            if (success)
            {
                QRgenerator.MakeQRfromkey(cloudanchorID);
                Debug.Log("MadeQRcheckdevice  hosting completed");
            }
            else
            {
                Debug.LogError("Anchor hosting failed.");
            }
        }));

    }

    public IEnumerator tryhostinganchor(System.Action<bool> Oncompleted)
    {
        placedanchor = directionManager.anchor;
        if (placedanchor == null)
        {
            Debug.Log("no anchor to place");
            yield return new WaitForEndOfFrame();
            Oncompleted?.Invoke(false);
            yield break;

        }
        if (hostAnchorPromise != null || hostingresult != null)
        {
            Debug.Log("another hosting is ongoing");
            yield return new WaitForEndOfFrame();
            Oncompleted?.Invoke(false);
            yield break;
        }

        
        while (mapQuality < FeatureMapQuality.Sufficient)
        {
            mapQuality = controller.AnchorManager.EstimateFeatureMapQualityForHosting(new Pose(Camera.main.transform.position,Camera.main.transform.rotation));
            Debug.Log(mapQuality.ToString());
            yield return null;
        }
       
        hostAnchorPromise = controller.AnchorManager.HostCloudAnchorAsync(placedanchor, 1);
        yield return hostAnchorPromise;

        if (hostAnchorPromise.State == PromiseState.Done)
        {
            if (hostAnchorPromise.Result.CloudAnchorState == CloudAnchorState.Success)
            {
                cloudanchorID = hostAnchorPromise.Result.CloudAnchorId;
                Debug.Log("Cloud Anchor hosted successfully with ID: " + cloudanchorID);
               // CopyToAndroidClipboard(cloudanchorID);
                
                Oncompleted?.Invoke(true);
            }
            else
            {
                Debug.LogError("Failed to host Cloud Anchor: " + hostAnchorPromise.Result.CloudAnchorState.ToString());
                Oncompleted?.Invoke(false);
                yield break;
            }

        }
        else if(hostAnchorPromise.State == PromiseState.Cancelled)
        {
            Debug.LogError("Cloud Anchor hosting was cancelled.");
            Oncompleted?.Invoke(false);
            yield return false;
        }
        else
        {
            Debug.LogError("Unexpected PromiseState: " + hostAnchorPromise.State);
            Oncompleted?.Invoke(false);
            yield return false;
        }
    }



    //private void CopyToAndroidClipboard(string text)
    //{
    //    if (Application.platform == RuntimePlatform.Android)
    //    {
    //        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
    //        using (AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
    //        using (AndroidJavaObject clipboardManager = context.Call<AndroidJavaObject>("getSystemService", "clipboard"))
    //        using (AndroidJavaClass clipDataClass = new AndroidJavaClass("android.content.ClipData"))
    //        using (AndroidJavaObject clipData = clipDataClass.CallStatic<AndroidJavaObject>("newPlainText", "label", text))
    //        {
    //            clipboardManager.Call("setPrimaryClip", clipData);
    //            Debug.Log("Cloud Anchor ID copied to Android clipboard!");
    //        }
    //    }
    //}

    
    //private AndroidJavaObject GetAndroidContext()
    //{
    //    using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
    //    {
    //        return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    //    }
    //}
}




