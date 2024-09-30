using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D RbP;
    public GameObject bulletPrefab; // Prefab do projétil
    public Transform firePoint;     // Ponto de onde o projétil será disparado
    float speed = 5f;
    public float bulletSpeed = 10f; // Velocidade do projétil
    public float fireRate = 0.5f;   // Intervalo entre disparos
    private float nextFireTime = 0f;

    private void Start()
    {
        RbP = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        move();
    }

    void Update()
    {
        RotateTowardsMouse(); // Rotaciona o tanque em direção ao mouse

        // Atirar se pressionar a tecla espaço e o tempo de fogo permitir
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate; // Controla o tempo entre disparos
        }
    }

    void move()
    {
        Vector2 movement = Vector2.zero;

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

        RbP.velocity = movement * speed;
    }

    // Função para rotacionar o tanque em direção ao mouse
    void RotateTowardsMouse()
    {
        // Pegar a posição do mouse na tela e convertê-la para o mundo 2D
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized; // Calcula a direção do tanque ao mouse

        // Calcula o ângulo para rotacionar o tanque
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle); // Rotaciona o tanque para apontar para o mouse
    }

    void Fire()
    {
        // Instancia o projétil na posição do ponto de disparo
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Aplica velocidade ao projétil na direção do firePoint (que segue a direção do mouse)
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = firePoint.right * bulletSpeed; // Mova o projétil na direção que o tanque está apontando
    }
}
