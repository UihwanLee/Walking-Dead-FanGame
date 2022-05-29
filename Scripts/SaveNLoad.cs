using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
    // 플레이어 정보
    public Vector3 playerPos; // 플레이어 위치
    public Vector3 playerRot; // 플레이어 회전값
    public int playerHP; // 플레이어 체력
    public bool theGun_InField; // 상태변수 - 데저트 이글(In Field)
    public bool theGun_OutField; // 상태변수 - 데저트 이글(Out Field)
    public bool theBag; // 상태변수 - 가방
    public bool isGunMode; // 상태변수 - 건모드
    public bool isShotMode; // 상태변수 - 샷모드

    // 인벤토리 정보
    public List<int> invenArrayNumber = new List<int>(); // 아이템 저장 위치
    public List<string> invenItemName = new List<string>(); // 아이템 저장 이름
    public List<int> invenItemNumber = new List<int>(); // 아이템 저장 개수

    // 필드 아이템 정보
    public List<bool> items = new List<bool>(); // 필드 아이템 활성화/비활성화 정보

    // 애니메이션 정보
    public bool animCondition; // 마구간 좀비 애니메이션 상태

    // Setting 정보
    public float musicVolume; // BGM 볼륨
    public bool musicMute; // BGM mute 상태
    public float SFXVolume; // SFX 볼륨
    public bool SFXMute; // SFX mute 상태
    public float inFieldSensitivity; // In-Field Mouse 감도 
    public float outFieldSensitivity; // Out-Field Mouse 감도
    public float shotModeSensitivity; // Shot-Mode Mouse 감도

    // 날씨(밤/낮) 정보
    public bool isNight; // 밤인지 낮인지 확인하는 상태 변수

    // 케니 자동차 상태
    public Vector3 kennyCarPos; // 케니 자동차 위치
    public Vector3 kennyCarRot; // 케니 자동차 회전값

    // 좀비(봇)들의 상태
    public List<int> zombieBotsIndex = new List<int>(); // 좀비 봇 인덱스
    public List<bool> zombieBotsState = new List<bool>(); // 좀비 봇 활성화/비활성화 판별
    public List<bool> zombieBotsDead = new List<bool>(); // 좀비 봇 죽음 판별
    public List<Vector3> zombieBotsPos = new List<Vector3>(); // 좀비 봇 위치
    public List<Vector3> zombieBotsRot = new List<Vector3>(); // 좀비 봇 회전값

    // 미션 정보
    public List<bool> talkEventNPC = new List<bool>(); // NPC 및 오브젝트 상태 변수
    public List<bool> talkEventOneNPC = new List<bool>(); // 주요 NPC 대화 상태 변수
    public List<bool> talkEventObject = new List<bool>(); // 오브젝트 상태변수
    public List<bool> talkEventOutMission = new List<bool>(); // OutField 미션 상태 변수
    public List<bool> talkEventOutPoint = new List<bool>(); // OutField 포인트 상태 변수
    public string currentMission; // 저장 미션 정보.
}

public class SaveNLoad : MonoBehaviour {

    private SaveData saveData = new SaveData();

    private string SAVE_DATA_DIRECTORY;
    private string SAVE_FILENAME = "/SaveFile.txt";

    // 필요 컴포넌트 및 스크립트
    private PlayerController thePlayer;
    private Inventory theInven;
    private ItemManager itemManager;
    private Sun theSun;
    private Talk_Event_Stall theAnim;
    private TalkConditionTrigger talkConditionTrigger;
    private OutFieldMissionManager outFieldMissionManager;
    private Talk_Event_KennyCar theKennyCar;
    private ZombieSpawManager zombieSpawnManager;
    private PauseMenu thePauseMenu;

    // 로딩 스크립트
    private UiEventTrigger uiEventManager;

    // Use this for initialization
    void Start () {
        SAVE_DATA_DIRECTORY = Application.dataPath + "/Saves";

        if (!Directory.Exists(SAVE_DATA_DIRECTORY))
            Directory.CreateDirectory(SAVE_DATA_DIRECTORY);
	}
	
