using UnityEngine;
using System.Collections;
using System;

public class PlayerInput {

    public PlayerInput() {

    }
    
    public virtual void Update() {
        
    }

    public Vector2 GetMovement() {
        return new Vector2(this.GetHorizontalAxis(), this.GetVerticalAxis());
    }

    public virtual float GetHorizontalAxis() {
        return Input.GetAxis("Horizontal");
    }

    public virtual float GetVerticalAxis() {
        return Input.GetAxis("Vertical");
    }

    public virtual bool IsAction1Held() {
        return Input.GetButton("Fire1");
    }

    public virtual bool IsAction2Held() {
        return Input.GetButton("Fire2");
    }
}