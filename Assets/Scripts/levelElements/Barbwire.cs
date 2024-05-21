using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barbwire : MonoBehaviour
{
    [SerializeField] private GameObject barbwire;
    private float width;
    private void Start()
    {
        width = barbwire.GetComponent<SpriteRenderer>().size.x;
    }

    public void AnimComplete()
    {
        int posx = (int)Random.Range(0, width - .01f);

        while (posx == (int)transform.localPosition.x)
        {
            posx = (int)Random.Range(0, width - .01f);
        }

        transform.localPosition = new Vector3(posx, 0f, 0f);
    }
}
