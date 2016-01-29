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

    public override bool IsAction1Held() {
        return this.device.Action1;
    }

    public override bool IsAction2Held() {
        return this.device.Action2;
    }
}
