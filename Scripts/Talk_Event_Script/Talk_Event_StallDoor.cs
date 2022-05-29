using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class Talk_Event_StallDoor : MonoBehaviour
{

    /*
     * StallDoor 컷씬
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
    [SerializeField] private GameObject icon;

    // 컷씬 변수
    [SerializeField] private GameObject anim1;
    [SerializeField] private GameObject anim2;

    // Subtitle
    private string[] subtitle1;
    private string[] subtitle2;

    // UI 변수
    [SerializeField] private GameObject textBox;

    // System Script 변수
    private UiEventTrigger uiEventMangaer;
    private Talk_EventTrigger talkEventManager;
    private PlayerController playercontroller;
    private TalkConditionTrigger talkCondionTrigger;
    private SubtitleManager subtitleManager;
    
    // 이벤트 변수
    private Talk_Event_Stall talkEventStall;

    // 인벤토리
    private Inventory theInventory;

    // 열쇠 아이템
    [SerializeField] private Item stallKey;

    // Use this for initialization
    void Start()
    {
        anim1.SetActive(false);
        anim2.SetActive(false);

        // 스크립트 및 오브젝트 찾기
        uiEventMangaer = FindObjectOfType<UiEventTrigger>();
        playercontroller = FindObjectOfType<PlayerController>();
        talkEventManager = NPC.GetComponent<Talk_EventTrigger>();
        talkCondionTrigger = FindObjectOfType<TalkConditionTrigger>();
        subtitleManager = FindObjectOfType<SubtitleManager>();

        // 이벤트 변수 초기화
        talkEventStall = FindObjectOfType<Talk_Event_Stall>();

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
                Talk_StallFarm();
            }
        }
        else
        {
            talkEventManager.isTalking = false;
        }
    }

    private void CheckSubtitle()
    {
        subtitle1 = subtitleManager.getSubtitles("StallDoorAnim1");
        subtitle2 = subtitleManager.getSubtitles("StallDoorAnim2");
    }

    private void Talk_StallFarm()
    {
        anim1.SetActive(true);
        PlayableDirector scene1 = anim1.GetComponent<PlayableDirector>();
        scene1.Stop();

        anim2.SetActive(true);
        PlayableDirector scene2 = anim2.GetComponent<PlayableDirector>();
        scene2.Stop();

        player.transform.position = new Vector3(1.134f, 0f, -12.715f);
        player.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));

        if (talkCondionTrigger.getHershelTalk() == true && talkCondionTrigger.getGun() == true)
        {
            scene2.Play();
            StartCoroutine(Subtiles2());
            anim1.SetActive(false);
        }
        else
        {
            scene1.Play();
            StartCoroutine(Subtiles());
            anim2.SetActive(false);
        }
    }

    IEnumerator Subtiles()
    {
        subtitleManager.SetColor("CLEM");
        yield return new WaitForSeconds(5f);
        if (talkCondionTrigger.getHershelTalk() == true) StartCoroutine(_typing(subtitle1[0]));
        else StartCoroutine(_typing(subtitle1[1]));
        yield return new WaitForSeconds(2.3f);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(2.7f);

        anim1.SetActive(false);
        playercontroller.val = false;
        talkEventManager.isTalking = false;
    }

    IEnumerator Subtiles2()
    {
        subtitleManager.SetColor("CLEM");
        yield return new WaitForSeconds(6f);
        StartCoroutine(_typing(subtitle2[0]));
        yield return new WaitForSeconds(4f);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f); // 11
        StartCoroutine(_typing(subtitle2[1]));
        yield return new WaitForSeconds(1.5f); // 12.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(_typing(subtitle2[2]));
        yield return new WaitForSeconds(2.5f); // 15.30
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(3f); // 18.30
        uiEventMangaer.canvasLongFade(4f, "허쉘농장 마구간 열쇠를 사용하였습니다.");
        theInventory.DeleteItem(stallKey);
        yield return new WaitForSeconds(4f); // 22.30
        StartCoroutine(_typing(subtitle2[3]));
        yield return new WaitForSeconds(1.5f); // 24
        StartCoroutine(_typing(""));
        uiEventMangaer.startFade();
        yield return new WaitForSeconds(2.7f);
        // Stall Anim 실행
        anim2.SetActive(false);
        talkEventStall.Talk_Stall();
        yield return new WaitForSeconds(0.56f); // 27.16

        uiEventMangaer.resetCanvas(1);
        yield return new WaitForSeconds(5f);
        uiEventMangaer.resetCanvas(3);

        talkCondionTrigger.SetTalkTrigger("StallDoor");
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
