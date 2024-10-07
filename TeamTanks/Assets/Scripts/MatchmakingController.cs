using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MatchmakingController : MonoBehaviourPunCallbacks
{
    // Nome da sala
    public string roomName = "TankBattleRoom";
    public byte maxPlayers = 4; // N�mero m�ximo de jogadores na sala

    // Conectar ao Photon quando a cena � carregada
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Conecta usando as configura��es padr�o do Photon
    }

    // Chamado quando voc� se conecta ao Photon Cloud
    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado ao Master Server!");

        // Op��o 1: Entrar em uma sala aleat�ria
        PhotonNetwork.JoinRandomRoom();
    }

    // Chamado quando n�o h� salas para entrar
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Falha ao entrar em uma sala aleat�ria. Criando nova sala...");

        // Cria uma nova sala
        CreateRoom();
    }

    // Cria��o da sala
    void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    // Chamado quando voc� entra em uma sala com sucesso
    public override void OnJoinedRoom()
    {
        Debug.Log("Entrou na sala: " + PhotonNetwork.CurrentRoom.Name);

        // Aqui voc� pode carregar a cena do jogo quando a sala estiver cheia ou pronta
        // Exemplo: Carregar cena do jogo para todos os jogadores
        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayers)
        {
            PhotonNetwork.LoadLevel("NomeDaCenaDoJogo");
        }
    }

    // Chamado quando um jogador entra na sala
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " entrou na sala!");

        // Checa se a sala est� cheia e inicia o jogo
        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayers)
        {
            PhotonNetwork.LoadLevel("NomeDaCenaDoJogo");
        }
    }
}
