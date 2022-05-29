using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public static bool pauseMenuActivated = false;

    [SerializeField] private GameObject go_BaseUI;
    [SerializeField] private SaveNLoad theSaveNLoad;
    [SerializeField] private GameObject settingMenu;

    // Setting 변수
    private bool isSetting;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Text musicVolumePersentage;
    [SerializeField] private Text sfxVolumePersentage;
    [SerializeField] private Text Sensitivity_InField;
    [SerializeField] private Text Sensitivity_OutField;
    [SerializeField] private Text Sensitivity_ShotMode;

    [SerializeField] private Slider musicVolumeSldier;
    [SerializeField] private Slider sfxVolumeSldier;
    [SerializeField] private Slider Sensitivity_InFieldSldier;
    [SerializeField] private Slider Sensitivity_OutFieldSldier;
    [SerializeField] private Slider Sensitivity_ShotModeSldier;

    [SerializeField] private Toggle musicVolumeToggle;
    [SerializeField] private Toggle sfxVolumeToggle;

    private UiEventTrigger uiEventManager;
    private ZombieSpawManager theZombieSpawnManager;
    private Sun theSun;

    // 로딩
    public void LoadToSetting(float musicVol, bool musicMute, float SFXVol, bool SFXMute, float InFieldSens, float OutFieldSnes, float ShotModeSens)
    {
        musicVolumeSldier.value = musicVol;
        musicVolumeToggle.isOn = musicMute;
        sfxVolumeSldier.value = SFXVol;
        sfxVolumeToggle.isOn = SFXMute;

        Sensitivity_InFieldSldier.value = InFieldSens;
        Sensitivity_OutFieldSldier.value = OutFieldSnes;
        Sensitivity_ShotModeSldier.value = ShotModeSens;

        SetMusicVolume(musicVolumeSldier);
        MuteMusice(musicVolumeToggle);
        SetSFXVolume(sfxVolumeSldier);
        MuteSFX(sfxVolumeToggle);

        SetInFieldSentivity(Sensitivity_InFieldSldier);
        SetOutFieldSentivity(Sensitivity_OutFieldSldier);
        SetShotModeSentivity(Sensitivity_ShotModeSldier);
    }

    void Start()
    {
        isSetting = false;
        settingMenu.SetActive(false);
        uiEventManager = FindObjectOfType<UiEventTrigger>();
        theZombieSpawnManager = FindObjectOfType<ZombieSpawManager>();
        theSun = FindObjectOfType<Sun>();
    }

    // Update is called once per frame
    void Update()
    {
        TryOpenPauseMenu();
        TrySave();
    }

    // 인벤토리 열기/닫기 시도
    private void TryOpenPauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            pauseMenuActivated = !pauseMenuActivated;

            if (pauseMenuActivated)
            {
                OpenPauseMenu();
            }
            else
            {
                ClosePauseMenu();
            }
        }
    }

    // 메뉴 열기
    private void OpenPauseMenu()
    {
        AudioManager.instance.StopBGM();
        theZombieSpawnManager.SetAudioMute();
        go_BaseUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;        // 시간 정지
    }

    // 메뉴 닫기
    private void ClosePauseMenu()
    {
        if (AudioManager.instance.currentBGM == "Background_Zombie")
            AudioManager.instance.ContinueBGM();
        else AudioManager.instance.PlayBGM(AudioManager.instance.currentBGM);
        theZombieSpawnManager.SetAudioPlay();
        go_BaseUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;        // 시간 풀기
    }

    public void ClickSave()
    {
        if(theSun.isNight)
        {
            // 좀비가 따라오는 상태이면 세이브 불가능.
            if (!theZombieSpawnManager.CheckFollowing())
            {
                theSaveNLoad.SaveData();
                pauseMenuActivated = false;
                ClosePauseMenu();
            }
            else
            {
                uiEventManager.canvasExplainFade(3f, "주변에 워커가 쫓아오고 있어 세이브 할 수 없습니다.");
                pauseMenuActivated = false;
                ClosePauseMenu();
            }
        }
    }

    private void TrySave()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            if (theSun.isNight)
            {
                // 좀비가 따라오는 상태이면 세이브 불가능.
                if (!theZombieSpawnManager.CheckFollowing())
                {
                    theSaveNLoad.SaveData();
                    pauseMenuActivated = false;
                    ClosePauseMenu();
                }
                else
                {
                    uiEventManager.canvasExplainFade(3f, "주변에 워커가 쫓아오고 있어 세이브 할 수 없습니다.");
                    pauseMenuActivated = false;
                    ClosePauseMenu();
                }
            }
        }
    }

    public void ClickLoad()
    {
        theSaveNLoad.LoadData();
        pauseMenuActivated = false;
        ClosePauseMenu();

        // 로딩 화면 띄우기
        uiEventManager.startLoading(3f, "로딩 중...");
        uiEventManager.startLoadingBase(5f);
    }

    public void ClickSetting(Text _text)
    {
        isSetting = true;
        settingMenu.SetActive(true);
        _text.fontSize = 70;
        _text.color = new Color32(255, 255, 255, 255);
    }

    public void ClickExit()
    {
        AudioManager.instance.PlayBGM(AudioManager.instance.currentBGM);
        go_BaseUI.SetActive(false);
        Time.timeScale = 1;        // 시간 풀기
        Application.Quit();
    }

    public void PointerEnter(Text _text)
    {
        AudioManager.instance.PlaySE("ButtonClick"); // Button Click 효과음
        if (_text.text == "Setting")
        {
            settingMenu.SetActive(true);
            isSetting = true;
        }
        else
        {
            isSetting = false;
            settingMenu.SetActive(false);
        }
        if (_text.text == "Save" && !theSun.isNight)
        {
            _text.fontSize = 60;
            _text.color = new Color32(150, 150, 150, 255);
        }
        else
        {
            _text.fontSize = 70;
            _text.color = new Color32(255, 255, 255, 255);
        }
    }

    public void PointerExit(Text _text)
    {
        if (_text.text != "Setting")
        {
            settingMenu.SetActive(false);
        }
        _text.fontSize = 60;
        _text.color = new Color32(150, 150, 150, 255);
    }

    // Setting

    public void SetMusicVolume(Slider _slider)
    {
        float musicVolmue = _slider.value;
        musicVolumePersentage.text = Mathf.RoundToInt(musicVolmue * 100) + "%";
        AudioManager.instance.SetMusicVolume(musicVolmue);
    }

    public void MuteMusice(Toggle _toggle)
    {
        if (_toggle.isOn) AudioManager.instance.SetMusicMute();
        else AudioManager.instance.SetMusicPlay();
    }

    public void SetSFXVolume(Slider _slider)
    {
        float sfxVolmue = _slider.value;
        sfxVolumePersentage.text = Mathf.RoundToInt(sfxVolmue * 100) + "%";
        AudioManager.instance.SetSFXVolume(sfxVolmue);
        theZombieSpawnManager.SetAudioVolume(sfxVolmue);
    }

    public void MuteSFX(Toggle _toggle)
    {
        if (_toggle.isOn)
        {
            AudioManager.instance.SetSFXMute();
            theZombieSpawnManager.SetAudioMute();
        }
        else
        {
            AudioManager.instance.SetSFXPlay();
            theZombieSpawnManager.SetAudioPlay();
        }
    }

    // Sensitivity
    //   In-Field Sensitivity: 50 ~ 250 (Default:150)
    //   Out-Field Sensitivity: 40 ~ 100 (Default:70)
    //   Shot-Mode Sensitivity: 10 ~ 50 (Default:30)

    public void SetInFieldSentivity(Slider _slider)
    {
        float inFieldSens = (_slider.value * 200) + 50;
        Sensitivity_InField.text = Mathf.RoundToInt(inFieldSens).ToString();
        cameraController.SetInFieldSensitivity(inFieldSens);
    }

    public void SetOutFieldSentivity(Slider _slider)
    {
        float OutFieldSens = (_slider.value * 60) + 40;
        Sensitivity_OutField.text = Mathf.RoundToInt(OutFieldSens).ToString();
        cameraController.SetOutFieldSensitivity(OutFieldSens);
    }

    public void SetShotModeSentivity(Slider _slider)
    {
        float ShotModeSens = (_slider.value * 40) + 10;
        Sensitivity_ShotMode.text = Mathf.RoundToInt(ShotModeSens).ToString();
        cameraController.SetShotModeSensitivity(ShotModeSens);
    }

    public float getMusicVolume() { return musicVolumeSldier.value; }
    public float getSFXVolume() { return sfxVolumeSldier.value; }
    public bool getMusicMute() { return musicVolumeToggle.isOn; }
    public bool getSFXMute() { return sfxVolumeToggle.isOn; }

    public float getInFieldSensitivity() { return Sensitivity_InFieldSldier.value; }
    public float getOutFieldSensitivity() { return Sensitivity_OutFieldSldier.value; }
    public float getShotModeSensitivity() { return Sensitivity_ShotModeSldier.value; }

}
