using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour {

    public static bool flashLightActivated = false;

    [SerializeField] private GameObject flashLight;
    [SerializeField] private Inventory theInventory;

    // 시스템 스크립트
    private PlayerController playerController;
    private UiEventTrigger uiEventManager;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        uiEventManager = FindObjectOfType<UiEventTrigger>();
    }

    // Update is called once per frame
    void Update () {
		if(theInventory.FindItem("플래시 라이트"))
        {
            if(CheckCondition())
            {
                TryFlashLight();
            }
        }
	}

    private bool CheckCondition()
    {
        if(!uiEventManager.isHelpActivated)
        {
            if(!Inventory.inventoryActivated)
            {
                if (!(playerController.isTalking == true || playerController.val == true))
                {
                    return true;
                }
            }
            return false;
        }
        return false;
    }

    private void TryFlashLight()
    {
        if (Input.GetKey(KeyCode.F))
        {
            flashLightActivated = !flashLightActivated;

            if (flashLightActivated)
            {
                OnFlashLight();
            }
            else
            {
                OutFlashLight();
            }
        }
    }

    private void OnFlashLight()
    {
        flashLight.SetActive(true);
    }

    private void OutFlashLight()
    {
        flashLight.SetActive(false);
    }
}
