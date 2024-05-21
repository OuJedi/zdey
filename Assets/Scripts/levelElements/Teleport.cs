
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] private GameObject destination;
    [SerializeField] private Transform target;
    [SerializeField] private GameObject mask;

    private CinemachineVirtualCamera virtualCamera;
    private Transform m_Follow;
    private Transform originalFollowTarget;
    private SpriteMask spriteMask;
    private SpriteMask spriteMaskDest;

    void Start()
    {
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        m_Follow = virtualCamera.Follow;
        originalFollowTarget = m_Follow;
        spriteMask = mask.GetComponent<SpriteMask>();
        spriteMaskDest = destination.GetComponentInChildren<SpriteMask>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        float direction = Mathf.Sign(gameObject.transform.localScale.x);
        float destDirection = Mathf.Sign(destination.transform.localScale.x);

        if (collision.CompareTag("Player") && !Tools.Instance.IsFacingTarget(collision.gameObject, gameObject))
        {
            spriteMask.enabled = true;
            spriteMaskDest.enabled = true;

            collision.GetComponent<PlayerControler>().teleporting = true;
            collision.GetComponent<PlayerControler>().playAnim("run");
            collision.transform.DOMoveX(transform.position.x + 2.4f * direction, 1f).onComplete = () =>
            {
                target.position = m_Follow.position;
                virtualCamera.Follow = target;

                Vector2 dest = new Vector2(destination.transform.position.x, destination.transform.position.y + .8f);

                target.DOMove(dest, 1f).SetDelay(.25f).onComplete = () =>
                {
                    collision.GetComponent<PlayerControler>().flip(destDirection);
                    collision.transform.position = new Vector2(destination.transform.position.x - 2.3f * destDirection, destination.transform.position.y + .8f);

                    collision.transform.DOMoveX(destination.transform.position.x, .5f).onComplete = () =>
                    {
                        spriteMask.enabled = false;
                        spriteMaskDest.enabled = false;
                        virtualCamera.Follow = originalFollowTarget;
                        collision.GetComponent<PlayerControler>().playAnim("stand");
                        Tools.Instance.Delay(.2f, () =>
                        {
                            collision.GetComponent<PlayerControler>().teleporting = false;
                        });

                    };

                };

            };

        }


    }

    void Update()
    {

    }
}
