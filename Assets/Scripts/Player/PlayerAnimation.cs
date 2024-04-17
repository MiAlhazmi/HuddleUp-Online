using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;
    private PlayerMovement _playerMovement;
    private PlayerHit _playerHit;

    private const string IS_IDLE = "isIdle";
    private const string IS_WALKING = "isWalking";
    private const string IS_SPRINTING = "isSprinting";
    private const string IS_HITTING = "isHitting";
    private bool isHitting;

    /// <summary>
    /// This class is for character animation purposes only.
    /// Every move has an animation (walking, running, hitting, jumping)
    /// For the animations that happens continuously we need a bool parameter
    /// And for the ones that needs a trigger(button) we need a trigger parameter.
    /// 
    /// What I learned here is that if we have an animation that should be played
    /// when a player click a button we should call a trigger value not a bool value
    /// </summary>
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerHit = GetComponent<PlayerHit>();
    }

    // Update is called once per frame
    void Update()
    {
        _animator.SetBool(IS_WALKING,  _playerMovement.GetIsWalking());
        _animator.SetBool(IS_SPRINTING,  _playerMovement.GetIsSprinting());
    }

    public void IsHittingTrigger()
    {
        _animator.SetTrigger("Hitting");
    }
}
