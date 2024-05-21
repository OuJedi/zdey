using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretBarrel : MonoBehaviour
{
    [SerializeField] private GameObject barrelOpen;
    [SerializeField] private GameObject barrelClose;
    [SerializeField] private GameObject barrelTarget;
    [SerializeField] private BoxCollider2D collider;
    [SerializeField] private PolygonCollider2D _virtualCameraBoundingShape2D;

    private Animator openAnimator;
    private bool _isOpened;
    // Start is called before the first frame update
    void Start()
    {
        openAnimator = barrelOpen.GetComponent<Animator>();
    }

    public void Action()
    {
        barrelClose.SetActive(false);
        barrelOpen.SetActive(true);
        openAnimator.speed = PlayerAnimationSpeed.barrelOpen;
        openAnimator.Play("root.secretBarrel");
        collider.enabled = false;
        _isOpened = true;
    }

    public Transform barrelDestination
    {
        get { return barrelTarget.transform; }
    }

    public PolygonCollider2D virtualCameraBoundingShape2D
    {
        get { return _virtualCameraBoundingShape2D; }
    }

    public bool isOpened
    {
        get { return _isOpened; }
    }



}
