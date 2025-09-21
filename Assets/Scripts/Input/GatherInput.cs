using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GatherInput : MonoBehaviour
{
    public int directionX = 0;

    private void Update(){
        if (Keyboard.current.dKey.isPressed) {
            directionX = 1;
        }else if (Keyboard.current.aKey.isPressed) {
            directionX = -1;
        }
        else {
            directionX = 0;
        }
    }
}
