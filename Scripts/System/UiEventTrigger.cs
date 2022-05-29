using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiEventTrigger : MonoBehaviour {

    // Canvas Group
    [SerializeField]
    private GameObject canvasShort;
    [SerializeField]
    private GameObject canvasLong;
    [SerializeField]
    private GameObject canvasCollect;

    [SerializeField]
    private GameObject canvasFadeInOut;

    [SerializeField]
    private Sprite button_image1;
    [SerializeField]
    private Sprite button_image2;

    // 타이머
    [SerializeField]
    private GameObject slider;
    [SerializeField]
    private GameObject timeSlider;

    private Vector3 originalScale;
    private Color startColor;

    // 총 조준점
    [SerializeField]
    private GameObject canvasGun;

    // 총 탄창 UI
    [SerializeField]
    private GameObject canvasHUD;

    // 시스템 미션 UI
    [SerializeField]
    private GameObject canvasMission;

    // 시스템 설명 UI
    [SerializeField]
    private GameObject canvasExplain;

    // 피격 UI
    [SerializeField]
    private GameObject canvasDamage;
    [SerializeField]
    private GameObject inventoryDamage;
    [SerializeField]
    private GameObject inventoryHealth;

    // Slot UI
    [SerializeField] private Sprite slot_On;
    [SerializeField] private Sprite slot_Out;

    // Help UI
    [SerializeField] private GameObject canvasHelp;
    [SerializeField] private Text help_Title; // 타이틀
    [SerializeField] private Image help_Image1; // 도움 사진1
    [SerializeField] private Image help_Image2; // 도움 사진2
    [SerializeField] private Text help_Message; // 설명
    public bool isHelpActivated = false;

    // NotePad UI
    [SerializeField] private GameObject notePadUI;

    // Loading UI
    [SerializeField] private GameObject canvasLoading;
    [SerializeField] private Text loadingMessage;

    [SerializeField] private GameObject canvasLoadingBase;
    [SerializeField] private Help[] help_Message_Loading;
    [SerializeField] private Text loadingBaseMessage;

    // Dead UI
    [SerializeField] private GameObject canvasDead;

    // 인벤토리
    private Inventory theInventory;

    // 세이브 스크립트
    [SerializeField] private SaveNLoad theSaveNLoad;

    private PlayerController playerController;

    public void Start()
    {
        // 캔버스 초기화
        retAlpha(canvasShort);
        retAlpha(canvasLong);
        retAlpha(canvasCollect);
        retAlpha(canvasFadeInOut);
        retAlpha(canvasMission);
        retAlpha(canvasExplain);
        retAlpha(inventoryHealth);
        retAlpha(notePadUI);
        retAlpha(canvasLoading);
        retAlpha(canvasDead);
        retAlpha(canvasLoadingBase);

        // 타이머 초기화
        slider.SetActive(false);

        // 총 조준점 초기화
        canvasGun.SetActive(false);

        // 피격 UI 초기화

        canvasDamage.SetActive(false);
        inventoryDamage.SetActive(false);

        // Help UI 초기화
        canvasHelp.SetActive(false);

        // 인벤토리
        theInventory = FindObjectOfType<Inventory>();

        playerController = FindObjectOfType<PlayerController>();
    }

    // 캔버스 알파값 초기화
    private void retAlpha(GameObject canvas)
    {
        canvas.SetActive(true);
        canvas.GetComponent<CanvasGroup>().alpha = 0f;
        canvas.SetActive(false);
    }

    /*
     * UI 버튼 함수 
    */

    // 대사 선택 버튼 마우스 올려놓을 시
    public void OnMouse(Button button)
    {
        button.image.enabled = true;
    }

    // 대사 선택 버튼 마우스 때어놓을 시
    public void OutMouse(Button button)
    {
        button.image.enabled = false;
    }

    // 도구 선택 버튼 마우스를 올려놓을 시
    public void ButtonOnMouse(Button button)
    {
        button.image.sprite = button_image2;
        button.GetComponentInChildren<Text>().color = new Color32(224, 92, 88, 207);
    }

    // 도구 선택 버튼 마우스를 때어놓을 시
    public void ButtonOutMouse(Button button)
    {
        button.image.sprite = button_image1;
        button.GetComponentInChildren<Text>().color = new Color32(200, 185, 177, 207);
    }

    // 아이템 슬롯 사용 버튼, Help 닫기 버튼에 마우스를 올려놓을 시
    public void ItemSlotInterectOnMouse(Image image)
    {
        AudioManager.instance.PlaySE("ButtonClick"); // Button Click 효과음
        image.sprite = slot_Out;
    }

    // 아이템 슬롯 사용 버튼, Help 닫기 버튼에 마우스를 때어놓을 시
    public void ItemSlotInterectOutMouse(Image image)
    {
        image.sprite = slot_On;
    }

    // 플레이어 슬롯에 마우스를 올려놓을 시
    public void PlayerOnMouse(Image image)
    {
        AudioManager.instance.PlaySE("ButtonClick"); // Button Click 효과음
        image.sprite = slot_Out;
        theInventory.ShowPlayerInfo();
    }

    // 플레이어 슬롯에 마우스를 때어놓을 시
    public void PlayerOutMouse(Image image)
    {
        image.sprite = slot_On;
        theInventory.HideItemInfo();
    }

    // Help UI 닫기 버튼 누를 시
    public void HelpCloseButton()
    {
        canvasHelp.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1;        // 시간 풀기

        isHelpActivated = false;
    }

    // Canvas Dead 세이브 지점 버튼
    public void LoadSavePointButton()
    {
        theSaveNLoad.LoadData();
        retAlpha(canvasDead);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerController.val = false;

        Time.timeScale = 1;        // 시간 풀기

        // 로딩 화면 띄우기
        startLoading(3f, "로딩 중...");
        startLoadingBase(5f);
    }

    /*
     * UI 페이드 인/아웃 (Canvas Group)
     * 
     * canvasShort : "그는 기억할 것입니다" or "그녀는 기억할 것입니다"
     * canvasLong : "클레멘타인은 XXX를 선택하였습니다"
     * canvasCollet : "아이템: 이름 / 설명
     * 
    */

    // ???는 기억할 것입니다.
    public void canvasShortFade(float Duration, string message)
    {
        canvasShort.SetActive(true);
        canvasShort.GetComponentInChildren<Text>().text = message;
        StartCoroutine(canvasFade(canvasShort, Duration));
    }

    // ???는 ???을 선택하였습니다.
    public void canvasLongFade(float Duration, string message)
    {
        canvasLong.SetActive(true);
        canvasLong.GetComponentInChildren<Text>().text = message;
        StartCoroutine(canvasFade(canvasLong, Duration));
    }

    // 아이템 수집 및 아이템 설명
    public void canvasCollectFade(float Duration, string message1, string message2)
    {
        canvasCollect.SetActive(true);
        canvasCollect.transform.GetChild(3).GetComponent<Text>().text = message1;
        canvasCollect.transform.GetChild(4).GetComponent<Text>().text = message2;
        StartCoroutine(canvasFade(canvasCollect, Duration));
    }

    // 미션 내용 갱신 및 UI 표시
    public void canvasMissionFade(float Duration, string message)
    {
        TalkConditionTrigger.currentMission = message;
        theInventory.UpdateMissionInfo(message);
        canvasMission.SetActive(true);
        canvasMission.transform.GetChild(1).GetComponent<Text>().text = message;
        StartCoroutine(canvasFade(canvasMission, Duration));
    }

    // 도움말 UI 표시
    public void canvasExplainFade(float Duration, string message)
    {
        canvasExplain.SetActive(true);
        canvasExplain.transform.GetChild(1).GetComponent<Text>().text = message;
        StartCoroutine(canvasFade(canvasExplain, Duration));
    }

    // 캔버스 UI 리셋
    public void resetCanvas(int num)
    {
        if (num == 0) canvasShort.SetActive(false);
        else if (num == 1) canvasLong.SetActive(false);
        else if (num == 2) canvasCollect.SetActive(false);
        else if (num == 3) canvasFadeInOut.SetActive(false);
        else if (num == 4) canvasMission.SetActive(false);
        else if (num == 5) canvasExplain.SetActive(false);
        else Debug.Log("ERROR_UIEventTrigger_resetCanvas");
    }

    // Damage 이펙트 UI 제거
    public void canvasDamageDelete()
    {
        canvasDamage.SetActive(false);
        inventoryDamage.SetActive(false);
    }

    // Damage 이펙트 UI 생성
    public void canvasDamageFade(float alpha)
    {
        canvasDamage.SetActive(true);
        Image image1 = canvasDamage.GetComponentInChildren<Image>();
        var tempColor1 = image1.color;
        tempColor1.a = alpha;
        image1.color = tempColor1;

        if(!inventoryHealth.activeSelf)
        {
            inventoryDamage.SetActive(true);
            Image image2 = inventoryDamage.GetComponentInChildren<Image>();
            var tempColor2 = image2.color;
            tempColor2.a = alpha;
            image2.color = tempColor2;
        }
    }

    // Heal 이펙트 효과
    public void canvasHealthFade(float Duration)
    {
        inventoryHealth.SetActive(true);
        inventoryDamage.SetActive(false);
        StartCoroutine(canvasFadeHeal(inventoryHealth, Duration));
    }

    // 캔버스 페이드 인/아웃 코루틴
    IEnumerator canvasFade(GameObject obj, float Duration)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        StartCoroutine(Fade(cg, cg.alpha, 1, 1.5f)); // Fade in
        yield return new WaitForSeconds(Duration);
        StartCoroutine(Fade(cg, cg.alpha, 0, 1.5f)); // Fade out

        yield return new WaitForSeconds(1.5f);
        obj.SetActive(false);
    }

    // 힐 페이드 아웃 코루틴
    IEnumerator canvasFadeHeal(GameObject obj, float Duration)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        StartCoroutine(Fade(cg, cg.alpha, 1, 0.5f)); // Fade in
        yield return new WaitForSeconds(Duration);
        StartCoroutine(Fade(cg, cg.alpha, 0, 0.5f)); // Fade out

        yield return new WaitForSeconds(1.5f);
        obj.SetActive(false);
    }

    // 캔버스 페이드 코루틴
    IEnumerator Fade(CanvasGroup cg, float start, float end, float Duration)
    {
        float counter = 0f;

        while (counter < Duration)
        {
            counter += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, counter / Duration);

            yield return null;
        }
    }

    // 타이머 UI 생성
    public void startTimer(float time)
    {
        slider.SetActive(true);
        StartCoroutine(timer(time));
        StartCoroutine(ChangeColorTimer(time));
        StartCoroutine(resetTimeUp(time));
    }

    // 타이머 UI 초기화 코루틴
    IEnumerator resetTimeUp(float time)
    {
        yield return new WaitForSeconds(time + 1f);
        resetTimer();
    }

    // 타이머 UI 초기화
    public void resetTimer()
    {
        timeSlider.transform.localScale = originalScale;
        timeSlider.GetComponent<Image>().color = startColor;
        slider.SetActive(false);
    }

    // 선택지 타이머
    IEnumerator timer(float time)
    {
        originalScale = timeSlider.transform.localScale;
        Vector3 destinationScale = new Vector3(0.0f, 0.1f, 1f);

        float currentTime = 0.0f;

        do
        {
            timeSlider.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= time);
    }

    // 타이머 색깔 바꾸기
    IEnumerator ChangeColorTimer(float cycleTime)
    {
        startColor = timeSlider.GetComponent<Image>().color;
        Color endColor = new Color32(224, 92, 88, 207);
        float currentTime = 0;

        while (currentTime < cycleTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / cycleTime;
            Color currentColor = Color.Lerp(startColor, endColor, t);
            timeSlider.GetComponent<Image>().color = currentColor;
            yield return null;
        }
    }

    // 화면 페이드 인/아웃
    public void startFade()
    {
        canvasFadeInOut.SetActive(true);
        StartCoroutine(startFade(canvasFadeInOut));
    }

    // 화면 페이드 인/아웃 코루틴
    IEnumerator startFade(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        StartCoroutine(Fade(cg, cg.alpha, 1, 1.5f)); // Fade in
        yield return new WaitForSeconds(2.5f);
        StartCoroutine(Fade(cg, cg.alpha, 0, 1.5f)); // Fade out
    }

    // 총 조준점 생성
    public void setShot()
    {
        canvasGun.SetActive(true);
    }

    // 총 조준점 생성
    public void retShot()
    {
        canvasGun.SetActive(false);
    }

    // 총 탄창 UI 켜기
    public void setHUD()
    {
        // 총알 확인
        canvasHUD.SetActive(true);
    }

    // 총 탄창 UI 끄기
    public void retHUD()
    {
        canvasHUD.SetActive(false);
    }

    // Help UI 지정 및 생성
    public void setCanvasHelp(Help help)
    {
        isHelpActivated = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0;        // 시간 정지

        canvasHelp.SetActive(true);

        help_Title.text = help.title;
        help_Image1.sprite = help.image1;
        help_Image2.sprite = help.image2;
        help_Message.text = help.message;
    }

    // NotePad UI 생성
    public void setNotePadUI(float time)
    {
        notePadUI.SetActive(true);
        StartCoroutine(NotePadFade(notePadUI, time));
    }

    // NotePad UI 페이드 코루틴
    IEnumerator NotePadFade(GameObject obj, float Duration)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        StartCoroutine(Fade(cg, cg.alpha, 1, 1f)); // Fade in
        yield return new WaitForSeconds(Duration);
        StartCoroutine(Fade(cg, cg.alpha, 0, 1f)); // Fade out

        yield return new WaitForSeconds(1.5f);
        obj.SetActive(false);
    }

    // canvasLoading UI 생성
    public void startLoading(float time, string message)
    {
        canvasLoading.SetActive(true);
        loadingMessage.text = message;
        StartCoroutine(startLoadingCorutine(canvasLoading, time));
    }

    // canvasLoading 페이드 코루틴
    IEnumerator startLoadingCorutine(GameObject obj, float Duration)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        StartCoroutine(Fade(cg, cg.alpha, 1, 1f)); // Fade in
        yield return new WaitForSeconds(Duration);
        StartCoroutine(Fade(cg, cg.alpha, 0, 1f)); // Fade out

        yield return new WaitForSeconds(1.5f);
        obj.SetActive(false);
    }

    // canvasLoadingBase UI 생성
    public void startLoadingBase(float time)
    {
        int _random = Random.Range(0, 10); // 랜덤 도움말

        canvasLoadingBase.SetActive(true);
        canvasLoadingBase.GetComponent<CanvasGroup>().alpha = 1f;
        loadingBaseMessage.text = help_Message_Loading[_random].message;
        StartCoroutine(startLoadingBaseCorutine(canvasLoadingBase, time));
    }
    
    public void startLoadingByScene(string message)
    {
        int _random = Random.Range(0, 10); // 랜덤 도움말

        canvasLoadingBase.SetActive(true);
        canvasLoadingBase.GetComponent<CanvasGroup>().alpha = 1f;
        loadingBaseMessage.text = help_Message_Loading[_random].message;

        canvasLoading.SetActive(true);
        canvasLoading.GetComponent<CanvasGroup>().alpha = 1f;
        loadingMessage.text = message;
    }

    public void stopLoadingByScene()
    {
        CanvasGroup cg = canvasLoadingBase.GetComponent<CanvasGroup>();
        StartCoroutine(Fade(cg, cg.alpha, 0, 1f)); // Fade out

        CanvasGroup cg2 = canvasLoading.GetComponent<CanvasGroup>();
        StartCoroutine(Fade(cg2, cg2.alpha, 0, 1f)); // Fade out

        StartCoroutine(ActiveFalse(1.5f));
    }

    IEnumerator ActiveFalse(float time)
    {
        yield return new WaitForSeconds(time);
        canvasLoading.SetActive(false);
        canvasLoadingBase.SetActive(false);
    }

    // canvasLoading 페이드 코루틴
    IEnumerator startLoadingBaseCorutine(GameObject obj, float Duration)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        yield return new WaitForSeconds(Duration);
        StartCoroutine(Fade(cg, cg.alpha, 0, 1f)); // Fade out

        yield return new WaitForSeconds(1.5f);
        obj.SetActive(false);
    }

    // canvasDead UI 생성
    public void setDeadUI()
    {
        canvasDead.SetActive(true);
        StartCoroutine(stopTimeCorutine());
    }

    IEnumerator stopTimeCorutine()
    {
        CanvasGroup cg = canvasDead.GetComponent<CanvasGroup>();
        StartCoroutine(Fade(cg, cg.alpha, 1, 1.5f)); // Fade in

        yield return new WaitForSeconds(1.5f);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0;        // 시간 정지
    }
}
