
using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

#if (UNITY_EDITOR)
[ExecuteInEditMode]
#endif

public class Pigeon : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private bool enableMove = true;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float radius = 2f;
    [Range(-1f, 1f)][SerializeField] private float startPosition = 0f;
    [SerializeField] private bool isVertical = false;
    [SerializeField] private LineRenderer line;
    [SerializeField] private GameObject begin;
    [SerializeField] private GameObject end;
    [SerializeField] private float reviveTime = 5;

    private float value = 0f;
    private float radiusLine = 0f;
    private float currentRadius = 0f;
    private float currentPosX = 0f;
    private bool isFalling = false;
    private bool _enable;
    private bool _currentVerticalStatus;
    private Vector2 position = Vector2.zero;
    private Vector2 movePos = Vector2.zero;
    private float levelScale;
    private Animator animator;
    private BoxCollider2D bc;
    private Vector2 initPosition;
    private Coroutine delayCoroutine;

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
        animator = GetComponent<Animator>();
        animator.Play("root.flight", 0, Random.value);
        bc = GetComponent<BoxCollider2D>();
        initPosition = transform.position;

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
            float sign = Mathf.Sign(movePos.x - currentPosX);
            transform.localScale = new Vector3(sign * Mathf.Abs(transform.localScale.x), transform.localScale.y);
            currentPosX = movePos.x;
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




    public void Fall()
    {
        if (isFalling)
        {
            return;
        }
        isFalling = true;
        animator.speed = PlayerAnimationSpeed.pigeonFall;
        animator.Play("root.fall", 0, 0);
        bc.isTrigger = true;
        transform.DOMoveY(transform.position.y - 4f, 1f).SetEase(Ease.InExpo).onComplete = () =>
        {
            gameObject.SetActive(false);
            delayCoroutine = Tools.Instance.Delay(reviveTime, reset);
        };

    }

    void reset()
    {
        transform.position = initPosition;
        animator.speed = PlayerAnimationSpeed.pigeonFlight;
        animator.Play("root.flight", 0, 0);
        bc.isTrigger = false;
        isFalling = false;
        gameObject.SetActive(true);
    }




    void Update()
    {
        if (!Application.isPlaying || !enable || isFalling)
        {
            return;
        }

        value += speed * Time.deltaTime;

        move(value);

    }

    private void OnDestroy()
    {
        if (Tools.Instance)
        {
            Tools.Instance.KillDelay(delayCoroutine);
        }
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


}
