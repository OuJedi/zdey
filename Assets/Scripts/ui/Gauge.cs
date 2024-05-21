using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Gauge : MonoBehaviour
{

    [SerializeField] private Image[] gauges;

    private float _value;

    // Start is called before the first frame update
    private void Awake()
    {
        value = 3;
    }

    public float value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;

            if (_value <= gauges.Length - 1)
            {
                for (int i = 0; i < gauges.Length; i++)
                {
                    gauges[i].enabled = false;
                }
                gauges[Mathf.RoundToInt(_value)].enabled = true;           
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

}
