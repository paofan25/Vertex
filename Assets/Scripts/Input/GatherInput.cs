using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GatherInput : MonoBehaviour
{
    private PlayerControls myCustomControls;
    public float valueX;
    public bool jumpInput;
    private void Awake(){
        myCustomControls = new PlayerControls();
    }

    private void OnEnable(){
        myCustomControls.Player.Move.performed += StartMove;//得到按下按键后的数值
        myCustomControls.Player.Move.canceled += StopMove;//抬起按键后取消移动
        myCustomControls.Player.Jump.performed += JumpStart;
        myCustomControls.Player.Jump.canceled += JumpStop;
        myCustomControls.Player.Enable();
    }

    private void OnDisable(){
        myCustomControls.Player.Move.performed -= StartMove; 
        myCustomControls.Player.Move.canceled -= StopMove;
        myCustomControls.Player.Jump.performed -= JumpStart; 
        myCustomControls.Player.Jump.canceled -= JumpStop;
        myCustomControls.Player.Disable();
        // myCustomControls.Disable();
    }
    private void StartMove(InputAction.CallbackContext context){
        valueX = Mathf.RoundToInt(context.ReadValue<float>());
        // Debug.Log("PlayerInput is working");
    }

    private void StopMove(InputAction.CallbackContext context){
        valueX = 0;
    }

    private void JumpStart(InputAction.CallbackContext context){
        jumpInput = true;
    }
    private void JumpStop(InputAction.CallbackContext context){
        jumpInput = false;
    }

}
