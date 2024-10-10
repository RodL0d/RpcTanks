using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints; // Array com os pontos de spawn
    private List<int> usedSpawnIndices = new List<int>(); // Lista de índices já usados

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        // Cada jogador escolhe um ponto de spawn aleatório
        int spawnIndex = GetRandomSpawnPoint();
        Vector3 spawnPosition = spawnPoints[spawnIndex].position;

        // Instancia o jogador na posição de spawn
        GameObject player = PhotonNetwork.Instantiate("PlayerPrefab", spawnPosition, Quaternion.identity);


        // Se este jogador for o local, atribua a câmera para seguir
        if (player.GetComponent<PhotonView>().IsMine)
        {
            CameraController cameraController = FindObjectOfType<CameraController>();
            if (cameraController != null)
            {
                cameraController.SetCameraFollow(player.transform); // Define o jogador local como alvo da câmera
            }
        }

        // Chame o RPC para marcar o ponto de spawn como utilizado
        PhotonView playerPhotonView = player.GetComponent<PhotonView>();
        photonView.RPC("MarkSpawnPointAsUsed", RpcTarget.AllBuffered, spawnIndex);
    }

    int GetRandomSpawnPoint()
    {
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, spawnPoints.Length);
        } while (usedSpawnIndices.Contains(randomIndex));

        return randomIndex;
    }

    [PunRPC]
    void MarkSpawnPointAsUsed(int spawnIndex)
    {
        if (!usedSpawnIndices.Contains(spawnIndex))
        {
            usedSpawnIndices.Add(spawnIndex);
        }
    }
}