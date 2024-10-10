using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{
    Rigidbody2D RbP;
    public GameObject bulletPrefab; // Prefab do projétil
    public Transform firePoint;     // Ponto de onde o projétil será disparado
    float speed = 5f;
    public float bulletSpeed = 10f; // Velocidade do projétil
    public float fireRate = 0.5f;   // Intervalo entre disparos
    private float nextFireTime = 0f;

    private CinemachineVirtualCamera cinemachineCam; // Referência à CinemachineVirtualCamera
    public float zoomedOutSize = 10f; // Tamanho da câmera ao dar zoom out
    public float normalCameraSize = 5f; // Tamanho normal da câmera
    private bool canMove = true; // Controle de movimento

    // Start is called before the first frame update
    private void Start()
    {
        RbP = GetComponent<Rigidbody2D>();

        // Se o jogador é o local (photonView.IsMine), encontre a câmera e configure-a para seguir este jogador
        if (photonView.IsMine)
        {
            cinemachineCam = FindObjectOfType<CinemachineVirtualCamera>();
            if (cinemachineCam != null)
            {
                cinemachineCam.Follow = this.transform; // Atribui o tanque atual para a câmera seguir
            }
        }
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            Move(); // Só move o tanque se puder
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine) // Verifica se este é o jogador local
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
    }

// Faz a movimentação do player
void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        RbP.velocity = movement * speed;
    }

    // Função para rotacionar o tanque em direção ao mouse
    void RotateTowardsMouse()
    {
        // Pega a posição do mouse na tela e converte para o mundo 2D
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Define a posição Z como 0, já que estamos no plano 2D
        mousePos.z = 0;

        // Calcula a direção para a rotação
        Vector2 direction = mousePos - transform.position;

        // Calcula o ângulo para rotacionar o tanque
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotaciona o tanque para apontar para o mouse
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }


    void Fire()
    {
        // Instancia o projétil via PhotonNetwork
        GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation);

        // Aplica velocidade ao projétil na direção do firePoint (que segue a direção do mouse)
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = firePoint.right * bulletSpeed;
    }

    // Função para entrar no modo de zoom e parar o movimento
    void EnterZoomMode()
    {
        canMove = false; // Desabilita o movimento do tanque
        RbP.velocity = Vector2.zero; // Para o tanque
        if (cinemachineCam != null)
        {
            cinemachineCam.m_Lens.OrthographicSize = zoomedOutSize; // Altera o zoom da Cinemachine 
            Debug.Log("Entrou no modo de zoom");
        }

    }


    // Função para sair do modo de zoom e voltar ao normal
    void ExitZoomMode()
    {
        canMove = true; // Habilita o movimento novamente
        if (cinemachineCam != null)
        {
            cinemachineCam.m_Lens.OrthographicSize = normalCameraSize; // Retorna o zoom normal
        }
    }


    public void SetCameraFollow(Transform playerTransform)
    {
        cinemachineCam.Follow = playerTransform; // Atribui o jogador local para ser seguido pela câmera
    }


}