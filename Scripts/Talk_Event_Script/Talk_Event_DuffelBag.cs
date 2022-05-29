using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using Cinemachine;

public class Talk_Event_DuffelBag : MonoBehaviour {

    /*
     * DuffelBag 컷씬
     * 권총을 얻기 위한 컷씬
     * 
     * 전제조건: 리와 대화하기
     * 
     * 더플 백 버튼 클릭 시 : 권총 흭득
     * 
     */

    [SerializeField] private GameObject NPC;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject icon;

    // 흭득하는 오브젝트
    [SerializeField] private GameObject desertEagle;
    [SerializeField] private GameObject desertEagle_RE;
    [SerializeField] private Item desertEagleAmmo;

    // 컷씬 변수
    [SerializeField] private GameObject anim1;
    [SerializeField] private GameObject anim2;

    // Subtitle
    private string[] subtitle1;
    private string[] subtitle2;

    // UI 변수
    [SerializeField] private GameObject textBox;
    [SerializeField] private GameObject buttonCanvas;

    // 대사
    private string[][] script;

    // System Script 변수
    private UiEventTrigger uiEventManager;
    private Talk_EventTrigger talkEventManager;
    private PlayerController playercontroller;
    private TalkConditionTrigger talkCondionTrigger;
    private SubtitleManager subtitleManager;

    // 인벤토리
    private Inventory theInventory;

    // Use this for initialization
    void Start()
    {
        anim1.SetActive(false);
        anim2.SetActive(false);
        desertEagle.SetActive(false);

        // 스크립트 및 오브젝트 찾기
        uiEventManager = FindObjectOfType<UiEventTrigger>();
        playercontroller = FindObjectOfType<PlayerController>();
        talkEventManager = NPC.GetComponent<Talk_EventTrigger>();
        subtitleManager = FindObjectOfType<SubtitleManager>();
        talkCondionTrigger = FindObjectOfType<TalkConditionTrigger>();

        // 인벤토리
        theInventory = FindObjectOfType<Inventory>();
    }

    // Update is called once per frame
    void Update () {
        CheckTalkEvent();
        CheckSubtitle();
    }

    private void CheckTalkEvent()
    {
        if (Vector3.Distance(player.transform.position, this.transform.position) < 4)
        {
            if (Input.GetKeyDown(KeyCode.F) && talkEventManager.isTalking == false && icon.activeSelf == true)
            {
                talkEventManager.isTalking = true;
                playercontroller.val = true;
                Talk_DuffelBag();
            }
        }
        else
        {
            talkEventManager.isTalking = false;
        }
    }

    private void CheckSubtitle()
    {
        subtitle1 = subtitleManager.getSubtitles("DuffelBagAnim1");
        subtitle2 = subtitleManager.getSubtitles("DuffelBagAnim2");
    }

    private void Talk_DuffelBag()
    {
        anim1.SetActive(true);
        PlayableDirector scene1 = anim1.GetComponent<PlayableDirector>();
        scene1.Stop();

        player.transform.position = new Vector3(14.557f, 0f, 25.371f);
        player.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));

        scene1.Play();
        StartCoroutine(Subtiles());
    }

    IEnumerator Subtiles()
    {
        subtitleManager.SetColor("CLEM");
        yield return new WaitForSeconds(2.5f);
        StartCoroutine(_typing(subtitle1[0]));
        yield return new WaitForSeconds(3f); // 5.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(7f); // 12.30
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        buttonCanvas.SetActive(true);
        uiEventManager.startTimer(20f);
        yield return new WaitForSeconds(20f);
        if (anim1.activeSelf == true) selectDuffelBag();
    }

    public void selectDuffelBag()
    {
        PlayableDirector scene1 = anim1.GetComponent<PlayableDirector>();
        scene1.Stop();
        anim1.SetActive(false);

        uiEventManager.resetTimer();

        // 더플 백 선택 시 anim2 재생
        anim2.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        buttonCanvas.SetActive(false);
        StartCoroutine(Subtitle2());
    }

    IEnumerator Subtitle2()
    {
        subtitleManager.SetColor("CLEM");
        yield return new WaitForSeconds(1f);
        PlayableDirector scene2 = anim2.GetComponent<PlayableDirector>();
        scene2.Play();
        yield return new WaitForSeconds(1.9f);
        desertEagle.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(_typing(subtitle2[0]));
        yield return new WaitForSeconds(5f); // 9.30
        textBox.GetComponent<Text>().text = "";

        // 데저트 이글 인벤토리 Get
        desertEagle_RE.SetActive(true);
        Item item = desertEagle_RE.GetComponent<ItemGet>().item;
        theInventory.AcquireItem(item, item.count);

        // 데저트 이글 탄 인벤토리 Get
        theInventory.AcquireItem(desertEagleAmmo, desertEagleAmmo.count);

        yield return new WaitForSeconds(5.5f); // 15

        anim2.SetActive(false);
        playercontroller.val = false;
        talkEventManager.isTalking = false;

        talkCondionTrigger.hasObj("Gun");

        if(talkCondionTrigger.getHershelTalk())
            uiEventManager.canvasMissionFade(4f, "마구간 입구로 가기");

        talkCondionTrigger.SetTalkTrigger("DuffelBag");

        uiEventManager.canvasCollectFade(5f, item.itemName, item.itemInfo);
        desertEagle_RE.SetActive(false);

        yield return new WaitForSeconds(10f);
        uiEventManager.canvasCollectFade(5f, desertEagleAmmo.itemName, desertEagleAmmo.itemInfo);
        uiEventManager.resetCanvas(2);
        uiEventManager.resetCanvas(4);
    }

        // 자막 타이핑 이펙트
        IEnumerator _typing(string text)
    {
        for (int i = 0; i <= text.Length; i++)
        {
            textBox.GetComponent<Text>().text = text.Substring(0, i);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
