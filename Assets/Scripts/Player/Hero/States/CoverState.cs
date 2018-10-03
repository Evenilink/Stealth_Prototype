using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverState : IHeroState {

    private Hero hero;
    private float movSpeed = 3f;
    private Vector3 rightDir;

    public void Enter(Hero hero) {
        Debug.Log("IHeroState: Entered 'CoverState'.");
        this.hero = hero;
        hero.SetActiveCamera(hero.GetTPCamera());
        rightDir = Quaternion.Euler(0, -90, 0) * hero.GetCoverComponent().GetCoverNormal();
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
        return null;
    }

    private void UpdateMovement() {
        float hInput = Input.GetAxis("Horizontal");

        // Debug.Log(hInput * rightDir * movSpeed + ", " + hInput * -rightDir * movSpeed);

        if (hInput > 0.2f && hero.GetCoverComponent().CanMoveRight())
            hero.transform.position += hInput * rightDir * movSpeed * Time.deltaTime;
        else if (hInput < -0.2f && hero.GetCoverComponent().CanMoveLeft())
            hero.transform.position -= hInput * -rightDir * movSpeed * Time.deltaTime;  // Why??? It should only require one minus!
    }
}
