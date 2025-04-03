using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.Events;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }
    public DatabaseReference DBReference { get; private set; }

    public MakeRoute Router;

    public Directions directionPresets;

    [HideInInspector]public UnityEvent OnFirebaseInitialize = new UnityEvent();

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
            OnFirebaseInitialize.Invoke();
        });
    }
    public void SaveRoute(string cloudanchorid)
    {
        Route route = Router.GetCurrentRoute();
        string json = JsonUtility.ToJson(route);
        Debug.Log(json);
        DBReference.Child("Routes").Child(cloudanchorid).SetRawJsonValueAsync(json);
    }

    public void LoadRoute(string cloudanchorid,GameObject resolvedAnchor)
    {
        DBReference.Child("Routes").Child(cloudanchorid).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                string json = task.Result.GetRawJsonValue();
                Route loadedRoute = JsonUtility.FromJson<Route>(json);

                if (resolvedAnchor == null)
                {
                    Debug.LogError("Resolved anchor is null. Cannot spawn directions.");
                    return;
                }

                for (int i = 1; i < loadedRoute.directions.Count; i++)
                {
                    Direction direction = loadedRoute.directions[i];

                    Vector3 relativePosition = new Vector3(direction.position.x, direction.position.y, direction.position.z);
                    Quaternion relativeRotation = new Quaternion(direction.rotation.x, direction.rotation.y, direction.rotation.z, direction.rotation.w);

                    GameObject prefab = directionPresets.getDirection(direction.DirectionType);
                    if (prefab != null)
                    {
                        GameObject instance = Instantiate(prefab, resolvedAnchor.transform);
                        instance.transform.localPosition = relativePosition;
                        instance.transform.localRotation = relativeRotation;

                        Debug.Log($"Spawned {direction.DirectionType} at relative position {relativePosition}");
                    }
                    else
                    {
                        Debug.LogError($"Prefab not found for direction type: {direction.DirectionType}");
                    }
                }
            }
            else
            {
                Debug.LogError("Failed to load route or route does not exist.");
            }
        });
    }

}
