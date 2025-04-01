using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeRoute : MonoBehaviour
{
    public  Route CurrentRoute;
    public void InitializeRoute(string RouteName)
    {
        CurrentRoute = new Route(RouteName);

    }

    public void AddRouteDirections(Direction direction)
    {
        CurrentRoute.AddDirection(direction);
    }


}
