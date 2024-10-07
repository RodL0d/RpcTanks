using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D RbP;
    public GameObject bulletPrefab; // Prefab do projétil
    public Transform firePoint;     // Ponto de onde o projétil será disparado
    float speed = 5f;
    public float bulletSpeed = 10f; // Velocidade do projétil
    public float fireRate = 0.5f;   // Intervalo entre disparos
    private float nextFireTime = 0f;

    public CinemachineVirtualCamera cinemachineCam; // Referência à CinemachineVirtualCamera
    public float zoomedOutSize = 10f; // Tamanho da câmera ao dar zoom out
    public float normalCameraSize = 5f; // Tamanho normal da câmera
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
            move(); // Só move o tanque se puder
        }
    }

    // Update is called once per frame
    void Update()
    {
        RotateTowardsMouse(); // Rotaciona o tanque em direção ao mouse

        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            Fire(); // Atira
            nextFireTime = Time.time + fireRate; 
        }

        // Verifica se o botão direito do mouse foi pressionado
        if (Input.GetMouseButtonDown(1))
        {
            EnterZoomMode(); 
        }

        // Verifica se o botão direito do mouse foi solto
        if (Input.GetMouseButtonUp(1))
        {
            ExitZoomMode(); 
        }
    }

    //faz a movimentação do player
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

    // Função para entrar no modo de zoom e parar o movimento
    void EnterZoomMode()
    {
        canMove = false; // Desabilita o movimento do tanque
        RbP.velocity = Vector2.zero; // Para o tanque
        cinemachineCam.m_Lens.OrthographicSize = zoomedOutSize; // Altera o zoom da Cinemachine 
    }

    // Função para sair do modo de zoom e voltar ao normal
    void ExitZoomMode()
    {
        canMove = true; // Habilita o movimento novamente
        cinemachineCam.m_Lens.OrthographicSize = normalCameraSize; // Retorna o zoom normal da Cinemachine
    }

}
