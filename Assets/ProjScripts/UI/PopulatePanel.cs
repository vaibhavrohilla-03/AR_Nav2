using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopulatePanel : MonoBehaviour
{
    public static PopulatePanel Instance;

    public RectTransform Panel;
    public Vector3 initialPos;
    private Vector3 NextPos;
    public float padding_plusWidth;
    public GameObject ButtonPrefab;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        NextPos = initialPos;
    }
    public GameObject Addbutton(string text)
    {
        GameObject instance = Instantiate(ButtonPrefab);
        instance.transform.SetParent(Panel,false);
        instance.transform.localPosition = NextPos;
        NextPos.y -= padding_plusWidth;

        TextMeshProUGUI textmeshComp = instance.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textmeshComp.text = text;


        return instance;
        
    }
}
