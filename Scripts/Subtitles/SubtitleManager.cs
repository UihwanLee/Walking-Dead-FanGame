using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleManager : MonoBehaviour {

    /*
     * 
     * 관리 자료형: Dictonary
     * 
     * 인게임 대사 관리 스크립트
     * (한국어, 영어)
     * 
     * [UI]
     * UI Text
     * 
     * [NPC]
     * (리, 케니, 캇챠&덕, 허쉘)
     * 
     * [Object]
     * (농장 문, 작업용 책상, 더플 백, 마구간 입구)
     * 
    */

    // 딕셔너리 자료형 선언
    private Dictionary<string, string[]> subtitles;

    private string language; // 언어 설정 변수

    // 대사 Text
    [SerializeField] private Text textBox;

    // 색깔 지정
    private Color CLEM = new Color32(255, 255, 255, 255);
    private Color LEE = new Color32(112, 254, 255, 255);
    private Color KENNY = new Color32(54, 214, 7, 255);
    private Color HERSHEL = new Color32(198, 7, 153, 255);
    private Color KATJAA = new Color32(198, 108, 29, 255);
    private Color DUCK = new Color32(255, 229, 112, 255);

    public void Start()
    {
        subtitles = new Dictionary<string, string[]>();
        language = "_KOR"; // 한국어로 초기화
    }

    // Subtitles Update
    public void Update_Subtitles(string _name, string[] _subtitle)
    {
        subtitles.Add(_name, _subtitle);
    }

    public string[] getSubtitles(string _name)
    {
        string key = _name + language;
        if (subtitles.ContainsKey(key))
            return subtitles[key];
        else
        {
            //Debug.Log(_name + "자막을 찾을 수 없습니다.");
            return null;
        }
    }

    public void SetColor(string COL)
    {
        switch (COL)
        {
            case "CLEM":
                textBox.color = CLEM;
                break;
            case "LEE":
                textBox.color = LEE;
                break;
            case "KENNY":
                textBox.color = KENNY;
                break;
            case "HERSHEL":
                textBox.color = HERSHEL;
                break;
            case "KATJAA":
                textBox.color = KATJAA;
                break;
            case "DUCK":
                textBox.color = DUCK;
                break;
            default:
                Debug.Log("할당된 색이 없습니다.");
                break;
        }
    }
}
