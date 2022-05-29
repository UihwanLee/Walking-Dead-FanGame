using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkConditionTrigger : MonoBehaviour {

    /*
     *  [대화 시스템 조건 판별 스크립트]
     *  
     *  - 주어진 미션에 따라 NPC 및 오브젝트 대화/상호작용 조건이 해제된다.
     *  
     *  <Mission>
     *  [1] 허쉘 농장 문 확인하기
     *  Clear -> NPC([리], [좀비])
     *  [2] 리와 대화하기
     *  Clear -> NPC([케니], [Katjaa&Duck], [허쉘]), Object([더플백], [작업용 책상]) 
     *  [3] 농장 문으로 모이기
     * 
     *  @ NPC는 대화 후 짧은 대화 컷씬으로 넘어가며 오브젝트 상호작용은 사라진다. 
     * 
     */

    // 미션
    /*
     * 1. 농장 문 확인하기
     * 2. 리와 대화하기
     * 3. 가방 찾기
     * 4. 허쉘 만나기
     * 5. 마구간 워커를 처리할 총 찾기
     * 6. 마구간 입구로 가기
     * 7. 워커를 처리하고 식량 챙기기
     * 8. 농장 문으로 가기
     * 9. 케니 자동차 확인하기.
     * 10. 케니 자동차를 고칠 부품 찾기.
     * 11. 케니 자동차 타고 탈출하기.
    */

    // 미션
    public string[] missions;

    // 현재 미션
    public static string currentMission = null;

    // NPC Trigger 변수
    [SerializeField] private GameObject farmDoorEvent;
    [SerializeField] private GameObject leeEvent;
    [SerializeField] private GameObject kennyEvent;
    [SerializeField] private GameObject hurshelEvent;
    [SerializeField] private GameObject katjaaEvent;

    // Object Trigger 변수
    [SerializeField] private GameObject deskEvent;
    [SerializeField] private GameObject duffleBagEvent;
    [SerializeField] private GameObject stallDoorEvent;
    [SerializeField] private GameObject stallInsideEvent;
    [SerializeField] private GameObject foodBoxEvent;
    [SerializeField] private GameObject kennyCarBoxEvent;
    [SerializeField] private GameObject kennyCarEvent;

    // NPC icon 변수
    [SerializeField] private GameObject farmDoorEventIcon;
    [SerializeField] private GameObject leeEventIcon;
    [SerializeField] private GameObject kennyEventIcon;
    [SerializeField] private GameObject hurshelEventIcon;
    [SerializeField] private GameObject katjaaEventIcon;

    // Object icon 변수
    [SerializeField] private GameObject deskEventIcon;
    [SerializeField] private GameObject duffleBagEventIcon;
    [SerializeField] private GameObject stallDoorEventIcon;
    [SerializeField] private GameObject stallInsideEventIcon;
    [SerializeField] private GameObject foodBoxEventIcon;
    [SerializeField] private GameObject kennyCarBoxEventIcon;
    [SerializeField] private GameObject kennyCarEventIcon;

    // 모든 NPC
    [SerializeField] private GameObject AllNPC;

    // Bag Object
    [SerializeField] private GameObject theBag;

    // 상태 변수
    // NPC 대화 혹은 오브젝트 소유 여부에 따른 참값 판단 변수
    private bool isTalkFarmDoor;
    private bool isTalkLee; // 가방, 권총, 식량 찾기 미션
    private bool isTalkDuck; // 캇챠, 덕 이벤트 완료
    private bool isTalkHershel; // 식량 찾기 미션
    private bool isTalkStall; // 좀비 죽이기 미션 열림
    public bool isKillZombie; 
    private bool isTalkFarmDoor2;

    // 주요 NPC(리, 케니, 허쉘)과의 대화 변수
    private bool isTalkOneLee;
    private bool isTalkOneKenny;
    private bool isTalkOneDuck;
    private bool isTalkOneHershel;

    // 오브젝트(권총, 가방, 식량)
    private bool hasGun;
    private bool hasBag;
    private bool hasFood;
    private bool hasCheeseCrack;

    // Out Field 미션 아이템
    private bool isMission1;
    private bool isMission2;
    private bool isMission3;
    private bool isMission4;

    // 시스템 스크립트
    private UiEventTrigger uiEventManager;
    private Inventory theInven;

    public void LoadToTalkEvnet(string savedMission, List<bool> savedNPCTalk, List<bool> savedOneNPCTalk , List<bool> savedObjectTalk, List<bool> savedOutMission)
    {
        // NPC 및 오브젝트 상호작용
        isTalkFarmDoor = savedNPCTalk[0];
        isTalkLee = savedNPCTalk[1];
        isTalkDuck = savedNPCTalk[2];
        isTalkHershel = savedNPCTalk[3];
        isTalkStall = savedNPCTalk[4];
        isKillZombie = savedNPCTalk[5];
        isTalkFarmDoor2 = savedNPCTalk[6];

        // 주요 NPC(리, 케니, 허쉘)과의 대화 변수
        isTalkOneLee = savedOneNPCTalk[0];
        isTalkOneKenny = savedOneNPCTalk[1];
        isTalkOneDuck = savedOneNPCTalk[2];
        isTalkOneHershel = savedOneNPCTalk[3];

        // 오브젝트 상태변수
        hasGun = savedObjectTalk[0];
        hasBag = savedObjectTalk[1];
        hasFood = savedObjectTalk[2];
        hasCheeseCrack = savedObjectTalk[3];

        // OutField 미션 상태변수
        isMission1 = savedOutMission[0];
        isMission2 = savedOutMission[1];
        isMission3 = savedOutMission[2];
        isMission4 = savedOutMission[3];

        currentMission = savedMission;
        theInven.UpdateMissionInfo(savedMission);
        TalkeEventByMission(currentMission);

        if (hasBag) theBag.SetActive(false);

        uiEventManager.canvasMissionFade(4f, currentMission);
    }

    // Use this for initialization
    void Start () {

        // 스크립트 초기화
        farmDoorEvent.SetActive(true);
        leeEvent.SetActive(false);
        kennyEvent.SetActive(false);
        hurshelEvent.SetActive(false);
        katjaaEvent.SetActive(false);

        deskEvent.SetActive(false);
        duffleBagEvent.SetActive(false);
        stallDoorEvent.SetActive(false);
        stallInsideEvent.SetActive(false);
        foodBoxEvent.SetActive(false);

        farmDoorEventIcon.SetActive(true);
        leeEventIcon.SetActive(false);
        kennyEventIcon.SetActive(false);
        hurshelEventIcon.SetActive(false);
        katjaaEventIcon.SetActive(false);

        deskEventIcon.SetActive(false);
        duffleBagEventIcon.SetActive(false);
        stallDoorEventIcon.SetActive(false);
        stallInsideEventIcon.SetActive(false);
        foodBoxEventIcon.SetActive(false);

        // 참값 변수 초기화
        isTalkFarmDoor = false;
        isTalkLee = false;
        isTalkDuck = false;
        isTalkHershel = false;
        isTalkStall = false;

        isKillZombie = false;

        isTalkOneLee = false;
        isTalkOneKenny = false;
        isTalkOneDuck = false;
        isTalkOneHershel = false;

        hasBag = false;
        hasGun = false;
        hasFood = false;
        hasCheeseCrack = false;

        isMission1 = false;
        isMission2 = false;
        isMission3 = false;
        isMission4 = false;

        // 시스템 스크립트
        uiEventManager = FindObjectOfType<UiEventTrigger>();
        theInven = FindObjectOfType<Inventory>();

        // 초기화
        ClearTalk();

        // Inventory에서 자동저장한 미션 내용을 가져와 미션 초기화 시키기
        SetFirstMission();

        //Test();
    }

    public void MissionComplete(int _mission)
    {
        if (_mission == 0) isTalkFarmDoor = true;
        else if (_mission == 1) isTalkLee = true;
        else if (_mission == 2) isTalkHershel = true;
        else if (_mission == 3) isTalkStall = true;
        else if (_mission == 4) isKillZombie = true;
        else if (_mission == 5) isTalkFarmDoor2 = true;
        else if (_mission == 6) isTalkDuck = true;
        else Debug.Log("ERROR_TALKCONDITION_MISSONCOMPLETE");
    }

    // 초기 미션
    private void SetFirstMission()
    {
        uiEventManager.canvasMissionFade(4f, missions[0]);

        farmDoorEvent.SetActive(true);
        farmDoorEventIcon.SetActive(true);

        StartCoroutine(showHelpInfo());
    }

    IEnumerator showHelpInfo()
    {
        yield return new WaitForSeconds(6f);
        uiEventManager.canvasExplainFade(3f, "P키를 눌러 옵션창을 열기/끄기 할 수 있습니다.");
    }

    // 미션 조정
    public void TalkeEventByMission(string _mission)
    {
        if (_mission == missions[0])
        {
            ClearTalk();

            farmDoorEvent.SetActive(true);
            farmDoorEventIcon.SetActive(true);
        }
        if (_mission == missions[1])
        {
            ClearTalk();

            farmDoorEvent.SetActive(false);
            farmDoorEventIcon.SetActive(false);

            leeEvent.SetActive(true);
            leeEventIcon.SetActive(true);
        }
        if (_mission == missions[2])
        {
            ClearTalk();

            farmDoorEvent.SetActive(false);
            farmDoorEventIcon.SetActive(false);

            leeEvent.SetActive(true);
            leeEventIcon.SetActive(true);

            deskEvent.SetActive(true);
            deskEventIcon.SetActive(true);
        }
        if (_mission == missions[3])
        {
            farmDoorEvent.SetActive(false);
            farmDoorEventIcon.SetActive(false);

            leeEvent.SetActive(true);
            leeEventIcon.SetActive(true);

            kennyEvent.SetActive(true);
            kennyEventIcon.SetActive(true);

            hurshelEvent.SetActive(true);
            hurshelEventIcon.SetActive(true);

            katjaaEvent.SetActive(true);
            katjaaEventIcon.SetActive(true);

            duffleBagEvent.SetActive(true);
            duffleBagEventIcon.SetActive(true);

            stallDoorEvent.SetActive(true);
            stallDoorEventIcon.SetActive(true);

            foodBoxEvent.SetActive(true);
            foodBoxEventIcon.SetActive(true);
        }
        if (_mission == missions[7])
        {
            farmDoorEvent.SetActive(true);
            farmDoorEventIcon.SetActive(true);

            leeEvent.SetActive(true);
            leeEventIcon.SetActive(true);

            kennyEvent.SetActive(true);
            kennyEventIcon.SetActive(true);

            hurshelEvent.SetActive(true);
            hurshelEventIcon.SetActive(true);

            katjaaEvent.SetActive(true);
            katjaaEventIcon.SetActive(true);

            stallDoorEvent.SetActive(false);
            stallDoorEventIcon.SetActive(false);
        }
        if (_mission == missions[8])
        {
            ClearTalk();
            AllNPC.SetActive(false);

            kennyCarBoxEvent.SetActive(true);
            kennyCarBoxEventIcon.SetActive(true);
        }
        if (_mission == missions[9])
        {
            ClearTalk();
            AllNPC.SetActive(false);

            kennyCarEvent.SetActive(true);
            kennyEventIcon.SetActive(true);
        }
        if (_mission == missions[10])
        {
            ClearTalk();
            AllNPC.SetActive(false);

            kennyCarEvent.SetActive(true);
            kennyEventIcon.SetActive(true);
        }
    }

    private void TalkEventByBoolVal()
    {
        if(isTalkDuck)
        {
            katjaaEvent.SetActive(false);
            katjaaEventIcon.SetActive(false);
        }
        if(hasBag)
        {
            deskEvent.SetActive(false);
            deskEventIcon.SetActive(false);
        }
        if (hasGun)
        {
            duffleBagEvent.SetActive(false);
            duffleBagEventIcon.SetActive(false);
        }
    }

    // Talk Event 조정
    public void SetTalkTrigger(string _condition)
    {
        if (_condition == "Duck")
        {
            katjaaEvent.SetActive(false);
            katjaaEventIcon.SetActive(false);
        }
        if (_condition == "Desk")
        {
            deskEvent.SetActive(false);
            deskEventIcon.SetActive(false);
        }
        if (_condition == "DuffelBag")
        {
            duffleBagEvent.SetActive(false);
            duffleBagEventIcon.SetActive(false);
        }
        if (_condition == "FoodBox")
        {
            foodBoxEvent.SetActive(false);
            foodBoxEventIcon.SetActive(false);
        }
        if (_condition == "StallDoor")
        {
            stallDoorEvent.SetActive(false);
            stallDoorEventIcon.SetActive(false);
        }
        if (_condition == "Stall")
        {
            stallInsideEvent.SetActive(true);
            stallInsideEventIcon.SetActive(true);
        }
        if (_condition == "StallDelete")
        {
            stallInsideEvent.SetActive(false);
            stallInsideEventIcon.SetActive(false);
        }
    }

    // 주요 NPC(리, 케니, 허쉘)과의 대화
    public void TalkOneNPC(int _condition)
    {
        if (_condition == 0) isTalkOneLee = true;
        else if (_condition == 1) isTalkOneKenny = true;
        else if (_condition == 2) isTalkOneDuck = true;
        else if (_condition == 3) isTalkOneHershel = true;
    }

    // Talk Event 올 클리어
    public void ClearTalk()
    {
        farmDoorEvent.SetActive(false);
        farmDoorEventIcon.SetActive(false);

        leeEvent.SetActive(false);
        leeEventIcon.SetActive(false);

        kennyEvent.SetActive(false);
        kennyEventIcon.SetActive(false);

        hurshelEvent.SetActive(false);
        hurshelEventIcon.SetActive(false);

        katjaaEvent.SetActive(false);
        katjaaEventIcon.SetActive(false);

        deskEvent.SetActive(false);
        deskEventIcon.SetActive(false);

        duffleBagEvent.SetActive(false);
        duffleBagEventIcon.SetActive(false);

        stallDoorEvent.SetActive(false);
        stallDoorEventIcon.SetActive(false);

        stallInsideEvent.SetActive(false);
        stallInsideEventIcon.SetActive(false);

        foodBoxEvent.SetActive(false);
        foodBoxEventIcon.SetActive(false);

        kennyCarBoxEvent.SetActive(false);
        kennyCarBoxEventIcon.SetActive(false);

        kennyCarEvent.SetActive(false);
        kennyCarEventIcon.SetActive(false);
    }

    // Test
    public void Test()
    {
        farmDoorEvent.SetActive(true);
        farmDoorEventIcon.SetActive(true);

        leeEvent.SetActive(true);
        leeEventIcon.SetActive(true);

        kennyEvent.SetActive(true);
        kennyEventIcon.SetActive(true);

        hurshelEvent.SetActive(true);
        hurshelEventIcon.SetActive(true);

        katjaaEvent.SetActive(true);
        katjaaEventIcon.SetActive(true);

        deskEvent.SetActive(true);
        deskEventIcon.SetActive(true);

        duffleBagEvent.SetActive(true);
        duffleBagEventIcon.SetActive(true);

        stallDoorEvent.SetActive(true);
        stallDoorEventIcon.SetActive(true);

        foodBoxEvent.SetActive(true);
        foodBoxEventIcon.SetActive(true);
    }

    public void isTalk(string NPC)
    {
        /* 첫번째 미션 농장 문 상호작용 시
         * 
         * (NPC)
         * 리 대화 On
         * 
         * (Object)
         * 좀비 상호작용 On
         * 
        */
        if (NPC == "FarmDoor")
        {
            isTalkFarmDoor = true;

            leeEventIcon.SetActive(true);
        }

        /* 두번째 리 대화 시
         * 
         * (NPC)
         * 케니 대화 On
         * 허쉘 대화 On
         * 덕가족 대화 On
         * 
         * (Object)
         * 작업용 책상 상호작용 On
         * 더플 백 상호작용 On
         * 
        */
        if (NPC == "Lee")
        {
            isTalkLee = true;

            kennyEventIcon.SetActive(true);
            deskEventIcon.SetActive(true);

            duffleBagEventIcon.SetActive(true);
        }

        if (NPC == "Hershel")
        {
            isTalkHershel = true;
        }
    }

    public bool getFarmDoor1Talk() { return isTalkFarmDoor; }
    public bool getLeeTalk() { return isTalkLee; }
    public bool getDuckTalk() { return isTalkDuck; }
    public bool getHershelTalk() { return isTalkHershel; }
    public bool getStallTalk() { return isTalkStall; }
    public bool getFarmDoor2Talk() { return isTalkFarmDoor2; }
    public bool getKillZombie() { return isKillZombie; }

    public bool getTalkOneLee() { return isTalkOneLee; }
    public bool getTalkOneKenny() { return isTalkOneKenny; }
    public bool getTalkOneDuck() { return isTalkOneDuck; }
    public bool getTalkOneHershel() { return isTalkOneHershel; }

    public void hasObj(string Obj)
    {
        if (Obj == "Bag") hasBag = true;
        else if (Obj == "Gun") hasGun = true;
        else if (Obj == "Food") hasFood = true;
        else if (Obj == "CheeseCrack") hasCheeseCrack = true;
        else Debug.Log("ERROR_TALKCONDTIONTRIGGER_HASOBJ");
    }

    public void hasOutMission(int num)
    {
        if (num == 1) isMission1 = true;
        else if (num == 2) isMission2 = true;
        else if (num == 3) isMission3 = true;
        else if (num == 4) isMission4 = true;
        else Debug.Log("ERROR_TALKCONDTIONTRIGGER_HASOBJ");
    }

    public bool getBag() { return hasBag; }
    public bool getGun() { return hasGun; }
    public bool getFood() { return hasFood; }
    public bool getCheeseCrack() { return hasCheeseCrack; }

    public bool getMission1() { return isMission1; }
    public bool getMission2() { return isMission2; }
    public bool getMission3() { return isMission3; }
    public bool getMission4() { return isMission4; }

}
