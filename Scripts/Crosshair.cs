using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair: MonoBehaviour {

    [SerializeField]
    private Animator anim;

    [SerializeField]
    private Image[] crosshair;

    // 크로스헤어 상태에 따른 총의 정확도
    private float gunAccuracy;

    // 크로스헤어 비활성화

	public void WalkingAinmation(bool _flag)
    {
        anim.SetBool("Walking", _flag);
    }

    public void FireAnimation()
    {
        if (anim.GetBool("Walking"))
            anim.SetTrigger("Walk_Fire");
        else
            anim.SetTrigger("Idle_Fire");
    }

    public void HitEnemy()
    {
        StartCoroutine(RedCrosshair());
    }

    IEnumerator RedCrosshair()
    {
        crosshair[0].color = new Color(200, 0, 0);
        crosshair[1].color = new Color(200, 0, 0);
        crosshair[2].color = new Color(200, 0, 0);
        crosshair[3].color = new Color(200, 0, 0);
        yield return new WaitForSeconds(0.2f);
        crosshair[0].color = new Color(255, 255, 255);
        crosshair[1].color = new Color(255, 255, 255);
        crosshair[2].color = new Color(255, 255, 255);
        crosshair[3].color = new Color(255, 255, 255);
    }
}
