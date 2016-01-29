using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;

public class InControlPlayerInput : PlayerInput {

    private InputDevice device;

    public InControlPlayerInput(InputDevice device) {
        this.device = device;
    }

    public override float GetHorizontalAxis() {
        return this.device.LeftStick.X;
    }

    public override float GetVerticalAxis() {
        return this.device.LeftStick.Y;
    }
    public override bool IsJumpHeld() {
        return this.device.Action1;
    }

    public override bool IsUseHeld() {
        return this.device.Action2;
    }

    public override bool IsRunHeld() {
        return this.device.Action3;
    }

    public override bool IsSwapHeld() {
        return this.device.Action4;
    }
}
