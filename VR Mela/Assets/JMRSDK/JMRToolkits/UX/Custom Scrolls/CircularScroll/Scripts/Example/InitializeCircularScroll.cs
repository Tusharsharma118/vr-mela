using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JMRSDK.Toolkit;
public class InitializeCircularScroll : MonoBehaviour, IScrollController
{
    [SerializeField]
    private JMRCircularScroll cs;
    [SerializeField]
    private int itemsCount;

    void Start()
    {
        cs.OnFill += InitItemData;
        cs.InitScrollData(itemsCount);
    }

    void OnDestroy()
    {
        cs.OnFill -= InitItemData;
    }

    public void InitItemData(int index, GameObject gameObject)
    {
        gameObject.GetComponent<ThumbData>().SetThumbnaiData(index);
    }
}
