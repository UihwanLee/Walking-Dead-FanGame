using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {

    public static bool mapActivated = false;
    public static bool miniMapActivated = false;

    // 맵 컨트롤러 스크립트

    [SerializeField] private GameObject map; // 지도 맵
    [SerializeField] private GameObject playerMark; // 플레이어 마크
    [SerializeField] private GameObject miniMap; // 미니 맵


    // 시스템 스크립트
    private PlayerController playerController;
    private UiEventTrigger uiEventManager;

    // 인벤토리
    [SerializeField] private Inventory theInventory;

    // Use this for initialization
    void Start () {
        playerController = FindObjectOfType<PlayerController>();
        uiEventManager = FindObjectOfType<UiEventTrigger>();

	}
	
	// Update is called once per frame
	void Update () {
        if(CheckMapItem())
        {
            TryOpenMap();
            TryOpenMiniMap();
        }
    }

    public bool CheckMapItem()
    {
        if (theInventory.FindItem("지도 맵")) return true;
        else return false;
    }
    
    private void TryOpenMap()
    {
        if(!uiEventManager.isHelpActivated)
        {
            if(!PauseMenu.pauseMenuActivated)
            {
                if (!(playerController.isTalking == true || playerController.val == true))
                {
                    if (Input.GetKeyDown(KeyCode.M))
                    {
                        mapActivated = !mapActivated;

                        AudioManager.instance.PlaySE("Pick_Map");

                        if (mapActivated)
                        {
                            OpenMap();
                        }
                        else
                        {
                            CloseMap();
                        }
                    }
                }
            }
        }
    }

    private void OpenMap()
    {
        map.SetActive(true);
        playerMark.SetActive(true);
    }

    private void CloseMap()
    {
        map.SetActive(false);
        playerMark.SetActive(false);
    }

    private void TryOpenMiniMap()
    {
        if (!uiEventManager.isHelpActivated)
        {
            if(!Inventory.inventoryActivated)
            {
                if (!(playerController.isTalking == true || playerController.val == true))
                {
                    if (Input.GetKeyDown(KeyCode.U))
                    {
                        miniMapActivated = !miniMapActivated;

                        if (miniMapActivated)
                        {
                            OpenMiniMap();
                        }
                        else
                        {
                            CloseMiniMap();
                        }
                    }
                }
            }
        }
    }

    private void OpenMiniMap()
    {
        miniMap.SetActive(true);
    }

    private void CloseMiniMap()
    {
        miniMap.SetActive(false);
    }

    public void CloseMiniMap2()
    {
        miniMapActivated = false;
        miniMap.SetActive(false);
    }
}
