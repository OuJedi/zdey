using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventDispatcher : MonoBehaviour
{
    [SerializeField] private PlayerControler playerControler;
    public void onAnimationClipComplete(string name)
    {
        playerControler.onAnimationClipComplete(name);
    }
}
