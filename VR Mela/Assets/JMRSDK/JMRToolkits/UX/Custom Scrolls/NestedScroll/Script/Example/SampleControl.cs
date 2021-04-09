using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using JMRSDK.InputModule;

public class SampleControl : MonoBehaviour,IFocusable
{
    [SerializeField]
    private TMP_Text value;
    public Image img;
    int actualIndex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetValue(int indx)
    {
        value.text = indx.ToString();
    }

    public void OnFocusEnter()
    {
        actualIndex = transform.GetSiblingIndex();
        transform.SetAsLastSibling();
        //Debug.LogError("Pref Index : " + actualIndex + "current Index : " + transform.GetSiblingIndex());
    }

    public void OnFocusExit()
    {
        transform.SetSiblingIndex(actualIndex);
    }
}
