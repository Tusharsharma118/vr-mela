using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UpdateFill : MonoBehaviour
{
    public Image UiFillImage;
    public float speed = 2.0f;
    public float waitTime = 5.0f;
    public bool repeat;
    public bool start;

    public IEnumerator Start()
    {
        while (repeat)
        {
            yield return ChangeFill(1.0f, 0.0f, waitTime);
            yield return ChangeFill(0.0f, 1.0f, waitTime);
        }
    }

    public void restartFill()
    {
        StartCoroutine(Start());
    }
    public IEnumerator ChangeFill(float start, float end, float time)
    {
        float i = 0.0f;
        float rate = (1.0f / time) * speed;
        while( i < 1.0f)
        {
            i += Time.deltaTime * rate;
            UiFillImage.fillAmount = Mathf.Lerp(start, end, i);
            yield return null;
        }
    }
}
