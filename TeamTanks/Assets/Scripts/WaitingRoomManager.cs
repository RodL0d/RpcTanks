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
    public GameObject startGameButton; // Bot�o para o Master Client iniciar o jogo
    public TMP_Text roomNameText;  // Refer�ncia ao TMP_Text para mostrar o nome da sala

    void Start()
    {
        UpdatePlayerList();
        roomNameText.text = "Sala: " + PhotonNetwork.CurrentRoom.Name;

        // Apenas o Master Client pode ver o bot�o de iniciar o jogo
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
        }
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

    // M�todo chamado quando um novo jogador entra na sala
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList(); // Atualiza a lista de jogadores
    }

    // M�todo chamado quando um jogador sai da sala
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList(); // Atualiza a lista de jogadores
    }

    // Bot�o que o Master Client usa para iniciar o jogo
    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Carrega a cena do jogo em todas as m�quinas
            PhotonNetwork.LoadLevel("GameScene");
        }
    }
}
