using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

public class PatrollingState : IGuardState {

    [Header("Constants")]
    private const float AGENT_SPEED = 3.5f;
    private const float DEST_ACCEPTABLE_DISTANCE = 0.5f;

    [Header("Defaults")]
    private int currTargetIndex = -1;   // Starts at -1 because of the way we're calculating the first iteration of this value.
    private float stayTime = 0;
    private Enemy self;

    public void Enter(Enemy self) {
        this.self = self;
        self.GetNavMeshAgent().speed = AGENT_SPEED;
    }

    public void Exit() {
        self.GetNavMeshAgent().ResetPath();
    }

    public IGuardState Update() {
        if (self.GetTargets().Length == 0)
            return null;

        if (!self.GetNavMeshAgent().pathPending && self.GetNavMeshAgent().remainingDistance <= DEST_ACCEPTABLE_DISTANCE) {
            stayTime -= Time.deltaTime;
            if (stayTime <= 0) {
                currTargetIndex = (currTargetIndex + 1) % self.GetTargets().Length;
                self.GetNavMeshAgent().destination = self.GetTargets()[currTargetIndex].transform.position;
                stayTime = self.GetTargets()[currTargetIndex].stayTime;
            }
        }

        return null;
    }

    public IGuardState OnSeePawnHandler(GameObject gameObject) {
        if (gameObject.GetComponent<Hero>() != null)
            return new AlertState(gameObject);
        return null;
    }

    public IGuardState OnHearNoise(GameObject instigator, float loudness, Vector3 noisePosition) {
        return new InvestigateState(noisePosition);
    }
}
