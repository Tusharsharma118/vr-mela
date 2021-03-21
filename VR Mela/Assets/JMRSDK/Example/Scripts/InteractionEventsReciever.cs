using JMRSDK.InputModule;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionEventsReciever : MonoBehaviour, ISelectHandler, ISwipeHandler
{
    private Text _text;

    private void Start()
    {
        _text = GetComponent<Text>();
    }

    public void OnSelectDown(SelectEventData eventData) => _text.text = $"OnSelectDown \n {eventData.SourceId}";
    public void OnSelectUp(SelectEventData eventData) => _text.text = $"OnSelectUp \n {eventData.SourceId}";
    public void OnSwipeLeft(SwipeEventData eventData, float value) => _text.text = $"OnSwipeLeft \n {eventData.SourceId}";
    public void OnSwipeRight(SwipeEventData eventData, float value) => _text.text = $"OnSwipeRight \n {eventData.SourceId}";
    public void OnSwipeUp(SwipeEventData eventData, float value) => _text.text = $"OnSwipeUp \n {eventData.SourceId}";
    public void OnSwipeDown(SwipeEventData eventData, float value) => _text.text = $"OnSwipeDown \n {eventData.SourceId}";
    public void OnSwipeStarted(SwipeEventData eventData) => _text.text = $"OnSwipeStarted \n {eventData.SourceId}";
    public void OnSwipeUpdated(SwipeEventData eventData, Vector2 swipeData) { } // => _text.text = $"OnSwipeUpdated : {eventData.SourceId}";
    public void OnSwipeCompleted(SwipeEventData eventData) { } // => _text.text = $"OnSwipeCompleted : {eventData.SourceId}";
    public void OnSwipeCanceled(SwipeEventData eventData) { } // => _text.text = $"OnSwipeCanceled : {eventData.SourceId}";
}