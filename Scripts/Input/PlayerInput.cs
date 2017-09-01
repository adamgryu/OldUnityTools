using UnityEngine;
using System.Collections;
using System;

namespace QuickUnityTools.Input {
    public class PlayerInput {

        public PlayerInput() {

        }

        public virtual void Update() {

        }

        public Vector2 GetMovement() {
            return new Vector2(this.GetHorizontalAxis(), this.GetVerticalAxis());
        }

        public virtual float GetHorizontalAxis() {
            return UnityEngine.Input.GetAxis("Horizontal");
        }

        public virtual float GetVerticalAxis() {
            return UnityEngine.Input.GetAxis("Vertical");
        }

        public virtual bool IsAction1Held() {
            return UnityEngine.Input.GetButton("Fire1");
        }

        public virtual bool IsAction2Held() {
            return UnityEngine.Input.GetButton("Fire2");
        }
    }
}