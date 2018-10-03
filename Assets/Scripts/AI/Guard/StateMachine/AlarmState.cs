using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

public class AlarmState : IGuardState {

    public void Enter(Enemy self) {
        throw new System.NotImplementedException();
    }

    public void Exit() {
        throw new System.NotImplementedException();
    }

    public IGuardState OnHearNoise(GameObject instigator, float loudness, Vector3 noisePosition) {
        throw new System.NotImplementedException();
    }

    public IGuardState OnSeePawnHandler(GameObject gameObject) {
        throw new System.NotImplementedException();
    }

    public IGuardState Update() {
        throw new System.NotImplementedException();
    }
}
