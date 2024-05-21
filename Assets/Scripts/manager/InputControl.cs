using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputControl : MonoBehaviour
{

    private UnityEvent<string, float> _onInteractionEvent;
    private static InputControl _instance;
    private static float _dirX;
    private static float _dirY;

    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        _instance = this;
        _onInteractionEvent = new UnityEvent<string, float>();
    }

    public static InputControl Instance
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

    public UnityEvent<string, float> OnInteractionEvent
    {
        get { return _onInteractionEvent; }
    }

    public float dirX
    {
        set { _dirX = value; }
        get { return _dirX; }
    }

    public float dirY
    {
        set { _dirY = value; }
        get { return _dirY; }
    }

    // Update is called once per frame
    void Update()
    {



        dirX = Input.GetAxisRaw("Horizontal");
        dirY = Input.GetAxisRaw("Vertical");


        if(dirY != 0)
        {
            _onInteractionEvent.Invoke("Vertical", dirY);
        }
        
        if(dirX != 0)
        {
            _onInteractionEvent.Invoke("Horizontal", dirX);
        }

        if (Input.GetButtonDown("Horizontal"))
        {
            _onInteractionEvent.Invoke("HorizontalDown", dirX);
        }

        if (Input.GetButtonDown("Vertical"))
        {
           
            _onInteractionEvent.Invoke("VerticalDown", dirY);
        }

        if (Input.GetButtonUp("Horizontal"))
        {
            _onInteractionEvent.Invoke("HorizontalUp", dirX);
        }

        if (Input.GetButtonUp("Vertical"))
        {
            _onInteractionEvent.Invoke("VerticalUp", dirY);
        }

        if (Input.GetButtonDown("Jump"))
        {
            _onInteractionEvent.Invoke("JumpDown", 0f);
        }

        if (Input.GetButtonDown("Action"))
        {
            _onInteractionEvent.Invoke("ActionDown", 0f);
        }

        if (Input.GetButtonDown("Spray"))
        {
            _onInteractionEvent.Invoke("SprayDown", 0f);
            Debug.Log("Spray");
        }
        
        if (Input.GetButtonUp("Spray"))
        {
            _onInteractionEvent.Invoke("SprayUp", 0f);
            Debug.Log("SprayUp");
        }

       

    }
}
