using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

public class InvestigateState : IGuardState {

    private const float AGENT_SPEED = 1.0f;
    private Vector3 investigate;
    private float investigateTime;
    private Enemy self;

    public InvestigateState(Vector3 investigate) {
        this.investigate = investigate;
    }

    public void Enter(Enemy self) {
        this.self = self;
        SetNewTarget(investigate);
    }

    public void Exit() { }

    public IGuardState OnHearNoise(GameObject instigator, float loudness, Vector3 noisePosition) {
        SetNewTarget(noisePosition);
        return null;
    }

    public IGuardState OnSeePawnHandler(GameObject gameObject) {
        return new AlertState(gameObject);
    }

    IGuardState IGuardState.Update() {
        if (self.GetNavMeshAgent().remainingDistance <= 0.5f) {
            investigateTime -= Time.deltaTime;
            // When the investigate time is over, go back to patrol state.
            if (investigateTime <= 0)
                return new PatrollingState();
        }
        return null;
    }

    // Sets all the information needed when a new point of interest in this state is set.
    private void SetNewTarget(Vector3 target) {
        investigateTime = Random.Range(3, 5);
        self.PlayClip(Enemy.SoundType.HEAR);
        self.GetNavMeshAgent().destination = target;
    }
}
