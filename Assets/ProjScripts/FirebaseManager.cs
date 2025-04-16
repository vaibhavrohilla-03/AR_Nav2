using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }
    public DatabaseReference DBReference { get; private set; }

    public MakeRoute Router;

    public Directions directionPresets;

    private List<GameObject> StoredDirections = new List<GameObject>();

    private List<GameObject> storedButtons = new List<GameObject>();

    //[HideInInspector]public UnityEvent OnFirebaseInitialize = new UnityEvent();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeFirebase();
        
    }

    public void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogError("Failed to initialize Firebase: " + task.Exception);
                return;
            }
            Debug.Log("Firebase Initialized");
            DBReference = FirebaseDatabase.DefaultInstance.RootReference; 
            Debug.Log(DBReference.Database.App.Options.DatabaseUrl);
            //OnFirebaseInitialize.Invoke();
        });
    }
    public void SaveRoute(string startingpointName,string destination,string cloudanchorid)
    {
        Route route = Router.GetCurrentRoute();
        if (route == null || string.IsNullOrEmpty(route.RouteName))
        {
            Debug.Log("current route is empty");
            return;
        }
        string json = JsonUtility.ToJson(route);
        Debug.Log(json);


        DBReference.Child("startingPoints").Child(cloudanchorid).Child(startingpointName).Child(destination).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
                Debug.Log("Route saved under starting point: " + startingpointName);
            else
                Debug.LogError("Failed to save route: " + task.Exception);
        }
        );
    }

    public void SaveStartingPoint(string startingPointName, string cloudAnchorID)
    {
        DatabaseReference reference = DBReference
            .Child("startingPoints")   
            .Child(cloudAnchorID)  
            .Child(startingPointName);     

        reference.SetValueAsync(true).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
                Debug.Log($"Starting point '{startingPointName}' registered with anchor '{cloudAnchorID}'");
            else
                Debug.LogError("Failed to register starting point: " + task.Exception);
        });
    }


    public void LoadRouteForDestination(string cloudAnchorId, string startingPointName, string destinationName, GameObject resolvedAnchor)
    {   



        DBReference
            .Child("startingPoints")
            .Child(cloudAnchorId)
            .Child(startingPointName)
            .Child(destinationName)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (!task.IsCompleted || !task.Result.Exists)
                {
                    Debug.LogWarning("Route data missing or failed to load.");
                    return;
                }

                string json = task.Result.GetRawJsonValue();
                Route loadedRoute = JsonUtility.FromJson<Route>(json);

                if (resolvedAnchor == null)
                {
                    Debug.LogError("Resolved anchor is null. Cannot spawn directions.");
                    return;
                }

                ClearList(StoredDirections);

                foreach (var direction in loadedRoute.directions)
                {
                    Vector3 relPos = new Vector3(direction.position.x, direction.position.y, direction.position.z);
                    Quaternion relRot = new Quaternion(direction.rotation.x, direction.rotation.y, direction.rotation.z, direction.rotation.w);

                    GameObject prefab = directionPresets.getDirection(direction.DirectionType);
                    if (prefab != null)
                    {
                        GameObject instance = Instantiate(prefab);
                        instance.transform.SetParent(resolvedAnchor.transform, false);
                        instance.transform.localPosition = relPos;
                        instance.transform.localRotation = relRot;
                        StoredDirections.Add(instance);

                        Debug.Log($"Spawned {direction.DirectionType} at {relPos}");
                    }
                    else
                    {
                        Debug.LogError($"Missing prefab for {direction.DirectionType}");
                    }
                }
            });
    }

    public void GetDestinations(string CloudAnchorID,GameObject resolvedanchor)
    {
        DatabaseReference reference = DBReference
       .Child("startingPoints")
       .Child(CloudAnchorID);


        reference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.Result == null)
            {
                Debug.LogError("Failed to retrieve starting points.");
                return;
            }

            DataSnapshot anchorSnapshot = task.Result;

            DataSnapshot firstStartingPointSnap = null;
            foreach (DataSnapshot snap in anchorSnapshot.Children)
            {
                firstStartingPointSnap = snap;
                break;
            }

            if (firstStartingPointSnap == null)
            {
                Debug.LogWarning("No starting points found.");
                return;
            }

            string startingPointName = firstStartingPointSnap.Key;

            int testincreament = 0;
            foreach (DataSnapshot destinationsnap in firstStartingPointSnap.Children) 
            {
                Debug.Log($"Found destination key: {destinationsnap.Key}");

                if (string.IsNullOrEmpty(destinationsnap.Key)) continue;

                string destinationName = destinationsnap.Key;
                GameObject instance = PopulatePanel.Instance.Addbutton(destinationName);
                storedButtons.Add(instance);
                Button button = instance.GetComponent<Button>();
                string capturedDestination = destinationName;
                button.onClick.AddListener(() => LoadRouteForDestination(CloudAnchorID, startingPointName,capturedDestination, resolvedanchor));

                Debug.Log("loop called for " + testincreament+"th"+" time");
                testincreament++;
            }
            
        });
    }

    public void ClearList(List<GameObject> List)
    {
        if (List.Count > 0)
        {
            foreach (var go in List)
            {
                if (go != null)
                    Destroy(go);
            }
            List.Clear();
        }
    }


}
