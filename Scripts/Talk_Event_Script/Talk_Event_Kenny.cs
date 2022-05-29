using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using Cinemachine;

public class Talk_Event_Kenny : MonoBehaviour {

    /*
     * Kenny 컷씬
     * 기본적으로 2개의 컷신(첫번째 상호작용)과 1개의 컷씬(두번째 상호작용)이 있다.
     * 첫번째 상호작용의 경우 선택지가 주어지는 컷씬이 등장하며
     * 두번째 상호작용시 일방향적인 컷씬이 등장한다.
     */

    // 필요한 게임오브젝트
    [SerializeField] private GameObject Kenny;
    [SerializeField] private GameObject NPC;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject icon;

    // 컷씬 변수
    [SerializeField] private GameObject anim1;
    [SerializeField] private GameObject anim2;
    [SerializeField] private GameObject anim3;

    // Subtitle
    private string[] subtitle1;
    private string[] subtitle2;
    private string[] subtitle3;

    // UI 변수
    [SerializeField] private GameObject textBox;
    [SerializeField] private GameObject selectBox;

    // 참값 변수
    public bool isSelect = false;
    public bool isNext = false;

    private bool isTalk;

    // 선택 변수
    public int selection = 0;

    // System Script 변수
    private UiEventTrigger uiEventMangaer;
    private Talk_EventTrigger talkEventManager;
    private TalkConditionTrigger talkConditionTriger;
    private PlayerController playercontroller;
    private SubtitleManager subtitleManager;

    // Use this for initialization
    void Start()
    {
        anim1.SetActive(false);
        anim2.SetActive(false);
        anim3.SetActive(false);

        // 스크립트 및 오브젝트 찾기
        uiEventMangaer = FindObjectOfType<UiEventTrigger>();
        playercontroller = FindObjectOfType<PlayerController>();
        talkEventManager = NPC.GetComponent<Talk_EventTrigger>();
        talkConditionTriger = FindObjectOfType<TalkConditionTrigger>();
        subtitleManager = FindObjectOfType<SubtitleManager>();

        isTalk = talkConditionTriger.getTalkOneKenny();
    }

    // Update is called once per frame
    void Update ()
    {
        CheckTalkEvent();
        CheckSubtitle();
    }

    private void CheckTalkEvent()
    {
        if (Vector3.Distance(player.transform.position, this.transform.position) < 4.5)
        {
            if (Input.GetKeyDown(KeyCode.F) && talkEventManager.isTalking == false && NPC.activeSelf == true)
            {
                playercontroller.isTalking = true;
                talkEventManager.isTalking = true;
                Talk_Kenny();
            }
        }
        else
        {
            talkEventManager.isTalking = false;
        }
        if (anim1.activeSelf == false && anim2.activeSelf == false)
        {
            playercontroller.isTalking = false;
            talkEventManager.isTalking = false;
        }
        if (anim3.activeSelf == true)
        {
            playercontroller.isTalking = true;
            talkEventManager.isTalking = true;
        }
    }

    private void CheckSubtitle()
    {
        subtitle1 = subtitleManager.getSubtitles("KennyAnim1");
        subtitle2 = subtitleManager.getSubtitles("KennyAnim2");
        subtitle3 = subtitleManager.getSubtitles("KennyAnim3");
    }

