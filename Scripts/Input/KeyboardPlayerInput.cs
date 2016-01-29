using UnityEngine;
using System.Collections;
using System;

public enum KeyboardPlayerInputSetting {
	DualKeyboardOne,
	DualKeyboardTwo,
	SingleKeyboard
}

[System.Serializable]
public struct KeyboardMapping {
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode useKey;
    public KeyCode cancelKey;
    public KeyCode grappleKey;
    public KeyCode dropKey;
    public KeyCode tauntKey;
}

public class KeyboardPlayerInput : PlayerInput {
    /*
	public KeyboardPlayerInputSetting method { get; private set; }
	private KeyboardMapping mapping;
	private bool prevIsUsePressed;
	private bool prevIsCancelPressed;
	private bool prevIsMenuUsePressed;
	private bool prevIsMenuCancelPressed;
	private bool doNotAllowGrappleUntilRelease;

	public KeyboardPlayerInput(KeyboardPlayerInputSetting method) {
		this.method = method;
	}

	public override void LateUpdate() {
		// This is needed to stop the grapple from extended while using non-player-freezing menus.
		if (this.isUsePressedConsumed) {
			this.doNotAllowGrappleUntilRelease = true;
		}
		if (this.doNotAllowGrappleUntilRelease && !Input.GetKey(mapping.grappleKey)) {
			this.doNotAllowGrappleUntilRelease = false;
		}

		base.LateUpdate();

		this.prevIsUsePressed = this.isUsePressed;
		this.prevIsCancelPressed = this.isCancelPressed;
		this.prevIsMenuUsePressed = this.IsMenuUsePressed();
		this.prevIsMenuCancelPressed = this.IsMenuCancelPressed();
	}

	#region Controls

	public override bool isKeyboardInput {
		get {
			return true;
		}
	}

	private bool isUsePressed {
		get {
			return Input.GetKey(mapping.useKey);
		}
	}

	private bool isCancelPressed {
		get {
			return Input.GetKey(mapping.cancelKey);
		}
	}

	public override bool isCancelTapped {
		get {
			return this.isCancelPressed && !this.prevIsCancelPressed;
		}
	}

	public override bool isGrapplePressed {
		get {
			// NOTE: I know this looks like bad encapsulation- and you're probably right.
			// But, as a side effect of USE and GRAPPLE being mapped to the same key on ONLY keyboard;
			// I might argue that we are handling the problem at the source.
			return Input.GetKey(mapping.grappleKey) && !this.doNotAllowGrappleUntilRelease && !this.isUsePressedConsumed;
		}
	}

	public override bool isDropPressed {
		get {
			return Input.GetKey(mapping.dropKey);
		}
	}

	public override bool isUseTapped {
		get {
			return this.isUsePressed && !this.prevIsUsePressed;
		}
	}

	private bool IsMenuUsePressed() {
		return base.IsMenuUseTapped() || Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Z);
	}

	private bool IsMenuCancelPressed() {
		return base.IsMenuCancelTapped() || Input.GetKey(KeyCode.Escape);
	}

	public override bool IsMenuUseTapped() {
		return this.IsMenuUsePressed() && !this.prevIsMenuUsePressed;
	}

	public override bool IsMenuCancelTapped() {
		return this.IsMenuCancelPressed() && !this.prevIsMenuCancelPressed;
	}

	public override Vector2 GetMovement() {
		if (Input.GetKey(mapping.tauntKey)) {
			return Vector2.zero;
		}
		return new Vector2(
			(Input.GetKey(mapping.leftKey) ? -1 : 0) + (Input.GetKey(mapping.rightKey) ? 1 : 0),
			(Input.GetKey(mapping.downKey) ? -1 : 0) + (Input.GetKey(mapping.upKey) ? 1 : 0));
	}

	public override Vector2 GetAiming() {
		return Vector3.zero;
	}

	public override bool IsTauntPressed(int tauntIndex) {
		if (Input.GetKey(mapping.tauntKey)) {
			switch (tauntIndex) {
				case 0:
					return Input.GetKey(mapping.upKey);
				case 1:
					return Input.GetKey(mapping.leftKey);
				case 2:
					return Input.GetKey(mapping.rightKey);
				case 3:
					return Input.GetKey(mapping.downKey);
			}
		}
		return false;
	}

	#endregion

	public override string GetUseButtonText() {
		return this.mapping.useKey.ToString();
	}

	public override string GetCancelButtonText() {
		return this.mapping.cancelKey.ToString();
	}

	public override string GetStartButtonText() {
		return "ESCAPE";
	}

	public override string GetGrappleButtonText() {
		return this.mapping.grappleKey.ToString();
	}

	public override string GetDropButtonText() {
		return this.mapping.dropKey.ToString();
	}

    public override string GetAccelerateButtonText() {
        if (this.mapping.upKey == KeyCode.UpArrow) {
            return "Up";
        }
        return this.mapping.upKey.ToString();
    }

    public override string GetReverseButtonText() {
        if (this.mapping.downKey == KeyCode.DownArrow) {
            return "Down";
        }
        return this.mapping.downKey.ToString();
    }*/
}
