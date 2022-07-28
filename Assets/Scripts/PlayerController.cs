using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb;
    bool isGrounded;
    SpriteRenderer spriteRenderer;

    public LayerMask whatIsGround;
    public float movementSpeed = 5;
    public Transform groudCheckPosition;
    public float jumpForce = 10;
    public GameObject endGameText;
    
    //Coyote Time
    public float hangTime = 0.3f;
    float hangCounter;

    //Jump Before Player Hits Ground
    public float jumpBufferLenght = 0.2f;
    private float jumpBufferCount;

    //Health
    public float health;
    public TMP_Text healthText;

    //Items
    private float items;
    public TMP_Text itemsText;
    public GameObject itemFeedback;

    //Fall faster afterjump
    public float fallMultiplier = 2.5f;

    //Sounds
    AudioSource audioSource;
    public AudioClip jumpClip, hurtClip, itemClip;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * movementSpeed, rb.velocity.y);

        isGrounded = Physics2D.OverlapCircle(groudCheckPosition.position, .1f, whatIsGround);

        anim.SetBool("grounded", isGrounded);
        //Handle Coyote Time
        if (isGrounded)
        {
            hangCounter = hangTime;
        }
        else
        {
            hangCounter -= Time.deltaTime;
        }

        //Handle Jump Buffer
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCount = jumpBufferLenght;
        }
        else
        {
            jumpBufferCount -= Time.deltaTime;
        }

        //Jump
        if(jumpBufferCount >= 0 && hangCounter > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpBufferCount = 0;
            anim.SetTrigger("jump");
            anim.SetBool("grounded", false);
            audioSource.clip = jumpClip;
            audioSource.Play();
        }

        if(Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        if(rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        if(Input.GetAxisRaw("Horizontal") > 0)
        {
            spriteRenderer.flipX = false;
            anim.SetBool("running", true);
        }
        else if(Input.GetAxisRaw("Horizontal") < 0)
        {
            spriteRenderer.flipX = true;
            anim.SetBool("running", true);
        }
        else
        {
            anim.SetBool("running", false);
        }
    }

    public void TakeDamage()
    {
        health--;

        healthText.text = "HP. " + health.ToString();

        Vector2 pushForce = new Vector2(-rb.velocity.x, -rb.velocity.y) * 1000;

        anim.SetTrigger("hurt");

        audioSource.clip = hurtClip;
        audioSource.Play();

        //rb.AddForce(pushForce, ForceMode2D.Force);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if(rb.velocity.y < 0)
            {
                collision.gameObject.GetComponent<EnemyController>().Die();
                audioSource.clip = itemClip;
                audioSource.Play();
            }
            else
            {
                TakeDamage();
            }
        }

        if (collision.gameObject.CompareTag("Item"))
        {
            items++;

            itemsText.text = items.ToString();

            audioSource.clip = itemClip;
            audioSource.Play();

            collision.gameObject.GetComponent<Collectable>().Die();
        }

        if (collision.gameObject.CompareTag("Finish"))
        {
            this.enabled = false;
            endGameText.SetActive(true);
        }
    }
}
