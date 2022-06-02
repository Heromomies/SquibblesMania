using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerAnimation : MonoBehaviour
{
    [Header("DASH POWER ANIM PARAMETERS")]
    public RectTransform dashRectTransform;
    private static float timeInSecondsDash = 2f;
    private WaitForSeconds _waitForSecondsDash = new WaitForSeconds(timeInSecondsDash);
    
    [Header("SWAP POWER ANIM PARAMETERS")]
    [SerializeField]
    private RectTransform swapTransform1;
    [SerializeField] private TextMeshProUGUI swapText1;
    [SerializeField] private RectTransform swapTransform2;
    [SerializeField] private TextMeshProUGUI swapText2;
    [Header("MIRROR POWER ANIM PARAMETERS")]
    public GameObject mirror;
    private RectTransform mirrorTransform1;
    private RectTransform mirrorTransform2;

    [Header("JUMP POWER ANIM PARAMETERS")]
    [SerializeField]
    private RectTransform jumpRectTransform;
  

    private static PowerAnimation _powerAnimation;
    public static PowerAnimation Instance => _powerAnimation;

    private void Awake()
    {
        _powerAnimation = this;
    }


    void Start()
    {
        mirrorTransform1 = mirror.transform.GetChild(0).GetComponent<RectTransform>();
        mirrorTransform2 = mirror.transform.GetChild(1).GetComponent<RectTransform>();
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(SwapAnim());
        }
    }

    public IEnumerator DashAnim()
    {
        if (GameManager.Instance.actualCamPreset.presetNumber == 1 |
            GameManager.Instance.actualCamPreset.presetNumber == 2)
        {
            dashRectTransform.rotation = Quaternion.Euler(0, 0, 0);
            dashRectTransform.anchoredPosition = new Vector3(-Screen.currentResolution.width, 0f, 0f);
            LeanTween.move(dashRectTransform, Vector3.zero, 0.2f);
            LeanTween.move(dashRectTransform, new Vector3(Screen.currentResolution.width, 0f, 0f), 0.2f).setDelay(1f);
            yield return _waitForSecondsDash;
            dashRectTransform.anchoredPosition = new Vector3(-Screen.currentResolution.width, 0f, 0f);
        }
        else
        {
            dashRectTransform.rotation = Quaternion.Euler(0, 0, 180);
            dashRectTransform.anchoredPosition = new Vector3(Screen.currentResolution.width, 0f, 0f);
            LeanTween.move(dashRectTransform, Vector3.zero, 0.2f);
            LeanTween.move(dashRectTransform, new Vector3(-Screen.currentResolution.width, 0f, 0f), 0.2f).setDelay(1f);
            yield return _waitForSecondsDash;
        }
    }

    public IEnumerator SwapAnim()
    {
        if (GameManager.Instance.actualCamPreset.presetNumber == 1 |
            GameManager.Instance.actualCamPreset.presetNumber == 2)
        {
            swapText1.color = Color.blue;
            swapText2.color = Color.red;

            swapTransform1.rotation = Quaternion.Euler(0, 0, 0);
            swapTransform2.rotation = Quaternion.Euler(0, 0, 0);

            LeanTween.move(swapTransform1, new Vector3(-100f, 0f, 0f), 0.3f);
            LeanTween.move(swapTransform2, new Vector3(100f, 0f, 0f), 0.3f);

            yield return new WaitForSeconds(0.7f);

            swapText1.color = Color.red;
            swapText2.color = Color.blue;

            yield return new WaitForSeconds(0.5f);

            LeanTween.move(swapTransform1, new Vector3(-Screen.currentResolution.width / 2f, 0f, 0f), 0.3f);
            LeanTween.move(swapTransform2, new Vector3(Screen.currentResolution.width / 2f, 0f, 0f), 0.3f);

            yield return new WaitForSeconds(0.6f);
            
            swapText1.color = Color.blue;
            swapText2.color = Color.red;
        }
        else
        {
            swapText1.color = Color.blue;
            swapText2.color = Color.red;

            swapTransform1.rotation = Quaternion.Euler(0, 0, 180);
            swapTransform2.rotation = Quaternion.Euler(0, 0, 180);

            LeanTween.move(swapTransform1, new Vector3(-125f, 0f, 0f), 0.3f);
            LeanTween.move(swapTransform2, new Vector3(125f, 0f, 0f), 0.3f);

            yield return new WaitForSeconds(0.7f);

         
            swapText1.color = Color.red;
            swapText2.color = Color.blue;

            yield return new WaitForSeconds(0.5f);

            LeanTween.move(swapTransform1, new Vector3(-Screen.currentResolution.width / 2f, 0f, 0f), 0.3f);
            LeanTween.move(swapTransform2, new Vector3(Screen.currentResolution.width / 2f, 0f, 0f), 0.3f);

            yield return new WaitForSeconds(0.6f);

            swapText1.color = Color.blue;
            swapText2.color = Color.red;
        }
    }

    public IEnumerator MirrorAnim()
    {
        if (GameManager.Instance.actualCamPreset.presetNumber == 1 |
            GameManager.Instance.actualCamPreset.presetNumber == 2)
        {
            mirror.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.blue;
            mirror.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;

            mirrorTransform1.rotation = Quaternion.Euler(0, 0, 0);
            mirrorTransform2.rotation = Quaternion.Euler(0, 0, 0);

            LeanTween.move(mirrorTransform1, new Vector3(-100f, 0f, 0f), 0.3f);
            LeanTween.move(mirrorTransform2, new Vector3(100f, 0f, 0f), 0.3f);

            yield return new WaitForSeconds(0.7f);

            LeanTween.move(mirrorTransform1, new Vector3(0f, Screen.currentResolution.height / 6f, 0f), 0.3f);
            LeanTween.move(mirrorTransform2, new Vector3(0f, -Screen.currentResolution.height / 6f, 0f), 0.3f);

            yield return new WaitForSeconds(0.5f);

            LeanTween.move(mirrorTransform1, new Vector3(-100f, 0f, 0f), 0.3f);
            LeanTween.move(mirrorTransform2, new Vector3(100f, 0f, 0f), 0.3f);

            yield return new WaitForSeconds(0.5f);

            LeanTween.move(mirrorTransform1, new Vector3(-Screen.currentResolution.width / 4f, 0f, 0f), 0.3f);
            LeanTween.move(mirrorTransform2, new Vector3(Screen.currentResolution.width / 4f, 0f, 0f), 0.3f);
        }
        else
        {
            mirrorTransform1.rotation = Quaternion.Euler(0, 0, 180);
            mirrorTransform2.rotation = Quaternion.Euler(0, 0, 180);

            LeanTween.move(mirrorTransform1, new Vector3(-100f, 0f, 0f), 0.3f);
            LeanTween.move(mirrorTransform2, new Vector3(100f, 0f, 0f), 0.3f);

            yield return new WaitForSeconds(0.7f);

            LeanTween.move(mirrorTransform1, new Vector3(0f, Screen.currentResolution.height / 6f, 0f), 0.3f);
            LeanTween.move(mirrorTransform2, new Vector3(0f, -Screen.currentResolution.height / 6f, 0f), 0.3f);

            yield return new WaitForSeconds(0.5f);

            LeanTween.move(mirrorTransform1, new Vector3(-100f, 0f, 0f), 0.3f);
            LeanTween.move(mirrorTransform2, new Vector3(100f, 0f, 0f), 0.3f);

            yield return new WaitForSeconds(0.5f);

            LeanTween.move(mirrorTransform1, new Vector3(-Screen.currentResolution.width / 4f, 0f, 0f), 0.3f);
            LeanTween.move(mirrorTransform2, new Vector3(Screen.currentResolution.width / 4f, 0f, 0f), 0.3f);
        }
    }

    public IEnumerator JumpAnim()
    {
        if (GameManager.Instance.actualCamPreset.presetNumber == 1 |
            GameManager.Instance.actualCamPreset.presetNumber == 2)
        {
            jumpRectTransform.anchoredPosition = new Vector3(-Screen.currentResolution.width, 0f, 0f);
            jumpRectTransform.rotation = Quaternion.Euler(0, 0, 0);

            LeanTween.move(jumpRectTransform,
                new Vector3(-Screen.currentResolution.height / 3, Screen.currentResolution.height / 3f, 0f), 0.3f);

            yield return new WaitForSeconds(0.2f);

            LeanTween.move(jumpRectTransform, new Vector3(-0f, 0f, 0f), 0.3f);

            yield return new WaitForSeconds(0.6f);

            LeanTween.move(jumpRectTransform,
                new Vector3(Screen.currentResolution.height / 3, Screen.currentResolution.height / 3, 0f), 0.3f);

            yield return new WaitForSeconds(0.2f);

            LeanTween.move(jumpRectTransform, new Vector3(Screen.currentResolution.width, 0f, 0f), 0.3f);
        }
        else
        {
            jumpRectTransform.anchoredPosition = new Vector3(Screen.currentResolution.height, 0f, 0f);
            jumpRectTransform.rotation = Quaternion.Euler(0, 0, 180);

            LeanTween.move(jumpRectTransform,
                new Vector3(Screen.currentResolution.height / 2f, -Screen.currentResolution.height / 2f, 0f), 0.3f);

            yield return new WaitForSeconds(0.2f);

            LeanTween.move(jumpRectTransform, new Vector3(-0f, 0f, 0f), 0.3f);

            yield return new WaitForSeconds(0.6f);

            LeanTween.move(jumpRectTransform,
                new Vector3(-Screen.currentResolution.height / 2f, -Screen.currentResolution.height / 2f, 0f), 0.3f);

            yield return new WaitForSeconds(0.2f);

            LeanTween.move(jumpRectTransform, new Vector3(-Screen.currentResolution.width, 0f, 0f), 0.3f);
        }
    }
}