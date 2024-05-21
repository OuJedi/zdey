using UnityEngine;

public class Background : MonoBehaviour
{
    private Renderer renderer;
    private float offsetX = 0;
    private void Awake()
    {
        renderer = GetComponent<Renderer>();

      
    }

    // Update is called once per frame
    void Update()
    {
        offsetX += .02f * Time.deltaTime;
        renderer.materials[0].SetTextureOffset("_MainTex", new Vector2(offsetX, 0f));
    }
}
