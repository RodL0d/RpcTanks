using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D RbP;
    float speed = 5f;

    private void Start()
    {
        RbP = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        move();
    }

    void move()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            RbP.velocity = Vector2.up * speed;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            RbP.velocity = Vector2.down * speed;
        }
        else if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            RbP.velocity = Vector2.zero;
        }
    }
}
