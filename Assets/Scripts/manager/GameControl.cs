using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    [SerializeField] private string[] levels;
    [SerializeField] private GameObject uiButtons;
    [SerializeField] private GameObject interlayer;
    [SerializeField] private TextMeshProUGUI interlayerText;
    [SerializeField] private bool _autoRunDebug;

    private int currenLevelId = 0;
    private bool inputAccess = true;
    private int _headLife = 0;
    private static GameControl _instance;
    public static GameControl Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("The Singlton is NULL.");
            }

            return _instance;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _instance = this;

        Debug.Log("Mémoire système disponible: " + SystemInfo.systemMemorySize + " Mo");
    }

    private void Start()
    {
        InputControl.Instance.OnInteractionEvent.AddListener(onInteractionEvent);
        uiButtons.SetActive(false);
        interlayer.SetActive(false);
        headLife = Config.headMaxValue;

        StartCoroutine(loadAsyncScene("Home"));
    }

    private void onInteractionEvent(string type, float value)
    {
        if (!inputAccess)
        {
            return;
        }

        if (type == "JumpDown" || type == "ActionDown" || type == "SprayDown")
        {
            LoadLevel(levels[currenLevelId]);
        }

    }

    public int headLife
    {
        set
        {
            _headLife = value;
            UiManager.Instance.headNbr = _headLife + "";
        }
        get { return _headLife; }
    }

    public void LoadLevel(string levelName)
    {
        UiManager.Instance.pressToStartText.SetActive(false);
        inputAccess = false;
        uiButtons.SetActive(false);

        PostProcessingControl.Instance.blackFadeIn(() =>
        {

            UiManager.Instance.ResetUi();

            Tools.Instance.Delay(1f, () =>
            {
                interlayer.SetActive(true);
                interlayerText.text = levelName == "Home" ? "Thank you for playing" : levelName;
            });

            Tools.Instance.Delay(2f, () =>
            {
                interlayer.SetActive(false);
                StartCoroutine(loadAsyncScene(levelName));
            });

        }, false);
    }

    public void Continue()
    {
        headLife--;

        if (_headLife < 0)
        {
            Debug.Log("Game Over");
            GoHome();
        }
        else
        {
            LoadLevel(levels[currenLevelId]);
        }
    }

    public void GoHome()
    {
        Debug.Log("GoHome");
        uiButtons.SetActive(false);
        LoadLevel("Home");
    }

    public void nextlevel()
    {
        Debug.Log("Nextlevel");

        if (currenLevelId < levels.Length - 1)
        {
            currenLevelId++;
            LoadLevel(levels[currenLevelId]);
        }
        else
        {
            GoHome();
        }

    }

    IEnumerator loadAsyncScene(string sceneName)
    {

        Debug.Log("loadAsyncScene:" + sceneName);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        GC.Collect();
        Resources.UnloadUnusedAssets();

        Debug.Log("graphicsMemorySize:" + SystemInfo.graphicsMemorySize);
        Debug.Log("systemMemorySize:" + SystemInfo.systemMemorySize);

        //Scene scene = SceneManager.GetActiveScene();              

        if (sceneName == "Home")
        {
            UiManager.Instance.pressToStartText.SetActive(true);
            inputAccess = true;
            headLife = Config.headMaxValue;
            currenLevelId = 0;

            if (_autoRunDebug)
            {
                currenLevelId = -1;
                Tools.Instance.Delay(2, nextlevel);
            }

        }
        else
        {
            uiButtons.SetActive(true);
        }

        PostProcessingControl.Instance.blackFadeOut(null, 0, false);

    }

    void Update()
    {

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        if ((Input.anyKeyDown || Input.GetMouseButtonDown(0)) && inputAccess)
        {
            LoadLevel(levels[currenLevelId]);
        }


    }
}
