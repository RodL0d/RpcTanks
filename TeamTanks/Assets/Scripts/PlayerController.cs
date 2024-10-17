using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks
{
    Rigidbody2D RbP;
    public GameObject bulletPrefab; // Prefab do projétil
    public Transform firePoint;     // Ponto de onde o projétil será disparado
    float speed = 5f;
    public float bulletSpeed = 10f; // Velocidade do projétil
    public float fireRate = 0.5f;   // Intervalo entre disparos
    private float nextFireTime = 0f;

    public Vector3 spawnPoint; // Ponto de spawn original
    public int maxHealth = 100; // Vida máxima
    private int currentHealth;  // Vida atual

    public Image healthBarImage; // Referência à imagem da barra de vida
    public Transform healthBarTransform; // Transform da barra de vida para seguir o tanque

    private CinemachineVirtualCamera cinemachineCam; // Referência à CinemachineVirtualCamera
    public float zoomedOutSize = 10f; // Tamanho da câmera ao dar zoom out
    public float normalCameraSize = 5f; // Tamanho normal da câmera
    private bool canMove = true; // Controle de movimento


    private void Start()
    {
        RbP = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth; // Inicializa com vida cheia

        // Inicializa a barra de vida como completamente cheia
        UpdateHealthBarUI();

        spawnPoint = transform.position; // Armazena o ponto de spawn inicial

        if (photonView.IsMine)
        {
            cinemachineCam = FindObjectOfType<CinemachineVirtualCamera>();
            if (cinemachineCam != null)
            {
                cinemachineCam.Follow = this.transform; // Atribui o tanque atual para a câmera seguir
                ExitZoomMode();
            }
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine && canMove)
        {
            Move();
        }
    }


    void Update()
    {
        if (photonView.IsMine)
        {
            RotateTowardsMouse();

            if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
            {
                Fire(); // Chama o método Fire
                nextFireTime = Time.time + fireRate;
            }

            if (Input.GetMouseButtonDown(1))
            {
                EnterZoomMode();
            }

            if (Input.GetMouseButtonUp(1))
            {
                ExitZoomMode();
            }
        }


    }

    // Modifique a função Move para incluir uma RPC
    [PunRPC]
    void MoveTank(float horizontal, float vertical)
    {
        if (canMove == true)
        {
            Vector2 movement = new Vector2(horizontal, vertical);
            RbP.velocity = movement * speed;
        }
    }

    // Método para receber dano
    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (!photonView.IsMine) return; // Apenas o tanque local pode receber dano diretamente

        currentHealth -= damage;
        Debug.Log("Vida atual: " + currentHealth);

        // Atualiza o valor da barra de vida localmente
        UpdateHealthBarUI();

        // Envia a atualização da vida para todos os jogadores
        photonView.RPC("UpdateHealthBar", RpcTarget.All, currentHealth);

        if (currentHealth <= 0)
        {
            Respawn(); // Restaura o tanque
        }
    }

    // Método RPC para atualizar a barra de vida em todos os clientes
    [PunRPC]
    public void UpdateHealthBar(int health)
    {
        currentHealth = health;
        UpdateHealthBarUI();
    }

    // Atualiza a imagem da barra de vida localmente
    void UpdateHealthBarUI()
    {
        // Calcula o preenchimento com base na vida atual
        healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }


    // Método para restaurar o tanque no ponto de spawn com vida cheia
    void Respawn()
    {
        // Reseta a vida
        currentHealth = maxHealth;

        // Atualiza a barra de vida para cheia novamente localmente
        UpdateHealthBarUI();

        // Movimenta o tanque de volta ao ponto de spawn
        transform.position = spawnPoint;

        // Envia a atualização de respawn para todos os jogadores
        photonView.RPC("UpdateHealthBar", RpcTarget.All, currentHealth);

        // Opcional: adicionar um pequeno delay antes de restaurar o movimento
        StartCoroutine(RespawnDelay());
    }

    IEnumerator RespawnDelay()
    {
        canMove = false;  // Impede o movimento durante o respawn
        yield return new WaitForSeconds(1.0f); // Aguarda 1 segundo antes de reativar o tanque
        canMove = true;   // Permite o movimento novamente
    }

    // Atualize o movimento local para enviar a posição ao servidor
    void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Chame o RPC para atualizar a movimentação na rede
        photonView.RPC("MoveTank", RpcTarget.All, moveHorizontal, moveVertical);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Envia a posição e a rotação do jogador
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // Recebe a posição e a rotação do jogador
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
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

    // Função para criar o projétil localmente
    void CreateBullet()
    {
        // Instancia o projétil localmente
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Aplica velocidade ao projétil
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = firePoint.right * bulletSpeed;
    }

    [PunRPC]
    void RPCFire()
    {
        // Apenas os jogadores remotos criarão o projétil
        if (!photonView.IsMine)
        {
            CreateBullet();
        }
    }

    void Fire()
    {
        // O jogador local cria o projétil e dispara
        if (photonView.IsMine)
        {
            CreateBullet();
        }

        // Chama o método RPC para os outros jogadores criarem o projétil
        photonView.RPC("RPCFire", RpcTarget.Others);
    }


    IEnumerator SmoothZoom(float targetSize)
    {
        float initialSize = cinemachineCam.m_Lens.OrthographicSize;
        float elapsed = 0f;
        float duration = 0.5f; // Duração da transição

        while (elapsed < duration)
        {
            cinemachineCam.m_Lens.OrthographicSize = Mathf.Lerp(initialSize, targetSize, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cinemachineCam.m_Lens.OrthographicSize = targetSize; // Garante que o valor final seja exato
    }

    void EnterZoomMode()
    {
        canMove = false;
        Debug.Log("não se move");
        RbP.velocity = Vector2.zero;
        if (cinemachineCam != null)
        {
            StartCoroutine(SmoothZoom(zoomedOutSize));
        }
    }

    void ExitZoomMode()
    {
        canMove = true;
        if (cinemachineCam != null)
        {
            StartCoroutine(SmoothZoom(normalCameraSize));
        }
    }


    public void SetCameraFollow(Transform playerTransform)
    {
        cinemachineCam.Follow = playerTransform; // Atribui o jogador local para ser seguido pela câmera
    }


}