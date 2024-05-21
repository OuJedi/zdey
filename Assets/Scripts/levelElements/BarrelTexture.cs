
using UnityEngine;

#if (UNITY_EDITOR)
[ExecuteInEditMode]
#endif

public class BarrelTexture : MonoBehaviour
{

    [SerializeField] private Type sprite = Type.Default;
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

public enum Type
{
    Default, Flat, Sauce, Chains
}
