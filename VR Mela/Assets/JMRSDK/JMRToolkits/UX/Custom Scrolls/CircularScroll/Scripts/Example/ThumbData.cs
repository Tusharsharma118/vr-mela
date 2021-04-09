using UnityEngine;
using TMPro;
using JMRSDK.Toolkit;
using JMRSDK.InputModule;

public class ThumbData : MonoBehaviour, ISwipeHandler,ISelectClickHandler
{
    JMRCircularScroll tempScroll;
    private void Start()
    {
        tempScroll = transform.root.gameObject.GetComponent<JMRCircularScroll>();
    }

    [SerializeField] private int index;
    public int Index
    {
        get => index;
        set => index = value;
        //set { index = value; Debug.Log("thumb index = " + index); }
    }

    public TextMeshProUGUI selectedText;

    public void SetThumbnaiData(int index)
    {
        selectedText.text = index.ToString();
    }

    public void OnSelectClicked(SelectClickEventData eventData)
    {
        Debug.LogError("Im clicked");
    }

    public void OnSwipeLeft(SwipeEventData eventData, float value)
    {
        tempScroll.OnSwipeLeft(eventData,value);
    }

    public void OnSwipeRight(SwipeEventData eventData, float value)
    {
        tempScroll.OnSwipeRight(eventData, value);
    }

    public void OnSwipeUp(SwipeEventData eventData, float value)
    {
        tempScroll.OnSwipeUp(eventData, value);
    }

    public void OnSwipeDown(SwipeEventData eventData, float value)
    {
        tempScroll.OnSwipeDown(eventData, value);
    }

    public void OnSwipeStarted(SwipeEventData eventData)
    {
        
    }

    public void OnSwipeUpdated(SwipeEventData eventData, Vector2 swipeData)
    {
        
    }

    public void OnSwipeCompleted(SwipeEventData eventData)
    {
        
    }

    public void OnSwipeCanceled(SwipeEventData eventData)
    {
        
    }
}
