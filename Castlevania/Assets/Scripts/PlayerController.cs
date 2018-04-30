using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum OnGround { onPlatform, onSlope, onAir}

public class PlayerController : MonoBehaviour {

    public float speed = 6f;
    public float jumpForce = 20f;
    public float jumpForceMin = 5f;
    public float checkRadius = 0.5f;
    private bool isJump = false;

    private Rigidbody2D rb2d;
    private float tempGravity;

    private SpriteRenderer spr;
    private float width;

    private Animator anim;
    private bool isLeft = false;
    private float move;
    private Vector2 movenment = Vector2.zero;
    //private bool jump = false;

    private OnGround onGround; 
    private Ray2D ray;
    private Ray2D ray2;
    private RaycastHit2D hit;
    private RaycastHit2D hit2;
    public LayerMask whatIsGround;

    private bool isAttack = false;
    // Use this for initialization
    void Start ()
    {
        spr = GetComponent<SpriteRenderer>();
        width = spr.sprite.rect.size.x / (2*spr.sprite.pixelsPerUnit);

        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        tempGravity = rb2d.gravityScale;
    }
	
	// Update is called once per frame
	void Update ()
    {
        onGround = IsGrounded();

        if(!isAttack)
            Movenment();

            Jumping();

        if(Input.GetKeyDown(KeyCode.X) && !isAttack)
        {
            isAttack = true;
            StartCoroutine(OnAttack());

            if (onGround != OnGround.onAir)
            {
                rb2d.velocity = Vector2.zero;
            }
        }

        if(onGround == OnGround.onSlope)
        {
            rb2d.gravityScale = 0;
        }
        else
        {
            if (rb2d.gravityScale == 0)
                rb2d.gravityScale = tempGravity;
        }
        
    }

    void FixedUpdate()
    {
        anim.SetFloat("Speed", Mathf.Abs(rb2d.velocity.x));
        anim.SetFloat("vSpeed", rb2d.velocity.y);
        anim.SetBool("isAttack", isAttack);
        if (onGround == OnGround.onAir)
            anim.SetBool("isGround", false);
        else
            anim.SetBool("isGround", true);
    }

    IEnumerator OnAttack()
    {
        yield return new WaitForSeconds(0.35f);
        isAttack = false;
        //yield return new WaitForSeconds(0.08f);
        yield return new WaitForSeconds(0.05f);
    }

    void Movenment()
    {
        move = Input.GetAxis("Horizontal");
        if (move > 0 && isLeft)
            Flip();
        else
            if (move < 0 && !isLeft)
            Flip();
        /*if (onGround == OnGround.onPlatform || onGround == OnGround.onAir)
        {
            movenment = new Vector2(move * speed, rb2d.velocity.y);
        }
        else*/
        if(onGround != OnGround.onAir && !isJump)
        {
            movenment = ((hit2.point - hit.point) / (hit2.point - hit.point).magnitude).normalized;
            movenment = new Vector2(movenment.x  , movenment.y) * speed * Mathf.Abs(move);
            if (move == 0)
                movenment = Vector2.zero;
            //print(movenment);
        }
        else
            movenment = new Vector2(move * speed, rb2d.velocity.y);
        rb2d.velocity = movenment; // new Vector2(movenment.x, movenment.y);

    }

    void Jumping()
    {
        if (Input.GetKeyDown(KeyCode.Z) && onGround != OnGround.onAir)
        {
            //print("Jump");
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
            onGround = OnGround.onAir;
            isJump = true;
            StartCoroutine(OnWait());
        }
        else
    if (Input.GetKeyUp(KeyCode.Z))
        {
            //print("no jump");
            if (rb2d.velocity.y > 0 && rb2d.velocity.y <= jumpForce)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForceMin);
            }
        }
    }

    void Flip()
    {
        //меняем направление движения персонажа
        isLeft = !isLeft;
        //получаем размеры персонажа
        Vector3 theScale = transform.localScale;
        //зеркально отражаем персонажа по оси Х
        theScale.x *= -1;
        //задаем новый размер персонажа, равный старому, но зеркально отраженный
        transform.localScale = theScale;

    }

    private OnGround IsGrounded()
    {
        ray = new Ray2D(transform.position, Vector2.down);
        hit = Physics2D.Raycast(ray.origin, ray.direction, checkRadius, whatIsGround);
        ray2 = new Ray2D(new Vector2(transform.position.x + (width-0.2f)*transform.localScale.x, transform.position.y+0.3f), Vector2.down);
        hit2 = Physics2D.Raycast(ray2.origin, ray2.direction, checkRadius*5f, whatIsGround);

        Debug.DrawRay(ray.origin, ray.direction * checkRadius);
        Debug.DrawRay(ray2.origin, ray2.direction * checkRadius*5f);

        Debug.DrawRay(hit.point, hit.normal * checkRadius*5f);
        Debug.DrawRay(hit2.point, hit2.normal * checkRadius * 5f);

        if (hit && hit2)
        {
            if (hit.normal != Vector2.up || hit2.normal != Vector2.up)
            {
                //Debug.Log("OnSlope");
                //Debug.Log(hit.normal.ToString());
                //isJump = false;
                return OnGround.onSlope;
            }
            else
            if (hit.normal == Vector2.up &&  hit2.normal == Vector2.up && (hit.point - new Vector2(transform.position.x, transform.position.y)).magnitude < 0.08f)
            {
                //Debug.Log("OnPlatform");
                //Debug.Log((hit.transform.position - transform.position).magnitude);
                //isJump = false;
                return OnGround.onPlatform;
                
            }
            else return OnGround.onAir;
        }
        else
        {
            //Debug.Log("OnAir");
            return OnGround.onAir;
        }

    }


    IEnumerator OnWait()
    {
        yield return new WaitForSeconds(0.1f);
        isJump = false;
    }
}
