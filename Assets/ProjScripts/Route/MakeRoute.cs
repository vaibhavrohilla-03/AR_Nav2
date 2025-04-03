using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MakeRoute : MonoBehaviour
{
    private  Route CurrentRoute;

    public void InitializeRoute(string RouteName)
    {
        CurrentRoute = new Route(RouteName);
    }
    public void AddRouteDirections(Direction direction,Vector3 position)
    {
        CurrentRoute.AddDirection(direction);
        direction.position = new DirectionPosition(position);
    }

    public Route GetCurrentRoute()
    {
        return CurrentRoute;
    }
}
