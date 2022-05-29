using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickUp : MonoBehaviour {

    private GameObject player;
    public GameObject obj;
    public Item[] items;

    [SerializeField] private GameObject talk_icon;
    [SerializeField] private GameObject canvas;
    [SerializeField] private SpriteRenderer spriteRender;
    [SerializeField] private Sprite talk_icon1;
    [SerializeField] private Sprite talk_icon2;

    [SerializeField] private GameObject textBox;

    private ItemManager itemManager;
    private Inventory theInventory;
    private SubtitleManager subtitleManager;

    private string message = "가방 속에 공간이 없어...";

    private void Start()
    {
        player = GameObject.Find("Clementine");
        itemManager = FindObjectOfType<ItemManager>();
        theInventory = FindObjectOfType<Inventory>();
        subtitleManager = FindObjectOfType<SubtitleManager>();
    }

    // Update is called once per frame
    void Update()
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
                    // 인벤토리 여부 공간 체크
                    if (theInventory.CheckInventoryNull())
                    {
                        // System에 Item 변수 넘겨주기
                        itemManager.GetItem(items);
                        talk_icon.SetActive(false);
                        obj.SetActive(false);
                    }
                    else
                    {
                        if(items[0].itemType == Item.ItemType.Ammo)
                        {
                            // System에 Item 변수 넘겨주기
                            itemManager.GetItem(items);
                            talk_icon.SetActive(false);
                            obj.SetActive(false);
                        }
                        else
                            StartCoroutine(Effect());
                    }
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

    // 자막 타이핑 이펙트
    IEnumerator Effect()
    {
        subtitleManager.SetColor("CLEM");
        StartCoroutine(_typing(message));
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
}
