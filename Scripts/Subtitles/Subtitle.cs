using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subtitle : MonoBehaviour {

    [SerializeField] private string subtitle_name_KOR; // 대사 이름
    [SerializeField] private string[] subtiles_KOR; // 대사 내용

    private SubtitleManager subtitleManager;

    private void Start()
    {
        subtitleManager = FindObjectOfType<SubtitleManager>();
        subtitleManager.Update_Subtitles(subtitle_name_KOR, subtiles_KOR);
        //Debug.Log(subtitle_name_KOR + subtiles_KOR);
    }
}
