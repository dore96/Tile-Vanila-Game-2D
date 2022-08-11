using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovment : MonoBehaviour
{
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private float climSpeed = 5f;
    [SerializeField] private Vector2 deathKick = new Vector2(10f,10f);
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform gun;



    private Vector2 moveInput;
    private Rigidbody2D myRigidbody;
    private Animator myAnimator;
    private CapsuleCollider2D myBodyCollider;
    private BoxCollider2D myFeetCollider;
    private bool isMovingHoriz;
    private bool isMovingVert;
    private bool isAlive = true;
    private float gravityStart;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityStart = myRigidbody.gravityScale;
    }

    void Update()
    {
        if (!isAlive) { return;}
        Die();
        FlipSing();
        Run();
        ClimLadder();
    }

    void OnFire(InputValue value)
    {
        if (!isAlive) { return; }
        Instantiate(bullet, gun.position, transform.rotation);
    }

    void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!isAlive) { return; }
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return;}

        if (value.isPressed)
        {
            myRigidbody.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    void Run()
    {
        Vector2 playerVelocety = new Vector2(moveInput.x * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocety;

        myAnimator.SetBool("IsRunning", isMovingHoriz);
    }

    void FlipSing()
    {
        isMovingHoriz = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;

        if (isMovingHoriz)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }

    void ClimLadder()
    {

        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            myRigidbody.gravityScale = gravityStart;
            myAnimator.SetBool("IsCliming", false);
            return;
        }
        Vector2 climVelocety = new Vector2(myRigidbody.velocity.x, moveInput.y * climSpeed);
        myRigidbody.velocity = climVelocety;
        myRigidbody.gravityScale = 0f;

        isMovingVert = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("IsCliming", isMovingVert);

    }

    private void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Death");
            myRigidbody.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }

}
