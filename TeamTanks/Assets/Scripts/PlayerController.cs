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

    // FixedUpdate é ideal para manipulação de física
    void FixedUpdate()
    {
        move();
    }

    void move()
    {
        // Inicializar uma direção com zero
        Vector2 movement = Vector2.zero;

        // Verificar se W ou S estão sendo pressionados
        if (Input.GetKey(KeyCode.W))
        {
            movement += Vector2.up;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement += Vector2.down;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement += Vector2.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement += Vector2.right;
        }

        // Aplicar a velocidade ao rigidbody
        RbP.velocity = movement * speed;
    }
}

