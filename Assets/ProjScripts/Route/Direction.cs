using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


[System.Serializable]
public class Direction
{
    public DirectionType DirectionType;
    public DirectionPosition position;
    public DirectionRotation rotation;

    public Direction(DirectionType type)
    {
        DirectionType = type;
        position = new DirectionPosition(Vector3.zero);
        rotation = new DirectionRotation(Quaternion.identity);
    }
    public Direction(DirectionType directionType, Vector3 position, Quaternion rotation)
    {
        DirectionType = directionType;
        this.position = new DirectionPosition(position);
        this.rotation = new DirectionRotation(rotation);
    }
    public GameObject AddintoScene(Directions preset,Vector3 position, Quaternion rotation)
    {
        this.position =  new DirectionPosition(position);
        this.rotation =  new DirectionRotation(rotation);
        return Object.Instantiate(preset.getDirection(DirectionType), position, rotation);
    }

    public GameObject AddintoSceneasChild(Directions preset, Vector3 position, Quaternion rotation, GameObject parent)
    {
        GameObject instance = Object.Instantiate(preset.getDirection(DirectionType), position, rotation);
        instance.transform.SetParent(parent.transform);
        Vector3 localposition  = parent.transform.InverseTransformPoint(position);
        instance.transform.localPosition = localposition;
        this.position = new DirectionPosition(instance.transform.localPosition);
        this.rotation = new DirectionRotation(instance.transform.localRotation);

        return instance;

    }
}

[System.Serializable]
public class DirectionPosition
{
    public float x;
    public float y;
    public float z;

    public DirectionPosition(Vector3 position)
    {
        this.x = position.x;
        this.y = position.y;
        this.z = position.z;
    }
}

[System.Serializable]
public class DirectionRotation
{
    public float x;
    public float y;
    public float z;
    public float w;


    public DirectionRotation(Quaternion rotation) 
    {
        
        this.x = rotation.x;
        this.y = rotation.y;
        this.z = rotation.z;
        this.w = rotation.w;
    }
}