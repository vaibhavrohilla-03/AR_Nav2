using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class AnchorPlacer : MonoBehaviour
{

    [HideInInspector]public ARAnchor placedAnchor {  get; private set; }
    public DrawPlacer placerUI;
    private ARcontroller controller;
    private void Start()
    {
        controller = ARcontroller.Instance;
    }
    public void placeAnchor()
    {

        if (placerUI != null)
        {
            Debug.Log("placer is not null");
            var plane = placerUI.getplane();
            Pose pose = placerUI.getpose();

            if (plane == null)
            {
                Debug.LogWarning("No valid plane found to place anchor.");
                return;
            }
            else
            {
                placedAnchor = controller.AnchorManager.AttachAnchor(plane, pose);
                //placedAnchor.transform.position = placer.getplacerpos();

            }
        }
        else
        {
            Debug.Log("placer is null");
        }
       
    }
}
