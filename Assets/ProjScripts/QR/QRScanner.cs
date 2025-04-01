using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;

public class QRScanner : MonoBehaviour
{

    private IBarcodeReader barcodeReader;
    private Texture2D cameraTexture;
    private Texture2D ARcameratexture;
    private static string RouteKey;
    public ResolveAnchor Anchorresolve;

    public ARCameraManager cameraManager;
    private void Start()
    {
        barcodeReader = new BarcodeReader();
        cameraManager.frameReceived += OnCameraFrameReceived;
    }


    public void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        if ((Time.frameCount % 15) != 0)
            return;
        if (cameraManager.TryAcquireLatestCpuImage(out XRCpuImage cpuImage))
        {
            StartCoroutine(ProcessQRcode(cpuImage, (resultText) =>
            {
                RouteKey = resultText;
                Debug.Log("resolve initiated" + "with routekey" + RouteKey);
                Anchorresolve.StartResolveAnchor(RouteKey);
                
                

            },
            (found) =>
            {
                Debug.Log("QR Code detected: " + found);
            }
            ));
            
        }
    }

    private IEnumerator ProcessQRcode(XRCpuImage image, System.Action<string> key, System.Action<bool> isthere)
    {
        XRCpuImage.ConversionParams conversionParams = new XRCpuImage.ConversionParams
        {
            inputRect = new RectInt(0, 0, image.width, image.height),
            outputDimensions = new Vector2Int(image.width / 2, image.height / 2),
            outputFormat = TextureFormat.RGB24
        };

        var rawTextureData = new Texture2D(conversionParams.outputDimensions.x, conversionParams.outputDimensions.y, conversionParams.outputFormat, false);
        NativeArray<byte> rawData = new NativeArray<byte>(image.GetConvertedDataSize(conversionParams), Allocator.Temp);
 
        image.Convert(conversionParams, rawData);

   
        rawTextureData.LoadRawTextureData(rawData);
        rawTextureData.Apply();

        rawData.Dispose();
        image.Dispose();  

        if (cameraTexture != null)
            Destroy(cameraTexture);

        cameraTexture = rawTextureData;

        // Decode QR Code
        var source = new RGBLuminanceSource(cameraTexture.GetRawTextureData(), cameraTexture.width, cameraTexture.height);
        var result = barcodeReader.Decode(source);

        if (result != null)
        {
            Debug.Log("QR Code Detected: " + result.Text);
            isthere?.Invoke(true);
            key?.Invoke(result.Text);
        }
        else
        {
            isthere?.Invoke(false);
        }
        yield return null;
    }
}
