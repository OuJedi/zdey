
using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float jumpHightLimit = 14.0f;
    [SerializeField] private float maxVelocityLimit = 40.0f;
    [SerializeField] private float landingShowVelocityLimit = 30f;
    [SerializeField] private float gravityScale = 4.8f;
    [SerializeField] private CharacterController2D controller;
    [SerializeField] private GameObject animWrapper;
    [SerializeField] private GameObject windSprite;
    [SerializeField] private GameObject jumpDustSprite;
    [SerializeField] private GameObject impactSprite;
    [SerializeField] private GameObject sprayAttackPrefab;
    [SerializeField] private GameObject sprayText;
    [SerializeField] private LadderClimb ladderClimb;

    private Animator animator;
    private Animator wind;
    private Animator jumpDust;
    private Animator impact;
    private GameObject sprayAttack;
    private Rigidbody2D rb2d;
    private Transform parent;
    private float dirX;
    private bool isIdle = false;
    private bool isJumping = false;
    private bool isMoving = false;
    private bool isFalling = false;
    private bool isStopMoving = false;
    private bool isInvincible = false;
    private bool isAction = false;
    private bool jumpLandingShow = false;
    private bool isHited = false;
    private bool currentInteractiveElementGrounded = false;
    private bool isOnBorder = false;
    private bool isOnPlatform = false;
    private bool actionAccess = false;
    private bool spraying = false;
    private bool sprayEndAnimation = true;
    private bool sprayAttackStart = false;
    private bool graffitiStart = false;
    private string currentClip = "";
    private Tween animWrapperTween;
    private float blinkDuration = 20f;
    private float blinkDelay = 0.06f;
    private bool disposed = false;
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
    private CinemachineConfiner2D cinemachineConfiner2D;
    private PolygonCollider2D defaultVirtualCameraBoundingShape2D;
    private float cameraIntensity = 5;
    private GameObject currentLever;
    private GameObject currentGarbage;
    private GameObject currentBarrel;
    private GameObject originalBarrel;
    private GameObject currentDistributeur;
    private GameObject currentGraffitiRect;
    private Vector2 initImpactPos;
    private bool secreteZoneIn;
    private bool isLevelComplete;

    // temporairement, à bouger vers UI

    private int life = Config.lifeMaxValue;
    private int currentlife = Config.lifeMaxValue;
    private float currentGauge = Config.gaugeMaxValue;
    private Coroutine idleRoutine;
    private bool isSprayDown;
    private bool isTeleporting;
    private bool barrelOpenProcess;
    private GameObject currentTeleport;
    private bool overstepProcess;

    private void Awake()
    {
        animator = animWrapper.GetComponentInChildren<Animator>();
        wind = windSprite.GetComponentInChildren<Animator>();
        jumpDust = jumpDustSprite.GetComponentInChildren<Animator>();
        impact = impactSprite.GetComponentInChildren<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        parent = transform.parent;
        windSprite.SetActive(false);
        jumpDustSprite.SetActive(false);
        impactSprite.SetActive(false);
        sprayText.SetActive(false);
        initImpactPos = impactSprite.transform.localPosition;
        cinemachineVirtualCamera = FindFirstObjectByType<CinemachineVirtualCamera>();
        cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineConfiner2D = cinemachineVirtualCamera.GetComponent<CinemachineConfiner2D>();
        defaultVirtualCameraBoundingShape2D = (PolygonCollider2D)cinemachineConfiner2D.m_BoundingShape2D;

    }

    private void Start()
    {
        initEvents();
    }

    void initEvents()
    {
        controller.OnLandEvent.AddListener(onLandEvent);
        InputControl.Instance.OnInteractionEvent.AddListener(onInteractionEvent);
        ladderClimb.onStartClimbing.AddListener(onStartClimbing);
        ladderClimb.onStopClimbing.AddListener(onStopClimbing);
        ladderClimb.onStopMoving.AddListener(onStopMoving);
        ladderClimb.onStartMoving.AddListener(onStartMoving);
    }

    void onLandEvent()
    {
        if (jumpLandingShow)
        {
            jumpLanding();
        }
        else
        {
            isJumping = false;
        }
    }



    private void onInteractionEvent(string type, float value)
    {

        if (teleporting || isLevelComplete)
        {
            return;
        }

        switch (type)
        {
            case "HorizontalDown":

                break;

            case "HorizontalUp":

                rb2d.velocity = new Vector2(0f, rb2d.velocity.y);

                break;

            case "VerticalDown":

                break;

            case "VerticalUp":

                break;

            case "JumpDown":

                if (!isStopMoving)
                {
                    if (controller.IsGrounded() || currentInteractiveElementGrounded)
                    {
                        playClip("jump", value != -1);
                    }
                    controller.Jump(isJumping);
                    isJumping = true;
                    ladderClimb.isClimbing = false;
                }

                break;

            case "ActionDown":

                if (sprayEndAnimation && currentLever)
                {
                    action();
                    playClip("kick1");
                }


                //if (currentGarbage && Tools.Instance.IsFacingTarget(gameObject, currentGarbage))
                if (sprayEndAnimation && currentGarbage)
                {
                    action();
                    playClip("kick1");
                }


                // if (currentDistributeur && Tools.Instance.IsFacingTarget(gameObject, currentDistributeur))
                if (sprayEndAnimation && currentDistributeur)
                {
                    action();
                    playClip("kick1");
                }



                if (sprayEndAnimation && actionAccess && currentBarrel && Tools.Instance.IsFacingTarget(gameObject, currentBarrel))
                {
                    action();
                    playClip("barrelOpen");
                    barrelOpenProcess = true;
                }

                if (sprayEndAnimation && currentTeleport)
                {
                    Teleport teleport = currentTeleport.GetComponent<Teleport>();
                    teleport.action(GetComponent<Collider2D>());
                    currentTeleport = null;
                }

                break;



            case "SprayDown":

                isSprayDown = true;

                if (
                    !isFalling &&
                    !isJumping &&
                    !ladderClimb.isClimbing &&
                    !teleporting &&
                    !barrelOpenProcess &&
                    !overstepProcess
                    )
                {
                    spraying = true;
                    isStopMoving = true;
                    rb2d.velocity = new Vector2();

                    if (UiManager.Instance.gauge.value > 0)
                    {
                        if (currentGraffitiRect)
                        {
                            Debug.Log("Graffiti action");
                            playClip("graffiti");
                            sprayText.transform.localScale = new Vector3(transform.localScale.x, 1f, 1f);
                            sprayText.SetActive(true);
                        }
                        else
                        {
                            playClip("spray");
                        }

                    }
                    else
                    {
                        playClip("noSpray");
                    }

                }
                break;


            case "SprayUp":

                isSprayDown = false;

                if (
                    (sprayEndAnimation || graffitiStart) &&
                    !teleporting &&
                    !barrelOpenProcess &&
                    !overstepProcess
                    )
                {
                    disableSprying();
                }



                break;


            default: break;
        }

    }



    void action()
    {
        isAction = true;
        isStopMoving = true;
        rb2d.velocity = new Vector2();
    }



    void onStartClimbing()
    {
        isJumping = false;
    }

    void onStopClimbing()
    {
        jumpLandingShow = false;
        if (isJumping)
        {
            playClip("jump", false);
        }

    }


    void onStopMoving()
    {
        if (currentClip == "ladder")
        {
            animator.speed = 0f; // for pause animator
        }

    }

    void onStartMoving()
    {
        if (currentClip != "ladder")
        {
            playClip("ladder");
        }
        else
        {
            animator.speed = PlayerAnimationSpeed.ladder;
        }
    }



    void idle()
    {
        isIdle = true;
        animator.speed = PlayerAnimationSpeed.idle;
        animator.Play("playerAnimations.idle", 0, 0);
    }

    void stand()
    {
        animator.speed = PlayerAnimationSpeed.stand;
        animator.Play("playerAnimations.stand", 0, 0);

        isIdle = false;
        Tools.Instance.KillDelay(idleRoutine);
        idleRoutine = Tools.Instance.Delay(Config.idleDelay, () =>
        {
            playClip("idle");
        });
    }

    void jump(bool showDust)
    {
        animator.speed = PlayerAnimationSpeed.jump;
        animator.Play("playerAnimations.jump", 0, 0);

        if (showDust)
        {
            jumpDust.speed = PlayerAnimationSpeed.jumpDust;
            jumpDust.Play("root.jumpDust", 0, 0);
            jumpDustSprite.SetActive(true);
            jumpDustSprite.transform.position = new Vector3(transform.position.x, transform.position.y - .8f, transform.position.z);
            jumpDustSprite.transform.parent = transform.parent;

        }
    }



    void fall()
    {
        animator.speed = PlayerAnimationSpeed.fall;
        animator.Play("playerAnimations.fall", 0, 0);
    }

    void ladder()
    {
        animator.speed = PlayerAnimationSpeed.ladder;
        animator.Play("playerAnimations.ladder", 0, 0);
    }

    void kick0()
    {
        animator.speed = PlayerAnimationSpeed.kick0;
        animator.Play("playerAnimations.kick0", 0, 0);
    }

    void kick1()
    {
        animator.speed = PlayerAnimationSpeed.kick1;
        animator.Play("playerAnimations.kick1", 0, 0);
    }

    void jumpLanding()
    {
        ladderClimb.activate = false;
        animator.speed = PlayerAnimationSpeed.jumpLanding;
        animator.Play("playerAnimations.jumpLanding", 0, 0);
        isStopMoving = true;
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = cameraIntensity;
        DOTween.To(() => cinemachineBasicMultiChannelPerlin.m_AmplitudeGain, x => cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = x, 0f, 1f).onComplete = () =>
        {
            ladderClimb.activate = true;
            isStopMoving = false;
        };
        rb2d.velocity = new Vector2(rb2d.velocity.x * .5f, rb2d.velocity.y);
    }



    void run()
    {
        animator.speed = PlayerAnimationSpeed.run;
        animator.Play("playerAnimations.run", 0, 0);

        wind.speed = PlayerAnimationSpeed.wind;
        wind.Play("root.wind", 0, 0);
        windSprite.SetActive(true);
        windSprite.transform.position = new Vector3(transform.position.x, transform.position.y - .8f, transform.position.z);
        windSprite.transform.parent = transform.parent;
    }



    void dead()
    {
        animator.speed = PlayerAnimationSpeed.dead;
        animator.Play("playerAnimations.dead", 0, 0);
    }

    void dead2()
    {
        animator.speed = PlayerAnimationSpeed.dead2;
        animator.Play("playerAnimations.dead2", 0, 0);
    }

    void hit()
    {
        animator.speed = PlayerAnimationSpeed.hit;
        animator.Play("playerAnimations.hit", 0, 0);
    }

    void equilibrium()
    {
        animator.speed = PlayerAnimationSpeed.equilibrium;
        animator.Play("playerAnimations.equilibrium", 0, 0);
    }

    void barrelOpen()
    {
        animator.speed = PlayerAnimationSpeed.barrelZdeyOpen;
        animator.Play("playerAnimations.barrelOpen", 0, 0);
    }

    void barrelOn()
    {
        animator.speed = PlayerAnimationSpeed.barrelZdeyOn;
        animator.Play("playerAnimations.barrelOn", 0, 0);
    }

    void barrelOut()
    {
        animator.speed = PlayerAnimationSpeed.barrelZdeyOut;
        animator.Play("playerAnimations.barrelOut", 0, 0);
    }

    void spray()
    {
        sprayEndAnimation = false;
        animator.speed = PlayerAnimationSpeed.startSpray;
        animator.Play("playerAnimations.spray", 0, 0);
    }
    void noSpray()
    {
        sprayEndAnimation = false;
        animator.speed = PlayerAnimationSpeed.spray;
        animator.Play("playerAnimations.noSpray", 0, 0);
    }
    void graffiti()
    {
        graffitiStart = true;
        sprayEndAnimation = false;
        animator.speed = PlayerAnimationSpeed.graffiti;
        animator.Play("playerAnimations.graffiti", 0, 0);
    }

    void overstep()
    {
        animator.speed = PlayerAnimationSpeed.overstep;
        animator.Play("playerAnimations.overstep", 0, 0);
    }



    public void onAnimationClipComplete(string name)
    {

        switch (name)
        {

            case "wind":
                windSprite.SetActive(false);
                break;


            case "jumpDust":
                jumpDustSprite.SetActive(false);
                break;

            case "jumpLanding":
                isJumping = false;
                jumpLandingShow = false;
                isFalling = false;
                isStopMoving = false;
                currentClip = null;
                stand();
                ladderClimb.activate = true;
                break;

            case "dead":
                animWrapperTween?.Kill();
                animWrapper.SetActive(false);
                Tools.Instance.Delay(1, () =>
                {
                    if (secreteZoneIn)
                    {
                        barrelOnTransition(secreteZoneIn);
                        secreteZoneIn = false;
                        life = currentlife;
                        UiManager.Instance.life.value = life;
                        Debug.Log("secreteZone Out life: " + life);

                        currentGauge = UiManager.Instance.gauge.value;
                        UiManager.Instance.gauge.value = Config.gaugeMaxValue;
                    }
                    else
                    {
                        GameControl.Instance.Continue();
                    }
                });
                break;

            case "kick":

                isAction = false;
                isStopMoving = false;

                break;

            case "kickHit":

                impact.speed = PlayerAnimationSpeed.impact;
                impact.Play("root.impact", 0, 0);
                impactSprite.SetActive(true);
                impactSprite.transform.localPosition = initImpactPos;
                impactSprite.transform.parent = transform.parent;

                if (currentLever) { currentLever.GetComponent<Lever>().Action(); }
                if (currentGarbage) { currentGarbage.GetComponent<Garbage>().Action(); }
                if (currentDistributeur) { currentDistributeur.GetComponent<Distributeur>().Action(); }

                break;

            case "impact":
                impactSprite.SetActive(false);
                impactSprite.transform.parent = transform;
                break;

            case "barrelOpen":
                Debug.Log("onAnimationClipComplete barrelOpen");
                if (currentBarrel) { currentBarrel.GetComponent<SecretBarrel>().Action(); }
                break;

            case "barrelOn":
                Debug.Log("onAnimationClipComplete barrelOn");

                if (!secreteZoneIn)
                {
                    actionAccess = true;
                    secreteZoneIn = true;
                    currentlife = life;
                    life = Config.lifeMaxValue;
                    UiManager.Instance.life.value = life;
                    Debug.Log("secreteZone In");

                    currentGauge = UiManager.Instance.gauge.value;
                    UiManager.Instance.gauge.value = Config.gaugeMaxValue;

                }
                else
                {
                    secreteZoneIn = false;
                    life = currentlife;
                    UiManager.Instance.life.value = life;

                    UiManager.Instance.gauge.value = currentGauge;
                }

                barrelOnTransition();

                break;

            case "barrelOut":
                Debug.Log("onAnimationClipComplete barrelOut");

                barrelOpenProcess = false;
                isAction = false;
                isStopMoving = false;

                break;

            case "spray":

                Debug.Log("onAnimationClipComplete spray");

                animator.speed = PlayerAnimationSpeed.sprayLoop;
                sprayAttackStart = true;


                break;

            case "sprayLoop":

                Debug.Log("onAnimationClipComplete sprayLoop");

                sprayEndAnimation = true;

                if (!isSprayDown || graffitiStart)
                {
                    disableSprying();
                }




                break;

            case "noSpray":

                Debug.Log("onAnimationClipComplete noSpray");

                sprayEndAnimation = true;

                break;


            case "sprayAttackStart":

                Debug.Log("onAnimationClipComplete sprayAttackStart");

                sprayAttack = Instantiate(sprayAttackPrefab);
                sprayAttack.transform.position = new Vector3(transform.position.x - 1.4f * transform.localScale.x, transform.position.y + 2.2f, transform.position.z);
                sprayAttack.transform.localScale = new Vector3(transform.localScale.x, 1f, 1f);
                sprayAttack.transform.parent = parent;

                wind.speed = PlayerAnimationSpeed.wind;
                wind.Play("root.wind", 0, 0);
                windSprite.SetActive(true);
                windSprite.transform.position = new Vector3(transform.position.x - .5f * transform.localScale.x, transform.position.y - .8f, transform.position.z);
                windSprite.transform.parent = transform.parent;

                break;

            case "graffiti":

                Debug.Log("onAnimationClipComplete graffiti");

                sprayEndAnimation = true;

                break;

            case "idle":

                Debug.Log("onAnimationClipComplete idle");

                currentClip = null;
                Tools.Instance.KillDelay(idleRoutine);
                idleRoutine = Tools.Instance.Delay(Config.idleDelay, () =>
                {
                    playClip("idle");
                });

                break;


            case "overstep":

                Debug.Log("onAnimationClipComplete overstep");

                isAction = false;

                transform.position = new Vector2(transform.position.x + 1 * transform.localScale.x, transform.position.y + 1);

                isStopMoving = false;

                overstepProcess = false;

                rb2d.gravityScale = gravityScale;

                break;

        }


    }


    void disableSprying()
    {
        spraying = false;
        isAction = false;
        isStopMoving = false;
        sprayAttackStart = false;
        graffitiStart = false;
        sprayText.SetActive(false);
    }


    void stopSpraying()
    {
        Debug.Log("stopSpraying");

        sprayText.SetActive(false);
        playClip("stand");
        spraying = false;
        isStopMoving = false;

    }




    void barrelOnTransition(bool fromSecreteZone = false)
    {

        GameObject barrelDestination = fromSecreteZone ? originalBarrel : currentBarrel;

        PostProcessingControl.Instance.blackFadeIn(() =>
        {
            Vector2 pos = fromSecreteZone ? barrelDestination.transform.position : barrelDestination.GetComponent<SecretBarrel>().barrelDestination.position;
            PolygonCollider2D virtualCameraBoundingShape2D = barrelDestination.GetComponent<SecretBarrel>().virtualCameraBoundingShape2D;
            Debug.Log("virtualCameraBoundingShape2D:" + virtualCameraBoundingShape2D);
            cinemachineConfiner2D.m_BoundingShape2D = fromSecreteZone ? defaultVirtualCameraBoundingShape2D : virtualCameraBoundingShape2D;
            transform.position = new Vector2(pos.x - 1.6f * Mathf.Sign(transform.localScale.x), pos.y);

            PostProcessingControl.Instance.blackFadeOut(() =>
            {
                isInvincible = false;
                isHited = false;
                animWrapper.SetActive(true);
                playClip("barrelOut");
                currentBarrel = null;
            }, .5f);
        });
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {

        //  traitement des plans obliques
        if (collision.gameObject.CompareTag("Oblic"))
        {
            freeze();
        }
        else
        {
            rb2d.gravityScale = gravityScale;
        }

        if (collision.gameObject.CompareTag("Platform"))
        {
            transform.parent = collision.gameObject.transform;
            isOnPlatform = true;
        }

        if (collision.gameObject.CompareTag("Pigeon"))
        {
            if (controller.IsGrounded())
            {
                impact.speed = PlayerAnimationSpeed.impact;
                impact.Play("root.impact", 0, 0);
                impactSprite.SetActive(true);
                impactSprite.transform.localPosition = new Vector2(0f, -0.8f);
                impactSprite.transform.parent = transform.parent;

                Tools.Instance.Delay(.05f, () =>
                {
                    onInteractionEvent("JumpDown", -1);
                });

                collision.gameObject.GetComponent<Pigeon>().Fall();
            }
        }

        if (collision.gameObject.CompareTag("Garbage") || collision.gameObject.CompareTag("SecretZone") || collision.gameObject.CompareTag("Distributeur"))
        {
            onLandEvent();
            currentInteractiveElementGrounded = true;
        }



    }



    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Oblic"))
        {
            rb2d.gravityScale = gravityScale;
        }

        if (collision.gameObject.CompareTag("Platform"))
        {
            transform.parent = parent;
            isOnPlatform = false;
        }

        if (collision.gameObject.CompareTag("Garbage") || collision.gameObject.CompareTag("SecretZone") || collision.gameObject.CompareTag("Distributeur"))
        {
            currentInteractiveElementGrounded = false;
        }

    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeadZone"))
        {
            gameOver(true);
        }

        if (collision.CompareTag("Ladder"))
        {
            ladderClimb.onLadderTriggerEnter(collision);
        }

        if (collision.CompareTag("Lever"))
        {
            currentLever = collision.gameObject;
        }

        if (collision.CompareTag("Garbage"))
        {
            currentGarbage = collision.gameObject;
        }

        if (collision.CompareTag("Distributeur"))
        {
            currentDistributeur = collision.gameObject;
        }

        if (collision.CompareTag("SecretZone"))
        {
            originalBarrel = currentBarrel = collision.gameObject.GetComponent<SecretBarrel>().isOpened ? null : collision.gameObject;
            actionAccess = true;
        }

        if (collision.CompareTag("EndZone"))
        {
            levelComplete();
        }

        if (collision.CompareTag("GraffitiRect"))
        {
            Debug.Log("GraffitiRect IN");

            currentGraffitiRect = !collision.gameObject.GetComponent<GraffitiRect>().done ? collision.gameObject : null;

        }

        if (collision.CompareTag("Teleport"))
        {
            Debug.Log("Teleport IN");
            currentTeleport = collision.gameObject;
        }

        if (collision.CompareTag("OverstepZone") && Tools.Instance.IsFacingTarget(gameObject, collision.gameObject) && !isFalling)
        {
            Debug.Log("OverstepZone IN");
            rb2d.gravityScale = 0;
            overstepProcess = true;
            action();
            playClip("overstep");

        }


    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            ladderClimb.onLadderTriggerExit(collision);
        }

        if (collision.CompareTag("Lever"))
        {
            currentLever = null;
        }

        if (collision.CompareTag("Garbage"))
        {
            currentGarbage = null;
        }

        if (collision.CompareTag("Distributeur"))
        {
            currentDistributeur = null;
        }

        if (collision.CompareTag("BorderZone"))
        {
            isOnBorder = false;
        }

        if (collision.CompareTag("SecretZone"))
        {
            actionAccess = false;
        }

        if (collision.CompareTag("GraffitiRect"))
        {
            Debug.Log("GraffitiRect OUT");
            currentGraffitiRect = null;
        }

        if (collision.CompareTag("Teleport"))
        {
            Debug.Log("Teleport OUT");

            currentTeleport = null;

        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.CompareTag("Ladder"))
        {
            ladderClimb.onLadderTriggerStay(collision);
        }

        if (collision.CompareTag("BarbWire"))
        {
            onHit();
        }

        if (collision.CompareTag("BorderZone"))
        {
            if (!isMoving && !isOnPlatform)
            {
                playClip("equilibrium");
                isOnBorder = true;
            }
        }

    }





    void playClip(string name, bool arg = true)
    {

        // Debug.Log("playClip:" + name);

        if (currentClip == name)
        {
            return;
        }

        currentClip = name;

        Tools.Instance.KillDelay(idleRoutine);
        isIdle = false;

        switch (name)
        {
            case "idle":
                idle();
                break;

            case "run":
                run();
                break;

            case "jump":
                jump(arg);
                break;

            case "fall":
                fall();
                break;

            case "stand":
                stand();
                break;

            case "dead":
                dead();
                break;

            case "dead2":
                dead2();
                break;

            case "ladder":
                ladder();
                break;

            case "kick0":
                kick0();
                break;

            case "kick1":
                kick1();
                break;

            case "hit":
                hit();
                break;

            case "equilibrium":
                equilibrium();
                break;

            case "barrelOpen":
                barrelOpen();
                break;

            case "barrelOn":
                barrelOn();
                break;

            case "barrelOut":
                barrelOut();
                break;

            case "spray":
                spray();
                break;

            case "noSpray":
                noSpray();
                break;

            case "graffiti":
                graffiti();
                break;

            case "overstep":
                overstep();
                break;

        }


    }



    void freeze(float x = 0f, float y = 0f)
    {
        rb2d.gravityScale = 0;
        rb2d.velocity = new Vector2(x, y);
    }




    void gameOver(bool byFalling = false)
    {
        Debug.Log("game over!");

        freeze(0f, rb2d.velocity.y);

        // by falling
        if (byFalling)
        {
            DOTween.To(() => rb2d.velocity, x => rb2d.velocity = x, new Vector2(0, 0), .5f).onComplete = () =>
            {
                if (!secreteZoneIn)
                {
                    dispose();
                }

                float downWrapperPos = 0f;
                float upWrapperPos = 0f;
                float delayTime = 0f;

                if (animWrapper.transform.position.y > 0)
                {
                    downWrapperPos = -8f;
                    upWrapperPos = 10f;
                    delayTime = .4f;
                }
                else
                {
                    downWrapperPos = 0f;
                    upWrapperPos = -animWrapper.transform.position.y * 2f;
                }

                animWrapperTween = animWrapper.transform.DOMoveY(animWrapper.transform.position.y + downWrapperPos, .3f);
                animWrapperTween.onComplete = () =>
                {
                    playClip("dead");
                    animWrapperTween = animWrapper.transform.DOMoveY(animWrapper.transform.position.y + upWrapperPos, 2f).SetDelay(delayTime);
                };


            };
        }
        else
        {
            if (!secreteZoneIn)
            {
                dispose();
            }
            playClip("dead2");
        }
    }




    void levelComplete()
    {
        isLevelComplete = true;
        stopMoving = true;
        playAnim("stand");

        Tools.Instance.Delay(.5f, () =>
        {
            transform.DOMoveX(transform.position.x + 2f, .5f);
            playAnim("run");
            GameControl.Instance.nextlevel();
        });

    }




    void onHit()
    {

        if (isInvincible)
        {
            return;
        }

        jumpLandingShow = false;
        isHited = true;
        isStopMoving = true;
        isInvincible = true;
        rb2d.velocity = new Vector2(0f, rb2d.velocity.y);
        playClip("hit");
        life--;
        UiManager.Instance.life.value = life;

        Tools.Instance.Delay(.5f, () =>
        {
            if (life > 0)
            {
                isHited = false;
                isStopMoving = false;
                StartCoroutine(blink());
            }
            else
            {
                gameOver();
            }
        });






    }



    IEnumerator blink()
    {
        float elapsedTime = 0f;
        bool visible = false;
        SpriteRenderer sprite = animWrapper.GetComponent<SpriteRenderer>();

        while (elapsedTime < blinkDuration)
        {
            sprite.enabled = visible;

            elapsedTime += 1;
            visible = !visible;
            yield return new WaitForSeconds(blinkDelay);
        }

        sprite.enabled = true;
        isInvincible = false;
        rb2d.velocity = new Vector2(rb2d.velocity.x + .0001f, rb2d.velocity.y); //provoquer le "OnTriggerStay2D"
    }







    private void Update()
    {

        if (disposed) return;

        if (!isStopMoving && !teleporting)
        {

            dirX = InputControl.Instance.dirX * moveSpeed;

            if (dirX != 0 && !ladderClimb.isClimbing)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }

            if (rb2d.velocity.y < -0.1f)
            {
                isFalling = true;
            }
            else
            {
                isFalling = false;
            }


            if (isMoving && !isFalling && !isJumping)
            {
                playClip("run");
            }

            if (isFalling)
            {
                if (!ladderClimb.isClimbing)
                {
                    playClip("fall");
                }
            }


            if (
                !isMoving &&
                !isJumping &&
                !isFalling &&
                !disposed &&
                !ladderClimb.isClimbing &&
                !isAction &&
                !isHited &&
                !isOnBorder &&
                !spraying &&
                !isIdle
                )
            {
                playClip("stand");
                isStopMoving = false;
            }


            // limit le saut
            if (rb2d.velocity.y > jumpHightLimit)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, jumpHightLimit);
            }

            windSprite.transform.localScale = transform.localScale;

        }

        if (spraying && UiManager.Instance.gauge.value > 0)
        {
            if (sprayAttackStart || graffitiStart)
            {
                if (!Config.infinitGauge)
                {
                    UiManager.Instance.gauge.value -= Config.sprayIncValue;
                }
            }


            if (currentGraffitiRect)
            {
                GraffitiRect graffitiRect = currentGraffitiRect.GetComponent<GraffitiRect>();
                if (!graffitiRect.done)
                {
                    graffitiRect.progress();
                }
                else
                {
                    currentGraffitiRect = null;
                    stopSpraying();
                }
            }
        }
        else
        {
            if (spraying && sprayEndAnimation)
            {
                stopSpraying();
            }
        }





    }



    private void FixedUpdate()
    {


        if (disposed || isStopMoving || ladderClimb.isClimbing || teleporting)
        {
            if (ladderClimb.isClimbing)
            {
                controller.FlipCharacter(dirX);
            }
            return;
        }

        controller.Move(dirX * Time.deltaTime);

        //Limiter la vitesse de la chute libre
        if (Mathf.Abs(rb2d.velocity.y) >= landingShowVelocityLimit && !jumpLandingShow)
        {
            jumpLandingShow = true;
        }

        //Limiter la vitesse de la chute libre
        if (Mathf.Abs(rb2d.velocity.y) > maxVelocityLimit)
        {
            float sign = Mathf.Sign(rb2d.velocity.y);
            rb2d.velocity = new Vector2(rb2d.velocity.x, sign * maxVelocityLimit);
        }


    }

    public bool stopMoving
    {
        get { return isStopMoving; }
        set
        {
            isStopMoving = value;
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            playClip("stand");
        }
    }

    public bool teleporting
    {
        get { return isTeleporting; }
        set
        {
            rb2d.velocity = Vector2.zero;
            isStopMoving = value;
            isTeleporting = value;
            if (value == false)
            {
                //  playClip("stand");
            }

        }
    }

    public void playAnim(string name)
    {
        playClip(name);
    }

    public void flip(float dirX)
    {
        controller.FlipCharacter(dirX);
    }




    public void dispose()
    {
        disposed = true;
        controller.OnLandEvent.RemoveListener(onLandEvent);
        InputControl.Instance.OnInteractionEvent.RemoveListener(onInteractionEvent);

        ladderClimb.onStartClimbing.RemoveListener(onStartClimbing);
        ladderClimb.onStopClimbing.RemoveListener(onStopClimbing);
        ladderClimb.onStopMoving.RemoveListener(onStopMoving);
        ladderClimb.onStartMoving.RemoveListener(onStartMoving);
        ladderClimb.dispose();

    }


}
