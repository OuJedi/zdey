using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Life : MonoBehaviour
{

    [SerializeField] private Image life;
    [SerializeField] private Image[] lifes;

    private int _value;

    // Start is called before the first frame update
    private void Awake()
    {
        life.enabled = false;
        for (int i = 0; i < lifes.Length; i++)
        {
            lifes[i].enabled = false;
        }
        lifes[lifes.Length - 1].enabled = true;
    }

    public int value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;

            if (_value <= lifes.Length - 1)
            {
                for (int i = 0; i < lifes.Length; i++)
                {
                    lifes[i].enabled = false;
                }
                lifes[_value].enabled = true;
                life.enabled = true;
                life.DOFade(1f, 0f);
                life.DOFade(0f, .3f).SetDelay(.25f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

}
