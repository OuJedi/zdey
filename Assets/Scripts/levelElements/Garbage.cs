using System.Collections;
using UnityEngine;

#if (UNITY_EDITOR)
[ExecuteInEditMode]
#endif

public class Garbage : MonoBehaviour
{
    [SerializeField] private GarbageType sprite = GarbageType.Yellow;
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeMagnitude = 5f;
    [SerializeField] private float shakeDelay = .1f;
    [SerializeField] private bool flip = false;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Sprite[] openTetxures;
    [SerializeField] private GameObject mouches;

    private int currentType;
    private bool currentFlip;
    private float initialRotationZ;
    private Quaternion initialRotation;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mouches.SetActive(false);
    }

    void Start()
    {
        initialRotation = transform.rotation;
        initialRotationZ = transform.localEulerAngles.z;

    }

    public void Action()
    {
        currentType = (int)sprite;
        spriteRenderer.sprite = openTetxures[currentType];
        StartCoroutine(Shake());
        mouches.SetActive(true);
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

    private void OnGUI()
    {

        if (Application.isPlaying)
        {
            return;
        }

        if (currentType != (int)sprite)
        {
            currentType = (int)sprite;
            spriteRenderer.sprite = sprites[currentType];
        }

        if (currentFlip != flip)
        {
            currentFlip = flip;
            spriteRenderer.transform.localScale = new Vector2(flip ? -1 : 1, 1);
        }
    }

}

public enum GarbageType
{
    Yellow, Green, Brown
}
