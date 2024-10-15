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

    // Método RPC para disparar
    [PunRPC]
    void RPCFire()
    {
        // Instancia o projétil via PhotonNetwork
        GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation);

        // Aplica velocidade ao projétil
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = firePoint.right * bulletSpeed;
    }

    void Fire()
    {
        // Chama o método RPC para todos os jogadores
        photonView.RPC("RPCFire", RpcTarget.All);
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