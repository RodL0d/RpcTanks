using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomNameInput; // Usando TMP_InputField para o TextMeshPro
    public TMP_Text roomListText; // Usando TMP_Text para a lista de salas
    public byte maxPlayers = 4;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Conecta-se ao Photon
    }

    // Chamado quando o jogador se conecta ao servidor mestre do Photon
    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado ao Master Server!");

        // Entra no lobby principal
        PhotonNetwork.JoinLobby();
    }

    // Chamado quando o jogador entra no lobby
    public override void OnJoinedLobby()
    {
        Debug.Log("Entrou no lobby!");
    }

    // Chamado quando a lista de salas � atualizada
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // Limpa o texto da lista de salas
        roomListText.text = "";

        // Atualiza a lista de salas no TextMeshPro
        foreach (RoomInfo room in roomList)
        {
            roomListText.text += room.Name + " (" + room.PlayerCount + "/" + room.MaxPlayers + ")\n";
        }
    }

    // Fun��o chamada pelo bot�o para criar uma sala
    public void CreateRoom()
    {
        string roomName = roomNameInput.text; // Obt�m o nome da sala do InputField do TextMeshPro
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Sala" + Random.Range(1000, 9999); // Nome aleat�rio se o campo estiver vazio
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
    

    // Fun��o chamada pelo bot�o para entrar em uma sala
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    // Chamado quando o jogador entra em uma sala de espera 
    public override void OnJoinedRoom()
    {
        Debug.Log("Entrou na sala: " + PhotonNetwork.CurrentRoom.Name);

        // Carrega a cena de espera (waiting room)
        PhotonNetwork.LoadLevel("WaitingRoom");
    }

    // Chamado se houver falha ao entrar em uma sala
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Falha ao entrar na sala: " + message);
    }
}
