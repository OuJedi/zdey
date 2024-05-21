
using UnityEngine;
using UnityEngine.Events;

public class Lever : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private GameObject leverOn;
    [SerializeField] private GameObject leverOff;

    private Animator leverOnAnimator;
    private Animator leverOffAnimator;
    private bool isActive = false;

    void Start()
    {
      

        leverOnAnimator = leverOn.GetComponent<Animator>();
        leverOffAnimator = leverOff.GetComponent<Animator>();
        leverOnAnimator.speed = 0f;
        leverOffAnimator.speed = 0f;
        leverOff.SetActive(false);
    }

    public void Action()
    {
        if (isActive)
        {
            leverOffAnimator.speed = PlayerAnimationSpeed.lever;
            leverOff.SetActive(true);
            leverOnAnimator.speed = 0f;
            leverOn.SetActive(false);
            isActive = false;
        }
        else
        {
            leverOnAnimator.speed = PlayerAnimationSpeed.lever;
            leverOn.SetActive(true);
            leverOffAnimator.speed = 0f;
            leverOff.SetActive(false);
            isActive = true;
        }

        LeverEventDispatcher.Instance.action(isActive, id);
    }

   


}
