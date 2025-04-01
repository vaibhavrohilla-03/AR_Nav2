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

    public UnityEvent OnFirebaseInitialize = new UnityEvent();

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
        SaveRoute();
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


    public void SaveRoute()
    {
        Route testRoute = new Route("TestRoute");
        testRoute.AddDirection(new Direction(DirectionType.Start,Vector3.zero,Quaternion.identity));
        string json = JsonUtility.ToJson(testRoute);
        Debug.Log(json);
    }

    public void LoadRoute()
    {

    }

}
