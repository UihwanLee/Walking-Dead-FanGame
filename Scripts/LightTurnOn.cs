using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTurnOn : MonoBehaviour {

    [SerializeField] private GameObject player;

    [SerializeField] private GameObject[] lights;
    [SerializeField] private GameObject lightSwitch;

    [SerializeField] private GameObject talk_icon;
    [SerializeField] private GameObject canvas;
    [SerializeField] private SpriteRenderer spriteRender;
    [SerializeField] private Sprite talk_icon1;
    [SerializeField] private Sprite talk_icon2;

    // Use this for initialization
    void Start () {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (Vector3.Distance(player.transform.position, lightSwitch.transform.position) < 7.5)
        {
            talk_icon.SetActive(true);
            if (talk_icon.activeSelf == true)
                spriteRender = talk_icon.GetComponent<SpriteRenderer>();
            if (Vector3.Distance(player.transform.position, lightSwitch.transform.position) < 4)
            {
                canvas.SetActive(true);
                spriteRender.sprite = talk_icon2;
                if (Input.GetKeyDown(KeyCode.F))
                {
                    // Switch_TurnOn
                    AudioManager.instance.PlaySE("Switch_TurnOn");
                    for (int i = 0; i < lights.Length; i++)
                    {
                        lights[i].SetActive(true);
                    }
                    lightSwitch.SetActive(false);
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
}
