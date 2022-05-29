using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using Cinemachine;

public class Talk_Event_KatjaaDuck : MonoBehaviour {

    /*
     * Katjaa&Duck 컷씬
     * 
     * Anim1: Katjaa 와 Duck 모두 대화하는 컷씬
     * Anim2: Duck에게 다가가는 컷씬 (Anim2 -> Anim3 연계)
     * Anim3: Duck에게 치즈클래커를 줄지 선택하는 컷씬 [전제조건: 치츠클래커]
     *    *** Anim1에서 치즈크래커를 가지고 있을시 Anim1 -> Anim3 전개
     *    
     * (선택)
     * Anim4: Duck에게 치즈 크래커를 준다
     * Anim5: Duck에게 치즈 크래커를 주지 않는다.
     * 
     */

    [SerializeField] private GameObject NPC;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject cheeseCracker;
    [SerializeField] private GameObject icon;

    // 컷씬 변수
    [SerializeField] private GameObject anim1;
    [SerializeField] private GameObject anim2;
    [SerializeField] private GameObject anim3;
    [SerializeField] private GameObject anim4;
    [SerializeField] private GameObject anim5;

    // Subtitle
    private string[] subtitle1;
    private string[] subtitle3;
    private string[] subtitle4;
    private string[] subtitle5;

    // UI 변수
    [SerializeField] private GameObject textBox;
    [SerializeField] private GameObject selectCanvas;

    // System Script 변수
    private UiEventTrigger uiEventMangaer;
    private Talk_EventTrigger talkEventManager;
    private PlayerController playercontroller;
    private TalkConditionTrigger talkCondionTrigger;
    private SubtitleManager subtitleManager;

    // 참값 변수
    private bool isTalk;

