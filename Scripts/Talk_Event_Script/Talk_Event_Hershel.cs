using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class Talk_Event_Hershel : MonoBehaviour {

    /*
     * Hershel 컷씬
     * 
     * 식량 위치 정보를 얻기 위한 컷씬
     * 
     * (변화) 총을 가지고 있음/없음
     * 
     * 총 있을 시 : 괜찮아요, 저한테는 총이 준비되어 있어요.
     * 총 없을 시 : 알겠습니다. 대비할만한 무언가 먼저 찾을게요.
     * 
     * (미션 갱신)
     * "마구간 안의 좀비를 처리할 총을 찾으시오."
     * 
     * clear
     * "마구간 안으로 들어가 식량을 찾으시오."
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
    private UiEventTrigger uiEventManager;
    private Talk_EventTrigger talkEventManager;
    private PlayerController playercontroller;
    private TalkConditionTrigger talkCondionTrigger;
    private SubtitleManager subtitleManager;

    // 참값 변수
    private bool isTalk;

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
        uiEventManager = FindObjectOfType<UiEventTrigger>();
        playercontroller = FindObjectOfType<PlayerController>();
        talkEventManager = NPC.GetComponent<Talk_EventTrigger>();
        talkCondionTrigger = FindObjectOfType<TalkConditionTrigger>();
        subtitleManager = FindObjectOfType<SubtitleManager>();

        // 인벤토리
        theInventory = FindObjectOfType<Inventory>();

        isTalk = talkCondionTrigger.getTalkOneHershel();
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
                Talk_Hershel();
            }
        }
        else
        {
            talkEventManager.isTalking = false;
        }
    }

    private void CheckSubtitle()
    {
        subtitle1 = subtitleManager.getSubtitles("HershelAnim1");
        subtitle2 = subtitleManager.getSubtitles("HershelAnim2");
    }

    private void Talk_Hershel()
    {
        talkCondionTrigger.isTalk("Hershel");

        anim1.SetActive(true);
        PlayableDirector scene1 = anim1.GetComponent<PlayableDirector>();
        scene1.Stop();

        anim2.SetActive(true);
        PlayableDirector scene2 = anim2.GetComponent<PlayableDirector>();
        scene2.Stop();

        player.transform.position = new Vector3(20.13f, 0f, 13f);
        player.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));

        isTalk = talkCondionTrigger.getTalkOneHershel();

        if (isTalk == false)
        {
            scene1.Play();
            StartCoroutine(Subtiles());
            anim2.SetActive(false);
        }
        else
        {
            scene2.Play();
            StartCoroutine(Subtiles2());
            anim1.SetActive(false);
        }
    }

    IEnumerator Subtiles()
    {
        yield return new WaitForSeconds(4f);
        subtitleManager.SetColor("HERSHEL");
        StartCoroutine(_typing(subtitle1[0]));
        yield return new WaitForSeconds(4f); 
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f);
        subtitleManager.SetColor("CLEM");
        StartCoroutine(_typing(subtitle1[1]));
        yield return new WaitForSeconds(5f);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f);
        subtitleManager.SetColor("HERSHEL");
        StartCoroutine(_typing(subtitle1[2]));
        yield return new WaitForSeconds(5f);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f);
        subtitleManager.SetColor("CLEM");
        StartCoroutine(_typing(subtitle1[3]));
        yield return new WaitForSeconds(6f);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f);
        subtitleManager.SetColor("HERSHEL");
        StartCoroutine(_typing(subtitle1[4]));
        yield return new WaitForSeconds(5.5f);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f);
        StartCoroutine(_typing(subtitle1[5]));
        yield return new WaitForSeconds(4f);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f);
        StartCoroutine(_typing(subtitle1[6]));
        yield return new WaitForSeconds(6f);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f);
        subtitleManager.SetColor("CLEM");
        if (talkCondionTrigger.getGun()== true) StartCoroutine(_typing(subtitle1[7]));
        else StartCoroutine(_typing(subtitle1[8]));
        yield return new WaitForSeconds(5f);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(5f);

        anim1.SetActive(false);
        playercontroller.val = false;
        talkEventManager.isTalking = false;

        // 마구간 열쇠 인벤토리 Get
        theInventory.AcquireItem(stallKey, stallKey.count);
        uiEventManager.canvasCollectFade(5f, stallKey.itemName, stallKey.itemInfo);

        isTalk = true;

        // 미션 컴플리트
        talkCondionTrigger.MissionComplete(2);
        if (talkCondionTrigger.getGun())
            uiEventManager.canvasMissionFade(4f, talkCondionTrigger.missions[5]);
        else
            uiEventManager.canvasMissionFade(4f, talkCondionTrigger.missions[4]);

        talkCondionTrigger.TalkOneNPC(3);

        yield return new WaitForSeconds(10f);
        uiEventManager.resetCanvas(2);
        uiEventManager.resetCanvas(4);
    }

    IEnumerator Subtiles2()
    {
        yield return new WaitForSeconds(3.5f);
        subtitleManager.SetColor("CLEM");
        StartCoroutine(_typing(subtitle2[0]));
        yield return new WaitForSeconds(4f);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f);
        subtitleManager.SetColor("HERSHEL");
        StartCoroutine(_typing(subtitle2[1]));
        yield return new WaitForSeconds(5f);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(3.5f);
        
        anim2.SetActive(false);
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
