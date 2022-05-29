using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Talk_Event_FarmDoor : MonoBehaviour {

    /*
     * Farm_Door 컷씬
     * 
     * anim1: 일방향적인 대화
     * 
     * [권총, 가방, 손전등, 칼(기본장착), Hat(미정) 소지시]
     * anim2: 밖으로 나가는 컷씬
     */

    [SerializeField] private GameObject NPC;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject icon;

    [SerializeField] private GameObject anim;
    [SerializeField] private GameObject anim2;
    [SerializeField] private GameObject anim3;
    [SerializeField] private PlayerController playercontroller;

    // UI 변수
    [SerializeField] private GameObject textBox;
    [SerializeField] private GameObject selectBox;

    private Talk_EventTrigger talkEventManager;

    // Subtitle
    private string[] subtitle1;
    private string[] subtitle2;
    private string[] subtitle3;

    // 참값 변수
    public bool isSelect = false;
    public bool isNext = false;

    // 선택 변수
    public int selection = 0;

    // 시스템 스크립트
    private TalkConditionTrigger talkConditionTrigger;
    private UiEventTrigger uiEventManager;
    private SubtitleManager subtitleManager;
    private Sun sun;

    // 필요 NPC 및 오브젝트
    [SerializeField] private GameObject Lee;
    [SerializeField] private GameObject Kenny;
    [SerializeField] private GameObject Hershel;
    [SerializeField] private GameObject Katjaa;
    [SerializeField] private GameObject Duck;

    [SerializeField] private GameObject FoodBox;
    [SerializeField] private GameObject ammo;

    [SerializeField] private GameObject theGun;
    [SerializeField] private GameObject theGun_RE;

    // 케니 자동차
    [SerializeField] private GameObject kennyCar;
    [SerializeField] private GameObject kennyCarStuff;

    // Help
    [SerializeField] private Help help_Save;

    // 세이브 데이터
    [SerializeField] private SaveNLoad theSaveNLoad;

    // Use this for initialization
    void Start()
    {
        anim.SetActive(false);
        anim2.SetActive(false);
        anim3.SetActive(false);
        talkEventManager = NPC.GetComponent<Talk_EventTrigger>();
        talkConditionTrigger = FindObjectOfType<TalkConditionTrigger>();
        uiEventManager = FindObjectOfType<UiEventTrigger>();
        subtitleManager = FindObjectOfType<SubtitleManager>();
        sun = FindObjectOfType<Sun>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckTalkEvent();
        CheckSubtitle();
    }

    private void CheckTalkEvent()
    {
        if(Vector3.Distance(player.transform.position, this.transform.position) < 15)
        {
            //talkConditionTrigger.getBag() && talkConditionTrigger.getGun() && talkConditionTrigger.getFood() && 
            if (talkConditionTrigger.getBag() && talkConditionTrigger.getGun() && talkConditionTrigger.getFood() && talkEventManager.isTalking == false && icon.activeSelf == true)
            {
                talkConditionTrigger.ClearTalk();
                playercontroller.isTalking = true;
                talkEventManager.isTalking = true;
                Talk_FarmDoor2();
            }
            else if (Vector3.Distance(player.transform.position, this.transform.position) < 4)
            {
                if (Input.GetKeyDown(KeyCode.F) && talkEventManager.isTalking == false && icon.activeSelf == true)
                {
                    playercontroller.isTalking = true;
                    talkEventManager.isTalking = true;
                    Talk_FarmDoor();
                }
            }
        }
        else
        {
            talkEventManager.isTalking = false;
        }
        if (anim.activeSelf == true)
        {
            playercontroller.isTalking = true;
            talkEventManager.isTalking = true;
        }
    }

    private void CheckSubtitle()
    {
        subtitle1 = subtitleManager.getSubtitles("FarmDoorAnim1");
        subtitle2 = subtitleManager.getSubtitles("FarmDoorAnim2");
        subtitle3 = subtitleManager.getSubtitles("FarmDoorAnim3");
    }

    private void Talk_FarmDoor()
    {
        playercontroller.val = true;

        if (talkConditionTrigger.getBag() && talkConditionTrigger.getGun() && talkConditionTrigger.getFood())
        {
            anim2.SetActive(true);
            PlayableDirector scene2 = anim2.GetComponent<PlayableDirector>();
            scene2.Play();
            StartCoroutine(Subtiles2());
        }
        else
        {
            anim.SetActive(true);
            PlayableDirector scene = anim.GetComponent<PlayableDirector>();
            player.transform.position = new Vector3(-19.16f, 0f, 18.79f);
            player.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
            scene.Play();
            StartCoroutine(Subtiles());
            StartCoroutine(Wait(7.5f));
        }
    }

    private void Talk_FarmDoor2()
    {
        playercontroller.val = true;

        anim2.SetActive(true);
        PlayableDirector scene2 = anim2.GetComponent<PlayableDirector>();
        scene2.Play();
        StartCoroutine(Subtiles2());
    }

    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        PlayableDirector scene = anim.GetComponent<PlayableDirector>();
        scene.Stop();
        anim.SetActive(false);
        talkEventManager.isTalking = false;
        playercontroller.isTalking = false;
    }

    IEnumerator Subtiles()
    {
        subtitleManager.SetColor("CLEM");
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(_typing(subtitle1[0]));
        yield return new WaitForSeconds(3f);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(_typing(subtitle1[1]));
        yield return new WaitForSeconds(2.5f);
        textBox.GetComponent<Text>().text = "";

        // 미션 컴플리트
        talkConditionTrigger.MissionComplete(0);
        talkConditionTrigger.TalkeEventByMission(talkConditionTrigger.missions[1]);
        uiEventManager.canvasMissionFade(4f, talkConditionTrigger.missions[1]);

        playercontroller.val = false;

        theSaveNLoad.SaveData();
    }

    IEnumerator Subtiles2()
    {
        // 클리어
        talkConditionTrigger.ClearTalk();

        // 선택창
        StartCoroutine(select());

        // 캐릭터 초반 위치

        // 클렘
        player.transform.position = new Vector3(-13.22f, 0f, 7.96f);
        player.transform.rotation = Quaternion.Euler(new Vector3(0, -30.941f, 0));

        // 케니
        Kenny.transform.position = new Vector3(-18.6f, -0.09915947f, 32.01782f - 21.44f);
        Kenny.transform.rotation = Quaternion.Euler(new Vector3(0, 0f, 0));

        // 리 -49.28782
        Lee.transform.position = new Vector3(-12.86f, -0.06785274f, 32.01782f-17.27f);
        // Lee.transform.position = new Vector3(-12.86f, -0.06785274f, -17.27f);
        Lee.transform.rotation = Quaternion.Euler(new Vector3(0, 270f, 0));

        // 허쉘
        Hershel.transform.position = new Vector3(-18.5f, -0.09801505f, 32.01782f - 7.96f);
        Hershel.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));

        // 캇챠 -81.04401 -2.54 -38.30582
        Katjaa.transform.position = new Vector3(-46.81f + 34.234f, -1.325f + 1.215f, -7.14f - 0.852f + 32.01782f);
        //Katjaa.transform.position = new Vector3(-46.81f, -1.325f,  - 7.14f);
        Katjaa.transform.rotation = Quaternion.Euler(new Vector3(0, -152.681f, 0));

        // 덕
        Duck.transform.position = new Vector3(-46.04f + 34.234f, -1.294728f + 1.215f,  - 7.54f - 0.852f + 32.01782f);
        Duck.transform.rotation = Quaternion.Euler(new Vector3(0, -136.859f, 0));

        // Food Box
        FoodBox.SetActive(true);
        /*
        FoodBox.transform.position = new Vector3(-31.896f, -2.88f, 25.212f);
        FoodBox.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        */
        /*
        FoodBox.transform.position = new Vector3(-20.49f, -0.16f, 24.619f);
        FoodBox.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        */

        yield return new WaitForSeconds(1.5f);
        subtitleManager.SetColor("KENNY");
        StartCoroutine(_typing(subtitle2[0])); // 케니
        yield return new WaitForSeconds(2f); // 3.30
        textBox.GetComponent<Text>().text = "";
        // 클렘 (4초)
        player.transform.position = new Vector3(-15.05f, 0f, 9.65f);
        player.transform.rotation = Quaternion.Euler(new Vector3(0, -21.157f, 0));
        yield return new WaitForSeconds(1f); // 4.30
        subtitleManager.SetColor("CLEM");
        StartCoroutine(_typing(subtitle2[1])); // 클렘
        yield return new WaitForSeconds(2f); // 6.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 7.30
        subtitleManager.SetColor("LEE");
        StartCoroutine(_typing(subtitle2[2])); // 리
        yield return new WaitForSeconds(3f); // 10.30
        textBox.GetComponent<Text>().text = "";
        // 클렘 (11초)
        player.transform.position = new Vector3(-15.03f, 0f, 10.99f);
        player.transform.rotation = Quaternion.Euler(new Vector3(0, -13.998f, 0));
        yield return new WaitForSeconds(1f); // 11.30
        subtitleManager.SetColor("CLEM");
        StartCoroutine(_typing(subtitle2[3])); // 클렘
        yield return new WaitForSeconds(2f); // 13.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 14.30
        subtitleManager.SetColor("HERSHEL");
        StartCoroutine(_typing(subtitle2[4])); // 허쉘
        yield return new WaitForSeconds(4f); // 18.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 19.30
        subtitleManager.SetColor("HERSHEL");
        StartCoroutine(_typing(subtitle2[5])); // 허쉘
        yield return new WaitForSeconds(3f); // 22.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 23.30
        subtitleManager.SetColor("HERSHEL");
        StartCoroutine(_typing(subtitle2[6])); // 허쉘
        yield return new WaitForSeconds(4f); // 27.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 28.30
        subtitleManager.SetColor("CLEM");
        StartCoroutine(_typing(subtitle2[7])); // 클렘
        yield return new WaitForSeconds(4f); // 32.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 33.30
        subtitleManager.SetColor("KENNY");
        StartCoroutine(_typing(subtitle2[8])); // 케니
        yield return new WaitForSeconds(4f); // 37.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 38.30
        subtitleManager.SetColor("LEE");
        StartCoroutine(_typing(subtitle2[9])); // 리
        yield return new WaitForSeconds(4f); // 42.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 43.30
        subtitleManager.SetColor("KATJAA");
        StartCoroutine(_typing(subtitle2[10])); // 캇챠
        yield return new WaitForSeconds(4f); // 47.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 48.30
        subtitleManager.SetColor("HERSHEL");
        StartCoroutine(_typing(subtitle2[11])); // 허쉘
        yield return new WaitForSeconds(4f); // 52.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 53.30
        yield return new WaitForSeconds(6.5f); // 60
        textBox.GetComponent<Text>().text = "";

        // 20초 선택지 동안 나오는 자막
        /*
         * 기본적으로 NPC가 말하는 동안은 자동적으로 씬이 넘어가지 않으며
         * NPC가 말을 안할시/ 말을 다 한 후 씬이 넘어가는 시스템이다.
         * 
         * NPC가 말한 후 선택지를 선택했는지 isSelect Check 후 
         * 다음 씬으로 넘어가는지 여부를 가른다.
         * 
         * NPC가 다 말한 후 선택지 배정을 안하면 자동적으로 배정된다.
         
         */

        yield return new WaitForSeconds(4f); //4(64f)
        if (isSelect == true && isNext == false) nextScene();
        if (isSelect == false)
        {
            subtitleManager.SetColor("KENNY");
            StartCoroutine(_typing(subtitle2[12]));
        }
        yield return new WaitForSeconds(3f); //7
        if (isSelect == true && isNext == false) nextScene();
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(3.5f); //10.5
        if (isSelect == true && isNext == false) nextScene();
        if (isSelect == false)
        {
            subtitleManager.SetColor("KATJAA");
            StartCoroutine(_typing(subtitle2[13]));
        }
        yield return new WaitForSeconds(3f); //13.5
        if (isSelect == true && anim2.activeSelf == false) nextScene();
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(3.5f); //17
        if (isSelect == true && isNext == false) nextScene();
        if (isSelect == false)
        {
            subtitleManager.SetColor("LEE");
            StartCoroutine(_typing(subtitle2[14]));
        }
        yield return new WaitForSeconds(3f); //20
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
        yield return new WaitForSeconds(60f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        selectBox.SetActive(true);

        // 20초 동안 선택지 타이머
        uiEventManager.startTimer(20f);
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
        uiEventManager.resetTimer();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // scene 넘어가기
    public void nextScene()
    {
        talkConditionTrigger.MissionComplete(5);

        if(!isNext)
        {
            textBox.GetComponent<Text>().text = "";
            PlayableDirector scene2 = anim2.GetComponent<PlayableDirector>();
            scene2.Stop();
            anim2.SetActive(false);
            anim3.SetActive(true);
            PlayableDirector scene3 = anim3.GetComponent<PlayableDirector>();
            scene3.Play();
            StartCoroutine(Subtiles3());
        }

        isNext = true;
    }

    IEnumerator Subtiles3()
    {
        player.transform.position = new Vector3(-15.03f, 0f, 10.99f);
        player.transform.rotation = Quaternion.Euler(new Vector3(0, -13.998f, 0));

        yield return new WaitForSeconds(5.5f);
        subtitleManager.SetColor("CLEM");
        if (selection == 1) StartCoroutine(_typing(subtitle3[0]));
        else if (selection == 2) StartCoroutine(_typing(subtitle3[1]));
        else if (selection == 3) StartCoroutine(_typing(subtitle3[2]));
        else if (selection == 4) textBox.GetComponent<Text>().text = "";
        else Debug.Log("ERROR_SUBTITLE2_KENNY");
        yield return new WaitForSeconds(4f); // 9.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 10.30
        subtitleManager.SetColor("HERSHEL");
        if (selection == 1) StartCoroutine(_typing(subtitle3[4]));
        else if (selection == 2) StartCoroutine(_typing(subtitle3[5]));
        else if (selection == 3) StartCoroutine(_typing(subtitle3[6]));
        else if (selection == 4) StartCoroutine(_typing(subtitle3[7]));
        else Debug.Log("ERROR_SUBTITLE2_KENNY");
        yield return new WaitForSeconds(4f); // 14.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 15.30
        subtitleManager.SetColor("LEE");
        StartCoroutine(_typing(subtitle3[8]));
        yield return new WaitForSeconds(4.5f); // 20
        uiEventManager.startFade();
        yield return new WaitForSeconds(1.5f); // 21.30
        textBox.GetComponent<Text>().text = "";
        subtitleManager.SetColor("CLEM");
        yield return new WaitForSeconds(1.5f); // 23
        sun.SetRenderSettingSkyBox(10f);
        sun.isNight = true;
        yield return new WaitForSeconds(2f); // 25
        AudioManager.instance.PlayBGM("Background_Night");
        AudioManager.instance.currentBGM = "Background_Night";
        yield return new WaitForSeconds(17f); // 40.0
        // 플레이어 이동
        player.transform.position = new Vector3(-25.64f, 0f, 19.1f);
        player.transform.rotation = Quaternion.Euler(new Vector3(0, 270f, 0));

        // 케니 자동차 이동
        kennyCar.transform.position = new Vector3(-29.79f, 0f, 13.01f);
        kennyCar.transform.rotation = Quaternion.Euler(new Vector3(0, -16.682f, 0));
        kennyCarStuff.SetActive(true);

        // Food Box
        FoodBox.transform.position = new Vector3(-35.84f + 11.36827f, -2.88f + 2.720344f, 25.82f - 0.6212654f);
        FoodBox.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));

        ammo.SetActive(true);

        yield return new WaitForSeconds(9.5f); // 49.30
        StartCoroutine(_typing("좋아, 이제 이동해볼까?"));
        yield return new WaitForSeconds(3.5f); //53
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(6f); //59

        theGun.SetActive(false);
        theGun_RE.SetActive(true);

        playercontroller.isGunMode = true;
        playercontroller.val = false;

        uiEventManager.setCanvasHelp(help_Save);

        uiEventManager.canvasMissionFade(4f, talkConditionTrigger.missions[8]);
        talkConditionTrigger.TalkeEventByMission(talkConditionTrigger.missions[8]);

        // 세이브
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
