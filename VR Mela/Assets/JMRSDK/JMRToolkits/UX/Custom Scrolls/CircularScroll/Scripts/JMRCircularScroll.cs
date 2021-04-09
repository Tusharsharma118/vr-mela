using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JMRSDK.InputModule;

namespace JMRSDK.Toolkit
{
    public class JMRCircularScroll : MonoBehaviour, ISwipeHandler, IFocusable
    {
        #region Private_Members
        /// <summary>
        /// The index of the objects currently visible (_currItems)
        /// </summary>
        private int j_DisplayIndex;

        /// <summary>
        /// The index of the objects from the actual list we want to maintain (_items)
        /// </summary>
        private int j_ActualIndex;

        private int j_NumOfProducts = 0;
        private float j_Radius;
        private bool j_IsFocused = false;
        private int j_ActualActiveItems = 0;
        private int j_FwdIndicesNum = 0;
        private int j_BckIndicesNum = 0;
        private bool j_DisplaySongList = false;

        private float j_TimeSinceScorll = 0;
        const float j_ScrollDelay = 0.5f;
        private bool isScrolling = false;
        private List<Coroutine> j_LerpPool;
        private int j_ItemsCount;
        #endregion

        #region Serialized_Fields
        [SerializeField] private Transform container;
        [SerializeField] private Transform center;
        [SerializeField] private GameObject itemPrefab;

        [Tooltip("Value to be multiplied with PI to get the angles within a range.(e.g. 2 = 360, 1 = 180, 0.6 = 120)")]
        [SerializeField] private float piMultiplicationFactor;

        [Tooltip("Time it will take for each of the objects to lerp from current position to next position on scrolling")]
        [SerializeField] private float lerpTime;

        [Tooltip("Number of maximum active objects. Modifying this value will change the number of objects visible at a time")]
        [SerializeField] private int maxActiveItems;

        [Tooltip("Minimum value for the scale of the objects when they are not at the center")]
        [SerializeField] private Vector3 minScale;

        [Tooltip("Maximum value for the scale of the objects when they are not at the center")]
        [SerializeField] private Vector3 maxScale;

        [Tooltip("List of objects currently visible. Total count will be equal to maxActiveItems")]
        [SerializeField] private List<GameObject> _currItems = new List<GameObject>();

        /// <summary>
        /// Callback on item fill
        /// </summary>
        public Action<int, GameObject> OnFill = delegate { };
        Dictionary<int, Vector3> cachedPos;

        #endregion

        private void OnEnable()
        {
            isRotating = false;
            j_Radius = -center.localPosition.z;
            cachedPos = new Dictionary<int, Vector3>();
            j_LerpPool = new List<Coroutine>();
            _currItems = new List<GameObject>();
            j_NumOfProducts = container.childCount;
            j_ActualIndex = 0;
            j_DisplayIndex = 0;
        }

        /// <summary>
        /// For initializing the circular scrolling system. Call this function only after _items list has been initialized.
        /// </summary>
        /// <param name="startIndex">Represents the index of the object in the _items list you want selected</param>
        public void InitScrollData(int itemsCount, int startIndex = 0)
        {
            cachedPos.Clear();
            isRotating = false;
            this.j_ItemsCount = itemsCount;
            j_ActualIndex = startIndex;
            j_ActualActiveItems = itemsCount <= 0 ? maxActiveItems : itemsCount > maxActiveItems ? maxActiveItems : itemsCount;
            j_FwdIndicesNum = (j_ActualActiveItems - 1) / 2;
            //Debug.LogError(j_FwdIndicesNum);
            j_BckIndicesNum = j_ActualActiveItems % 2 == 0 ? j_FwdIndicesNum + 1 : j_FwdIndicesNum;

            PoolContent(itemPrefab, container, j_ActualActiveItems);

            j_DisplayIndex = j_ActualIndex < j_ActualActiveItems ? j_ActualIndex : j_ActualIndex % j_ActualActiveItems;

            int temp_item_index = j_ActualActiveItems - 1;
            for (int x = 0; x < j_ActualActiveItems; x++)
            {
                _currItems.Add(container.GetChild(x).gameObject);
            }

            FillCurrentItemsList();

            LerpRotation(SwipeDirection.Loading);
        }

