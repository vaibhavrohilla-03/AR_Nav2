using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using ZXing.QrCode;
public class QRGenerator : MonoBehaviour
{

    public void MakeQRfromkey(string key, string filename)
    {
        Texture2D QR = generateQR(key);
        if (QR == null)
        {
            Debug.LogError("QR Texture generation failed!");
            return;
        }

        if (string.IsNullOrEmpty(key))
        {
            Debug.LogError("QR Key is null or empty");
            return;
        }

        SaveToGallery(QR,filename);
    }


    private static Color32[] Encode(string textForEncoding,int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }

    private Texture2D generateQR(string text)
    {
        var encoded = new Texture2D(256, 256);
        var color32 = Encode(text, encoded.width, encoded.height);
        encoded.SetPixels32(color32);
        encoded.Apply();
        return encoded;
    }

    private void SaveToGallery(Texture2D QR,string QRname)
    {
        NativeGallery.SaveImageToGallery(QR, "AR navigation", QRname);
    }

}
