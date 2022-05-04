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

    public GameObject mirror;
    private RectTransform mirrorTransform1;
    private RectTransform mirrorTransform2;

    public GameObject jump;
    private RectTransform jumpTransform;


    // Start is called before the first frame update
    void Start()
    {
        dashTransform = dash.GetComponent<RectTransform>();

        swapTransform1 = swap.transform.GetChild(0).GetComponent<RectTransform>();
        swapTransform2 = swap.transform.GetChild(1).GetComponent<RectTransform>();

        mirrorTransform1 = mirror.transform.GetChild(0).GetComponent<RectTransform>();
        mirrorTransform2 = mirror.transform.GetChild(1).GetComponent<RectTransform>();

        jumpTransform = jump.transform.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            StartCoroutine(JumpAnim());
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

    IEnumerator MirrorAnim()
    {
        LeanTween.move(mirrorTransform1, new Vector3(-100f, 0f, 0f), 0.3f);
        LeanTween.move(mirrorTransform2, new Vector3(100f, 0f, 0f), 0.3f);

        yield return new WaitForSeconds(0.7f);

        LeanTween.move(mirrorTransform1, new Vector3(0f, 250f, 0f), 0.3f);
        LeanTween.move(mirrorTransform2, new Vector3(0f, -250f, 0f), 0.3f);

        yield return new WaitForSeconds(0.5f);

        LeanTween.move(mirrorTransform1, new Vector3(-100f, 0f, 0f), 0.3f);
        LeanTween.move(mirrorTransform2, new Vector3(100f, 0f, 0f), 0.3f);

        yield return new WaitForSeconds(0.5f);

        LeanTween.move(mirrorTransform1, new Vector3(-750f, 0f, 0f), 0.3f);
        LeanTween.move(mirrorTransform2, new Vector3(750f, 0f, 0f), 0.3f);


        yield return null;
    }

    IEnumerator JumpAnim()
    {
        LeanTween.move(jumpTransform, new Vector3(-300f, 300f, 0f), 0.3f);

        yield return new WaitForSeconds(0.2f);

        LeanTween.move(jumpTransform, new Vector3(-0f, 0f, 0f), 0.3f);

        yield return new WaitForSeconds(0.6f);

        LeanTween.move(jumpTransform, new Vector3(300f, 300f, 0f), 0.3f);

        yield return new WaitForSeconds(0.2f);

        LeanTween.move(jumpTransform, new Vector3(750f, 0f, 0f), 0.3f);

        yield return new WaitForSeconds(0.5f);

        jump.GetComponent<RectTransform>().anchoredPosition = new Vector3(-750f, 0f, 0f);

        yield return null;
    }



}
