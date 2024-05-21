
using UnityEngine;

#if (UNITY_EDITOR)
[ExecuteInEditMode]
#endif

public class BarbTexture : MonoBehaviour
{

    [SerializeField] private GlassType sprite = GlassType.Barb;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private GameObject[] shineArr;
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

            for (int i = 0; i < shineArr.Length; i++)
            {
                if (currentType != i)
                {
                    shineArr[i].SetActive(false);
                }
                else
                {
                    shineArr[i].SetActive(true);
                }
            }

        }
    }

}

public enum GlassType
{
    Barb, Glass1, Glass2
}
