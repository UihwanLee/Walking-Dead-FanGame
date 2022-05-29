using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutFieldMissionManager : MonoBehaviour {

    // OutFiled 퀘스트 관리 스크립트

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] point; // 포인트 지점
    [SerializeField] private GameObject[] missionItem; // 미션 아이템
    [SerializeField] private string[] message_Point; // 포인트 갱신 대사
    [SerializeField] private string[] message_Item; // 미션 아이템 갱신 대사

    [SerializeField] private GameObject textBox;

    private bool isMission1 = false;
    private bool isMission2 = false;
    private bool isMission3 = false;
    private bool isMission4 = false;

    private bool isPoint1 = false;
    private bool isPoint2 = false;
    private bool isPoint3 = false;
    private bool isPoint4 = false;

    // 시스템 스크립트
    [SerializeField] private Inventory theInventory;
    private SubtitleManager subtitleManager;
    private UiEventTrigger uiEventManager;
    private TalkConditionTrigger talkConditionTrigger;
    private Inventory theInven;

    const string mission1 = "자동차 메뉴얼 (미션 아이템)", mission2 = "연료통 (0%) (미션 아이템)", mission2_Complete = "연료통 (100%) (미션 아이템)",
        mission3 = "엔진 배터리 (미션 아이템)", mission4 = "타이어 (미션 아이템)";

    public void LoadToOutManager(List<bool> isPoints)
    {
        isPoint1 = isPoints[0];
        isPoint2 = isPoints[1];
        isPoint3 = isPoints[2];
        isPoint4 = isPoints[3];
    }

    void Start()
    {
        subtitleManager = FindObjectOfType<SubtitleManager>();
        uiEventManager = FindObjectOfType<UiEventTrigger>();
        talkConditionTrigger = FindObjectOfType<TalkConditionTrigger>();
    }

    // Update is called once per frame
    void Update () {
        CheckDistance();
    }

    private void CheckDistance()
    {
        // 포인트 지점
        for (int i = 0; i < point.Length; i++)
        {
            if (Vector3.Distance(player.transform.position, point[i].transform.position) < 20)
                PlayerInterectPoint(i);
        }

        // 미션 아이템
        for (int i = 0; i < point.Length; i++)
        {
            if (Vector3.Distance(player.transform.position, missionItem[i].transform.position) < 4)
                if (Input.GetKeyDown(KeyCode.F))
                {
                    StartCoroutine(WaitAndAct(i));
                }
        }
    }

    IEnumerator WaitAndAct(int index)
    {
        yield return new WaitForSeconds(0.5f);
        switch (index)
        {
            case 0:
                if (theInventory.FindItem(mission1)) PlayerInterectItem(index);
                break;
            case 1:
                if (theInventory.FindItem(mission2)) PlayerInterectItem(index);
                break;
            case 2:
                if (theInventory.FindItem(mission3)) PlayerInterectItem(index);
                isPoint3 = true;
                break;
            case 3:
                if (theInventory.FindItem(mission4)) PlayerInterectItem(index);
                isPoint4 = true;
                break;
            default:
                break;
        }
    }

    // 포인트 지점
    private void PlayerInterectPoint(int point)
    {
        switch (point)
        {
            case 0:
                if (!isPoint1) StartCoroutine(Effect(point, message_Point));
                isPoint1 = true;
                break;
            case 1:
                if (!isPoint2) StartCoroutine(Effect(point, message_Point));
                isPoint2 = true;
                break;
            case 2:
                if (!isPoint3) StartCoroutine(Effect(point, message_Point));
                isPoint3 = true;
                break;
            case 3:
                if (!isPoint4) StartCoroutine(Effect(point, message_Point));
                isPoint4 = true;
                break;
            default:
                break;
        }
    }

    // 미션 아이템
    private void PlayerInterectItem(int point)
    {
        switch (point)
        {
            case 0:
                isMission1 = talkConditionTrigger.getMission1();
                if (!isMission1)
                {
                    StartCoroutine(Effect(point, message_Item));
                    talkConditionTrigger.hasOutMission(1);
                    CheckMission();
                }
                isMission1 = talkConditionTrigger.getMission1();
                break;
            case 1:
                isMission2 = talkConditionTrigger.getMission2();
                if (!isMission2)
                {
                    StartCoroutine(Effect(point, message_Item));
                    talkConditionTrigger.hasOutMission(2);
                    CheckMission();
                }
                isMission2 = talkConditionTrigger.getMission2();
                break;
            case 2:
                isMission3 = talkConditionTrigger.getMission3();
                if (!isMission3)
                {
                    StartCoroutine(Effect(point, message_Item));
                    talkConditionTrigger.hasOutMission(3);
                    CheckMission();
                }
                isMission3 = talkConditionTrigger.getMission3();
                break;
            case 3:
                isMission4 = talkConditionTrigger.getMission4();
                if (!isMission4)
                {
                    StartCoroutine(Effect(point, message_Item));
                    talkConditionTrigger.hasOutMission(4);
                    CheckMission();
                }
                isMission4 = talkConditionTrigger.getMission4();
                break;
            default:
                break;
        }
    }

    // 미션 체크
    public void CheckMission()
    {
        if(theInventory.FindItem(mission1) && theInventory.FindItem(mission2_Complete) && theInventory.FindItem(mission3) && theInventory.FindItem(mission4))
        {
            StartCoroutine(AllMissionComplete());
        }
    }

    IEnumerator AllMissionComplete()
    {
        yield return new WaitForSeconds(6.5f);
        StartCoroutine(Effect_Normal("좋아 이제 다 모았으니까 케니 아저씨 자동차로 돌아가자."));
        uiEventManager.canvasMissionFade(4f, talkConditionTrigger.missions[10]);
        // talkEventTrigger 전달
    }

    // 자막 타이핑 이펙트
    IEnumerator Effect_Normal(string message)
    {
        yield return new WaitForSeconds(5f);
        subtitleManager.SetColor("CLEM");
        StartCoroutine(_typing(message));
        yield return new WaitForSeconds(5f);
        textBox.GetComponent<Text>().text = "";
    }

    // 자막 타이핑 이펙트
    IEnumerator Effect(int point, string[] message)
    {
        subtitleManager.SetColor("CLEM");
        StartCoroutine(_typing(message[point]));
        yield return new WaitForSeconds(5f); 
        textBox.GetComponent<Text>().text = "";
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

    public bool getPoint1() { return isPoint1; }
    public bool getPoint2() { return isPoint2; }
    public bool getPoint3() { return isPoint3; }
    public bool getPoint4() { return isPoint4; }
}
