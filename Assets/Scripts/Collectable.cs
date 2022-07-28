using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public GameObject itemFeedback;

    public void Die()
    {
        Instantiate(itemFeedback, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
