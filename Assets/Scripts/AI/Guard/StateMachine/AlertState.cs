using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

public class AlertState : IGuardState {

    private const float SUSPICIOUS = 0.6f;
    private const float ALERTED = 1f;
    private float suspiciousness = 0;
    private bool isSuspicious = false;
    private Enemy self;
    private GameObject player;

    /*private Suspiciousness suspiciousnessState;
    private enum Suspiciousness {
        OBSERVING,
        SUSPICIOUS,
        ALERTED,
    }*/

    public AlertState(GameObject player) {
        this.player = player;
    }

    public void Enter(Enemy self) {
        this.self = self;
    }

    public void Exit() {
        throw new System.NotImplementedException();
    }

    public IGuardState OnHearNoise(GameObject instigator, float loudness, Vector3 noisePosition) {
        return null;
    }

    public IGuardState OnSeePawnHandler(GameObject gameObject) {
        Debug.Log("Suspicious: " + suspiciousness);
        suspiciousness += 1 / 6;
        return null;
    }

    public IGuardState Update() {
        /*if (suspiciousness >= ALERTED)
            return new AlarmState();*/
        if (suspiciousness >= SUSPICIOUS && !isSuspicious) {
            isSuspicious = true;
            self.transform.LookAt(player.transform);
            self.GetNavMeshAgent().destination = player.transform.position;
        }
        return null;
    }
}
