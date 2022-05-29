using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using Cinemachine;

public class Talk_Event_Stall : MonoBehaviour {

    /*
     * Stall 컷씬
     * 
     * 마구간 안으로 들어가기 위한 컷씬
     * 
     * (변화) 총을 가지고 있음/없음
     * 
     * 총 있을 시 : Anim1 재생
     * 총 없을 시 : Anim2 재생 -> 마구간으로 들어감
     * 
     * 페이드 인/아웃 효과
     * 
     * (미션 갱신)
     * "좀비를 처리하고 식량을 찾으시오."
     * 
     */

    [SerializeField] private GameObject NPC;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject zombie;
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

    // 카메라
    [SerializeField] private GameObject camBase;

    // System Script 변수
    private UiEventTrigger uiEventManager;
    private PlayerController playercontroller;
    private TalkConditionTrigger talkCondionTrigger;
    private SubtitleManager subtitleManager;
    private Talk_EventTrigger talkEventManager;

    [SerializeField]
    private GameObject camControllerObj;
    private CameraController camController;

    [SerializeField] private GameObject gun1;
    [SerializeField] private GameObject gun2;

    // 애니메이터 & 카메라
    private Animator animator;

    // 좀비 AI_Test 스크립트
    [SerializeField]
    private Zombie_AI zombeAI;

    // 도움창 - 좀비
    [SerializeField] private Help help_ZombieKill;

    // 애니메이션 세이브 데이터
    public bool animCondition;

    // 세이브 데이터 스크립트
    [SerializeField] private SaveNLoad theSaveNLoad;

    public void LoadToAnim(bool animCondition)
    {
        if(animCondition)
        {
            StartCoroutine(WaitForLoading());
        }
    }

    IEnumerator WaitForLoading()
    {
        yield return new WaitForSeconds(4.5f);
        Talk_Stall();
    }

    // Use this for initialization
    void Start()
    {
        animCondition = false;

        anim1.SetActive(false);
        anim2.SetActive(false);
        anim3.SetActive(false);

        // 스크립트 및 오브젝트 찾기
        uiEventManager = FindObjectOfType<UiEventTrigger>();
        playercontroller = FindObjectOfType<PlayerController>();
        talkCondionTrigger = FindObjectOfType<TalkConditionTrigger>();
        camController = camControllerObj.GetComponent<CameraController>();
        subtitleManager = FindObjectOfType<SubtitleManager>();
        talkEventManager = NPC.GetComponent<Talk_EventTrigger>();

        // 애니메이터
        animator = player.GetComponent<Animator>();
    }

