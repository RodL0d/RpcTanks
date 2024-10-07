using TMPro; // Certifique-se de adicionar esta refer�ncia
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomNameInput; // Usando TMP_InputField para o TextMeshPro
    public TMP_Text roomListText; // Usando TMP_Text para a lista de salas
    public byte maxPlayers = 4;
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Conecta-se ao Photon
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(); // Entra no lobby
    }

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
        string roomName = roomNameInput.text; // Pega o nome da sala da UI
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Sala" + Random.Range(1000, 9999); // Se n�o houver nome, cria um aleat�rio
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers; // Define o n�mero m�ximo de jogadores por sala

        // Cria uma sala com as op��es definidas
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    // Fun��o chamada pelo bot�o para entrar em uma sala
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    // Chamado quando o jogador entra em uma sala
    public override void OnJoinedRoom()
    {
        Debug.Log("Entrou na sala: " + PhotonNetwork.CurrentRoom.Name);

        // Aqui voc� pode carregar a cena do jogo
        PhotonNetwork.LoadLevel("NomeDaCenaDoJogo");
    }

    // Chamado se houver falha ao entrar em uma sala
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Falha ao entrar na sala: " + message);
    }
}
