using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="QRCode",menuName ="RouteQR")]
public class QRCode : ScriptableObject
{
    public string destination;
    public Texture2D QRcode;
}
