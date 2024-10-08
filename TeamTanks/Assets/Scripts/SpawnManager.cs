using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;

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

        // Pega o PhotonView do jogador instanciado
        PhotonView playerPhotonView = player.GetComponent<PhotonView>();

        // Envia o RPC usando o PhotonView do jogador
        playerPhotonView.RPC("MarkSpawnPointAsUsed", RpcTarget.AllBuffered, spawnIndex);
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