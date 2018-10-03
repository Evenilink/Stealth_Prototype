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

    /*private Suspiciousness suspiciousnessState;
    private enum Suspiciousness {
        OBSERVING,
        SUSPICIOUS,
        ALERTED,
    }*/

    /*public AlertState(GameObject gameObject) {

    }*/

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
        throw new System.NotImplementedException();
    }

    public IGuardState Update() {
        suspiciousness += CalculateSuspiciousnessDelta();
        if (suspiciousness >= ALERTED)
            return new AlarmState();
        else if (suspiciousness >= SUSPICIOUS && !isSuspicious) {
            isSuspicious = true;
            //self.GetNavMeshAgent().destination = 
        }

            return null;
    }

    private float CalculateSuspiciousnessDelta() {
        return 1 / 3;
    }
}
