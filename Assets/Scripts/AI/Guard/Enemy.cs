using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AI;

public class Enemy : MonoBehaviour {

    [Header("Patrolling")]
    [SerializeField] private TargetInfo[] targets;

    [Header("Audio")]
    [SerializeField] private AudioClip[] hear;
    [SerializeField] private AudioClip[] alert;
    public enum SoundType {
        HEAR,
        ALERT
    }

    [Header("Components")]
    private NavMeshAgent agent;
    private AudioSource audioSource;

    [Header("Defaults")]
    private IGuardState state;
    private IEquipment equipment;

	void Start () {
        Random.InitState((int)System.DateTime.Now.Ticks);
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        PawnSensingComponent.OnSeePawn += OnSeePawnHandler;
        PawnSensingComponent.OnHearNoise += OnHearNoise;
        /*state = new PatrollingState();
        state.Enter(this);*/
	}

    void Update() {
        /*IGuardState newState = state.Update();
        if (newState != null) {
            state.Exit();
            state = newState;
            state.Enter(this);
        }*/
    }
	
    private void OnSeePawnHandler(GameObject gameObject) {
        /*Debug.Log("Enemy sensed!" + gameObject.name);
        IGuardState newState = state.OnSeePawnHandler(gameObject);
        if (newState != null) {
            state.Exit();
            state = newState;
            state.Enter(this);
        }*/
    }

    private void OnHearNoise(GameObject instigator, float loudness, Vector3 noisePosition) {
        Debug.Log(gameObject.name + " heard a sound at position " + noisePosition);
        IGuardState newState = state.OnHearNoise(instigator, loudness, noisePosition);
        if (newState != null) {
            state.Exit();
            state = newState;
            state.Enter(this);
        }
    }

    public void PlaySound(AudioClip clip) {
        audioSource.clip = clip;
        audioSource.Play();
    }

    public NavMeshAgent GetNavMeshAgent() {
        return agent;
    }

    public TargetInfo[] GetTargets() {
        return targets;
    }

    public void PlayClip(SoundType type) {
        switch (type) {
            case SoundType.ALERT:
                if (alert.Length == 0)
                    Debug.LogError("Trying to play an 'Alert' sound without an assignment.");
                audioSource.clip = alert[Random.Range(0, hear.Length)];
                audioSource.Play();
                break;
            case SoundType.HEAR:
                if (hear.Length == 0)
                    Debug.LogError("Trying to play an 'Hear' sound without an assignment.");
                audioSource.clip = hear[Random.Range(0, hear.Length)];
                audioSource.Play();
                break;
            default: break;
        }
    }

    private void OnDestroy() {
        PawnSensingComponent.OnSeePawn -= OnSeePawnHandler;
        PawnSensingComponent.OnHearNoise -= OnHearNoise;
    }
}