	public void SaveData()
    {
        // 로딩 스크립트
        uiEventManager = FindObjectOfType<UiEventTrigger>();

        // 필요 컴포넌트 및 스크립트
        thePlayer = FindObjectOfType<PlayerController>();
        theInven = FindObjectOfType<Inventory>();
        itemManager = FindObjectOfType<ItemManager>();
        theSun = FindObjectOfType<Sun>();
        theAnim = FindObjectOfType<Talk_Event_Stall>();
        talkConditionTrigger = FindObjectOfType<TalkConditionTrigger>();
        outFieldMissionManager = FindObjectOfType<OutFieldMissionManager>();
        theKennyCar = FindObjectOfType<Talk_Event_KennyCar>();
        zombieSpawnManager = FindObjectOfType<ZombieSpawManager>();
        thePauseMenu = FindObjectOfType<PauseMenu>();
        

        // 세이브 데이터 입력

        // 플레이어 정보
        saveData.playerPos = thePlayer.transform.position;
        saveData.playerRot = thePlayer.transform.eulerAngles;
        saveData.playerHP = thePlayer.getHP();
        saveData.theGun_InField = thePlayer.theGun_InField.activeSelf;
        saveData.theGun_OutField = thePlayer.theGun_OutField.activeSelf;
        saveData.theBag = thePlayer.theBag.activeSelf;
        saveData.isGunMode = thePlayer.isGunMode;
        saveData.isShotMode = thePlayer.isShotMode;

        // 인벤토리 정보
        saveData.invenArrayNumber.Clear();
        saveData.invenItemName.Clear();
        saveData.invenItemNumber.Clear();
        Slot[] slots = theInven.GetSlots();
        for (int i = 0; i < slots.Length; i++)
        {
            if(slots[i].item != null)
            {
                saveData.invenArrayNumber.Add(i);
                saveData.invenItemName.Add(slots[i].item.itemName);
                saveData.invenItemNumber.Add(slots[i].itemCount);
            }
        }

        // 필드 아이템 정보
        saveData.items.Clear();
        GameObject[] items = itemManager.GetItems();
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
            {
                saveData.items.Add(items[i].activeSelf);
            }
        }

        // Setting 정보
        saveData.musicVolume = thePauseMenu.getMusicVolume();
        saveData.musicMute = thePauseMenu.getMusicMute();
        saveData.SFXVolume = thePauseMenu.getSFXVolume();
        saveData.SFXMute = thePauseMenu.getSFXMute();
        saveData.inFieldSensitivity = thePauseMenu.getInFieldSensitivity();
        saveData.outFieldSensitivity = thePauseMenu.getOutFieldSensitivity();
        saveData.shotModeSensitivity = thePauseMenu.getShotModeSensitivity();

        // 애니메이션 정보
        saveData.animCondition = theAnim.animCondition;

        // 날씨(밤/낮) 정보
        saveData.isNight = theSun.isNight;

        // 케니 자동차
        saveData.kennyCarPos = theKennyCar.getKennyCarPos();
        saveData.kennyCarRot = theKennyCar.getKennyCarRot();

        // 좀비(봇)들의 상태 
        saveData.zombieBotsIndex.Clear();
        saveData.zombieBotsState.Clear();
        saveData.zombieBotsDead.Clear();
        saveData.zombieBotsPos.Clear();
        saveData.zombieBotsRot.Clear();
        GameObject[] zombieBots = zombieSpawnManager.getZombieBots();
        for (int i = 0; i < zombieBots.Length; i++)
        {
            saveData.zombieBotsIndex.Add(i);
            saveData.zombieBotsState.Add(zombieBots[i].activeSelf);
            saveData.zombieBotsDead.Add(zombieBots[i].GetComponent<Zombie_AI>().isDead);
            saveData.zombieBotsPos.Add(zombieBots[i].transform.position);
            saveData.zombieBotsRot.Add(zombieBots[i].transform.eulerAngles);
        }

        // 미션 정보
        saveData.talkEventNPC.Clear();
        saveData.talkEventOneNPC.Clear();
        saveData.talkEventObject.Clear();
        saveData.talkEventOutMission.Clear();
        saveData.talkEventOutPoint.Clear();

        saveData.talkEventNPC.Add(talkConditionTrigger.getFarmDoor1Talk()); saveData.talkEventNPC.Add(talkConditionTrigger.getLeeTalk());
        saveData.talkEventNPC.Add(talkConditionTrigger.getDuckTalk()); saveData.talkEventNPC.Add(talkConditionTrigger.getHershelTalk());
        saveData.talkEventNPC.Add(talkConditionTrigger.getStallTalk()); saveData.talkEventNPC.Add(talkConditionTrigger.getKillZombie());
        saveData.talkEventNPC.Add(talkConditionTrigger.getFarmDoor2Talk());

        saveData.talkEventOneNPC.Add(talkConditionTrigger.getTalkOneLee()); saveData.talkEventOneNPC.Add(talkConditionTrigger.getTalkOneKenny());
        saveData.talkEventOneNPC.Add(talkConditionTrigger.getTalkOneDuck()); saveData.talkEventOneNPC.Add(talkConditionTrigger.getTalkOneHershel());

