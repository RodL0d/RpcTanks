using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{
    Rigidbody2D RbP;
    public GameObject bulletPrefab; // Prefab do proj�til
    public Transform firePoint;     // Ponto de onde o proj�til ser� disparado
    float speed = 5f;
    public float bulletSpeed = 10f; // Velocidade do proj�til
    public float fireRate = 0.5f;   // Intervalo entre disparos
    private float nextFireTime = 0f;

    private CinemachineVirtualCamera cinemachineCam; // Refer�ncia � CinemachineVirtualCamera
    public float zoomedOutSize = 10f; // Tamanho da c�mera ao dar zoom out
    public float normalCameraSize = 5f; // Tamanho normal da c�mera
    private bool canMove = true; // Controle de movimento

    public int maxHealth = 100; // Vida m�xima
    public int currentHealth;  // Vida atual
    public Vector3 spawnPoint; // Ponto de spawn original



    // Start is called before the first frame update
    private void Start()
    {
        RbP = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth; // Inicializa com vida cheia

        // Armazena o ponto de spawn inicial
        spawnPoint = transform.position;

        if (photonView.IsMine)
        {
            cinemachineCam = FindObjectOfType<CinemachineVirtualCamera>();
            if (cinemachineCam != null)
            {
                cinemachineCam.Follow = this.transform; // Atribui o tanque atual para a c�mera seguir
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
                Fire(); // Chama o m�todo Fire
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

    // Modifique a fun��o Move para incluir uma RPC
    [PunRPC]
    void MoveTank(float horizontal, float vertical)
    {
        if (canMove == true)
        {
            Vector2 movement = new Vector2(horizontal, vertical);
            RbP.velocity = movement * speed;
        }
    }

    // M�todo para receber dano
    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (!photonView.IsMine) return; // Apenas o tanque local pode receber dano diretamente

        currentHealth -= damage;
        Debug.Log("Vida atual: " + currentHealth);

        if (currentHealth <= 0)
        {
            Respawn(); // Restaura o tanque
        }
    }

    // M�todo para restaurar o tanque no ponto de spawn com vida cheia
    void Respawn()
    {
        // Reseta a vida
        currentHealth = maxHealth;

        // Movimenta o tanque de volta ao ponto de spawn
        transform.position = spawnPoint;

        StartCoroutine(RespawnDelay());
    }
    IEnumerator RespawnDelay()
    {
        canMove = false;  // Impede o movimento durante o respawn
        yield return new WaitForSeconds(1.0f); // Aguarda 1 segundo antes de reativar o tanque
        canMove = true;   // Permite o movimento novamente
    }

    // Atualize o movimento local para enviar a posi��o ao servidor
    void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Chame o RPC para atualizar a movimenta��o na rede
        photonView.RPC("MoveTank", RpcTarget.All, moveHorizontal, moveVertical);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Envia a posi��o e a rota��o do jogador
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // Recebe a posi��o e a rota��o do jogador
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }

    // Fun��o para rotacionar o tanque em dire��o ao mouse
    void RotateTowardsMouse()
    {
        // Pega a posi��o do mouse na tela e converte para o mundo 2D
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Define a posi��o Z como 0, j� que estamos no plano 2D
        mousePos.z = 0;

        // Calcula a dire��o para a rota��o
        Vector2 direction = mousePos - transform.position;

        // Calcula o �ngulo para rotacionar o tanque
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotaciona o tanque para apontar para o mouse
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // Fun��o para criar o proj�til localmente
    void CreateBullet()
    {
        // Instancia o proj�til localmente
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Aplica velocidade ao proj�til
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = firePoint.right * bulletSpeed;
    }

    [PunRPC]
    void RPCFire()
    {
        // Apenas os jogadores remotos criar�o o proj�til
        if (!photonView.IsMine)
        {
            CreateBullet();
        }
    }

    void Fire()
    {
        // O jogador local cria o proj�til e dispara
        if (photonView.IsMine)
        {
            CreateBullet();
        }

        // Chama o m�todo RPC para os outros jogadores criarem o proj�til
        photonView.RPC("RPCFire", RpcTarget.Others);
    }


    IEnumerator SmoothZoom(float targetSize)
    {
        float initialSize = cinemachineCam.m_Lens.OrthographicSize;
        float elapsed = 0f;
        float duration = 0.5f; // Dura��o da transi��o

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
        Debug.Log("n�o se move");
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
        cinemachineCam.Follow = playerTransform; // Atribui o jogador local para ser seguido pela c�mera
    }


}