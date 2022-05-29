using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class Talk_Event_Fuel : MonoBehaviour {

    // 연료가 있는지 체크하는 변수.
    public int carNum; // 0, 1, 2 -> 연료가 들어있는 변수
    public bool isTalking = false;

    [SerializeField] private GameObject obj;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject talk_icon;
    [SerializeField] private GameObject canvas;
    [SerializeField] private SpriteRenderer spriteRender;
    [SerializeField] private Sprite talk_icon1;
    [SerializeField] private Sprite talk_icon2;

    // 컷씬 변수
    [SerializeField] private GameObject anim;

    // 오브잭트
    [SerializeField] private GameObject fuel;

    // System Script 변수
    private UiEventTrigger uiEventManager;
    private Talk_EventTrigger talkEventManager;
    private PlayerController playercontroller;
    private TalkConditionTrigger talkCondionTrigger;
    private SubtitleManager subtitleManager;
    private OutFieldMissionManager theOutFieldMissionManager;

    // 인벤토리
    private Inventory theInventory;

    // 플레이어 위치 조정 변수
    [SerializeField] private Vector3 pos;
    [SerializeField] private Vector3 rot;

    // 대사
    private string theMessage = "";
    [SerializeField] private string[] subtitle;

    // 연료 아이템 조건
    private const string Empty = "연료통 (0%) (미션 아이템)" , Little = "연료통 (30%) (미션 아이템)", 
        Pretty = "연료통 (70%) (미션 아이템)", Full = "연료통 (100%) (미션 아이템)", LittleInfo = "연료가 조금 들어있는 연료통이다.",
        PrettyInfo = "연료가 조금 많이 들어있는 연료통이다.", FullInfo = "연료가 꽉 찬 연료통이다.";

    // 텍스트
    [SerializeField] private GameObject textBox;

    // Use this for initialization
    void Start()
    {
        anim.SetActive(false);

        // 스크립트 및 오브젝트 찾기
        uiEventManager = FindObjectOfType<UiEventTrigger>();
        playercontroller = FindObjectOfType<PlayerController>();
        talkCondionTrigger = FindObjectOfType<TalkConditionTrigger>();
        subtitleManager = FindObjectOfType<SubtitleManager>();
        theOutFieldMissionManager = FindObjectOfType<OutFieldMissionManager>();

        // 인벤토리
        theInventory = FindObjectOfType<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        if(theInventory.FindItem(Empty) || theInventory.FindItem(Little) || theInventory.FindItem(Pretty))
            CheckIcon();
        else
        {
            talk_icon.SetActive(false);
            canvas.SetActive(false);
        }
    }

    private void CheckIcon()
    {
        if (!isTalking)
        {
            if (Vector3.Distance(player.transform.position, obj.transform.position) < 7.5)
            {
                talk_icon.SetActive(true);
                if (talk_icon.activeSelf == true)
                    spriteRender = talk_icon.GetComponent<SpriteRenderer>();
                if (Vector3.Distance(player.transform.position, obj.transform.position) < 4)
                {
                    canvas.SetActive(true);
                    spriteRender.sprite = talk_icon2;
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        if(carNum <= 2)
                        {
                            CheckFuelCondition();
                            playercontroller.val = true;
                            Talk_Fuel();
                        }
                        else
                        {
                            theMessage = subtitle[3];
                            StartCoroutine(Effect(theMessage));
                        }
                        isTalking = true;
                    }
                }
                else
                {
                    canvas.SetActive(false);
                    spriteRender.sprite = talk_icon1;
                }
            }
            else
            {
                talk_icon.SetActive(false);
            }
        }
        else
        {
            talk_icon.SetActive(false);
            canvas.SetActive(false);
        }
    }

    private void Talk_Fuel()
    {
        fuel.SetActive(true);
        talk_icon.SetActive(false);

        // 플레이어 위치 조정
        player.transform.position = pos;
        player.transform.rotation = Quaternion.Euler(rot);

        anim.SetActive(true);
        PlayableDirector scene = anim.GetComponent<PlayableDirector>();
        scene.Play();
        StartCoroutine(StartFuelPump());
    }

    // 자막 타이핑 이펙트
    IEnumerator StartFuelPump()
    {
        yield return new WaitForSeconds(9.5f);
        fuel.SetActive(false);
        playercontroller.val = false;
        StartCoroutine(Effect(theMessage));
    }

    // 인벤토리 연료 체크 함수
    private void CheckFuelCondition()
    {
        if (theInventory.FindItem(Empty))
        {
            theMessage = subtitle[0];
            theInventory.ChangeItemInfo(Empty, Little, LittleInfo);
        }
        else if (theInventory.FindItem(Little))
        {
            theMessage = subtitle[1];
            theInventory.ChangeItemInfo(Little, Pretty, PrettyInfo);
        }
        else if (theInventory.FindItem(Pretty))
        {
            theMessage = subtitle[2];
            theInventory.ChangeItemInfo(Pretty, Full, FullInfo);
            theOutFieldMissionManager.CheckMission();
        }
    }

    // 자막 타이핑 이펙트
    IEnumerator Effect(string message)
    {
        subtitleManager.SetColor("CLEM");
        StartCoroutine(_typing(message));
        yield return new WaitForSeconds(5f);
        textBox.GetComponent<Text>().text = "";

        obj.SetActive(false);
        talk_icon.SetActive(false);
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
