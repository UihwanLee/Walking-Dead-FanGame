using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Talk_Event_FoodBox : MonoBehaviour {

    /*
     * FoodBox 컷씬
     * 
     * 식량을 확보하기 위한 컷씬
     * 
     * (미션갱신) 가방을 가지고 있음/없음
     * 
     * 가방 있을 시 : 미션("농장 문으로 모이시오.")
     * 가방 없을 시 : 미션("가방을 찾으시오.")
     * 
     */

    [SerializeField] private GameObject NPC;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject icon;

    // 컷씬 변수
    [SerializeField] private GameObject anim1;

    // Subtitle
    private string[] subtitle1;

    // UI 변수
    [SerializeField] private GameObject textBox;

    // System Script 변수
    private UiEventTrigger uiEventManager;
    private Talk_EventTrigger talkEventManager;
    private PlayerController playercontroller;
    private TalkConditionTrigger talkCondionTrigger;
    private SubtitleManager subtitleManager;

    // Use this for initialization
    void Start () {
        anim1.SetActive(false);

        // 스크립트 및 오브젝트 찾기
        uiEventManager = FindObjectOfType<UiEventTrigger>();
        playercontroller = FindObjectOfType<PlayerController>();
        talkEventManager = NPC.GetComponent<Talk_EventTrigger>();
        talkCondionTrigger = FindObjectOfType<TalkConditionTrigger>();
        subtitleManager = FindObjectOfType<SubtitleManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        CheckTalkEvent();
        CheckSubtitle();   
    }

    private void CheckTalkEvent()
    {
        if (Vector3.Distance(player.transform.position, this.transform.position) < 2)
        {
            if (Input.GetKeyDown(KeyCode.F) && talkEventManager.isTalking == false && icon.activeSelf == true)
            {
                talkEventManager.isTalking = true;
                playercontroller.val = true;
                Talk_FoodBox();
            }
        }
        else
        {
            talkEventManager.isTalking = false;
        }
    }

    private void CheckSubtitle()
    {
        subtitle1 = subtitleManager.getSubtitles("FoodBoxAnim1");
    }

    private void Talk_FoodBox()
    {
        anim1.SetActive(true);
        PlayableDirector scene1 = anim1.GetComponent<PlayableDirector>();
        scene1.Stop();

        player.transform.position = new Vector3(2.761f, 0f, -30.327f);
        player.transform.rotation = Quaternion.Euler(new Vector3(0, 150, 0));

        scene1.Play();
        StartCoroutine(Subtiles());
    }

    IEnumerator Subtiles()
    {
        subtitleManager.SetColor("CLEM");
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(_typing(subtitle1[0]));
        yield return new WaitForSeconds(2f);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(1f);
        if (talkCondionTrigger.getBag()) StartCoroutine(_typing(subtitle1[1]));
        else StartCoroutine(_typing(subtitle1[2]));
        yield return new WaitForSeconds(2.5f);
        textBox.GetComponent<Text>().text = "";
        yield return new WaitForSeconds(2.5f);

        anim1.SetActive(false);
        playercontroller.val = false;
        talkEventManager.isTalking = false;

        uiEventManager.canvasMissionFade(4f, talkCondionTrigger.missions[7]);
        talkCondionTrigger.TalkeEventByMission(talkCondionTrigger.missions[7]);

        talkCondionTrigger.hasObj("Food");

        talkCondionTrigger.SetTalkTrigger("FoodBox");
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