        /// <summary>
        /// For instantiating new prefabs(itemPrefab) if required and/or reusing already instantiated prefabs
        /// </summary>
        /// <param name="prefab">The prefab to be instantiated</param>
        /// <param name="container">The container which will be the parent of all instantiated prefabs</param>
        /// <param name="activeItems">Total number of items which will be active</param>
        void PoolContent(GameObject prefab, Transform container, int activeItems)
        {
            int objectCount = container.transform.childCount;
            int dataCount = activeItems;
            int difference = Mathf.Abs(objectCount - dataCount);


            if (dataCount == 0)
            {
                Debug.Log("Data Empty");
            }
            else if (objectCount < dataCount)
            {
                for (int i = 0; i < difference; i++)
                {
                    GameObject obj = GameObject.Instantiate(prefab, container.transform);
                    obj.name = "Prefab_" + i;
                }
            }
            else if (objectCount > dataCount)
            {
                for (int i = objectCount; i > dataCount; i--)
                {
                    container.transform.GetChild(i - 1).gameObject.SetActive(false);
                }
            }

            for (int j = 0; j < dataCount; j++)
            {
                container.transform.GetChild(j).gameObject.SetActive(true);
            }
        }

        void FillCurrentItemsList()
        {
            for (int i = 0; i < j_ItemsCount; i++)
            {
                int validItem = GetValidItem(i);
                if (!validItem.Equals(-1))
                {
                    OnFill?.Invoke(i, _currItems[validItem]);
                }
            }
        }

        int GetValidItem(int index)
        {
            if (index == j_ActualIndex)
                return j_DisplayIndex;

            for (int j = 1; j <= j_FwdIndicesNum; j++)
            {
                if (j_ActualIndex + j > j_ItemsCount - 1)
                {
                    if (index == (j_ActualIndex + j) - j_ItemsCount)
                    {
                        return j_DisplayIndex + j > _currItems.Count - 1 ? j_DisplayIndex + j - _currItems.Count : j_DisplayIndex + j;
                    }
                }
                else if (index == (j_ActualIndex + j))
                {
                    return j_DisplayIndex + j > _currItems.Count - 1 ? j_DisplayIndex + j - _currItems.Count : j_DisplayIndex + j;
                }
            }

            for (int k = 1; k <= j_BckIndicesNum; k++)
            {
                if (j_ActualIndex - k < 0)
                {
                    if (index == j_ItemsCount - Mathf.Abs(j_ActualIndex - k))
                    {
                        return j_DisplayIndex - k < 0 ? _currItems.Count - Mathf.Abs(j_DisplayIndex - k) : j_DisplayIndex - k;
                    }
                }
                else if (index == (j_ActualIndex - k))
                {
                    return j_DisplayIndex - k < 0 ? _currItems.Count - Mathf.Abs(j_DisplayIndex - k) : j_DisplayIndex - k;
                }
            }

            return -1;
        }

        int GetForwardIndexValue<T>(int index, int item_count) => (index + j_FwdIndicesNum > item_count - 1) ? ((index + j_FwdIndicesNum) - item_count) : (index + j_FwdIndicesNum);

        int GetBackwardIndexValue<T>(int index, int item_count) => (index - j_BckIndicesNum < 0) ? (item_count - Mathf.Abs(index - j_BckIndicesNum)) : (index - j_BckIndicesNum);

        /// <summary>
        /// Scroll to previous object in the list
        /// </summary>
        void PreviousProduct()
        {
            if (isRotating)
            {
                return;
            }

            j_ActualIndex = --j_ActualIndex < 0 ? j_ItemsCount - 1 : j_ActualIndex;
            j_DisplayIndex = --j_DisplayIndex < 0 ? _currItems.Count - 1 : j_DisplayIndex;
            if (j_ItemsCount >= maxActiveItems)
            {
                int aIndex = GetBackwardIndexValue<int>(j_ActualIndex, j_ItemsCount);
                int cIndex = GetBackwardIndexValue<int>(j_DisplayIndex, _currItems.Count);
                OnFill?.Invoke(aIndex, _currItems[cIndex]);
            }
            LerpRotation(SwipeDirection.Previous);
        }

        /// <summary>
        /// Scroll to Next Object in the list
        /// </summary>
        void NextProduct()
        {
            if (isRotating)
            {
                return;
            }

            j_ActualIndex = ++j_ActualIndex > j_ItemsCount - 1 ? 0 : j_ActualIndex;
            j_DisplayIndex = ++j_DisplayIndex > _currItems.Count - 1 ? 0 : j_DisplayIndex;
            if (j_ItemsCount >= maxActiveItems)
            {
                int aIndex = GetForwardIndexValue<int>(j_ActualIndex, j_ItemsCount);
                int cIndex = GetForwardIndexValue<int>(j_DisplayIndex, _currItems.Count);
                OnFill?.Invoke(aIndex, _currItems[cIndex]);
            }
            LerpRotation(SwipeDirection.Next);
        }

        private enum SwipeDirection
        {
            Loading,
            Next,
            Previous
        }

