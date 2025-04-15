using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MakeRoute : MonoBehaviour
{
    private  Route CurrentRoute;

    public void InitializeRoute(string RouteName,List<Direction> directionlist)
    {
        CurrentRoute = new Route(RouteName,directionlist);
    }
    public void AddRouteDirections(Direction direction)
    {
        CurrentRoute.AddDirection(direction);
    }

    public Route GetCurrentRoute()
    {
        return CurrentRoute;
    }

    public void ClearCurrentRoute()
    {
        CurrentRoute = null;
    }
}
