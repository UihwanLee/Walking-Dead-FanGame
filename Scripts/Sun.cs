using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour {

    // SkyBox Material
    private Renderer currentRend;
    [SerializeField] private Material Sunny;
    [SerializeField] private Material Night;

    [SerializeField] private GameObject sun;

    public bool isNight;

    [SerializeField] private float fogDensityCalc; // 증감량 비율
    [SerializeField] private float nightFogDensity; // 밤 상태의 Fog 밀도
    private float dayFogDensity; // 낮 상태의 Fog 밀도
    private float currentFogDensity; // 계산

    // 날씨 정보 로드
    public void LoadToSun(bool _isNight)
    {
        isNight = _isNight;
        if(!isNight)
        {
            RenderSettings.skybox = Sunny;
            RenderSettings.skybox.SetColor("_Tint", new Color32(128, 128, 128, 128));
            RenderSettings.sun.color = new Color32(255, 255, 255, 128);
            RenderSettings.fogDensity = 0f;
        }
        else
        {
            RenderSettings.skybox = Night;
            RenderSettings.skybox.SetColor("_Tint", new Color32(50, 50, 50, 128));
            RenderSettings.sun.color = new Color32(20, 20, 20, 128);
            RenderSettings.fogDensity = 0.1f;
        }
    }

    // Use this for initialization
    void Start () {
        // SkyBox 색상 조정
        // 초기 - 낮 상태 유지
        isNight = false;
        Color currentColor = new Color32(128, 128, 128, 128);
        RenderSettings.skybox.SetColor("_Tint", currentColor);
    }

    // SkyBox Material Setting
    public void SetRenderSettingSkyBox(float time)
    {
        StartCoroutine(ChangingSkyBox(time));
    }

    // SkyBox Material Fade
    // Sunny -> Night Color로 다룬다.
    // Light(Sun) Color (255, 255, 255, 128) -> (120, 120, 120, 128) -> (20, 20, 20 ,128)
    IEnumerator ChangingSkyBox(float time)
    {
        StartCoroutine(ChangingLight(time / 2));
        StartCoroutine(ChangingSkyBox_Sunny(time / 2));
        yield return new WaitForSeconds(time / 2);
        StartCoroutine(ChangingFogDensity(time / 2));
        StartCoroutine(ChangingSkyBox_Night(time / 2));
    }

    IEnumerator ChangingSkyBox_Sunny(float cycleTime)
    {
        float currentTime = 0;

        Color startColor = new Color32(128,128,128, 128);
        Color endColor = new Color32(20, 18, 18, 128);

        while (currentTime < cycleTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / cycleTime;
            Color currentColor = Color.Lerp(startColor, endColor, t);
            RenderSettings.skybox.SetColor("_Tint", currentColor);
            yield return null;
        }
    }

    IEnumerator ChangingSkyBox_Night(float cycleTime)
    {
        // 밤으로 변환
        RenderSettings.skybox = Night;

        float currentTime = 0;

        Color startColor = new Color32(20, 18, 18, 128);
        Color endColor = new Color32(50, 50, 50, 128);

        while (currentTime < cycleTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / cycleTime;
            Color currentColor = Color.Lerp(startColor, endColor, t);
            //RenderSettings.ambientSkyColor = currentColor;
            RenderSettings.skybox.SetColor("_Tint", currentColor);
            yield return null;
        }
    }

    // Light 조정
    IEnumerator ChangingLight(float cycleTime)
    {
        RenderSettings.sun = sun.GetComponent<Light>();

        float currentTime = 0;

        Color startColor = new Color32(255, 255, 255, 128);
        Color endColor = new Color32(20, 20, 20, 128);

        while (currentTime < cycleTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / cycleTime;
            Color currentColor = Color.Lerp(RenderSettings.sun.color, endColor, t);
            RenderSettings.sun.color = currentColor;
            yield return null;
        }
    }

    // FogDensity 조정
    IEnumerator ChangingFogDensity(float cycleTime)
    {

        float currentTime = 0;

        while (currentTime < cycleTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / cycleTime;
            currentFogDensity = Mathf.Lerp(dayFogDensity, nightFogDensity, t);
            RenderSettings.fogDensity = currentFogDensity;
            yield return null;
        }
    }
}
