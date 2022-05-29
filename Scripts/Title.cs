using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour {

    public string sceneName = "GameStage";

    public static Title instance = null;

    private SaveNLoad theSaveNLoad;

    // Loading UI
    [SerializeField] private GameObject theTitle;
    [SerializeField] private GameObject canvasLoading;
    [SerializeField] private Text loadingMessage;

    [SerializeField] private GameObject canvasLoadingBase;
    [SerializeField] private Help[] help_Message_Loading;
    [SerializeField] private Text loadingBaseMessage;

    #region singleton
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
        {
            //if not, set instance to this
            instance = this;
        }
        //If instance already exists and it's not this:
        else if (instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        }

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }
    /*
    void Awake() // 객체 생성시 최초 실행.
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
            gameObject.SetActive(false);
    }
    */
    #endregion singleton

    void Start()
    {
        retAlpha(canvasLoading);
        retAlpha(canvasLoadingBase);
    }

    public void ClickStart()
    {
        // 로딩
        Debug.Log("로딩!");
        StartCoroutine(StartGamecoroutine());
    }

    IEnumerator StartGamecoroutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        int _random = Random.Range(0, 10); // 랜덤 도움말

        while (!operation.isDone)
        {
            startLoadingByScene(_random, "로딩 중...");
            yield return null;
        }
        theTitle.SetActive(false);
    }

    public void ClickLoad()
    {
        // 로드
        StartCoroutine(LoadCoroutine());
    }

    IEnumerator LoadCoroutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        int _random = Random.Range(0, 10); // 랜덤 도움말

        while (!operation.isDone)
        {
            startLoadingByScene(_random, "로딩 중...");
            yield return null;
        }
        theSaveNLoad = FindObjectOfType<SaveNLoad>();
        theSaveNLoad.LoadData();
        theTitle.SetActive(false);
    }

    public void ClickExit()
    {
        Application.Quit();
    }

    public void showCanvas()
    {
        gameObject.SetActive(true);
    }


    // 로딩 씬
    // 캔버스 알파값 초기화
    private void retAlpha(GameObject canvas)
    {
        canvasLoading.SetActive(false);
        canvasLoadingBase.SetActive(false);
    }

    public void startLoadingByScene(int _random, string message)
    {
        canvasLoadingBase.SetActive(true);
        canvasLoadingBase.GetComponent<CanvasGroup>().alpha = 1f;
        loadingBaseMessage.text = help_Message_Loading[_random].message;

        canvasLoading.SetActive(true);
        canvasLoading.GetComponent<CanvasGroup>().alpha = 1f;
        loadingMessage.text = message;
    }

    public void ResetLoading()
    {
        retAlpha(canvasLoading);
        retAlpha(canvasLoadingBase);
    }
}
