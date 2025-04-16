using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MaterialHelper
{
    public static void ApplyTransparentMaterial(GameObject obj, Material transparentMat)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] newMats = new Material[renderer.materials.Length];
            for (int i = 0; i < newMats.Length; i++)
            {
                newMats[i] = transparentMat;
            }
            renderer.materials = newMats;
        }
    }
}
