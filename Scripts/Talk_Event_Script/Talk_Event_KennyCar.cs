using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class Talk_Event_KennyCar : MonoBehaviour {

    [SerializeField] private GameObject Obj;
    [SerializeField] private GameObject kennyCar;
    [SerializeField] private GameObject box;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject icon1;
    [SerializeField] private GameObject icon2;

    // 컷씬 변수
    [SerializeField] private GameObject anim1;
    [SerializeField] private GameObject anim2;
    [SerializeField] private GameObject anim3;

    // Subtitle
    private string[] subtitle1;

    // UI 변수
    [SerializeField] private GameObject textBox;

    // System Script 변수
    private UiEventTrigger uiEventManager;
    private Talk_EventTrigger talkEventManager1;
    private Talk_EventTrigger talkEventManager2;
    private PlayerController playercontroller;
    private TalkConditionTrigger talkConditionTrigger;
    private SubtitleManager subtitleManager;
    private EndingScroll theEndingScroll;

    // 인벤토리
    private Inventory theInventory;

    // 오브젝트 삭제
    [SerializeField] private GameObject stuffs;

    // 흭득 아이템 및 오브젝트
    [SerializeField] private Item flashLight;
    [SerializeField] private Item map;
    [SerializeField] private Item notePad;

    // 필요 오브젝트
    [SerializeField] private GameObject zommbie3;
    [SerializeField] private GameObject zommbie4;
    [SerializeField] private GameObject zommbie5;
    [SerializeField] private GameObject zommbie6;

    // 도움창
    [SerializeField] private Help help_FlashLight;
    [SerializeField] private Help help_Map;

    // 세이브 데이터
    [SerializeField] private SaveNLoad theSaveNLoad;

    // 라이트 오브젝트
    [SerializeField] private GameObject[] Lights;

    public void LoadToKennyCar(Vector3 pos, Vector3 rot)
    {
        Obj.transform.position = pos;
        Obj.transform.eulerAngles = rot;
    }

    // Use this for initialization
    void Start()
    {
        anim1.SetActive(false);
        anim2.SetActive(false);
        anim3.SetActive(false);

        // 스크립트 및 오브젝트 찾기
        uiEventManager = FindObjectOfType<UiEventTrigger>();
        playercontroller = FindObjectOfType<PlayerController>();
        talkEventManager1 = box.GetComponent<Talk_EventTrigger>();
        talkEventManager2 = kennyCar.GetComponent<Talk_EventTrigger>();
        talkConditionTrigger = FindObjectOfType<TalkConditionTrigger>();
        subtitleManager = FindObjectOfType<SubtitleManager>();
        theEndingScroll = FindObjectOfType<EndingScroll>();

        // 인벤토리
        theInventory = FindObjectOfType<Inventory>();

        for (int i = 0; i < Lights.Length; i++)
        {
            Lights[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckTalkEvent_Box();
        CheckTalkEvent_KennyCar();
        CheckSubtitle();
    }

    private void CheckTalkEvent_Box()
    {
        // 박스
        if (Vector3.Distance(player.transform.position, box.transform.position) < 4)
        {
            if (Input.GetKeyDown(KeyCode.F) && talkEventManager1.isTalking == false && icon1.activeSelf == true)
            {
                talkEventManager1.isTalking = true;
                playercontroller.val = true;
                Talk_KennyCarBox();
            }
        }
        else
        {
            talkEventManager1.isTalking = false;
        }
    }

    private void CheckTalkEvent_KennyCar()
    {
        // 케니 자동차
        if (Vector3.Distance(player.transform.position, kennyCar.transform.position) < 5)
        {
            if (Input.GetKeyDown(KeyCode.F) && talkEventManager2.isTalking == false && icon2.activeSelf == true)
            {
                talkEventManager2.isTalking = true;
                playercontroller.val = true;
                Talk_KennyCar();
            }
        }
        else
        {
            talkEventManager2.isTalking = false;
        }
    }

    private void CheckSubtitle()
    {
        //subtitle1 = subtitleManager.getSubtitles("StallDoorAnim1");
    }

    private void Talk_KennyCarBox()
    {
        anim1.SetActive(true);
        PlayableDirector scene1 = anim1.GetComponent<PlayableDirector>();
        scene1.Stop();

        player.transform.position = new Vector3(-27.37f, 0f, 11.7f);
        player.transform.rotation = Quaternion.Euler(new Vector3(0, 245.083f, 0));

        scene1.Play();
        StartCoroutine(Subtiles());
    }

    private void Talk_KennyCar()
    {
        player.transform.position = new Vector3(-32.213f, 0f, 13.346f);
        player.transform.rotation = Quaternion.Euler(new Vector3(0, 80.43101f, 0));

        if (talkConditionTrigger.getMission1() && talkConditionTrigger.getMission2() && talkConditionTrigger.getMission3() && talkConditionTrigger.getMission4())
        {
            // 엔딩
            anim3.SetActive(true);
            PlayableDirector scene3 = anim3.GetComponent<PlayableDirector>();
            scene3.Play();
            StartCoroutine(Subtiles3());
        }
        else
        {
            anim2.SetActive(true);
            PlayableDirector scene2 = anim2.GetComponent<PlayableDirector>();
            scene2.Play();
            StartCoroutine(Subtiles2());
        }
    }

    IEnumerator Subtiles()
    {
        subtitleManager.SetColor("CLEM");
        yield return new WaitForSeconds(0.5f); // 0.30
        StartCoroutine(_typing("박스잖아?")); 
        yield return new WaitForSeconds(0.75f); // 1.15
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(0.25f); // 1.30
        yield return new WaitForSeconds(0.5f); // 2
        StartCoroutine(_typing("혹시 앞서 나간 리 아저씨 일행이 두고 간건가? 확인해봐야겠어.")); 
        yield return new WaitForSeconds(3.5f); // 5.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 6.30
        // 메모장 페이드 인/아웃(14.5초 동안 대사 이벤트)
        uiEventManager.setNotePadUI(12.5f);
        // 3초씩 
        yield return new WaitForSeconds(1.5f); // 8
        StartCoroutine(_typing("... 워커들이 생각보다 많다고?"));
        yield return new WaitForSeconds(3f); // 11
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 12
        StartCoroutine(_typing("아무래도 리 아저씨 일행은 먼저 대피한 모양이야."));
        yield return new WaitForSeconds(3f); // 15
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 16
        StartCoroutine(_typing("케니 아저씨 차를 다시 작동시킬만한 부품을 모아야 한다는 이건가..."));
        yield return new WaitForSeconds(3f); // 19
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(2.5f); // 21.30
        StartCoroutine(_typing("요약하자면 자동차를 작동시킬 부품들을 가져와 이 일대를 탈출하라는 건가."));
        yield return new WaitForSeconds(4f); // 25.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 26.30
        StartCoroutine(_typing("그래서 지도와 플래시 라이트를 두고 가신건가...."));
        yield return new WaitForSeconds(4f); // 30.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 31.30
        StartCoroutine(_typing("이거 쉽지 않겠는데..."));
        yield return new WaitForSeconds(4f); // 35.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(0.5f); // 36

        anim1.SetActive(false);
        playercontroller.val = false;
        talkEventManager1.isTalking = false;

        stuffs.SetActive(false);

        // 흭득 아이템 : 지도, 플래시 라이트, 메모장
        theInventory.AcquireItem(flashLight, flashLight.count);
        theInventory.AcquireItem(map, map.count);
        theInventory.AcquireItem(notePad, notePad.count);

        StartCoroutine(ShowHelp());
        StartCoroutine(ShotItemsInfo());
        StartCoroutine(PlayerSubtitle());

        uiEventManager.canvasMissionFade(4f, talkConditionTrigger.missions[9]);
        talkConditionTrigger.TalkeEventByMission(talkConditionTrigger.missions[9]);

        // 세이브
        theSaveNLoad.SaveData();
    }

    IEnumerator ShowHelp()
    {
        yield return new WaitForSeconds(1f);
        uiEventManager.setCanvasHelp(help_FlashLight);
        yield return new WaitForSeconds(0.5f);
        uiEventManager.setCanvasHelp(help_Map);
    }

    IEnumerator ShotItemsInfo()
    {
        uiEventManager.canvasCollectFade(5f, flashLight.name, flashLight.itemInfo);
        yield return new WaitForSeconds(10f);
        uiEventManager.canvasCollectFade(5f, map.name, map.itemInfo);
        yield return new WaitForSeconds(10f);
        uiEventManager.canvasCollectFade(5f, notePad.name, notePad.itemInfo);
    }

    IEnumerator PlayerSubtitle()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(_typing("리 아저씨가 주신 지도를 한번 확인해봐야겠어."));
        yield return new WaitForSeconds(4f);
        textBox.GetComponent<Text>().text = "";
    }

    IEnumerator Subtiles2()
    {
        subtitleManager.SetColor("CLEM");
        yield return new WaitForSeconds(2f); // 2
        StartCoroutine(_typing("아직 필요한걸 전부 챙겨오지 못했어..."));
        yield return new WaitForSeconds(3f); // 5
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(2f); // 7

        anim2.SetActive(false);
        playercontroller.val = false;
        talkEventManager2.isTalking = false;
    }

    IEnumerator Subtiles3()
    {
        subtitleManager.SetColor("CLEM");
        yield return new WaitForSeconds(2f); // 2
        StartCoroutine(_typing("모두 다 찾았으니 어서 탈출하자."));
        yield return new WaitForSeconds(3f); // 5
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(2f); // 7

        for (int i = 0; i < Lights.Length; i++)
        {
            Lights[i].SetActive(true);
        }

        zommbie3.SetActive(true);
        zommbie4.SetActive(true);
        zommbie5.SetActive(true);
        zommbie6.SetActive(true);

        player.transform.position = new Vector3(0f, 0f, 0);
        player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        // 케니 자동차 이동
        Obj.transform.position = new Vector3(-28f, 0f, 13.01f);
        Obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        // 좀비 봇3
        zommbie3.transform.position = new Vector3(-27.6f, 0f, 1.9f);
        zommbie3.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        // 좀비 봇4
        zommbie4.transform.position = new Vector3(-30.7f, 0f, 3.7f);
        zommbie4.transform.rotation = Quaternion.Euler(new Vector3(0, 14.632f, 0));

        // 좀비 봇5
        zommbie5.transform.position = new Vector3(-25.1f, 0f, 5.6f);
        zommbie5.transform.rotation = Quaternion.Euler(new Vector3(0, -17.692f, 0));

        // 좀비 봇6
        zommbie6.transform.position = new Vector3(-31.66f, 0f, 8.16f);
        zommbie6.transform.rotation = Quaternion.Euler(new Vector3(0, 24.415f, 0));

        yield return new WaitForSeconds(16f); // 23

        // 페이드 인/아웃
        uiEventManager.startFade();

        yield return new WaitForSeconds(2.5f); // 23

        theEndingScroll.StartEndingScroll();

        zommbie3.SetActive(false);
        zommbie4.SetActive(false);
        zommbie5.SetActive(false);
        zommbie6.SetActive(false);

        anim3.SetActive(false);
        playercontroller.val = false;
        talkEventManager2.isTalking = false;
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

    // 케니 자동차 위치
    public Vector3 getKennyCarPos() { return Obj.transform.position; }
    public Vector3 getKennyCarRot() { return Obj.transform.eulerAngles; }
}
