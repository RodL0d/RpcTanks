using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface que define o comportamento de movimenta��o
public interface IMovable
{
    // M�todo para movimenta��o
    void Move();

    // Rotaciona o tanque em dire��o ao mouse
    void RotateTowardsMouse();
}
