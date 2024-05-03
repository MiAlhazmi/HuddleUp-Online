using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlPlInteraction : MonoBehaviour
{
    private GameControl _gameCtrl;

    private void Awake()
    {
        _gameCtrl = GetComponent<GameControl>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HasHit(GameObject player, GameObject target)
    {
        DamagePlayer(player.transform.forward, target);
        if (IsTagger(player))
        {
            TransferTag(player, target);
        }
    }

    private bool IsTagger(GameObject player)
    {
        return player.GetInstanceID() == _gameCtrl.GetTagOwner().GetInstanceID();
        // or player.GetComponent<Player>.GetIsTagger()
    }

    // hit direction helps to identify which direction the target would be pushed towards
    private void DamagePlayer(Vector3 hitDirection, GameObject target)
    {
        // target.GetComponent<PlayerHit>().GetHit(hitDirection);
        // target.transform.position += (Vector3.up * 3);
        target.GetComponent<CharacterController>().Move(hitDirection + Vector3.up / 4);
        target.GetComponent<PlayerUI>().TakeDamageUI();
    }

    private void TransferTag(GameObject fromPlayer, GameObject toTarget)
    {
        fromPlayer.GetComponent<Player>().SetIsTagger(false);
        toTarget.GetComponent<Player>().SetIsTagger(true);
        _gameCtrl.SetTagOwner(toTarget);
    }
}