    public void Update()
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
                CheckConditiction();
            }
        }
        else
        {
            talkEventManager.isTalking = false;
        }
    }

    private void CheckSubtitle()
    {
        subtitle1 = subtitleManager.getSubtitles("StallAnim1");
        subtitle2 = subtitleManager.getSubtitles("StallAnim2");
        subtitle3 = subtitleManager.getSubtitles("StallAnim3");
    }

    private void CheckConditiction()
    {
        if (talkCondionTrigger.getFood())
        {
            StartCoroutine(Out());
        }
        else
        {
            anim3.SetActive(true);
            PlayableDirector scene3 = anim3.GetComponent<PlayableDirector>();
            scene3.Play();
            StartCoroutine(Subtiles3());
        }
    }

    public void Talk_Stall()
    {
        animCondition = true;

        playercontroller.val = true;
        anim1.SetActive(true);
        PlayableDirector scene1 = anim1.GetComponent<PlayableDirector>();
        scene1.Stop();

        anim2.SetActive(true);
        PlayableDirector scene2 = anim2.GetComponent<PlayableDirector>();
        scene2.Stop();

        if(!talkCondionTrigger.getKillZombie())
        {
            // 세이브 데이터 저장
            theSaveNLoad.SaveData();

            player.transform.position = new Vector3(0.61f, 0f, -15.09f);
            player.transform.rotation = Quaternion.Euler(new Vector3(0, 50, 0));

            // 좀비
            zombie.transform.position = new Vector3(1.54f, -0.09915876f, -31.83f);
            zombie.transform.rotation = Quaternion.Euler(new Vector3(0, 106.723f, 0));

            scene1.Play();
            StartCoroutine(Subtiles());

            camBase.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            camController.setPos();

            anim2.SetActive(false);
        }
        else
        {
            scene2.Play();
            StartCoroutine(Subtiles2());
            animator.SetLayerWeight(animator.GetLayerIndex("GunMode"), 0f);

            camController.resPos();
            uiEventManager.retShot();
            playercontroller.changeNormalMode();

            anim1.SetActive(false);

            AudioManager.instance.PlayBGM("Background_Normal");
        }
    }

    IEnumerator Subtiles()
    {
        subtitleManager.SetColor("CLEM");
        animator.SetLayerWeight(animator.GetLayerIndex("GunMode"), 1f);

        yield return new WaitForSeconds(4.5f);
        player.transform.position = new Vector3(0.83f, 0f, -15.97f);
        player.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0)); 
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(_typing(subtitle1[0]));
        yield return new WaitForSeconds(4.5f); // 9.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(0.5f);
        zombie.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0)); 
        yield return new WaitForSeconds(2.5f); // 12.30
        StartCoroutine(_typing(subtitle1[1]));
        yield return new WaitForSeconds(3f); // 15.30
        gun1.SetActive(false);
        gun2.SetActive(true);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(8.30f); // 24

        animator.SetBool("isShotMode", true);
        playercontroller.changeShotMode();

        uiEventManager.setShot();
        playercontroller.changeShotMode();
        anim1.SetActive(false);

        playercontroller.val = false;
        //talkEventManager.isTalking = false;

        AudioManager.instance.PlayBGM("Background_Tensy");

        // 미션 컴플리트
        talkCondionTrigger.MissionComplete(3);
        uiEventManager.canvasMissionFade(4f, talkCondionTrigger.missions[6]);

        // 도움창 띄우기
        uiEventManager.setCanvasHelp(help_ZombieKill);

        yield return new WaitForSeconds(8f);
        uiEventManager.resetCanvas(4);
        uiEventManager.canvasExplainFade(3f, "마우스를 움직여 조준 후 쏘세요.");
    }

    IEnumerator Subtiles2()
    {
        animCondition = false;

        subtitleManager.SetColor("CLEM");
        gun1.SetActive(true);
        gun2.SetActive(false);
        yield return new WaitForSeconds(3.5f);
        StartCoroutine(_typing(subtitle2[0]));
        yield return new WaitForSeconds(1.5f); // 4.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(_typing(subtitle2[1]));
        yield return new WaitForSeconds(2f); // 7.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(2.5f); // 10

        playercontroller.val = false;

        talkCondionTrigger.SetTalkTrigger("Stall");

        anim2.SetActive(false);
        animator.SetBool("isShotMode", false);

        yield return new WaitForSeconds(8f);
        uiEventManager.resetCanvas(4);

    }

    IEnumerator Subtiles3()
    {
        subtitleManager.SetColor("CLEM");
        player.transform.position = new Vector3(0.99f, 0f, -15.56f);
        player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        yield return new WaitForSeconds(1f);
        StartCoroutine(_typing(subtitle3[0]));
        yield return new WaitForSeconds(2.5f); // 3.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(2f); // 5.30

        playercontroller.val = false;
        talkEventManager.isTalking = false;

        anim3.SetActive(false);
    }

    IEnumerator Out()
    {
        uiEventManager.startFade();
        yield return new WaitForSeconds(2.7f);
  
        player.transform.position = new Vector3(0.99f, 0f, -12f);
        player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        playercontroller.val = false;
        talkEventManager.isTalking = false;

        // 세이브 데이터 저장
        theSaveNLoad.SaveData();
        talkCondionTrigger.SetTalkTrigger("StallDelete");
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
