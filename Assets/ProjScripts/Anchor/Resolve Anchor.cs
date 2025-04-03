using Google.XR.ARCoreExtensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ResolveAnchor : MonoBehaviour
{
   
    private ResolveCloudAnchorPromise resolveAnchorPromise;
    private ARCloudAnchor resolvedAnchor;
    private ARcontroller controller;

    public FirebaseManager firebasemanager;
    private void Start()
    {
        controller = ARcontroller.Instance;
    }

    public void StartResolveAnchor(string hostanchorid)
    {
        StartCoroutine(resolvingAnchor(hostanchorid, success =>
        {
            if (success && resolvedAnchor != null)
            {
                firebasemanager.LoadRoute(hostanchorid,resolvedAnchor.gameObject);
            }
            else
            {
                Debug.LogError("Failed to resolve anchor. Route loading skipped.");
            }
        }));
    }
    private IEnumerator resolvingAnchor(string hostedanchorid, System.Action<bool> Oncompleted)
    {

        if (resolveAnchorPromise != null)
        {
            Debug.Log("already resolving another session");
            yield return null;
        }

        Debug.Log("Resolving Anchor");
        resolveAnchorPromise = controller.AnchorManager.ResolveCloudAnchorAsync(hostedanchorid);
        yield return resolveAnchorPromise;

        if (resolveAnchorPromise.State == PromiseState.Done)
        {
            if (resolveAnchorPromise.Result.CloudAnchorState == CloudAnchorState.Success)
            {
               
                resolvedAnchor = resolveAnchorPromise.Result.Anchor;
                Debug.Log("Cloud Anchor resolved successfully. Anchor position: " + resolvedAnchor.transform.position);
                Debug.Log("Getting resolved anchor " + resolvedAnchor.pose.position);
                Oncompleted?.Invoke(true);


            }
            else
            {
                
                Debug.LogError("Failed to resolve Cloud Anchor: " + resolveAnchorPromise.Result.CloudAnchorState.ToString());
                Oncompleted?.Invoke(false);
            }
        }
        else if (resolveAnchorPromise.State == PromiseState.Cancelled)
        {
            
            Debug.LogError("Cloud Anchor resolving was cancelled.");
            Oncompleted?.Invoke(false);
        }
        else
        {
            
            Debug.LogError("Unexpected PromiseState: " + resolveAnchorPromise.State);
            Oncompleted?.Invoke(false);
        }
    }
   
}


