using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{

    public Item item; // 획득한 아이템.
    public int itemCount; // 획득한 아이템의 개수.
    public Image itemImage; // 아이템의 이미지.


    // 필요한 컴포넌트.
    [SerializeField]
    private Text text_Count;
    [SerializeField]
    private GameObject go_CountImage;

    // 보여지는 컴포넌트
    public GameObject itemInterect;

    [SerializeField] private Sprite slot_On;
    [SerializeField] private Sprite slot_Out;

    private Inventory theInventory;

    // ItemInterect 변수
    [SerializeField] private Sprite USE_Image;
    [SerializeField] private Sprite DELETE_Image;

    [SerializeField] private GameObject interctButton1;
    [SerializeField] private GameObject interctButton2;
    private const string USE = "사용", Equit = "장착", deEquit = "장착해제", DELETE = "폐기";

    void Start()
    {
        theInventory = FindObjectOfType<Inventory>();
    }

    // 이미지의 투명도 조절.
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // 아이템 획득
    public void AddItem(Item _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;

        if (item.itemType == Item.ItemType.Ammo)
        {
            go_CountImage.SetActive(true);
            text_Count.text = itemCount.ToString();
        }
        else
        {
            text_Count.text = "0";
            go_CountImage.SetActive(false);
        }

        SetColor(1);
    }


    // 아이템 개수 조정.
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    // 아이템 개수 삭제.
    public void CutSlotCount(int _count)
    {
        itemCount -= _count;
        text_Count.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    // 슬롯 초기화.
    public void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        text_Count.text = "0";
        go_CountImage.SetActive(false);
    }

    // 슬롯에 마우스를 올려놓을 시
    public void OnMouse(Image image)
    {
        if(!theInventory.CheckAllButton())
        {
            AudioManager.instance.PlaySE("ButtonClick"); // Button Click 효과음
            image.sprite = slot_Out;
            if (item != null)
            {
                theInventory.ShowItemInfo(item);
            }
        }
    }

    // 슬롯에 마우스를 때어놓을 시
    public void OutMouse(Image image)
    {
        if(!theInventory.CheckAllButton())
        {
            image.sprite = slot_On;
            theInventory.HideItemInfo();
        }
    }

    // 왼쪽 마우스로 눌렀을 시
    public void DownMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (item != null)
                AudioManager.instance.PlaySE(item.itemSoundName); // 효과음
        }
    }

    // 마우스 클릭 이벤트
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            theInventory.RemoveAllButton();
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 아이템 사용
            if (item != null)
            {
                if(!theInventory.CheckAllButton())
                    CheckItemInterct();
            }
        }
    }

    // ItemInterct 확인 함수
    private void CheckItemInterct()
    {
        itemInterect.SetActive(true);
        this.GetComponent<Image>().sprite = slot_Out;
        theInventory.ShowItemInfo(item);
        if (item.itemType == Item.ItemType.Equipment)
        {
            // 장착, 장착해제 체크
            /*
            if(item.itemPrefab.activeSelf) OneButton(USE_Image, deEquit);
            else OneButton(USE_Image, Equit);
            */

            // 장비 해제 및 장비 교환
            //StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(item.weaponType, item.itemName));

            interctButton1.SetActive(false);
            interctButton2.SetActive(false);
        }
        else if (item.itemType == Item.ItemType.Portion)
        {
            if(theInventory.GetPlayerHP() == 100)
            {
                OneButton(DELETE_Image, DELETE);
            }
            // 버튼 생성
            else
            {
                TwoButton(USE_Image, USE, DELETE_Image, DELETE);
            }
        }
        else
        {
            // 기타 AMMO, ETC
            // 버튼 생성
            OneButton(DELETE_Image, DELETE);
        }
    }

    // 1개의 버튼
    private void OneButton(Sprite _image, string _text)
    {
        interctButton1.GetComponentInChildren<Image>().sprite = _image;
        interctButton1.GetComponentInChildren<Text>().text = _text;
        interctButton2.SetActive(false);
    }

    // 2개의 버튼
    private void TwoButton(Sprite _image1, string _text1, Sprite _image2, string _text2)
    {
        interctButton2.SetActive(true);
        interctButton1.GetComponentInChildren<Image>().sprite = _image1;
        interctButton1.GetComponentInChildren<Text>().text = _text1;
        interctButton2.GetComponentInChildren<Image>().sprite = _image2;
        interctButton2.GetComponentInChildren<Text>().text = _text2;
    }

    // Button 내용에 따라 사용이 달라짐
    public void SelectButton(Button _button)
    {
        string purpose = _button.GetComponentInChildren<Text>().text;
        switch (purpose)
        {
            case Equit:
                ItemEquit();
                break;
            case USE:
                ItemUse();
                break;
            case DELETE:
                ItemDelete();
                break;
            default:
                Debug.Log("아이템의 사용 용도가 없습니다.");
                break;
        }
    }

    // 장착 아이템 사용
    private void ItemEquit()
    {
        if (item.itemType == Item.ItemType.Equipment)
        {
            bool isEquit = item.itemPrefab.activeSelf;
            item.itemPrefab.SetActive(!isEquit);
            AudioManager.instance.PlaySE(item.itemSoundName);
            Debug.Log("장착 및 장착해제!");
        }
    }

    // 포션 아이템 사용
    private void ItemUse()
    {
        if (item.itemType == Item.ItemType.Portion)
        {
            AudioManager.instance.PlaySE(item.itemSoundName);
            // 플레이어 체력 회복
            theInventory.HealthPlayer(item.health);
            // 사용 후 아이템 제거
            Debug.Log(item.itemName + " 을 사용했습니다");
            SetSlotCount(-1);
            itemInterect.SetActive(false);
        }
    }

    // 아이템 폐기
    private void ItemDelete()
    {
        theInventory.DeleteItem(item);
        itemInterect.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.dragSlot = this;

            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot.instance.dragSlot = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null)
        {
            if(DragSlot.instance.dragSlot.item != null)
                AudioManager.instance.PlaySE(DragSlot.instance.dragSlot.item.itemSoundName); // 효과음
            ChangeSlot();
        }
    }

    private void ChangeSlot()
    {
        Item _tempItem = item;
        int _tempItemCount = itemCount;

        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);

        if (_tempItem != null)
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount);
        else
            DragSlot.instance.dragSlot.ClearSlot();
    }
}
