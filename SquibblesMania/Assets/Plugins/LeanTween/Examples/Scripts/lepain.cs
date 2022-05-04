using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerAnimation : MonoBehaviour
{
    public GameObject dash;
    private RectTransform dashTransform;

    public GameObject swap;
    private RectTransform swapTransform1;
    private RectTransform swapTransform2;


    // Start is called before the first frame update
    void Start()
    {
        dashTransform = dash.GetComponent<RectTransform>();
        swapTransform1 = swap.transform.GetChild(0).GetComponent<RectTransform>();
        swapTransform2 = swap.transform.GetChild(1).GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            StartCoroutine(SwapAnim());
        }
            
    }

    IEnumerator DashAnim()
    {
        LeanTween.move(dashTransform, new Vector3(0f, 0f, 0f), 0.2f);
        LeanTween.move(dashTransform, new Vector3(750f, 0f, 0f), 0.2f).setDelay(1f);
        yield return new WaitForSeconds(2);
        dash.GetComponent<RectTransform>().anchoredPosition = new Vector3(-750f, 0f, 0f);

        yield return null;
        
    }

    IEnumerator SwapAnim()
    {
        LeanTween.move(swapTransform1, new Vector3(-100f, 0f, 0f), 0.3f);
        LeanTween.move(swapTransform2, new Vector3(100f, 0f, 0f), 0.3f);

        yield return new WaitForSeconds(0.7f);

        swap.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.Lerp(Color.blue, Color.red, 1);
        swap.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.Lerp(Color.red, Color.blue, 1);

        yield return new WaitForSeconds(0.5f);

        LeanTween.move(swapTransform1, new Vector3(-750f, 0f, 0f), 0.3f);
        LeanTween.move(swapTransform2, new Vector3(750f, 0f, 0f), 0.3f);

        yield return new WaitForSeconds(0.6f);

        swap.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.blue;
        swap.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.red;

        yield return null;
    }

}
