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
    private void Start()
    {
        controller = ARcontroller.Instance;
    }

    public void StartResolveAnchor(string hostanchorid)
    {
            StartCoroutine(resolvingAnchor(hostanchorid));   
    }
    private IEnumerator resolvingAnchor(string hostedanchorid)
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


            }
            else
            {
                
                Debug.LogError("Failed to resolve Cloud Anchor: " + resolveAnchorPromise.Result.CloudAnchorState.ToString());
            }
        }
        else if (resolveAnchorPromise.State == PromiseState.Cancelled)
        {
            
            Debug.LogError("Cloud Anchor resolving was cancelled.");
        }
        else
        {
            
            Debug.LogError("Unexpected PromiseState: " + resolveAnchorPromise.State);
        }
    }
   
}


