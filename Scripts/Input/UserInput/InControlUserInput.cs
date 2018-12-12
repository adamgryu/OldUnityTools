using UnityEngine;
using System.Collections;
using InControl;

namespace QuickUnityTools.Input {
    public class InControlUserInput : MonoBehaviourUserInput {

        protected InputDevice inputDevice { get { return InputManager.ActiveDevice; } }

        protected override ButtonState GetRawButton(Button button) {
            switch (button) {
                case Button.Button1: return ButtonFromInputControl(inputDevice.Action1);
                case Button.Button2: return ButtonFromInputControl(inputDevice.Action2);
                case Button.MenuButton: return new ButtonState(inputDevice.MenuWasPressed, inputDevice.MenuWasPressed, "Start");
            }
            return new ButtonState();
        }

        protected override StickState GetRawStick(Stick stick) {
            switch (stick) {
                case Stick.LeftStick: return StickFromInputControl(stick, inputDevice.LeftStick);
                case Stick.RightStick: return StickFromInputControl(stick, inputDevice.RightStick);
            }
            return new StickState();
        }

        private StickState StickFromInputControl(Stick stick, TwoAxisInputControl stickControl) {
            return new StickState(stickControl.Vector, prevStick[stick]);
        }

        private ButtonState ButtonFromInputControl(InputControl control) {
            return new ButtonState(control.IsPressed, control.WasPressed, control.Handle);
        }
    }
}