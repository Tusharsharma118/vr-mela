using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JMRSDK.Toolkit;

[RequireComponent(typeof(ScrollRect), typeof(JMRNestedInfiniteScroll))]
public class JMRSpaceEffect : MonoBehaviour
{
    #region Serialize Field
    [SerializeField]
    private float elasticSpace = 30;
    #endregion

    #region Private Fields
    private RectTransform j_Content;
    private Dictionary<int, Vector2> j_Items = new Dictionary<int, Vector2>();
    private int j_Counter = -1;
    private ScrollRect j_ScrollRect;
    private JMRNestedInfiniteScroll j_InfiniteScroll;
    private int j_LeftPadding, j_RightPadding, j_ItemSpacing;
    private bool iSLoaded;
    private float j_ContentWidth = 0;
    private float j_PrevSpaceFactor = 0;
    private float j_Itemwidth;
    #endregion



    // Start is called before the first frame update
    void Start()
    {
        j_ScrollRect = GetComponent<ScrollRect>();
        j_InfiniteScroll = GetComponent<JMRNestedInfiniteScroll>();
        j_Content = j_ScrollRect.content;
        j_InfiniteScroll.OnFill += OnFill;
    }

    private void OnFill(int arg1, GameObject arg2)
    {
        if (j_Items.ContainsKey(arg2.GetInstanceID()))
        {
            j_Items[arg2.GetInstanceID()] = arg2.GetComponent<RectTransform>().anchoredPosition;
        }
        else
        {
            j_Items.Add(arg2.GetInstanceID(), arg2.GetComponent<RectTransform>().anchoredPosition);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (j_ScrollRect.velocity.magnitude <= 0)
            return;

        float spaceFactor = Mathf.Abs((j_ScrollRect.velocity.x / 355.66f) * elasticSpace);
        float content_pos_x = Mathf.Abs(j_Content.anchoredPosition.x);
        foreach (RectTransform item in j_Content)
        {
            if (item.gameObject.activeInHierarchy)
            {
                if (j_Items.ContainsKey(item.gameObject.GetInstanceID()))
                {
                    float cntrl = (item.anchoredPosition.x - content_pos_x);
                    //if (cntrl > 0)
                    {
                        float val = Mathf.Floor(cntrl / item.sizeDelta.x);
                        item.anchoredPosition = Vector2.Lerp(item.anchoredPosition, new Vector2(j_Items[item.gameObject.GetInstanceID()].x + (val * spaceFactor), item.anchoredPosition.y), 2 * Time.deltaTime);
                    }
                }
            }
        }
    }
}
