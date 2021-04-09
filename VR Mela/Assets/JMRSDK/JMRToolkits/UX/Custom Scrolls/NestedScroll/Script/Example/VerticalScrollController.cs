using UnityEngine;
using JMRSDK.Toolkit;
using System;

public class VerticalScrollController : MonoBehaviour
{

    [SerializeField]
    JMRNestedInfiniteScroll scrollView;

    [SerializeField]
    private int _height,itemCount;


    private void Start()
    {
        scrollView.OnFill += OnFillItem;
        scrollView.OnHeight += GetItemHeight;
        scrollView.OnPull += OnPullDown;
        SetScrollViewData();
    }

    private void OnPullDown(JMRNestedInfiniteScroll.Direction obj)
    {
        scrollView.ApplyDataTo(itemCount, itemCount + 10,JMRNestedInfiniteScroll.Direction.Bottom);
        itemCount += 10;
    }

    void OnFillItem(int index, GameObject item)
    {
        // item.GetComponent<HorizontalScrollController>().SetText(index.ToString());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            OnPullDown(JMRNestedInfiniteScroll.Direction.Bottom);
    }

    public void SetScrollViewData()
    {
        scrollView.InitData(itemCount);
    }

    private int GetItemHeight(int index)
    {
        return _height;
    }

    private void OnDestroy()
    {
        scrollView.OnFill -= OnFillItem;
        scrollView.OnHeight -= GetItemHeight;
    }
}
