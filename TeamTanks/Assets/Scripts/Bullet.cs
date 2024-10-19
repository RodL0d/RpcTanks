using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public int damage = 20; //Define o dano da bala

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 1f); //destroi o objeto depois determinados segudos
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Collisao da bala com o player e ser destruida
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Acesse o PlayerController do tanque atingido
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if (player != null)
            {
                // Chame o método RPC para causar dano ao tanque
                player.photonView.RPC("TakeDamage", RpcTarget.All, damage);
            }
        }

        // Destrua a bala após a colisão
        Destroy(gameObject);
    }

}
