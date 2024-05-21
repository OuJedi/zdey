using System.Collections;
using UnityEngine;

#if (UNITY_EDITOR)
[ExecuteInEditMode]
#endif

public class Distributeur : MonoBehaviour
{

    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeMagnitude = 5f;
    [SerializeField] private float shakeDelay = .1f;
    [SerializeField] private bool flip = false;
    [SerializeField] private bool _active = true;
    [SerializeField] private Sprite spriteOpen;
    [SerializeField] private GameObject animation;
    [SerializeField] private GameObject light;

    private float initialRotationZ;
    private Quaternion initialRotation;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private int hitNbr = 0;
    private bool currentFlip = false;
    private bool __active;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = animation.GetComponent<Animator>();
        __active = !_active;
    }

    void Start()
    {
        initialRotation = transform.rotation;
        initialRotationZ = transform.localEulerAngles.z;
    }

    public void Action()
    {

        StartCoroutine(Shake());

        if (!active)
        {
            return;
        }

        if (hitNbr == 1)
        {
            spriteRenderer.sprite = spriteOpen;
        }
        else if (hitNbr == 2)
        {
            brokenAnim();
            _active = false;
            UiManager.Instance.gauge.value = Config.gaugeMaxValue;
        }        

        hitNbr++;

    }

    void brokenAnim()
    {
        spriteRenderer.sprite = null;
        animation.SetActive(true);
        light.SetActive(false);
        animator.speed = PlayerAnimationSpeed.distributeur;
        animator.Play("root.distributeur", 0, 0);
    }


    IEnumerator Shake()
    {
        float elapsedTime = 0f;
        int signe = 1;

        while (elapsedTime < shakeDuration)
        {
            float zRotation = initialRotationZ + shakeMagnitude * signe;

            transform.rotation = Quaternion.Euler(new Vector3(initialRotation.x, initialRotation.y, zRotation));

            elapsedTime += Time.deltaTime;
            signe = -signe;
            yield return new WaitForSeconds(shakeDelay);
        }

        // Réinitialiser la rotation
        transform.rotation = initialRotation;
    }



    public bool active
    {
        get { return _active; }
    }

    private void OnGUI()
    {
        if (Application.isPlaying)
        {
            return;
        }


        if (currentFlip != flip)
        {
            currentFlip = flip;
            spriteRenderer.transform.localScale = new Vector2(flip ? -1 : 1, 1);
        }

        if (__active != _active)
        {
            __active = _active;

            if (_active)
            {
                spriteRenderer.sprite = spriteOpen;
                animation.SetActive(false);
                light.SetActive(true);
            }
            else
            {
                brokenAnim();
            }
        }
    }

}
