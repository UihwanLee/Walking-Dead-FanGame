using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingScroll : MonoBehaviour {

    [SerializeField] private GameObject endingScrollBase;
    [SerializeField] private GameObject scroll;

	// Use this for initialization
	void Start () {
        endingScrollBase.SetActive(false);
    }
	
	public void StartEndingScroll()
    {
        AudioManager.instance.PlayBGM("Background_Ending");
        AudioManager.instance.currentBGM = "Background_Ending";
        AudioManager.instance.SetSFXMute();
        endingScrollBase.SetActive(true);
        scroll.SetActive(true);
        StartCoroutine(EndGame());
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(67f);
        Application.Quit();
    }
}
