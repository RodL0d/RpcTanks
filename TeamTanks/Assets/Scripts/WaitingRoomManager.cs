using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    public TMP_Text playerListText; // TextMeshPro para exibir os jogadores
    public GameObject startGameButton; // Botão para o Master Client iniciar o jogo
    public TMP_Text roomNameText;  // Referência ao TMP_Text para mostrar o nome da sala
    private MapType selectedMap = MapType.Mapa1;


    void Start()
    {
        UpdatePlayerList();
        roomNameText.text = "Sala: " + PhotonNetwork.CurrentRoom.Name;

        // Apenas o Master Client pode ver o botão de iniciar o jogo
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    void Update()
    {
        UpdatePlayerList(); // Atualiza a lista de jogadores a cada frame
    }

    // Atualiza a lista de jogadores exibida
    void UpdatePlayerList()
    {
        playerListText.text = "Jogadores na sala:\n";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerListText.text += player.NickName + "\n";
        }
    }

    // Método chamado quando um novo jogador entra na sala
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList(); // Atualiza a lista de jogadores
    }

    // Método chamado quando um jogador sai da sala
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList(); // Atualiza a lista de jogadores
    }

    // Botão que o Master Client usa para iniciar o jogo
    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.PlayerList.Length < 2)
            {
                Debug.Log("É necessário pelo menos 2 jogadores para iniciar o jogo.");
                return;
            }

            // Chama o RPC para iniciar o jogo
            photonView.RPC("gameStart", RpcTarget.All, (int)selectedMap); // Passa o mapa selecionado
        }
    }

    [PunRPC]
    public void gameStart(int mapIndex)
    {
        // Carrega o mapa selecionado
        string mapName = "Mapa" + (mapIndex + 1); // Supondo que os nomes dos mapas sejam "Mapa1", "Mapa2", "Mapa3"
        PhotonNetwork.LoadLevel(mapName);
    }

    public enum MapType
    {
        Mapa1,
        Mapa2,
        Mapa3
    }

    public void SelectMap(int mapIndex)
    {
        selectedMap = (MapType)mapIndex;
        Debug.Log("Mapa selecionado: " + selectedMap);
    }
    public void OnMap1Selected() { SelectMap(0); }
    public void OnMap2Selected() { SelectMap(1); }
    public void OnMap3Selected() { SelectMap(2); }

}
