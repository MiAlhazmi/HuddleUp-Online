using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyPlayerInputManager : NetworkBehaviour
{
    private PlayerInputActionsDefault _playerInputActions;
    public PlayerInputActionsDefault.PlayerActions _playerActions;
    private PlayerMovement movement;
    private PlayerLook look;
    private PlayerHit hit;
    private PlayerInteract playerInteract;

    [SerializeField] private Vector2 inputVector = Vector2.zero;
    [SerializeField] private Vector2 inputLook = Vector2.zero;

    void Awake()
    {
        _playerInputActions = new PlayerInputActionsDefault();
        _playerActions = _playerInputActions.Player;
        
        movement = GetComponent<PlayerMovement>();
        look = GetComponent<PlayerLook>();
        hit = GetComponent<PlayerHit>();
        playerInteract = GetComponent<PlayerInteract>();
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        if (base.IsOwner)
        {
            // pass movement input to PlayerMovement component
            inputVector = context.ReadValue<Vector2>();
        }
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        if (base.IsOwner)
        {
            // pass look(camera) input to PlayerLook component
            // look.ProcessLook() context.action.triggered;
            inputLook = context.ReadValue<Vector2>();
        }
    }
    
    public void OnHit(InputAction.CallbackContext context)
    {
        if (base.IsOwner)
        {
            // I put performed to avoid triple calls to the Hit() function (started, performed, cancelled)
            if (context.performed)
                CmdHit();
        }
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        if (base.IsOwner)
        {
            if (context.performed)
                CmdJump();
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (base.IsOwner)
        {
            if (context.performed)
                CmdCrouch();
        }
    }
    
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (base.IsOwner)
        {
            if (context.performed)
                CmdSprint();
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (base.IsOwner)
        {
            if (context.performed)
                CmdInteract();
        }
    }
    

    // Update is called once per frame
    void FixedUpdate()
    {
        if (base.IsOwner)
        {
            CmdProcessMove(inputVector);
        }
    }

    private void LateUpdate()
    {
        if (base.IsOwner)
        {
            CmdProcessLook(inputLook);
        }
    }

    [ServerRpc]
    private void CmdProcessMove(Vector2 input)
    {
        movement.ProcessMove(input);
    }

    [ServerRpc]
    private void CmdProcessLook(Vector2 input)
    {
        look.ProcessLook(input);
    }

    [ServerRpc]
    private void CmdHit()
    {
        hit.Hit();
    }

    [ServerRpc]
    private void CmdJump()
    {
        movement.Jump();
    }

    [ServerRpc]
    private void CmdCrouch()
    {
        movement.Crouch();
    }

    [ServerRpc]
    private void CmdSprint()
    {
        movement.Sprint();
    }

    [ServerRpc]
    private void CmdInteract()
    {
        playerInteract.Interact();
    }
    
    private void OnEnable()
    {
        _playerActions.Enable();
    }
    
    private void OnDisable()
    {
        _playerActions.Disable();
    }
}
