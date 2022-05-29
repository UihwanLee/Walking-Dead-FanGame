using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class Item : ScriptableObject
{

    public string itemName; // 아이템의 이름.
    public ItemType itemType; // 아이템의 유형.
    public Sprite itemImage; // 아이템의 이미지.
    public GameObject itemPrefab; // 아이템의 프리팹.

    public int count; // 아이템 개수
    public string itemInfo; // 아이템 정보
    public string itemSoundName; // 아이템 효과음 제목
    public int health; // 회복템일 경우 회복량

    public enum ItemType
    {
        Equipment,
        Ammo,
        Portion,
        ETC
    }

}
