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
            // Verifique se h� pelo menos 2 jogadores
            if (PhotonNetwork.PlayerList.Length < 1)
            {
                Debug.Log("� necess�rio pelo menos 2 jogadores para iniciar o jogo.");
                return;
            }

            // Chama o RPC para iniciar o jogo
            photonView.RPC("gameStart", RpcTarget.All); // Chama o m�todo RPC para todos os jogadores
        }
    }

    [PunRPC]
    public void gameStart()
    {
        PhotonNetwork.LoadLevel("Mapa aim");
    }
}
