using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [SerializeField] private float m_CoyoteTimeTolerance = 0f;
    [SerializeField] private float m_CoyoteCoef = 1.2f;
    //[Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;   // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    private bool wasGrounded;            // Whether or not the player is grounded.
    private Rigidbody2D m_Rigidbody2D;
    //private BoxCollider2D m_BoxCollider2D;
    private PolygonCollider2D m_BoxCollider2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.

    public UnityEvent OnLandEvent;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
       // m_BoxCollider2D = GetComponent<BoxCollider2D>();
        m_BoxCollider2D = GetComponent<PolygonCollider2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

    }


    private void FixedUpdate()
    {

        m_Grounded = IsGrounded();
        if (!wasGrounded && m_Grounded)
        {
            OnLandEvent.Invoke();
        }
        wasGrounded = m_Grounded;

    }


    public bool IsGrounded()
    {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(m_BoxCollider2D.bounds.center, m_BoxCollider2D.bounds.size, 0f, Vector2.down, k_GroundedRadius, m_WhatIsGround);
        return raycastHit2d.collider != null;
    }


    public void Move(float move)
    {



        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {

            m_Rigidbody2D.velocity = new Vector2(move, m_Rigidbody2D.velocity.y);


            // If the input is moving the player right and the player is facing left...
            FlipCharacter(move);
        }


    }

    public void Jump(bool isJumping)
    {

        if (IsGrounded())
        {
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
        else
        {
            if (!isJumping && m_Rigidbody2D.velocity.y > m_CoyoteTimeTolerance)
            {
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce * m_CoyoteCoef));
            }
        }

    }


    public void FlipCharacter(float move)
    {
        if (move > 0 && !m_FacingRight)
        {
            Flip();
        }
        else if (move < 0 && m_FacingRight)
        {
            Flip();
        }
    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
