using Google.XR.ARCoreExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ProgressBar : MonoBehaviour
{
    private Slider progressSlider;
    public float fillSpeed;
    private float targetValue;
    private  int pointCloudCount = 0;
    public int pointCloudThreshold = 50;
    public float progressBarThreshold = 0.5f;
    

    private ARcontroller controller;
    private void Awake()
    {
        progressSlider =  gameObject.GetComponent<Slider>();
        progressSlider.maxValue = 1;
    }

    private void Start()
    {
        
        controller = ARcontroller.Instance;
        controller.PointCloudManager.pointCloudsChanged += GetPointCloudCount;
        
    }

    private void Update()
    {
        if (progressSlider.value != targetValue)
        {
            progressSlider.value = Mathf.Lerp(progressSlider.value, targetValue, fillSpeed * Time.deltaTime);
        }
    }

    public void IncreamentValueOnMapQuality(FeatureMapQuality quality)
    {
        targetValue = (int)quality;
        Debug.Log("Incremented Value");
        
    }

    public void GetPointCloudCount(ARPointCloudChangedEventArgs args)
    {
        foreach (var pointCloud in controller.PointCloudManager.trackables)
        {
            if (pointCloud.gameObject.activeSelf) 
            {
                pointCloudCount++;
            }
        }
        StartCoroutine(UpdateProgressWithDelay());
    }

    private IEnumerator UpdateProgressWithDelay()
    {
        yield return new WaitForSeconds(0.5f); 

        float progress = Mathf.Clamp01((float)pointCloudCount / pointCloudThreshold) * progressSlider.maxValue;

        if (progress >= progressBarThreshold * progressSlider.maxValue)
        {
            controller.PointCloudManager.pointCloudsChanged -= GetPointCloudCount;
            Debug.Log("Unsubscribed from point cloud changed");
          //IncreamentValueOnMapQuality(ScanMapHelper.Instance.GetCurrentMapQuality());
          //ScanMapHelper.Instance.OnMapQualityChanged += IncreamentValueOnMapQuality;
        }
        else
        {
            targetValue = progress;

        }
    }

    private void OnEnable()
    {
        pointCloudCount = 0; 
        targetValue = 0;
    }
}
