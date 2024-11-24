using System.Collections;
using System.Collections.Generic;
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



    // Start is called before the first frame update
    void Start()
    {
        acc = maxSpeed / accTime;
        dec = maxSpeed / accTime;
        rb = GetComponent<Rigidbody2D>();
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

        else if (currentV.magnitude > 0)
        {
            currentV = currentV.normalized * Mathf.Clamp(Mathf.Abs(currentV.x) - dec * Time.deltaTime, 0, maxSpeed);
        }

        rb.velocity = currentV;
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
