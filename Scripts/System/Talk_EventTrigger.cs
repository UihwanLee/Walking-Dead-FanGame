using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talk_EventTrigger : MonoBehaviour {

    public bool isTalking = false;

    [SerializeField] private GameObject obj;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject talk_icon;
    [SerializeField] private GameObject canvas;
    [SerializeField] private SpriteRenderer spriteRender;
    [SerializeField] private Sprite talk_icon1;
    [SerializeField] private Sprite talk_icon2;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        CheckIcon();
    }

    private void CheckIcon()
    {
        if (!isTalking)
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
        else
        {
            talk_icon.SetActive(false);
            canvas.SetActive(false);
        }
    }
}
