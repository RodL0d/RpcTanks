using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints; // Array com os pontos de spawn
    private List<int> usedSpawnIndices = new List<int>(); // Lista de índices já usados

    void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            SpawnPlayer();
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        SpawnPlayer(); // Garantindo que o jogador spawne ao entrar na sala
    }

    void SpawnPlayer()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("Nenhum ponto de spawn foi atribuído.");
            return;
        }

        // Embaralha os pontos de spawn no início
        int spawnIndex = GetRandomSpawnPoint();
        Vector3 spawnPosition = spawnPoints[spawnIndex].position;

        GameObject player = PhotonNetwork.Instantiate("PlayerPrefab", spawnPosition, Quaternion.identity);

        if (player.GetComponent<PhotonView>().IsMine)
        {
            CameraController cameraController = FindObjectOfType<CameraController>();
            cameraController?.SetCameraFollow(player.transform);
        }

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
