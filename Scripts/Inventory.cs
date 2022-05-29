using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour, IPointerClickHandler
{

    public static bool inventoryActivated = false;


    // 필요한 컴포넌트
    [SerializeField]
    private GameObject go_InventoryBase;
    [SerializeField]
    private GameObject go_SlotsParent;
    [SerializeField] Text itemName;
    [SerializeField] Text itemInfo;

    // 미션 내용
    [SerializeField] Text missionInfo;

    // PlayerUI
    [SerializeField] GameObject playerUI;

    // UI
    [SerializeField] GameObject UI;

    // 슬롯들.
    private Slot[] slots;

    // 시스템 스크립트
    private PlayerController playerController;
    private UiEventTrigger uiEventManager;
    private MapController mapController;
    private TalkConditionTrigger talkConditionTrigger;

    // Gun Controller
    [SerializeField] private GameObject currentGun;

    public Slot[] GetSlots() { return slots; }

    [SerializeField] private Item[] items;

    public void ClearInven()
    {
        // 아이템 클리어
        for (int i = 0; i < slots.Length; i++)
            if (slots[i].item != null)
                slots[i].ClearSlot();
    }

    public void LoadToInven(int _arrayNum, string _itemName, int _itemNum)
    {
        // 인벤토리 세이브 데이터 적용
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].itemName == _itemName)
                slots[_arrayNum].AddItem(items[i], _itemNum);
        }
    }

    // Use this for initialization
    void Start()
    {
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();
        playerController = FindObjectOfType<PlayerController>();
        uiEventManager = FindObjectOfType<UiEventTrigger>();
        mapController = FindObjectOfType<MapController>();
        talkConditionTrigger = FindObjectOfType<TalkConditionTrigger>();
    }

    // Update is called once per frame
    void Update()
    {
        TryOpenInventory();
    }

    // 인벤토리 열기/닫기 시도
    private void TryOpenInventory()
    {
        if(!uiEventManager.isHelpActivated)
        {
            if(!PauseMenu.pauseMenuActivated)
            {
                if (!(playerController.isTalking == true || playerController.val == true))
                {
                    if(talkConditionTrigger.getBag())
                    {
                        if (Input.GetKeyDown(KeyCode.I))
                        {
                            inventoryActivated = !inventoryActivated;

                            if (inventoryActivated)
                            {
                                OpenInventory();
                            }
                            else
                            {
                                CloseInventory();
                            }
                        }
                    }
                }
            }
        }
    }

    // 인벤토리 열기
    private void OpenInventory()
    {
        playerUI.SetActive(true);
        UI.SetActive(false);
        go_InventoryBase.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //Time.timeScale = 0;        // 시간 정지

        // MiniMap 제거
        if (FindItem("지도 맵"))
            mapController.CloseMiniMap2();
    }

    // 인벤토리 닫기
    private void CloseInventory()
    {
        playerUI.SetActive(false);
        UI.SetActive(true);
        go_InventoryBase.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //Time.timeScale = 1;        // 시간 풀기
    }

    // 아이템 흭득 및 슬롯에 넣기
    public void AcquireItem(Item _item, int _count)
    {
        // 탄약 챙길 시
        if (Item.ItemType.Ammo == _item.itemType)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null)
                {
                    if (slots[i].item.itemName == _item.itemName)
                    {
                        slots[i].SetSlotCount(_count);
                        return;
                    }
                }
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].AddItem(_item, _count);
                return;
            }
        }
    }

    // 아이템 개수 삭제(보통 Ammo Item에 적용)
    // 아이템 '이름'으로 찾는다.
    public void CutItem(string _itemName, int _count)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                if (slots[i].item.itemName == _itemName)
                {
                    slots[i].CutSlotCount(_count);
                    return;
                }
            }
        }
    }

    // 아이템 완전 삭제
    public void DeleteItem(Item _item)
    {
        if (Item.ItemType.Equipment != _item.itemType)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null)
                {
                    if (slots[i].item.itemName == _item.itemName)
                    {
                        slots[i].ClearSlot();
                        return;
                    }
                }
            }
        }
    }

    // 플레이어 정보 On/Off
    public void ShowPlayerInfo()
    {
        itemName.text = "플레이어 정보";
        itemInfo.text = "플레이어 체력: " + playerController.getHP().ToString();
    }

    // 아이템 정보 On/Off
    public void ShowItemInfo(Item _item)
    {
        itemName.text = _item.itemName;
        itemInfo.text = _item.itemInfo;
    }
    public void HideItemInfo()
    {
        itemName.text = "";
        itemInfo.text = "";
    }

    // 미션 갱신
    public void UpdateMissionInfo(string _mission)
    {
        missionInfo.text = _mission;
    }

    // 슬롯 창 및 버튼 관리 함수
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            RemoveAllButton();
        }
    }

    // 버튼 체크
    public bool CheckAllButton()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemInterect.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    // 모든 버튼 관리
    public void RemoveAllButton()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if(slots[i].itemInterect.activeSelf)
            {
                slots[i].itemInterect.SetActive(false);
            }
        }
    }

    // 플레이어 HP Get
    public int GetPlayerHP()
    {
        return playerController.getHP();
    }

    // 플레이어 힐
    public void HealthPlayer(int healAmount)
    {
        playerController.Health(healAmount);
    }

    // Ammo 개수 찾기
    public int FindAmmoAmount(string findItem)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                if (slots[i].item.itemName == findItem)
                {
                    return slots[i].itemCount;
                }
            }
        }
        return 0;
    }

    // 아이템 이름으로 찾기
    public bool FindItem(string _itemName)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                if (slots[i].item.itemName == _itemName)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // 아이템 이름으로 아이템 정보 변환
    public void ChangeItemInfo(string _itemName, string _changeName, string _chagneInfo)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                if (slots[i].item.itemName == _itemName)
                {
                    slots[i].item.itemName = _changeName;
                    slots[i].item.itemInfo = _chagneInfo;
                }
            }
        }
    }

    // 인벤토리 아이템 null 체크
    public bool CheckInventoryNull()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return true;
            }
        }
        return false;
    }
}