        saveData.talkEventObject.Add(talkConditionTrigger.getGun()); saveData.talkEventObject.Add(talkConditionTrigger.getBag());
        saveData.talkEventObject.Add(talkConditionTrigger.getFood()); saveData.talkEventObject.Add(talkConditionTrigger.getCheeseCrack());

        saveData.talkEventOutMission.Add(talkConditionTrigger.getMission1()); saveData.talkEventOutMission.Add(talkConditionTrigger.getMission2());
        saveData.talkEventOutMission.Add(talkConditionTrigger.getMission3()); saveData.talkEventOutMission.Add(talkConditionTrigger.getMission4());

        saveData.talkEventOutPoint.Add(outFieldMissionManager.getPoint1()); saveData.talkEventOutPoint.Add(outFieldMissionManager.getPoint2());
        saveData.talkEventOutPoint.Add(outFieldMissionManager.getPoint3()); saveData.talkEventOutPoint.Add(outFieldMissionManager.getPoint4());

        saveData.currentMission = TalkConditionTrigger.currentMission;

        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME, json);

        // 로딩 UI 띄우기
        uiEventManager.startLoading(3f, "저장 중...");

        Debug.Log("저장 완료");
    }

    public void LoadData()
    {
        if(File.Exists(SAVE_DATA_DIRECTORY + SAVE_FILENAME))
        {
            string loadJson = File.ReadAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME);
            saveData = JsonUtility.FromJson<SaveData>(loadJson);

            thePlayer = FindObjectOfType<PlayerController>();
            theInven = FindObjectOfType<Inventory>();
            itemManager = FindObjectOfType<ItemManager>();
            theSun = FindObjectOfType<Sun>();
            theAnim = FindObjectOfType<Talk_Event_Stall>();
            talkConditionTrigger = FindObjectOfType<TalkConditionTrigger>();
            outFieldMissionManager = FindObjectOfType<OutFieldMissionManager>();
            theKennyCar = FindObjectOfType<Talk_Event_KennyCar>();
            zombieSpawnManager = FindObjectOfType<ZombieSpawManager>();
            thePauseMenu = FindObjectOfType<PauseMenu>();

            // saveData 적용

            // 플레이어 정보
            thePlayer.transform.position = saveData.playerPos;
            thePlayer.transform.eulerAngles = saveData.playerRot;
            thePlayer.LoadToPlayer(saveData.playerHP, saveData.theGun_InField, saveData.theGun_OutField, saveData.theBag, saveData.isGunMode, saveData.isShotMode);

            // 인벤토리 정보
            theInven.ClearInven();
            for(int i=0; i < saveData.invenItemName.Count; i++)
            {
                theInven.LoadToInven(saveData.invenArrayNumber[i], saveData.invenItemName[i], saveData.invenItemNumber[i]);
            }

            // 필드 아이템 정보
            for (int i = 0; i < saveData.items.Count; i++)
            {
                itemManager.LoadItmeActive(i, saveData.items[i]);
            }

            // Setting 정보
            thePauseMenu.LoadToSetting(saveData.musicVolume, saveData.musicMute, saveData.SFXVolume, saveData.SFXMute, saveData.inFieldSensitivity,
                saveData.outFieldSensitivity, saveData.shotModeSensitivity);

            // 오디오
            AudioManager.instance.LoadToAudio(saveData.isNight);

            // 애니메이션 정보
            theAnim.LoadToAnim(saveData.animCondition);

            // 날씨(밤/낮) 정보
            theSun.LoadToSun(saveData.isNight);

            // 케니 자동차
            theKennyCar.LoadToKennyCar(saveData.kennyCarPos, saveData.kennyCarRot);

            // 미션 정보
            talkConditionTrigger.LoadToTalkEvnet(saveData.currentMission, saveData.talkEventNPC, saveData.talkEventOneNPC, saveData.talkEventObject, saveData.talkEventOutMission);
            outFieldMissionManager.LoadToOutManager(saveData.talkEventOutPoint);

            // 좀비(봇)들의 상태 
            for (int i = 0; i < saveData.zombieBotsIndex.Count; i++)
            {
                zombieSpawnManager.LoadToAllZombieState(saveData.zombieBotsIndex[i],saveData.zombieBotsState[i], saveData.zombieBotsDead[i] ,saveData.zombieBotsPos[i], saveData.zombieBotsRot[i]);
            }

            Debug.Log("로드 완료");
        }
        else
            Debug.Log("세이브 데이터가 없습니다.");
    }
}
