using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable, IMovable
{
    Rigidbody2D RbP;
    public GameObject bulletPrefab;
    public Transform firePoint;
    float speed = 5f;
    public float bulletSpeed = 10f;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    public Vector3 spawnPoint;
    public int maxHealth = 100;
    private int currentHealth;

    public Image healthBarImage;
    public Transform healthBarTransform;

    private CinemachineVirtualCamera cinemachineCam;
    public float zoomedOutSize = 10f;
    public float normalCameraSize = 5f;
    private bool canMove = true;

    private void Start()
    {
        RbP = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        UpdateHealthBarUI();

        spawnPoint = transform.position;

        if (photonView.IsMine)
        {
            cinemachineCam = FindObjectOfType<CinemachineVirtualCamera>();
            if (cinemachineCam != null)
            {
                cinemachineCam.Follow = this.transform;
                ExitZoomMode();
            }
        }
    }

    // Responsável pela movimentação física do tanque
    void FixedUpdate()
    {
        if (photonView.IsMine && canMove)
        {
            Move();
        }
    }

    // Responsável por detectar entrada de disparo, zoom e rotação
    void Update()
    {
        if (photonView.IsMine)
        {
            RotateTowardsMouse();

            if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
            {
                Fire();
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

    // Responsável por enviar o movimento do tanque pela rede
    public void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        photonView.RPC("MoveTank", RpcTarget.All, moveHorizontal, moveVertical);
    }

    // Move o tanque utilizando a física
    [PunRPC]
    void MoveTank(float horizontal, float vertical)
    {
        if (canMove == true)
        {
            Vector2 movement = new Vector2(horizontal, vertical);
            RbP.velocity = movement * speed;
        }
    }

    public void RotateTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // Aplica dano ao tanque e atualiza a vida
    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (!photonView.IsMine) return;

        currentHealth -= damage;
        Debug.Log("Vida atual: " + currentHealth);
        UpdateHealthBarUI();
        photonView.RPC("UpdateHealthBar", RpcTarget.All, currentHealth);

        if (currentHealth <= 0)
        {
            Respawn();
        }
    }

    // Atualiza a barra de vida em todos os clientes
    [PunRPC]
    public void UpdateHealthBar(int health)
    {
        currentHealth = health;
        UpdateHealthBarUI();
    }

    // Atualiza a imagem da barra de vida
    void UpdateHealthBarUI()
    {
        healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }

    // Restaura o tanque no ponto de spawn
    void Respawn()
    {
        currentHealth = maxHealth;
        UpdateHealthBarUI();
        transform.position = spawnPoint;
        photonView.RPC("UpdateHealthBar", RpcTarget.All, currentHealth);
        StartCoroutine(RespawnDelay());
    }

    // Impede o movimento durante o respawn
    IEnumerator RespawnDelay()
    {
        canMove = false;
        yield return new WaitForSeconds(1.0f);
        canMove = true;
    }

    // Realiza o disparo do projétil
    void Fire()
    {
        if (photonView.IsMine)
        {
            CreateBullet();
        }

        photonView.RPC("RPCFire", RpcTarget.Others);
    }

    // Cria um projétil localmente
    void CreateBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = firePoint.right * bulletSpeed;
    }

    // Dispara o projétil localmente e notifica outros jogadores
    [PunRPC]
    void RPCFire()
    {
        if (!photonView.IsMine)
        {
            CreateBullet();
        }
    }

    // Suaviza a transição de zoom da câmera
    IEnumerator SmoothZoom(float targetSize)
    {
        float initialSize = cinemachineCam.m_Lens.OrthographicSize;
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            cinemachineCam.m_Lens.OrthographicSize = Mathf.Lerp(initialSize, targetSize, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cinemachineCam.m_Lens.OrthographicSize = targetSize;
    }

    // Entra no modo de zoom
    void EnterZoomMode()
    {
        canMove = false;
        RbP.velocity = Vector2.zero;
        if (cinemachineCam != null)
        {
            StartCoroutine(SmoothZoom(zoomedOutSize));
        }
    }

    // Sai do modo de zoom
    void ExitZoomMode()
    {
        canMove = true;
        if (cinemachineCam != null)
        {
            StartCoroutine(SmoothZoom(normalCameraSize));
        }
    }

    // Define qual jogador a câmera deve seguir
    public void SetCameraFollow(Transform playerTransform)
    {
        cinemachineCam.Follow = playerTransform;
    }
}
