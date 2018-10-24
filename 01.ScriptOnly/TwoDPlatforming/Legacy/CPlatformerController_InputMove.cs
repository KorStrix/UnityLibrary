using UnityEngine;
using System.Collections;

public class CPlatformerController_InputMove : CObjectBase {

    [Rename_Inspector("현재 움직임 멈춤")]
    public bool p_bMoveIsLock = false;

    [GetComponent]
    protected CPlatformerController _pPlayer = null;

    public void DoSet_MoveIsLock(bool bMoveLock)
    {
        p_bMoveIsLock = bMoveLock;

        // Debug.LogError(" DoSet_MoveIsLock : " + bMoveLock);
    }

    public override void OnUpdate(ref bool bCheckUpdateCount)
    {
        base.OnUpdate(ref bCheckUpdateCount);
        bCheckUpdateCount = true;

        MoveCharacter();
        JumpCharacter();
    }

    public void StopMoveCharacter()
    {
        _pPlayer.DoInputVelocity(Vector2.zero, false);
    }

    public void MoveCharacter()
    {
        if (p_bMoveIsLock)
        {
            _pPlayer.DoInputVelocity(Vector3.zero, false);
        }
        else
        {
            Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            _pPlayer.DoInputVelocity(directionalInput, Input.GetKey(KeyCode.LeftShift));
        }

    }

    public void JumpCharacter()
    {
        if (p_bMoveIsLock)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
            _pPlayer.DoJumpInputDown();

        if (Input.GetKeyUp(KeyCode.Space))
            _pPlayer.DoJumpInputUp();
    }
}
