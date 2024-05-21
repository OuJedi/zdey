
using UnityEngine;
using UnityEngine.Events;

public class LeverEventDispatcher : MonoBehaviour
{
    private static LeverEventDispatcher _instance;
    private UnityEvent<bool, int> onActionEvent;
    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        _instance = this;
        onActionEvent = new UnityEvent<bool, int>();
    }

    public static LeverEventDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("The Singlton is NULL.");
            }

            return _instance;
        }
    }

    public void action(bool isActive, int leverId)
    {
        onActionEvent.Invoke(isActive, leverId);
    }


    public UnityEvent<bool, int> OnActionEvent
    {
        get { return onActionEvent; }
    }


    private void OnDestroy()
    {
        Destroy(gameObject);
    }


}
