using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI {

    public interface IGuardState {

        IGuardState Update();
        void Enter(Enemy self);
        void Exit();
        IGuardState OnSeePawnHandler(GameObject gameObject);
        IGuardState OnHearNoise(GameObject instigator, float loudness, Vector3 noisePosition);
    }
}
