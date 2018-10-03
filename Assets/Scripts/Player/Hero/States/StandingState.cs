using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingState : IHeroState {

    private Hero hero;
    private float movSpeed = 5f;

    public void Enter(Hero hero) {
        Debug.Log("IHeroState: Entered 'StandingState'.");
        this.hero = hero;
        hero.SetActiveCamera(hero.GetFPCamera());
    }

    public void Exit() {
        Debug.Log("IHeroState: Exited 'StandingState'.");
    }

    public IHeroState Update() {
        UpdateMovement();

        if (Input.GetButtonDown("Cover")) {
            bool enterCover = hero.GetCoverComponent().ToogleCover();
            if (enterCover)
                return new CoverState();
        }
        if (Input.GetButtonDown("Crouch"))
            return new CrouchState();
        return null;
    }

    private void UpdateMovement() {
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");

        Vector3 forwardMovement = hero.GetActiveCamera().transform.forward;
        forwardMovement.y = 0;
        forwardMovement *= vInput;

        Vector3 rightMovement = hero.GetActiveCamera().transform.right;
        rightMovement.y = 0;
        rightMovement *= hInput;

        hero.transform.position += (forwardMovement + rightMovement) * movSpeed * Time.deltaTime;
    }
}
