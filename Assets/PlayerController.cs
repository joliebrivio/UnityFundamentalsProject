using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(InputSystem))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider))]
public class PlayerController : MonoBehaviour
{
    // Inspector visible variables
    [SerializeField, Tooltip("Speed for left/right player movement")] 
    private float moveSpeed;
    [SerializeField, Tooltip("Speed of the player jump force.")]
    private float jumpSpeed;
    [SerializeField, Tooltip("Speed for the Dash.")]
    private float dashSpeed;
    [SerializeField, Tooltip("Cooldown for the Dash ability in seconds.")]
    private float dashCooldown;
    [SerializeField, Tooltip("The distance the Dash ability moves you.")]
    private float dashDistance;
    [SerializeField, Tooltip("The knockback force on player damage taken.")]
    private float knockbackForce;

    // Inspector invisible variables.
    private Rigidbody2D rb2D;
    private float movementDirection;
    private bool isGrounded;
    private bool isDashing;
    private bool canDash;
    private int deaths;

    public float MovementDirection { get => movementDirection; private set => movementDirection = value; }
    public float MoveSpeed { get => moveSpeed; private set => moveSpeed = value; }

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        MovementDirection = 0f;
        isGrounded = true;
        isDashing = false;
        canDash = true;
        deaths = 0;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        // Only allow movement when the player is Grounded and is not Dashing.
        if (!isGrounded || isDashing)
        {   
            return;
        }
        Debug.Log("Moving");
        rb2D.velocity = new Vector2(MovementDirection * MoveSpeed, rb2D.velocity.y);
    }

    public void OnMove(InputValue value)
    {
        // When a move key is pressed, get whether they are moving in a negative (left) or positive (right) direction.
        MovementDirection = value.Get<float>();
    }

    public void OnJump()
    {
        // Only allow jumping when the player is grounded.
        if (!isGrounded)
        {
            return;
        }
        Debug.Log("Jumping");
        rb2D.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
    }

    public void OnDash()
    {
        // Only allows a dash if it is off cooldown and the player has charges.
        // If the player is not moving, don't allow them to dash.
        if (!canDash || MovementDirection == 0f)
        {
            return;
        }
        _ = StartCoroutine(HandleDash());
        _ = StartCoroutine(HandleDashCooldown());
    }


    IEnumerator HandleDash()
    {
        // Get the initial position and the force of the dash.
        Vector2 startDashPos = transform.position;  
        Vector2 dashForce = new(MovementDirection * dashSpeed, rb2D.velocity.y);
        // Apply the force of the dash.
        rb2D.AddForce(dashForce, ForceMode2D.Impulse);
        isDashing = true;
        // Wait until the dash has covered the dash distance.
        yield return new WaitUntil(() => Vector2.Distance(startDashPos, transform.position) >= dashDistance);
        isDashing = false;
        // Apply the negative dash force to remove the force of the dash.
        rb2D.AddForce(-dashForce, ForceMode2D.Impulse);
    }
    IEnumerator HandleDashCooldown()
    {
        // Allows the player dash to be used again after a cooldown.
        canDash = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Ground collision");
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Enemy collision");
            _ = StartCoroutine(HandleDeath());
        }
    }

    private IEnumerator HandleDeath()
    {
        Time.timeScale = 0.0f;
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
