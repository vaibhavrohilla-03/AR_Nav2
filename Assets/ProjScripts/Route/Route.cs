using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Route
{
    public string RouteName;
    public List<Direction> directions;

    public Route(string routename) 
    {
        RouteName = routename;
        directions = new List<Direction>();
    }
    public Route(string routename, List<Direction> directions)
    {
        RouteName = routename;
        this.directions = directions;
    }
    public void AddDirection(Direction direction)
    {
        directions.Add(direction);
    }
    public void RemoveDirection()
    {
        if(directions.Count > 0)
        {
            directions.RemoveAt(directions.Count - 1);
        }
    }
}

