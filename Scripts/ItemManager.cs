using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour {

    /*
     * 
     *  아이템 수집 관리 스크립트
     * 
     */

    private UiEventTrigger uiEventTrigger;

    [SerializeField]
    private Inventory theInventory;

    [SerializeField]
    private GameObject itemsParent; // 아이템 상위 오브젝트
    public GameObject[] items; // GameStage에 존재하는 모든 아이템들

    [SerializeField] private GameObject[] nonVisibleItems; // 초반에 비활성화 되어 있는 아이템들.

    // 연료 아이템 조건
    private const string Empty = "연료통 (0%) (미션 아이템)", Little = "연료통 (30%) (미션 아이템)",
        Pretty = "연료통 (70%) (미션 아이템)", Full = "연료통 (100%) (미션 아이템)", EmptyInfo = "연료가 비어있는 연료통이다.";

    // Use this for initialization
    void Start () {
        uiEventTrigger = FindObjectOfType<UiEventTrigger>();
        for (int i = 0; i < nonVisibleItems.Length; i++) nonVisibleItems[i].SetActive(true);
        items = GameObject.FindGameObjectsWithTag("Item");
        for (int i = 0; i < nonVisibleItems.Length; i++) nonVisibleItems[i].SetActive(false);
    }
	
	public void GetItem(Item[] items)
    {
        //Item item = obj.GetComponent<ItemPickUp>().item;
        for (int i = 0; i < items.Length; i++)
        {
            CheckFuelItem(items[0]);
            theInventory.AcquireItem(items[i], items[i].count);
            AudioManager.instance.PlaySE(items[i].itemSoundName); //  효과음
        }
        uiEventTrigger.canvasCollectFade(5f, items[0].itemName, items[0].itemInfo);
    }

    public GameObject[] GetItems() { return items; }

    // 아이템 활성화/비활성화 로드
    public void LoadItmeActive(int index, bool isAcitved)
    {
        if (items[index] != null)
            items[index].SetActive(isAcitved);
    }

    private void CheckFuelItem(Item _item)
    {
        if(_item.itemName == Empty || _item.itemName == Little || _item.itemName == Pretty || _item.itemName == Full)
        {
            _item.itemName = Empty;
            _item.itemInfo = EmptyInfo;
        }
    }
}
