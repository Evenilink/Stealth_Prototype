using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverState : IHeroState {

    private Hero hero;
    private float movSpeed = 3f;
    private Vector3 rightDir;
    private bool reachedRightEnd = false;
    private bool reachedLeftEnd = false;
    private float currSwapTriggerTime = 0f;

    public void Enter(Hero hero) {
        Debug.Log("IHeroState: Entered 'CoverState'.");
        this.hero = hero;
        hero.SetActiveCamera(hero.GetTPCamera());
    }

    public void Exit() {
        Debug.Log("IHeroState: Exited 'CoverState'.");
    }

    public IHeroState Update() {
        UpdateMovement();
        
        if (Input.GetButtonDown("Cover")) {
            bool exitCover = hero.GetCoverComponent().ToogleCover();
            if (exitCover)
                return new StandingState();
        }

        if (Input.GetButton("Cover Interaction") && hero.GetCoverComponent().IsSwapAvailable()) {
            currSwapTriggerTime += Time.deltaTime;
            if (currSwapTriggerTime >= hero.GetCoverComponent().GetSwapTriggerTime()) {
                hero.GetCoverComponent().Swap();
                currSwapTriggerTime = 0f;
            }
        }
        else currSwapTriggerTime = 0;

        return null;
    }

    private void UpdateMovement() {
        float hInput = Input.GetAxis("Horizontal");
        // Debug.Log(hInput * rightDir * movSpeed + ", " + hInput * -rightDir * movSpeed);

        if (hInput > 0f && hero.GetCoverComponent().CanMoveRight())
            hero.transform.position += hInput * -hero.transform.right * movSpeed * Time.deltaTime;
        else if (hInput < 0f && hero.GetCoverComponent().CanMoveLeft())
            hero.transform.position -= hInput * hero.transform.right * movSpeed * Time.deltaTime;  // Why??? It should only require one minus!
    }
}
