using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName ="Directions",menuName ="Direction")]
public class Directions : ScriptableObject
{
    public GameObject Start;
    public GameObject DirectionLeft;
    public GameObject DirectionRight;
    public GameObject DirectionStraight;
    public GameObject End;
    public GameObject getDirection(DirectionType direction)
    {   
        switch(direction)
        {
            case DirectionType.Start:
                return Start;

            case DirectionType.DirectionLeft:
                return DirectionLeft;

            case DirectionType.DirectionRight:
                return DirectionRight;

            case DirectionType.DirectionStraight:
                return DirectionStraight;

            case DirectionType.End:
                return End;

            default:
                return null;
        }
         
    }
}


