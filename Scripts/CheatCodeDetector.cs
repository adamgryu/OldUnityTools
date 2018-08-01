using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public abstract class CheatCodeDetector : MonoBehaviour {

    private class Cheat {
        public string cheatCode { get; private set; }
        private Action onActivate;
        private int cheatTypeIndex;
        private CheatCodeDetector controller;

        public Cheat(string cheatCode, Action onActivate, CheatCodeDetector controller) {
            this.cheatCode = cheatCode;
            this.onActivate = onActivate;
            this.cheatTypeIndex = 0;
            this.controller = controller;
        }

        public void Update() {
            if (Input.inputString != "") {
                if (Input.inputString[0] == this.cheatCode[this.cheatTypeIndex]) {
                    this.cheatTypeIndex++;
                    if (this.cheatTypeIndex == this.cheatCode.Length) {
                        this.cheatTypeIndex = 0;
                        Activate();
                    }
                } else {
                    this.cheatTypeIndex = 0;
                }
            }
        }

        public void Activate() {
            this.onActivate();
            this.controller.OnCheatActivation();
        }
    }

    private List<Cheat> cheats = new List<Cheat>();

    protected virtual void Update() {
        foreach (Cheat cheat in this.cheats) {
            cheat.Update();
        }
    }

    public void RegisterCheat(string cheatCode, Action onActivate) {
        this.cheats.Add(new Cheat(cheatCode, onActivate, this));
    }

    public void TriggerCheat(string name) {
        var cheat = cheats.FirstOrDefault(c => c.cheatCode == name);
        if (cheat != null) {
            cheat.Activate();
        }
    }

    protected virtual void OnCheatActivation() {
        // Triggers on every cheat activation.
    }
}