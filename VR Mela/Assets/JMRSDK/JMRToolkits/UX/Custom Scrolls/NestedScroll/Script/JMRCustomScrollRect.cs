using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using JMRSDK.InputModule;
using System.Collections;
namespace JMRSDK.Toolkit
{
    public class JMRCustomScrollRect : ScrollRect, ISwipeHandler
    {
        private ScrollRect j_ParentScroll;
        private PointerEventData j_PntrData;
        private Vector2 j_TempVelocity;
        private bool isVerticalScroll;

        protected override void Start()
        {
            base.Start();
            if (!Application.isPlaying)
                return;
            j_ParentScroll = GetScrollParent(transform);
            CreatePointerEventData();
        }

        private void CreatePointerEventData()
        {
            if (j_PntrData != null) { return; }
            StartCoroutine(WaitTillEventSystemLoads());
        }

        IEnumerator WaitTillEventSystemLoads()
        {
            while (!EventSystem.current)
            {
                yield return new WaitForEndOfFrame();
            }
            if (j_PntrData != null) { yield break; }
            j_PntrData = new PointerEventData(EventSystem.current);
            j_PntrData.Reset();
        }

        ScrollRect GetScrollParent(Transform t)
        {
            if (t.parent != null)
            {
                ScrollRect scroll = t.parent.GetComponent<ScrollRect>();
                if (scroll != null) { return scroll; }
                else return GetScrollParent(t.parent);
            }
            return null;
        }

        public override void OnScroll(PointerEventData data)
        {
            return;
        }

        private void Update()
        {
            if (!Application.isPlaying)
                return;

            velocity = Vector2.Lerp(velocity, Vector2.zero, scrollSensitivity * Time.deltaTime);
        }

        public void ProcessScroll(bool isXAxis, float eventData)
        {
            if (j_PntrData == null) { return; }

            isVerticalScroll = !isXAxis;
            j_TempVelocity.x = isXAxis ? eventData : 0;
            j_TempVelocity.y = !isXAxis ? eventData : 0;

            if (j_ParentScroll != null && ((j_ParentScroll.vertical && isVerticalScroll) || (j_ParentScroll.horizontal && !isVerticalScroll)))
            {
                j_ParentScroll.velocity = j_TempVelocity * j_ParentScroll.scrollSensitivity * 3000;
            }
            else
            {
                velocity = j_TempVelocity * scrollSensitivity * 3000;
            }
        }

        IEnumerator WaitToProcessScroll(bool isXAxis, float eventData)
        {
            yield return new WaitForEndOfFrame();

        }

        public void OnSwipeLeft(SwipeEventData eventData, float value)
        {
            ProcessScroll(true, value);
        }

        public void OnSwipeRight(SwipeEventData eventData, float value)
        {
            ProcessScroll(true, value);
        }

        public void OnSwipeUp(SwipeEventData eventData, float value)
        {
            ProcessScroll(false, value);
        }

        public void OnSwipeDown(SwipeEventData eventData, float value)
        {
            ProcessScroll(false, value);
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
}