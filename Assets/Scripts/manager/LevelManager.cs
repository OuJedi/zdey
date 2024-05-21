using DG.Tweening;
using UnityEngine;



public class LevelManager : MonoBehaviour
{

    private Vector3 cameraPosition = new Vector3(0, 0, -10);
    private Vector3 backPosition;
    private Vector3 initBackPosition;
    private Vector3 middlePosition;
    private Vector3 initMiddlePosition;
    private Vector3 nearPosition;
    private Vector3 initNearPosition;

    private GameObject back;
    private GameObject middle;
    private GameObject nearDetail;


    private Camera mainCamera;
    [SerializeField] private float backMoveSpeed = .7f;
    [SerializeField] private float middleMoveSpeed = .3f;
    [SerializeField] private float nearDetailMoveSpeed = .21f;
    [SerializeField] private Vector2 cameraSize = new Vector2(11, 0);
    [SerializeField] private bool _autoRunDebug = false;




    // Start is called before the first frame update
    void Start()
    {

        mainCamera = Camera.main;
        back = GameObject.FindGameObjectWithTag("Back");
        middle = GameObject.FindGameObjectWithTag("Middle");
        nearDetail = GameObject.FindGameObjectWithTag("NearDetail");
        backPosition = initBackPosition = back.transform.position;
        middlePosition = initMiddlePosition = middle.transform.position;
        nearPosition = initNearPosition = nearDetail.transform.position;

        if (_autoRunDebug)
        {
            autoRunDebug();
        }

    }


    void autoRunDebug()
    {
        GameObject end = GameObject.FindGameObjectWithTag("EndZone");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        player.transform.DOMove(end.transform.position, 60f);

    }



    void Update()
    {


        cameraPosition = mainCamera.transform.position;

        backPosition.x = (initBackPosition.x + cameraPosition.x * backMoveSpeed - cameraSize.x);
        middlePosition.x = (initMiddlePosition.x + cameraPosition.x * middleMoveSpeed - cameraSize.x);
        nearPosition.x = (initNearPosition.x - cameraPosition.x * nearDetailMoveSpeed - cameraSize.x);

        back.transform.position = backPosition;
        middle.transform.position = middlePosition;
        nearDetail.transform.position = nearPosition;


    }

    private void OnDestroy()
    {
        Destroy(gameObject);
        Debug.Log(this + "Destroyed");
    }
}

