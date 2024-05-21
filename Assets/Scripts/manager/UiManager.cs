using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class UiManager : MonoBehaviour
{
    [SerializeField] private Life _life;
    [SerializeField] private Gauge _gauge;
    [SerializeField] private TextMeshProUGUI _headNbr;
    [SerializeField] private GameObject _pressToStartText;
    [SerializeField] private float pressToStartTextAlphaSpeed;

    private float pressToStartTextAlphaVal = 0;
    private static UiManager _instance;
    private TextMeshProUGUI _pressToStartTextGUI;
    public static UiManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("The UiManager Singlton is NULL.");
            }

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        _pressToStartTextGUI = _pressToStartText.GetComponent<TextMeshProUGUI>();
    }

    public void ResetUi()
    {
        Debug.Log("ResetUi");
        life.value = Config.lifeMaxValue;
        gauge.value = Config.gaugeMaxValue;
    }


    public Life life
    {
        get { return _life; }

    }

    public Gauge gauge
    {
        get { return _gauge; }

    }

    public string headNbr
    {
        set { _headNbr.text = value; }
        get { return _headNbr.text; }
    }

    public GameObject pressToStartText
    {
        get { return _pressToStartText; }
    }

    void Start()
    {

    }

    void Update()
    {
        if (_pressToStartText.activeSelf)
        {
            pressToStartTextAlphaVal += pressToStartTextAlphaSpeed * Time.deltaTime;
            _pressToStartTextGUI.alpha = Mathf.Sin(pressToStartTextAlphaVal);
        }
    }
}
