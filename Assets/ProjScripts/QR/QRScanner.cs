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
    private static string AnchorKey;
    public ResolveAnchor Anchorresolve;

    private bool gotQR= false;
    private bool anchorResolved = false;
    private bool waitingToResolve = false;
    private int retryCooldown = 30;
    private int lastAttemptFrame = -30;

    public ARCameraManager cameraManager;
    public FirebaseManager firebaseManager;
    private void Start()
    {
        barcodeReader = new BarcodeReader();
        cameraManager.frameReceived += OnCameraFrameReceived;
    }
    public void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        if ((Time.frameCount % 15) != 0)
            return;

        if (anchorResolved)
        {
            cameraManager.frameReceived -= OnCameraFrameReceived;
            return;
        }

        if (cameraManager.TryAcquireLatestCpuImage(out XRCpuImage cpuImage))
        {
            if (!gotQR)
            {
                StartCoroutine(ProcessQRcode(cpuImage, (resultText) =>
                {
                    if (string.IsNullOrEmpty(AnchorKey))
                    {
                        AnchorKey = resultText;
                    }
                    Debug.Log("QR code scanned with anchor key: " + AnchorKey);
                },
                (found) =>
                {
                    if (found && !string.IsNullOrEmpty(AnchorKey))
                    {
                        Debug.Log("QR Code detected!");
                        gotQR = true;
                        waitingToResolve = true;
                        lastAttemptFrame = Time.frameCount - retryCooldown; // allow immediate first resolve
                    }
                }));


            }
            else if (waitingToResolve && Time.frameCount - lastAttemptFrame >= retryCooldown)
            {
                lastAttemptFrame = Time.frameCount;
                Debug.Log("Attempting to resolve anchor for anchorKey: " + AnchorKey);
                Anchorresolve.StartResolveAnchor(AnchorKey, success =>
                {
                    if (success)
                    {
                        anchorResolved = true;
                        waitingToResolve = false;
                        Debug.Log("Anchor resolved successfully.");

                    }
                    else
                    {
                        Debug.LogWarning("Anchor resolve failed. Will retry...");
                    }
                });
            }
            else
            {
                cpuImage.Dispose(); // prevent leak if skipped
            }
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
            key?.Invoke(result.Text);
            isthere?.Invoke(true);
        }
        else
        {
            isthere?.Invoke(false);
        }
        yield return null;
    }
}