        /// <summary>
        /// To move all the active/visible objects to their next position
        /// </summary>
        void LerpRotation(SwipeDirection direction)
        {
            DestroyLerps();
            for (int c = 0; c < j_ActualActiveItems; c++)
            {
                if (j_LerpPool.Count <= c)
                    j_LerpPool.Add(StartCoroutine(LerpRotationDelay(c, direction)));
                else
                    j_LerpPool[c] = StartCoroutine(LerpRotationDelay(c, direction));
            }

            isRotating = true;
            StartCoroutine(WaitTillLerpEnds());
        }

        private bool isRotating = false;
        IEnumerator WaitTillLerpEnds()
        {
            yield return new WaitForSeconds(lerpTime);
            isRotating = false;
        }

        IEnumerator LerpRotationDelay(int index, SwipeDirection direction)
        {
            float et = 0;

            if (!_currItems[index].gameObject.activeInHierarchy)
                _currItems[index].gameObject.SetActive(true);
            Vector3 currPos = _currItems[index].transform.localPosition;
            Vector3 newPos = Vector3.zero;
            if (!cachedPos.ContainsKey(index))
            {
                float angle = (index - j_DisplayIndex) * Mathf.PI * piMultiplicationFactor / j_ActualActiveItems;
                newPos = new Vector3(Mathf.Sin(angle) * j_Radius, 0, -Mathf.Cos(angle) * j_Radius) - center.localPosition;
                if (newPos.z > j_Radius)
                {
                    angle = -angle;
                    newPos = new Vector3(Mathf.Sin(angle) * j_Radius, 0, Mathf.Cos(angle) * j_Radius) - center.localPosition;
                }
                cachedPos.Add(index, newPos);
            }
            else
            {
                if (cachedPos.ContainsValue(currPos))
                {
                    int currIndx = 0;
                    foreach (KeyValuePair<int, Vector3> item in cachedPos)
                    {
                        if (item.Value == currPos)
                        {
                            break;
                        }
                        currIndx++;
                    }
                    if (direction == SwipeDirection.Next)
                    {
                        currIndx = currIndx == 0 ? j_ActualActiveItems - 1 : currIndx - 1;
                        newPos = cachedPos[currIndx];
                    }
                    else if(direction == SwipeDirection.Previous)
                    {
                        newPos = ++currIndx >= j_ActualActiveItems ? cachedPos[0] : cachedPos[currIndx];
                    }
                }
            }


            while (et < lerpTime)
            {
                _currItems[index].transform.localPosition = Vector3.Lerp(currPos, newPos, et / lerpTime);
                et += Time.deltaTime;
                yield return null;
            }

            _currItems[index].transform.localPosition = newPos;

            float scaleLerpTime = 0.15f;
            et = 0;
            Vector3 targetScale = index == j_DisplayIndex ? maxScale : minScale;
            Vector3 startScale = _currItems[index].transform.localScale;
            while (et < lerpTime)
            {
                _currItems[index].transform.localScale = Vector3.Lerp(startScale, targetScale, et / scaleLerpTime);

                et += Time.deltaTime;
                yield return null;
            }
            yield break;
        }

        void OnHorizontalSwipe(float delta)
        {
            if (j_IsFocused && !isScrolling)
            {
                if (delta < -0.01f)
                {
                    PreviousProduct();
                }
                else if (delta > 0.01f)
                {
                    NextProduct();
                }

                isScrolling = true;
            }
        }

        public void ResetCircularScroll()
        {
            for (int i = _currItems.Count - 1; i >= 0; i--)
            {
                _currItems[i].gameObject.SetActive(false);
            }

            j_DisplayIndex = 0;
            j_ActualIndex = 0;
        }

        private void Update()
        {
        }

        private void LateUpdate()
        {
            if (!isScrolling)
                return;

            if (j_TimeSinceScorll > j_ScrollDelay)
            {
                j_TimeSinceScorll = 0;
                isScrolling = false;
            }
            else
            {
                j_TimeSinceScorll += Time.deltaTime;
            }
        }

        void DestroyLerps()
        {
            for (int i = 0; i < j_LerpPool.Count; i++)
            {
                StopCoroutine(j_LerpPool[i]);
            }
        }

        public void OnFocusEnter()
        {
            j_IsFocused = true;
        }

        public void OnFocusExit()
        {
            j_TimeSinceScorll = 0;
            j_IsFocused = false;
            isScrolling = false;
        }

        public void OnSwipeLeft(SwipeEventData eventData, float value)
        {
            OnHorizontalSwipe(value);
        }

        public void OnSwipeRight(SwipeEventData eventData, float value)
        {
            OnHorizontalSwipe(value);
        }

        public void OnSwipeUp(SwipeEventData eventData, float value)
        {
            OnHorizontalSwipe(value);
        }

        public void OnSwipeDown(SwipeEventData eventData, float value)
        {
            OnHorizontalSwipe(value);
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
