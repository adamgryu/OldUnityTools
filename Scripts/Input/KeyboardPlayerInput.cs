using QuickUnityTools.Input;
using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class KeyboardMapping {
    public KeyCode upKey = KeyCode.UpArrow;
    public KeyCode downKey = KeyCode.DownArrow;
    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode rightKey = KeyCode.RightArrow;
    public KeyCode button1 = KeyCode.Z;
    public KeyCode button2 = KeyCode.X;
}

public class KeyboardPlayerInput : PlayerInput {
	private KeyboardMapping mapping;

	public KeyboardPlayerInput(KeyboardMapping mapping) {
        this.mapping = mapping;
        this.mapping.ToString(); // Suppress warning...
	}

    public override float GetVerticalAxis() {
        return base.GetVerticalAxis();
    }

    public override float GetHorizontalAxis() {
        return base.GetHorizontalAxis();
    }

    public override bool IsAction1Held() {
        return base.IsAction1Held();
    }

    public override bool IsAction2Held() {
        return base.IsAction2Held();
    }
}
