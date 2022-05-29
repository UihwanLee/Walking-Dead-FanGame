using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Help", menuName = "New Help/help")]
public class Help : ScriptableObject
{

    public string title; // 제목
    public Sprite image1; // 도움 사진1
    public Sprite image2; // 도움 사진2
    public string message; // 설명

}
