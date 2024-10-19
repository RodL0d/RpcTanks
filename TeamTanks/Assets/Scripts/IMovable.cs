using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface que define o comportamento de movimentação
public interface IMovable
{
    // Método para movimentação
    void Move();

    // Rotaciona o tanque em direção ao mouse
    void RotateTowardsMouse();
}
