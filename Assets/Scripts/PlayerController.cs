using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum FacingDirection
    {
        left, right
    }
    FacingDirection direction;

    public float accTime;
    public float maxSpeed;
    public float decTime;

    public float ray;

    private float acc;
    private float dec;
    private Rigidbody2D rb;

    public float apexH;
    public float apexT;
    public float termSpeed;
    public float coyoteT;

    private float jumpGrv;
    private float grv;
    private float iniJumpV;
    private bool IsJump = false;
    [SerializeField]private bool IsCoyote = false;
    private bool onGround;

    // Start is called before the first frame update
    void Start()
    {
        acc = maxSpeed / accTime;
        dec = maxSpeed / accTime;
        rb = GetComponent<Rigidbody2D>();

        grv = rb.gravityScale;
        jumpGrv = -2 * apexH / Mathf.Pow(apexT, 2);
        iniJumpV = 2 * apexH / apexT;

        onGround = IsGrounded();

    }

    // Update is called once per frame
    void Update()
    {
        // The input from the player needs to be determined and
        // then passed in the to the MovementUpdate which should
        // manage the actual movement of the character.
        Vector2 playerInput = new Vector2(Input.GetAxis("Horizontal"), 0);
        MovementUpdate(playerInput);
    }

    
    private void MovementUpdate(Vector2 playerInput)
    {
        Vector2 currentV = rb.velocity;

        if (playerInput != Vector2.zero)
        {
            direction = playerInput.x < 0 ? FacingDirection.left : FacingDirection.right;
            currentV += acc * playerInput * Time.deltaTime;
            currentV.x = Mathf.Clamp(currentV.x, -maxSpeed, maxSpeed);
        }

        else if (Mathf.Abs(currentV.x )> 0)
        {
            currentV.x = currentV.normalized.x * Mathf.Clamp(Mathf.Abs(currentV.x) - dec * Time.deltaTime, 0, maxSpeed);
        }
        
        if (!IsGrounded() && onGround)
        {
            StartCoroutine(coyoteTime());
        }
        onGround = IsGrounded();

        if (Input.GetButton("Jump") && (IsGrounded()||IsCoyote))
        {
            IsJump = true;
            IsCoyote = false;
            rb.gravityScale = 0;
            currentV.y = iniJumpV;
            StartCoroutine(Falling());            
        }
        if (IsJump)
        {
            currentV.y += jumpGrv * Time.deltaTime;
        }

        currentV.y = Mathf.Clamp(currentV.y, -termSpeed, iniJumpV);

        rb.velocity = currentV;
    }
    IEnumerator Falling()
    {
        yield return new WaitForSeconds(apexT);
        rb.gravityScale = grv;
        IsJump = false;
    }
    IEnumerator coyoteTime()
    {
        IsCoyote = true;
        yield return new WaitForSeconds(coyoteT);
        IsCoyote = false;
    }

    public bool IsWalking()
    {
        return rb.velocity.x != 0;
    }
    public bool IsGrounded()
    {
        Debug.DrawRay(transform.position, Vector2.down * ray, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, ray);
        return hit.collider != null;
    }

    public FacingDirection GetFacingDirection()
    {
        return direction;
    }
}
