using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = -1;
    Rigidbody2D rb;
    public GameObject deathFeedback;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 v = new Vector2(speed, 0);
        rb.velocity = v;
    }

    void Flip()
    {
        speed *= -1;

        var s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
    }

    public void Die()
    {
        Instantiate(deathFeedback, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Flip();
    }
}
