using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClementineUIController : MonoBehaviour {

    // 플레이어 컨트롤러
    private PlayerController playerController;

    // HP
    public int playerHP;

    // 애니메이터
    private Animator anim;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerHP = playerController.getHP();
        anim = this.GetComponent<Animator>();

        CheckHP(playerHP);
    }

    private void Update()
    {
        playerHP = playerController.getHP();
        CheckHP(playerHP);
    }

    private void CheckHP(int hp)
    {
        if (hp >= 90)
        {
            anim.SetBool("Damage1", false);
            anim.SetBool("Damage2", false);
        }
        else if (hp >= 70)
        {
            anim.SetBool("Damage1", true);
            anim.SetBool("Damage2", false);
        }
        else 
        {
            anim.SetBool("Damage1", true);
            anim.SetBool("Damage2", true);
        }
    }
}
