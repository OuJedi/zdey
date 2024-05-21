
using UnityEngine;

#if (UNITY_EDITOR)
[ExecuteInEditMode]
#endif

public class Platform : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private int leverId = -1;
    [SerializeField] private bool enableMove = true;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float radius = 2f;
    [Range(-1f, 1f)][SerializeField] private float startPosition = 0f;
    [SerializeField] private bool isVertical = false;
    [SerializeField] private LineRenderer line;
    [SerializeField] private GameObject begin;
    [SerializeField] private GameObject end;

    private float value = 0f;
    private float radiusLine = 0f;
    private float currentRadius = 0f;
    private bool _enable;
    private bool _currentVerticalStatus;
    private Vector2 position = Vector2.zero;
    private Vector2 movePos = Vector2.zero;
    private float levelScale;

    private void Awake()
    {
        enable = enableMove;
        position = transform.position;
        movePos = position;
        value = startPosition;

        if (Application.isPlaying)
        {
            line.enabled = false;
            begin.SetActive(false);
            end.SetActive(false);
            initMove(value);
        }
        else
        {
            line.enabled = true;
            begin.SetActive(true);
            end.SetActive(true);
            initMove(0f);
        }




    }

    private void Start()
    {
        LeverEventDispatcher.Instance.OnActionEvent.AddListener(onLeverAction);
    }


    private void onLeverAction(bool isAction, int id)
    {
        if (id == leverId)
        {
            enable = isAction;
        }
    }



    void move(float value)
    {
        if (isVertical)
        {
            movePos.y = position.y + Mathf.Sin(value) * radius;
        }
        else
        {
            movePos.x = position.x + Mathf.Sin(value) * radius;
        }

        transform.position = movePos;
    }


    void initMove(float value)
    {
        if (isVertical)
        {
            movePos.y = position.y + value * radius;
        }
        else
        {
            movePos.x = position.x + value * radius;
        }

        transform.position = movePos;
    }



    public bool enable
    {
        set
        {
            _enable = value;
        }

        get
        {
            return _enable;
        }
    }

    void Update()
    {
        if (!Application.isPlaying || !enable)
        {
            return;
        }

        value += speed * Time.deltaTime;

        move(value);

    }



#if (UNITY_EDITOR)

    private void OnGUI()
    {

        if (Application.isPlaying)
        {
            return;
        }

        // Debug.Log("OnGUI");
        if (currentRadius != radius)
        {

            currentRadius = radius;

            if (isVertical)
            {
                levelScale = GameObject.FindGameObjectWithTag("Level").transform.localScale.y;
                radiusLine = currentRadius / levelScale + GetComponent<SpriteRenderer>().size.y / 2;
            }
            else
            {
                levelScale = GameObject.FindGameObjectWithTag("Level").transform.localScale.x;
                radiusLine = currentRadius / levelScale + GetComponent<SpriteRenderer>().size.x / 2;
            }

            line.SetPosition(0, new Vector3(-radiusLine, 0, 0));
            line.SetPosition(1, new Vector3(radiusLine, 0, 0));
            begin.transform.localPosition = new Vector3(-radiusLine, 0, 0);
            end.transform.localPosition = new Vector3(radiusLine, 0, 0);

        }


        if (_currentVerticalStatus != isVertical)
        {
            _currentVerticalStatus = isVertical;
            if (isVertical)
            {
                line.transform.localEulerAngles = new Vector3(0, 0, 90);
            }
            else
            {
                line.transform.localEulerAngles = Vector3.zero;
            }
        }


        if (!enableMove)
        {
            line.enabled = false;
            begin.SetActive(false);
            end.SetActive(false);
        }
        else
        {
            line.enabled = true;
            begin.SetActive(true);
            end.SetActive(true);
        }


    }
#endif


    private void OnDestroy()
    {     
        LeverEventDispatcher.Instance.OnActionEvent.RemoveListener(onLeverAction);
    }


}
