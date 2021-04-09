using System.Collections.Generic;
using UnityEngine;
using JMRSDK.Toolkit;

public class HorizontalScrollController : MonoBehaviour
{

    [SerializeField]
    JMRNestedInfiniteScroll scrollView;

    [SerializeField]
    private int _width, itemCount;
    [SerializeField]
    private List<Sprite> Contents;


    private void Start()
    {
        scrollView.OnFill += OnFillItem;
        scrollView.OnWidth += GetItemWidth;
        SetScrollViewData();
    }

    void OnFillItem(int index, GameObject item)
    {
        //Debug.LogError("Scrollview name : " + gameObject.name + ", Index" + index);
        if (Contents.Count > 0)
            item.GetComponent<SampleControl>().img.sprite = Contents[Random.Range(0, Contents.Count)];
    }

    public void SetScrollViewData()
    {
        scrollView.InitData(itemCount);
    }

    private int GetItemWidth(int index)
    {
        return _width;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            scrollView.RecycleAll();
            scrollView.Prefab = scrollView.GetPrefabFromPool(Random.Range(0,3));
            scrollView.InitData(itemCount);
        }
    }

    private void OnDestroy()
    {
        scrollView.OnFill -= OnFillItem;
        scrollView.OnWidth -= GetItemWidth;
    }
}
