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
    private UIController UIController;
    private ARAnchor placedanchor;
    public  string cloudanchorID;
    private FeatureMapQuality mapQuality;
    public string startpointName;


    public QRGenerator QRgenerator;
    public DirectionManager directionManager;
    public MakeRoute Router;
    public FirebaseManager firebaseManager;
    private void Start()
    {
        if (ARcontroller.Instance == null)
        {
            Debug.LogError("ARcontroller instance is null! Ensure ARcontroller is initialized before calling Hostanchor().");
            return;
        }
        controller = ARcontroller.Instance;
        UIController = UIController.Instance;
        mapQuality = FeatureMapQuality.Insufficient;
        if(QRgenerator == null)
        {
            Debug.Log("QRgenerator null");
        }
    }

    public void Hostanchor()
    {
        StartCoroutine(tryhostinganchor((bool success) =>
        {
            if (success)
            {
                UIController.OpeninputField();
                StartCoroutine(UIController.Instance.WaitUntillInput(
                    inputtext =>
                    {
                        Debug.Log(inputtext);
                    },
                    onComplete =>
                    {
                        if (onComplete)
                        {
                            QRgenerator.MakeQRfromkey(cloudanchorID,UIController.UserInput);
                            firebaseManager.SaveStartingPoint(UIController.UserInput, cloudanchorID);
                            Debug.Log("MadeQR check device  hosting completed");
                            Debug.Log("start saved with routed " + cloudanchorID);
                            startpointName = UIController.UserInput;
                            UIController.EnableButton(UIController.directionpanel.gameObject);
                            UIController.EnableButton(UIController.AddDirectionButton);
                            UIController.DisableButton(UIController.HostStartButton);
                            UIController.CloseinputField();
                            Debug.Log("enabled");
                        }
                    }
                ));
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

        float checkInterval = 0.5f;
        while (mapQuality < FeatureMapQuality.Sufficient)
        {
            mapQuality = controller.AnchorManager.EstimateFeatureMapQualityForHosting(new Pose(Camera.main.transform.position,Camera.main.transform.rotation));
            Debug.Log(mapQuality.ToString());
            yield return new WaitForSeconds(checkInterval);
        }

        Debug.Log("Trying hosting...");

        hostAnchorPromise = controller.AnchorManager.HostCloudAnchorAsync(placedanchor, 1);
        yield return hostAnchorPromise;

        if (hostAnchorPromise.State == PromiseState.Done)
        {
            if (hostAnchorPromise.Result.CloudAnchorState == CloudAnchorState.Success)
            {
                cloudanchorID = hostAnchorPromise.Result.CloudAnchorId;
                Debug.Log("Cloud Anchor hosted successfully with ID: " + cloudanchorID);
               
                
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
}