    private void Talk_Kenny()
    {
        anim1.SetActive(true);
        PlayableDirector scene1 = anim1.GetComponent<PlayableDirector>();
        scene1.Stop();

        anim3.SetActive(true);
        PlayableDirector scene3 = anim1.GetComponent<PlayableDirector>();
        scene3.Stop();

        // 컷씬 시작 시 player 위치 및 상태
        player.transform.position = new Vector3(-5.57f, 0f, -7.8f);
        player.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));

        isTalk = talkConditionTriger.getTalkOneKenny();

        // 첫번째 상호작용 시 scene1 플레이
        // 두번째 상호작용 시 scene3 플레이
        if (isTalk == false)
        {
            scene1.Play();
            StartCoroutine(Subtiles());
            StartCoroutine(select());
            anim3.SetActive(false);
        }
        else
        {
            scene3.Play();
            StartCoroutine(Subtiles3());
            anim1.SetActive(false);
        }
    }

    // 자막_1
    IEnumerator Subtiles()
    {
        yield return new WaitForSeconds(4.2f);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(0.5f);
        subtitleManager.SetColor("CLEM");
        StartCoroutine(_typing(subtitle1[0]));
        yield return new WaitForSeconds(3.25f);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(0.5f);
        subtitleManager.SetColor("KENNY");
        StartCoroutine(_typing(subtitle1[1]));
        yield return new WaitForSeconds(4.5f);
        textBox.GetComponent<Text>().text = "";


        // 15초 선택지 동안 나오는 자막
        /*
         * 기본적으로 NPC가 말하는 동안은 자동적으로 씬이 넘어가지 않으며
         * NPC가 말을 안할시/ 말을 다 한 후 씬이 넘어가는 시스템이다.
         * 
         * NPC가 말한 후 선택지를 선택했는지 isSelect Check 후 
         * 다음 씬으로 넘어가는지 여부를 가른다.
         * 
         * NPC가 다 말한 후 선택지 배정을 안하면 자동적으로 배정된다.
         
         */

        yield return new WaitForSeconds(6f); //2
        if (isSelect == true && isNext == false) nextScene();
        if (isSelect == false) StartCoroutine(_typing(subtitle1[2])); 
        yield return new WaitForSeconds(3f); //4.5
        if (isSelect == true && isNext == false) nextScene();
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(2.5f); //7.5
        if (isSelect == true && isNext == false) nextScene();
        if (isSelect == false) StartCoroutine(_typing(subtitle1[3]));
        yield return new WaitForSeconds(3f); //10
        if (isSelect == true && anim2.activeSelf == false) nextScene();
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(2f); //12.5
        if (isSelect == true && isNext == false) nextScene();
        if (isSelect == false) StartCoroutine(_typing(subtitle1[4]));
        yield return new WaitForSeconds(2.5f); //15
        if (isSelect == true && isNext == false) nextScene();
        if (isSelect == false && isNext == false)
        {
            ClickSelection4();
            nextScene();
        }
    }

    // 선택창 띄우기
    IEnumerator select()
    {
        yield return new WaitForSeconds(17.2f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        selectBox.SetActive(true);

        // 15초 동안 선택지 타이머
        uiEventMangaer.startTimer(15f);
    }

    // 선택지
    public void ClickSelection1()
    {
        selection = 1;
        Set();
    }
    public void ClickSelection2()
    {
        selection = 2;
        Set();
    }
    public void ClickSelection3()
    {
        selection = 3;
        Set();
    }
    public void ClickSelection4()
    {
        selection = 4;
        Set();
    }

    // 선택지 선택
    public void Set()
    {
        isSelect = true;
        selectBox.SetActive(false);
        uiEventMangaer.resetTimer();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // scene 넘어가기
    public void nextScene()
    {
        isNext = true;
        textBox.GetComponent<Text>().text = "";
        PlayableDirector scene1 = anim1.GetComponent<PlayableDirector>();
        scene1.Stop();
        anim1.SetActive(false);
        anim2.SetActive(true);
        PlayableDirector scene2 = anim2.GetComponent<PlayableDirector>();
        scene2.Play();
        StartCoroutine(Subtiles2());
    }

    // 자막_2
    /*
     * 선택지를 선택하고 선택지에 따라 달라지는 자막
     * 선택지 선택용 자막 후 공용 자막이 추가 될 수 있다.
     */
    IEnumerator Subtiles2()
    {
        yield return new WaitForSeconds(1f);
        subtitleManager.SetColor("CLEM");
        if (selection == 1) StartCoroutine(_typing(subtitle2[0]));
        else if (selection == 2) StartCoroutine(_typing(subtitle2[1]));
        else if (selection == 3) StartCoroutine(_typing(subtitle2[2]));
        else if (selection == 4) textBox.GetComponent<Text>().text = "";
        else Debug.Log("ERROR_SUBTITLE2_KENNY");
        yield return new WaitForSeconds(3.9f);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(0.5f);
        subtitleManager.SetColor("KENNY");
        if (selection == 1) StartCoroutine(_typing(subtitle2[4]));
        else if (selection == 2) StartCoroutine(_typing(subtitle2[5]));
        else if (selection == 3) StartCoroutine(_typing(subtitle2[6]));
        else if (selection == 4) StartCoroutine(_typing(subtitle2[7]));
        else Debug.Log("ERROR_SUBTITLE2_KENNY");
        yield return new WaitForSeconds(4f);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(3.5f);
        anim2.SetActive(false);

        talkConditionTriger.TalkOneNPC(1);
    }

    IEnumerator Subtiles3()
    {
        //15.15
        subtitleManager.SetColor("CLEM");
        yield return new WaitForSeconds(2f);
        StartCoroutine(_typing(subtitle3[0]));
        yield return new WaitForSeconds(4f);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f);
        subtitleManager.SetColor("KENNY");
        StartCoroutine(_typing(subtitle3[1]));
        yield return new WaitForSeconds(4.5f);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(3.75f);
        anim3.SetActive(false);
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
