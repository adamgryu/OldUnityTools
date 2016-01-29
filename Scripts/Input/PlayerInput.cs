using UnityEngine;
using System.Collections;
using System;

public class PlayerInput {
    public PlayerInput() {

    }

    public Vector2 GetLookMovement() {
        return this.IsSwapHeld() ? new Vector2(this.GetHorizontalAxis(), this.GetVerticalAxis()) : new Vector2();
    }

    public float GetHorizontalMovement() {
        return this.IsSwapHeld() ? 0 : this.GetHorizontalAxis();
    }

    public virtual float GetHorizontalAxis() {
        return Input.GetAxis("Horizontal");
    }

    public virtual float GetVerticalAxis() {
        return Input.GetAxis("Vertical");
    }

    public virtual bool IsJumpHeld() {
        return Input.GetKey(KeyCode.Z) || Input.GetButton("Fire1");
    }

    public virtual bool IsUseHeld() {
        return Input.GetKey(KeyCode.X) || Input.GetButton("Fire2");
    }

    public virtual bool IsRunHeld() {
        return Input.GetKey(KeyCode.C) || Input.GetButton("Fire3");
    }

    public virtual bool IsSwapHeld() {
        return Input.GetKey(KeyCode.Space);
    }
}
;