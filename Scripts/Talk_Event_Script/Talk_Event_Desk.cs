using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using Cinemachine;

public class Talk_Event_Desk : MonoBehaviour {

    /*
     * Desk 컷씬
     * 가방을 얻기 위한 컷씬
     * 
     * 전제조건: 리와 대화하기
     * 
     * (선택)
     * 1. 박스 선택 시: 치즈 크래커와 가방 모두 흭득
     * 2. 가방 선택 시: 가방만 흭득
     */

    [SerializeField] private GameObject NPC;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject icon;

    // 흭득하는 오브젝트
    [SerializeField] private GameObject cheeseCracker;
    [SerializeField] private GameObject deskBag;
    [SerializeField] private GameObject clemBag;

    private bool isBox;

    // 컷씬 변수
    [SerializeField] private GameObject anim1;
    [SerializeField] private GameObject anim2;
    [SerializeField] private GameObject anim3;

    // Subtitle
    private string[] subtitle1;
    private string[] subtitle2;
    private string[] subtitle3;

    // UI 변수
    [SerializeField] private GameObject boxButton;
    [SerializeField] private GameObject buttonCanvas;
    [SerializeField] private GameObject textBox;

    // 도움창 - 인벤토리
    [SerializeField] private Help help_Inventory;

    // System Script 변수
    private UiEventTrigger uiEventMangaer;
    private Talk_EventTrigger talkEventManager;
    private PlayerController playercontroller;
    private TalkConditionTrigger talkCondionTrigger;
    private SubtitleManager subtitleManager;
    private SaveNLoad theSaveNLoad;

    // 인벤토리
    private Inventory theInventory;

    // 카메라
    [SerializeField] private GameObject cam;

