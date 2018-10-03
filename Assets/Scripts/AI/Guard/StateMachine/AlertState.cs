using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

public class AlertState : IGuardState {

    [Range(0f, ALERTED)] private const float SUSPICIOUS = 0.8f;
    private const float ALERTED = 1f;

    private const float CAN_SEE_PLAYER_WEIGHT = 0.1f;
    private const float SUSPICIOUSNESS_LOSS_RATE = 0.1f;

    private float suspiciousness = 0;
    private bool isSuspicious = false;
    private Enemy self;
    private GameObject player;

    public delegate void SuspiciousnessChange(float newValue);
    public static SuspiciousnessChange OnSuspiciousnessChange;

    public AlertState(GameObject player) {
        this.player = player;
    }

    public void Enter(Enemy self) {
        Debug.Log("Entered: Alert State.");
        this.self = self;
    }

    public void Exit() {
        Debug.Log("Exited: Alert State.");
    }

    public IGuardState OnHearNoise(GameObject instigator, float loudness, Vector3 noisePosition) {
        return null;
    }

    public IGuardState OnSeePawnHandler(GameObject gameObject) {
        /*if (gameObject == player) {
            suspiciousness += 1.0f / 6.0f;
            Debug.Log("Suspicious: " + suspiciousness);
        }*/
        return null;
    }

    public IGuardState Update() {
        float suspiciousnessDelta = CalculateSuspiciousnessDelta();
        if (suspiciousnessDelta > 0) {
            ApplySuspiciousnessDelta(suspiciousnessDelta);
            if (suspiciousness >= ALERTED)
                return new AlarmState();
            else if (suspiciousness >= SUSPICIOUS && !isSuspicious) {
                isSuspicious = true;
                self.transform.LookAt(player.transform);
                self.GetNavMeshAgent().destination = player.transform.position;
            }
            if (isSuspicious && !self.GetNavMeshAgent().pathPending && self.GetNavMeshAgent().remainingDistance <= 0.5f) {
                isSuspicious = false;
            }
        } else if (!isSuspicious) {
            // The agent only loses interest if he's not currently suspicious and there wasn't an increment to his suspiciousness.
            suspiciousnessDelta = - SUSPICIOUSNESS_LOSS_RATE * Time.deltaTime;
            ApplySuspiciousnessDelta(suspiciousnessDelta);
            if (suspiciousness <= 0)
                return new PatrollingState();
        }
        return null;
    }

    private void ApplySuspiciousnessDelta(float delta) {
        suspiciousness += delta;
        Debug.Log("Suspiciousness: " + suspiciousness);
        if (OnSuspiciousnessChange != null) // The HUD will catch this event.
            OnSuspiciousnessChange(suspiciousness);
    }

    // TODO: improve formula.
    private float CalculateSuspiciousnessDelta() {
        float delta = 0;
        bool canSeePlayer = self.GetSensingComp().CanSeeOther(player.transform);
        if (canSeePlayer)
            delta += CAN_SEE_PLAYER_WEIGHT * Time.deltaTime;
        return delta;
    }
}
