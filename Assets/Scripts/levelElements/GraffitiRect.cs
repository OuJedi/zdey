using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GraffitiRect : MonoBehaviour
{

    [SerializeField] private GraffitiId graffitiId = GraffitiId.Graffiti1;
    [SerializeField] private SpriteRenderer image;
    [SerializeField] private SpriteRenderer imageBg;
    [SerializeField] private SpriteMask mask;
    [SerializeField] private Sprite[] masks;
    [SerializeField] private Sprite[] garffitiList;


    private bool _done = false;
    private float _graffitiProgressVal = 0;
    private int _imageId = 0;
    private Animator animator;

    // Start is called before the first frame update
    private void Awake()
    {
        _imageId = (int)graffitiId;
        image.sprite = garffitiList[_imageId];
        imageBg.sprite = garffitiList[_imageId];
        mask.sprite = masks[0];
        animator = gameObject.GetComponentInChildren<Animator>();
    }

    public void progress()
    {
        if (_graffitiProgressVal < 1f)
        {
            _graffitiProgressVal += Config.graffitiProgressVal * Time.deltaTime;
            int id = (int)(_graffitiProgressVal * (masks.Length - 1));
            mask.sprite = masks[id];
        }
        else
        {
            done = true;
            animator.Play("root.graffitiDone", 0, 0);
            mask.sprite = masks[(masks.Length - 1)];
        }
    }

    public void onAnimationClipComplete(string name)
    {
        if (name == "done")
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }


    public bool done
    {
        set
        {
            _done = value;
        }
        get { return _done; }
    }

    public int imageId
    {
        set
        {
            _imageId = value;
            image.sprite = garffitiList[_imageId];
            imageBg.sprite = garffitiList[_imageId];

        }
        get { return _imageId; }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public enum GraffitiId
    {
        Graffiti1, Graffiti2, Graffiti3, Graffiti4, Graffiti5, Graffiti6
    }
}
