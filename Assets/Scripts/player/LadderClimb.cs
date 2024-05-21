
using UnityEngine;
using UnityEngine.Events;

public class LadderClimb : MonoBehaviour
{
    [SerializeField] private float climbSpeed = 8f;
    [SerializeField] private float gravityScale = 7f;

    public UnityEvent onStartClimbing;
    public UnityEvent onStopClimbing;
    public UnityEvent onStopMoving;
    public UnityEvent onStartMoving;

    private Collider2D _ladder;
    private GameObject _top;
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private bool _isClimbing;
    private bool _isOnLadder;
    private bool _activate = true;
    private float _dirY;
    private float _ladderHeight;
    private bool _isLeftTheLadder = false;
    private bool _verticalDown = false;
    private bool _onStopMovingDispached = false;
    private bool _topContact = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();

    }

    private void Start()
    {
        InputControl.Instance.OnInteractionEvent.AddListener(onInteractionEvent);
    }


    private void onInteractionEvent(string type, float value)
    {
        switch (type)
        {
            case "VerticalDown":
                if (_activate)
                {
                    startClimbing();
                }
                break;

            default: break;
        }
    }

    void FixedUpdate()
    {

        if (!_isOnLadder || !_activate)
        {
            return;
        }        

        _dirY = InputControl.Instance.dirY;

        // En haut
        // Calcul de la hauteur de l'échelle et du sommet
        float ladderTopHeight = _ladder.transform.position.y + _top.transform.localPosition.y + _top.transform.localScale.y / 2;
        float playerBottomHeight = transform.position.y - bc.size.y / 2;

        // Vérification si le joueur est proche du sommet de l'échelle et se déplace vers le haut
        bool isNearLadderTop = (ladderTopHeight - playerBottomHeight) <= 0.2f;
        bool isMovingUp = _dirY > 0;

        // Condition simplifiée
        if (isNearLadderTop && isMovingUp)
        {
            rb.velocity = new Vector2(0, 0);
            stopClimbing();
            return;
        }



        if (_dirY != 0)
        {
            if (!_verticalDown)
            {
                _verticalDown = true;
                _onStopMovingDispached = false;
                onStartMoving.Invoke();
                startClimbing();
            }
        }
        else
        {
            _verticalDown = false;
            if (!_onStopMovingDispached)
            {
                onStopMoving.Invoke();
                _onStopMovingDispached = true;
            }

        }

        // en bas
        if ((_ladder.transform.position.y - _ladderHeight) > (transform.position.y - bc.size.y / 2) && _dirY < 0)
        {
            if (!_isLeftTheLadder)
            {
                stopClimbing();
            }
        }
        else
        {
            _isLeftTheLadder = false;
        }

        if (_isClimbing)
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(0, _dirY * climbSpeed * Time.deltaTime);
            transform.position = new Vector2(_ladder.gameObject.transform.position.x, transform.position.y);
        }

    }

    public void onLadderTriggerEnter(Collider2D collision)
    {
        if (collision.tag == "Ladder")
        {
            Debug.Log("onLadderTriggerEnter ");
            _ladder = collision;
            _top = _ladder.gameObject.GetComponent<Ladder>().top;
            _ladderHeight = _ladder.gameObject.GetComponent<SpriteRenderer>().size.y;
            _isOnLadder = true;
        }
    }

    public void onLadderTriggerExit(Collider2D collision)
    {
        if (collision.tag == "Ladder")
        {
            Debug.Log("onLadderTriggerExit ");
            _isOnLadder = false;
            _ladder = null;
            _verticalDown = false;

            stopClimbing();
        }
    }

    void stopClimbing()
    {
        Debug.Log("stopClimbing ");
        _isClimbing = false;
        _isLeftTheLadder = true;
        rb.gravityScale = gravityScale;
        onStopClimbing.Invoke();
        _top.GetComponent<BoxCollider2D>().enabled = true;
    }

    void startClimbing()
    {
        if (!_isClimbing && _isOnLadder)
        {
            if (_ladder != null)
            {
                _isClimbing = true;
                onStartClimbing.Invoke();
                transform.position = new Vector2(_ladder.gameObject.transform.position.x, transform.position.y);
                _top.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }

    public void onLadderTriggerStay(Collider2D collision)
    {
        if (collision.tag == "Ladder")
        {
            _isOnLadder = true;
        }
    }


    public void exit()
    {
        Debug.Log("EXIT");
        _isOnLadder = false;
        _ladder = null;
        _verticalDown = false;
        stopClimbing();
    }


    public bool isClimbing
    {
        set
        {
            _isClimbing = value;
            _isOnLadder = value;
            _isLeftTheLadder = value;
            if (!value)
            {
                rb.gravityScale = gravityScale;
            }
        }
        get { return _isClimbing; }
    }

    public bool activate
    {
        set
        {
            _activate = value;
        }
        get { return _activate; }
    }
    

    public void dispose()
    {
        InputControl.Instance.OnInteractionEvent.RemoveListener(onInteractionEvent);
    }


}