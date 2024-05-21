
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

#if (UNITY_EDITOR)
[ExecuteInEditMode]
#endif

public class BoxTexture : MonoBehaviour
{

    [SerializeField] private int id = 0;
    [SerializeField] private BoxType sprite = BoxType.Default;
    [SerializeField] private Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    private int currentType;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnGUI()
    {
        if (!Application.isPlaying && currentType != (int)sprite)
        {
            currentType = (int)sprite;
            spriteRenderer.sprite = sprites[currentType];
        }
    }

}

public enum BoxType
{
    Default, Box2, Box3, Box4, Box5
}