    // Use this for initialization
    void Start()
    {
        anim1.SetActive(false);
        anim2.SetActive(false);
        anim3.SetActive(false);
        anim4.SetActive(false);
        anim5.SetActive(false);
        selectCanvas.SetActive(false);
        cheeseCracker.SetActive(false);

        // 스크립트 및 오브젝트 찾기
        uiEventMangaer = FindObjectOfType<UiEventTrigger>();
        playercontroller = FindObjectOfType<PlayerController>();
        talkEventManager = NPC.GetComponent<Talk_EventTrigger>();
        talkCondionTrigger = FindObjectOfType<TalkConditionTrigger>();
        subtitleManager = FindObjectOfType<SubtitleManager>();

        isTalk = talkCondionTrigger.getTalkOneDuck();
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
                Talk_KatjaaDuck();
            }
        }
        else
        {
            talkEventManager.isTalking = false;
        }
    }

    private void CheckSubtitle()
    {
        subtitle1 = subtitleManager.getSubtitles("KatjaaDuckAnim1");
        subtitle3 = subtitleManager.getSubtitles("KatjaaDuckAnim3");
        subtitle4 = subtitleManager.getSubtitles("KatjaaDuckAnim4");
        subtitle5 = subtitleManager.getSubtitles("KatjaaDuckAnim5");
    }

    private void Talk_KatjaaDuck()
    {
        anim1.SetActive(true);
        PlayableDirector scene1 = anim1.GetComponent<PlayableDirector>();
        scene1.Stop();

        anim2.SetActive(true);
        PlayableDirector scene2 = anim2.GetComponent<PlayableDirector>();
        scene2.Stop();

        isTalk = talkCondionTrigger.getTalkOneDuck();

        if (isTalk == true && talkCondionTrigger.getCheeseCrack() == true)
        {
            player.transform.position = new Vector3(33.61f, 0f, 30.745f);
            player.transform.rotation = Quaternion.Euler(new Vector3(0, 111.58f, 0));

            scene2.Play();
            StartCoroutine(Subtiles2());
            anim1.SetActive(false);
        }
        if(isTalk == false)
        {
            player.transform.position = new Vector3(30.69f, 0f, 30.83f);
            player.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));

            scene1.Play();
            StartCoroutine(Subtiles());
            anim2.SetActive(false);
        }
    }

    IEnumerator Subtiles()
    {
        talkCondionTrigger.TalkOneNPC(2);

        yield return new WaitForSeconds(2.5f);
        // 플레이어 위치 바꾸기 2:30
        player.transform.position = new Vector3(32.185f, 0f, 30.83f);
        yield return new WaitForSeconds(0.5f);
        subtitleManager.SetColor("KATJAA");
        StartCoroutine(_typing(subtitle1[0]));
        yield return new WaitForSeconds(2f);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 6
        subtitleManager.SetColor("CLEM");
        StartCoroutine(_typing(subtitle1[1]));
        yield return new WaitForSeconds(2f); // 8
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 9
        subtitleManager.SetColor("DUCK");
        StartCoroutine(_typing(subtitle1[2]));
        yield return new WaitForSeconds(2.5f); // 11.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 12.30
        // 플레이어 위치 바꾸기 12:30
        player.transform.position = new Vector3(33.61f, 0f, 30.745f); 
        player.transform.rotation = Quaternion.Euler(new Vector3(0, 111.58f, 0));
        yield return new WaitForSeconds(2f); // 14.30
        subtitleManager.SetColor("CLEM");
        StartCoroutine(_typing(subtitle1[3]));
        yield return new WaitForSeconds(2.5f); // 17
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 18
        subtitleManager.SetColor("DUCK");
        StartCoroutine(_typing(subtitle1[4]));
        yield return new WaitForSeconds(1.5f); // 19.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 20.30
        subtitleManager.SetColor("CLEM");
        StartCoroutine(_typing(subtitle1[5]));
        yield return new WaitForSeconds(5.5f); // 26
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 27
        subtitleManager.SetColor("DUCK");
        StartCoroutine(_typing(subtitle1[6]));
        yield return new WaitForSeconds(1.5f); // 28.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(0.5f); // 29
        // 29.0
        // 치즈 크래커 가지고 있을 시 Anim3 재생
        if(talkCondionTrigger.getCheeseCrack() == true)
        {
            anim1.SetActive(false);

            anim3.SetActive(true);
            PlayableDirector scene3 = anim3.GetComponent<PlayableDirector>();
            scene3.Play();

            StartCoroutine(Subtiles3());
        }
        yield return new WaitForSeconds(3.5f);  // 32.30

        if(anim1.activeSelf == true)
        {
            anim1.SetActive(false);
            playercontroller.val = false;
            talkEventManager.isTalking = false;
            talkCondionTrigger.SetTalkTrigger("Duck");
            talkCondionTrigger.MissionComplete(6);
        }
    }

    IEnumerator Subtiles2()
    {
        yield return new WaitForSeconds(6f);
        anim2.SetActive(false);

        // 바로 Anim3로 넘어간다.
        anim3.SetActive(true);
        PlayableDirector scene3 = anim3.GetComponent<PlayableDirector>();
        scene3.Play();

        StartCoroutine(Subtiles3());
    }

    IEnumerator Subtiles3()
    {
        subtitleManager.SetColor("CLEM");
        cheeseCracker.SetActive(true);
        yield return new WaitForSeconds(6.75f); // 5.45
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(0.75f); // 7.30
        StartCoroutine(_typing(subtitle3[0]));
        yield return new WaitForSeconds(2f); // 9.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 10.30
        StartCoroutine(_typing(subtitle3[1]));
        yield return new WaitForSeconds(5.5f); // 16
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(4f); // 20
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        selectCanvas.SetActive(true);
        uiEventMangaer.startTimer(20f);

        yield return new WaitForSeconds(20f); // 20
        if (anim3.activeSelf == true) selectNo();
    }

    public void selectYes()
    {
        PlayableDirector scene3 = anim3.GetComponent<PlayableDirector>();
        scene3.Stop();
        anim3.SetActive(false);

        uiEventMangaer.canvasLongFade(3f, "클레멘타인은 덕에게 치즈크래커를 주었습니다.");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        selectCanvas.SetActive(false);

        uiEventMangaer.resetTimer();

        // Yes 선택 시 Anim4 재생
        anim4.SetActive(true);
        PlayableDirector scene4 = anim4.GetComponent<PlayableDirector>();
        scene4.Play();
        StartCoroutine(Subtiles4());
    }

    public void selectNo()
    {
        PlayableDirector scene3 = anim3.GetComponent<PlayableDirector>();
        scene3.Stop();
        anim3.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        selectCanvas.SetActive(false);

        uiEventMangaer.resetTimer();

        // No 선택 시 Anim5 재생
        anim5.SetActive(true);
        PlayableDirector scene5 = anim5.GetComponent<PlayableDirector>();
        scene5.Play();
        StartCoroutine(Subtiles5());
    }

    IEnumerator Subtiles4()
    {
        yield return new WaitForSeconds(5.5f); // 5.30
        subtitleManager.SetColor("CLEM");
        StartCoroutine(_typing(subtitle4[0]));
        yield return new WaitForSeconds(4f); // 9.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1.5f); // 11
        StartCoroutine(_typing(subtitle4[1]));
        yield return new WaitForSeconds(1.5f); // 12.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 13.30
        subtitleManager.SetColor("DUCK");
        StartCoroutine(_typing(subtitle4[2]));
        yield return new WaitForSeconds(1f); // 14.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1.5f); // 16
        subtitleManager.SetColor("CLEM");
        StartCoroutine(_typing(subtitle4[3]));
        yield return new WaitForSeconds(4.5f); // 20.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 21.30
        subtitleManager.SetColor("DUCK");
        StartCoroutine(_typing(subtitle4[4]));
        yield return new WaitForSeconds(4f); // 24.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(4.5f); // 29

        cheeseCracker.SetActive(false);
        anim4.SetActive(false);
        playercontroller.val = false;
        talkEventManager.isTalking = false;

        talkCondionTrigger.SetTalkTrigger("Duck");
        talkCondionTrigger.MissionComplete(6);
    }

    IEnumerator Subtiles5()
    {
        subtitleManager.SetColor("CLEM");
        yield return new WaitForSeconds(6f); 
        StartCoroutine(_typing(subtitle5[0]));
        yield return new WaitForSeconds(4f); 
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(5f); // 15

        cheeseCracker.SetActive(false);
        anim5.SetActive(false);
        playercontroller.val = false;
        talkEventManager.isTalking = false;
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
