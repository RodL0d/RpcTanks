using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D RbP;
    public GameObject bulletPrefab; // Prefab do proj�til
    public Transform firePoint;     // Ponto de onde o proj�til ser� disparado
    float speed = 5f;
    public float bulletSpeed = 10f; // Velocidade do proj�til
    public float fireRate = 0.5f;   // Intervalo entre disparos
    private float nextFireTime = 0f;

    public CinemachineVirtualCamera cinemachineCam; // Refer�ncia � CinemachineVirtualCamera
    public float zoomedOutSize = 10f; // Tamanho da c�mera ao dar zoom out
    public float normalCameraSize = 5f; // Tamanho normal da c�mera
    private bool canMove = true; // Controle de movimento

    // Start is called before the first frame update
    private void Start()
    {
        RbP = GetComponent<Rigidbody2D>();
       
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            move(); // S� move o tanque se puder
        }
    }

    // Update is called once per frame
    void Update()
    {
        RotateTowardsMouse(); // Rotaciona o tanque em dire��o ao mouse

        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            Fire(); // Atira
            nextFireTime = Time.time + fireRate; 
        }

        // Verifica se o bot�o direito do mouse foi pressionado
        if (Input.GetMouseButtonDown(1))
        {
            EnterZoomMode(); 
        }

        // Verifica se o bot�o direito do mouse foi solto
        if (Input.GetMouseButtonUp(1))
        {
            ExitZoomMode(); 
        }
    }

    //faz a movimenta��o do player
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

    // Fun��o para rotacionar o tanque em dire��o ao mouse
    void RotateTowardsMouse()
    {
        // Pegar a posi��o do mouse na tela e convert�-la para o mundo 2D
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized; // Calcula a dire��o do tanque ao mouse

        // Calcula o �ngulo para rotacionar o tanque
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle); // Rotaciona o tanque para apontar para o mouse
    }

    void Fire()
    {
        // Instancia o proj�til na posi��o do ponto de disparo
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Aplica velocidade ao proj�til na dire��o do firePoint (que segue a dire��o do mouse)
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = firePoint.right * bulletSpeed; // Mova o proj�til na dire��o que o tanque est� apontando
    }

    // Fun��o para entrar no modo de zoom e parar o movimento
    void EnterZoomMode()
    {
        canMove = false; // Desabilita o movimento do tanque
        RbP.velocity = Vector2.zero; // Para o tanque
        cinemachineCam.m_Lens.OrthographicSize = zoomedOutSize; // Altera o zoom da Cinemachine 
    }

    // Fun��o para sair do modo de zoom e voltar ao normal
    void ExitZoomMode()
    {
        canMove = true; // Habilita o movimento novamente
        cinemachineCam.m_Lens.OrthographicSize = normalCameraSize; // Retorna o zoom normal da Cinemachine
    }

}