    // Use this for initialization
    void Start()
    {
        anim1.SetActive(false);
        anim2.SetActive(false);
        anim3.SetActive(false);
        buttonCanvas.SetActive(false);
        cheeseCracker.SetActive(false);
        isBox = false;
        clemBag.SetActive(false);

        // 스크립트 및 오브젝트 찾기
        uiEventMangaer = FindObjectOfType<UiEventTrigger>();
        playercontroller = FindObjectOfType<PlayerController>();
        talkEventManager = NPC.GetComponent<Talk_EventTrigger>();
        talkCondionTrigger = FindObjectOfType<TalkConditionTrigger>();
        subtitleManager = FindObjectOfType<SubtitleManager>();
        theSaveNLoad = FindObjectOfType<SaveNLoad>();

        // 인벤토리
        theInventory = FindObjectOfType<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
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
                Talk_Desk();
            }
        }
        else
        {
            talkEventManager.isTalking = false;
        }
    }

    private void CheckSubtitle()
    {
        subtitle1 = subtitleManager.getSubtitles("DeskAnim1");
        subtitle2 = subtitleManager.getSubtitles("DeskAnim2");
        subtitle3 = subtitleManager.getSubtitles("DeskAnim3");
    }

    private void Talk_Desk()
    {
        anim1.SetActive(true);
        PlayableDirector scene1 = anim1.GetComponent<PlayableDirector>();
        scene1.Stop();

        player.transform.position = new Vector3(13.4f, 0f, 38.84f);
        player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        scene1.Play();
        StartCoroutine(Subtiles());
    }

    IEnumerator Subtiles()
    {
        subtitleManager.SetColor("CLEM");
        // 카메라 고정
        cam.GetComponent<CinemachineVirtualCamera>().Priority = 111;

        yield return new WaitForSeconds(1.5f);
        StartCoroutine(_typing(subtitle1[0])); 
        yield return new WaitForSeconds(2f); 
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); 
        StartCoroutine(_typing(subtitle1[1]));
        yield return new WaitForSeconds(3f); 
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(2.5f); // 11

        PlayableDirector scene1 = anim1.GetComponent<PlayableDirector>();
        scene1.Stop();
        anim1.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        buttonCanvas.SetActive(true);
    }

    public void selectBox()
    {
        // 박스 선택 시 anim2 재생
        anim2.SetActive(true);
        isBox = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        buttonCanvas.SetActive(false);
        StartCoroutine(Subtitle2());

        talkCondionTrigger.hasObj("CheeseCrack");

        uiEventMangaer.canvasLongFade(4f, "클레멘타인은 박스상자를 먼저 선택하였습니다.");
    }

    public void selectBackpack()
    {
        // 가방 선택 시 anim3 재생
        anim3.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        buttonCanvas.SetActive(false);
        StartCoroutine(Subtitle3());

        talkCondionTrigger.hasObj("Bag");

        if(isBox == false)
        {
            uiEventMangaer.canvasLongFade(4f, "클레멘타인은 가방을 먼저 선택하였습니다.");
        }
    }

    IEnumerator Subtitle2()
    {
        subtitleManager.SetColor("CLEM");
        yield return new WaitForSeconds(1f);
        PlayableDirector scene2 = anim2.GetComponent<PlayableDirector>();
        scene2.Play();
        yield return new WaitForSeconds(4.5f);
        StartCoroutine(_typing(subtitle2[0]));
        yield return new WaitForSeconds(3f); // 7.30
        cheeseCracker.SetActive(true);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(5f); // 12.30
        StartCoroutine(_typing(subtitle2[1]));
        yield return new WaitForSeconds(4.5f); // 17
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(6.5f); // 23

        anim2.SetActive(false);

        buttonCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 치즈 크래커 인벤토리 Get
        Item item = cheeseCracker.GetComponent<ItemGet>().item;
        theInventory.AcquireItem(item, item.count);
        uiEventMangaer.canvasCollectFade(5f, item.itemName, item.itemInfo);


        cheeseCracker.SetActive(false);
        // 박스 선택 버튼은 사라진다.
        boxButton.SetActive(false);

    }

    IEnumerator Subtitle3()
    {
        subtitleManager.SetColor("CLEM");
        // 카메라 고정풀기
        cam.GetComponent<CinemachineVirtualCamera>().Priority = 10;

        yield return new WaitForSeconds(1f);
        PlayableDirector scene3 = anim3.GetComponent<PlayableDirector>();
        scene3.Play();
        yield return new WaitForSeconds(3.5f);
        if(isBox == true) StartCoroutine(_typing(subtitle3[0]));
        else StartCoroutine(_typing(subtitle3[1]));
        yield return new WaitForSeconds(3f); // 7.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(3.5f); // 11
        // 가방(Desk) 오브젝트 삭제
        deskBag.SetActive(false);
        yield return new WaitForSeconds(1.5f); // 12.30
        // 가방(Clem) 오브젝트 생성
        clemBag.SetActive(true);
        yield return new WaitForSeconds(10.5f); // 23
        StartCoroutine(_typing(subtitle3[2]));
        yield return new WaitForSeconds(4.5f); // 27.5
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(5.5f); // 33

        anim3.SetActive(false);
        playercontroller.val = false;
        talkEventManager.isTalking = false;

        // 가방 인벤토리 Get
        Item item = clemBag.GetComponent<ItemGet>().item;
        // theInventory.AcquireItem(item, item.count);
        uiEventMangaer.canvasCollectFade(5f, item.itemName, item.itemInfo);

        talkCondionTrigger.SetTalkTrigger("Desk");

        // 인벤토리 도움 창 띄우기
        uiEventMangaer.setCanvasHelp(help_Inventory);

        yield return new WaitForSeconds(1f);
        uiEventMangaer.canvasExplainFade(3f, "I키를 눌러 인벤토리 창을 열 수 있습니다.");
        yield return new WaitForSeconds(9f); 
        uiEventMangaer.resetCanvas(1);
        uiEventMangaer.resetCanvas(2);

        talkCondionTrigger.TalkeEventByMission(talkCondionTrigger.missions[3]);
        uiEventMangaer.canvasMissionFade(4f, talkCondionTrigger.missions[3]);

        theSaveNLoad.SaveData();
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
