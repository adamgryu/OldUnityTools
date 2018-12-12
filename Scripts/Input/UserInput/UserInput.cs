using UnityEngine;
using System.Collections;
using InControl;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickUnityTools.Input {

    /// <summary>
    /// The buttons on the controller that can be read.
    /// </summary>
    public enum Button {
        Button1,
        Button2,
        MenuButton,
    }

    /// <summary>
    /// The sticks on the controller that can be accessed.
    /// </summary>
    public enum Stick {
        LeftStick,
        RightStick,
    }

    /// <summary>
    /// The interface for UserInput.
    /// </summary>
    public interface IUserInput {
        ButtonState button1 { get; }
        ButtonState button2 { get; }
        StickState leftStick { get; }

        ButtonState GetButton(Button button);
        StickState GetStick(Stick stick);
    }

    public struct ButtonState {
        public bool isPressed;
        public bool wasPressed;
        public string name;

        public ButtonState(bool isPressed, bool wasPressed, string name) {
            this.isPressed = isPressed;
            this.wasPressed = wasPressed;
            this.name = name;
        }

        public bool ConsumePress() {
            throw new NotImplementedException(); // TODO: Implement this by keeping track in the input manager class.
        }
    }

    public struct StickState {
        public const float TAP_DISTANCE_SQR = 0.25f;

        public Vector2 vector;
        private Vector2 prevVector;

        public StickState(Vector2 vector, Vector2 prevVector) {
            this.vector = vector;
            this.prevVector = prevVector;
        }

        public bool WasDirectionTapped(Vector2 direction) {
            float currentDistance = (vector - direction).sqrMagnitude;
            float prevDistance = (prevVector - direction).sqrMagnitude;
            return currentDistance < TAP_DISTANCE_SQR && prevDistance >= TAP_DISTANCE_SQR;
        }
    }


    public abstract class MonoBehaviourUserInput : MonoBehaviour, IUserInput {
        private static int uniqueConsecutiveInputIds = int.MinValue;

        // Configuration
        public int priority = 0;
        public bool obeysPriority;

        // Control Implementions
        public ButtonState button1 => GetButton(Button.Button1);
        public ButtonState button2 => GetButton(Button.Button2);
        public StickState leftStick => GetStick(Stick.LeftStick);

        // Public Properties
        public bool hasFocus { get { return !obeysPriority || UserInputManager.instance.inputWithFocus == this; } }
        public int creationTime { get; private set; }

        // State
        protected Dictionary<Stick, Vector2> prevStick = new Dictionary<Stick, Vector2>();

        private void Awake() {
            creationTime = uniqueConsecutiveInputIds;
            uniqueConsecutiveInputIds++;
        }

        protected virtual void OnEnable() {
            UserInputManager.instance.RegisterInputReceiver(this);
        }

        protected virtual void OnDisable() {
            if (UserInputManager.instance != null) {
                UserInputManager.instance.UnregisterInputReceiver(this);
            }
        }

        protected virtual void Start() {
            CachePreviousStickState();
        }

        protected virtual void LateUpdate() {
            CachePreviousStickState();
        }

        private void CachePreviousStickState() {
            foreach (Stick stick in Enum.GetValues(typeof(Stick))) {
                prevStick[stick] = GetStick(stick).vector;
            }
        }

        public ButtonState GetButton(Button button) {
            var rawButton = GetRawButton(button);
            return hasFocus ? rawButton : new ButtonState(false, false, rawButton.name);
        }

        public StickState GetStick(Stick stick) {
            return hasFocus ? GetRawStick(stick) : new StickState();
        }

        protected abstract ButtonState GetRawButton(Button button);
        protected abstract StickState GetRawStick(Stick stick);
    }

    public class UserInputManager : Singleton<UserInputManager> {
        public MonoBehaviourUserInput inputWithFocus { get { return sortedUserInputs.Count > 0 ? sortedUserInputs[0] : null; } }

        private SortedList<PrioritySortingKey, MonoBehaviourUserInput> registeredUserInputs = new SortedList<PrioritySortingKey, MonoBehaviourUserInput>();
        private IList<MonoBehaviourUserInput> sortedUserInputs;

        private bool frameButtonPressAvailable = true;

        private void Awake() {
            sortedUserInputs = registeredUserInputs.Values; // Cache this list view to avoid GC.
        }

        private void Update() {
            // Remove elements that have been destroyed.
            for (int i = sortedUserInputs.Count - 1; i >= 0; i--) {
                if (sortedUserInputs[i] == null) {
                    registeredUserInputs.RemoveAt(i);
                }
            }
        }

        private void LateUpdate() {
            this.frameButtonPressAvailable = true;
        }

        public void RegisterInputReceiver(MonoBehaviourUserInput input) {
            this.registeredUserInputs.Add(new PrioritySortingKey(input.priority, input.creationTime), input);
        }

        public void UnregisterInputReceiver(MonoBehaviourUserInput userInput) {
            var key = this.registeredUserInputs.First(item => item.Value == userInput).Key;
            this.registeredUserInputs.Remove(key);
        }

        public bool ConsumeFrameButtonPress() {
            if (this.frameButtonPressAvailable) {
                this.frameButtonPressAvailable = false;
                return true;
            }
            return false;
        }

        public String GetInputStack() {
            String stack = "";
            foreach (var input in registeredUserInputs) {
                stack += "\n" + input.Key + " - " + input.Value.name;
            }
            return stack;
        }
    }
}