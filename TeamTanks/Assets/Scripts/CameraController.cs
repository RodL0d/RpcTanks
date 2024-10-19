using Photon.Pun;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera; //Camera do Cinema Machine

    // Start is called before the first frame update
    private void Start()
    {
        // Obtenha a referência à câmera virtual
        virtualCamera = GetComponent<CinemachineVirtualCamera>();

        // Verifica se o jogador local tem um PhotonView
        if (PhotonNetwork.IsConnected && PhotonNetwork.LocalPlayer != null)
        {
            // Encontre o jogador local
            GameObject localPlayer = GameObject.FindWithTag("Player"); // Certifique-se de que seu prefab do jogador tenha a tag "Player"
            if (localPlayer != null)
            {
                // Defina o jogador local como alvo da câmera
                virtualCamera.Follow = localPlayer.transform;
                virtualCamera.LookAt = localPlayer.transform; // Para que a câmera também olhe para o jogador
            }
        }
        else
        {
            Debug.LogError("Local player not found or not connected to Photon.");
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // Aqui você pode adicionar lógica para o que acontece com a posição do mouse, se necessário
        Vector3 mousePos;
        if (Camera.main != null)
        {
            mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            mousePos.z = 0; // Ajuste para 0 em jogos 2D
            // Adicione lógica adicional aqui se necessário
        }
        else
        {
            Debug.LogError("Camera.main is null. Please ensure a camera is tagged as MainCamera in the scene.");
        }
    }

    //Faz a camera seguir o jogador que está controlando
    public void SetCameraFollow(Transform playerTransform)
    {
        if (virtualCamera != null)
        {
            virtualCamera.Follow = playerTransform; // Atribui o jogador local para ser seguido pela câmera
        }
    }
}