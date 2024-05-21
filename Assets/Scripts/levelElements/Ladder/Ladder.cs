using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if (UNITY_EDITOR)
[ExecuteInEditMode]
#endif

public class Ladder : MonoBehaviour
{

    [SerializeField] GameObject _top;
    [SerializeField] private float _TopPosition = 0f;
    // Start is called before the first frame update
    void Start()
    {
        if (Application.isPlaying)
        {
            _top.GetComponent<SpriteRenderer>().enabled = false;
        }
        _top.transform.localPosition = new Vector2(0f, _TopPosition - _top.transform.localScale.y / 2);
    }


    public GameObject top
    {
        get { return _top; }
    }

    // Update is called once per frame
    void Update()
    {

    }




#if (UNITY_EDITOR)

    private void OnGUI()
    {

        if (Application.isPlaying)
        {
            return;
        }

        if (_top.transform.position.y != _TopPosition)
        {
            _top.transform.localPosition = new Vector2(0f, _TopPosition - _top.transform.localScale.y / 2);
        }

    }
#endif
}
