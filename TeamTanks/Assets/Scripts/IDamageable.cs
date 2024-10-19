using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface que define que um objeto pode receber dano
public interface IDamageable
{
    // Método para receber dano
    void TakeDamage(int damage);
    // Da um update na barra de vida
    void UpdateHealthBar(int health);
}
