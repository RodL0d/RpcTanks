using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using Cinemachine;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints; // Array com os pontos de spawn
    private List<int> usedSpawnIndices = new List<int>(); // Lista de índices já usados

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        // Gera um índice aleatório para um ponto de spawn não utilizado
        int spawnIndex = GetRandomSpawnPoint();
        Vector3 spawnPosition = spawnPoints[spawnIndex].position;

        // Instancia o jogador na posição de spawn
        GameObject player = PhotonNetwork.Instantiate("PlayerPrefab", spawnPosition, Quaternion.identity);

        // Marca o ponto de spawn como utilizado
        photonView.RPC("MarkSpawnPointAsUsed", RpcTarget.AllBuffered, spawnIndex);

        // Configura a câmera para seguir o jogador local
        if (player.GetComponent<PhotonView>().IsMine)
        {
            // Adicione o componente CinemachineVirtualCamera à câmera
            CinemachineVirtualCamera virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            if (virtualCamera != null)
            {
                virtualCamera.Follow = player.transform; // Faz a câmera seguir o jogador local
                virtualCamera.LookAt = player.transform;  // A câmera também olha para o jogador
            }
        }
    }

    int GetRandomSpawnPoint()
    {
        // Gera um índice aleatório para o spawn
        int randomIndex;

        // Garante que o índice gerado ainda não foi usado
        do
        {
            randomIndex = Random.Range(0, spawnPoints.Length);
        } while (usedSpawnIndices.Contains(randomIndex));

        return randomIndex;
    }

    [PunRPC]
    void MarkSpawnPointAsUsed(int spawnIndex)
    {
        // Marca o spawn point como utilizado para todos os jogadores
        if (!usedSpawnIndices.Contains(spawnIndex))
        {
            usedSpawnIndices.Add(spawnIndex);
        }
    }
}
