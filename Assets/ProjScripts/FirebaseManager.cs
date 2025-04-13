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
    public void SaveRoute(string cloudanchorid)
    {
        Route route = Router.GetCurrentRoute();
        string json = JsonUtility.ToJson(route);
        Debug.Log(json);
        DBReference.Child("Routes").Child(cloudanchorid).SetRawJsonValueAsync(json);
    }

    public void LoadRoute(string cloudanchorid,GameObject resolvedAnchor)
    {
        Debug.Log("function invoked");
        DBReference.Child("Routes").Child(cloudanchorid).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            Debug.Log($"Firebase task status: IsCompleted={task.IsCompleted}, IsFaulted={task.IsFaulted}, IsCanceled={task.IsCanceled}");

            if (task.Result.Exists)
            {
                Debug.Log("Route data exists.");
            }
            else
            {
                Debug.LogWarning("Route data does NOT exist for this anchor.");
                Debug.Log("Raw snapshot JSON: " + task.Result.GetRawJsonValue());
            }

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Firebase task failed or was canceled.");
                if (task.Exception != null)
                    Debug.LogError(task.Exception.ToString());
            }
            
            if (task.IsCompleted && task.Result.Exists)
            {
                string json = task.Result.GetRawJsonValue();
                Route loadedRoute = JsonUtility.FromJson<Route>(json);
                Debug.Log("loaded route -> " + json);
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
                        GameObject instance = Instantiate(prefab);
                        instance.transform.SetParent(resolvedAnchor.transform,false);
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
