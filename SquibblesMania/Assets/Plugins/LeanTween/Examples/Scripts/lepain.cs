using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerAnimation : MonoBehaviour
{
    public GameObject dash;

    public GameObject swap;

    // Start is called before the first frame update
    void Start()
    {
        

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
        LeanTween.move(dash.GetComponent<RectTransform>(), new Vector3(0f, 0f, 0f), 0.2f);
        LeanTween.move(dash.GetComponent<RectTransform>(), new Vector3(750f, 0f, 0f), 0.2f).setDelay(1f);
        yield return new WaitForSeconds(2);
        dash.GetComponent<RectTransform>().anchoredPosition = new Vector3(-750f, 0f, 0f);

        yield return null;
        
    }

    IEnumerator SwapAnim()
    {
        LeanTween.move(swap.transform.GetChild(0).GetComponent<RectTransform>(), new Vector3(-100f, 0f, 0f), 0.5f);
        LeanTween.move(swap.transform.GetChild(1).GetComponent<RectTransform>(), new Vector3(100f, 0f, 0f), 0.5f);

        yield return new WaitForSeconds(2);

        swap.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.blue;
        swap.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
        


        yield return new WaitForSeconds(20);
        dash.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector3(-750f, 0f, 0f);
        dash.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector3(750f, 0f, 0f);

        yield return null;
    }

}
