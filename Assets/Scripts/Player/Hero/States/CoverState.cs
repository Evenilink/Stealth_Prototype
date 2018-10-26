using UnityEngine;

public class CoverState : IHeroState {

    private Hero hero;
    private float movSpeed = 3f;
    private float currSwapTriggerTime = 0f;

    private float jumpSwapKeyPressTime = 0.2f;

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
        // Only jump swap if the player already pressed the swap button, but not for the necessary time it's required for a swap, so do a jump swap instead.
        else if (Input.GetButtonUp("Cover Interaction") && currSwapTriggerTime > 0 && hero.GetCoverComponent().IsJumpSwapAvailable()) {
            currSwapTriggerTime = 0f;
            hero.GetCoverComponent().JumpSwap();
        }

        /*if (Input.GetButton("Cover Interaction") && hero.GetCoverComponent().IsSwapAvailable()) {
            currSwapTriggerTime += Time.deltaTime;
            if (currSwapTriggerTime >= hero.GetCoverComponent().GetSwapTriggerTime()) {
                hero.GetCoverComponent().Swap();
                currSwapTriggerTime = 0f;
            }
        } else currSwapTriggerTime = 0;

        if (Input.GetButtonDown("Cover Interaction") && hero.GetCoverComponent().IsJumpSwapAvailable())
            hero.GetCoverComponent().JumpSwap();*/

        return null;
    }

    private void UpdateMovement() {
        float hInput = Input.GetAxis("Horizontal");
        if (hInput > 0)
            hero.GetCoverComponent().UpdateComponent(-hero.transform.right, CoverComponent.Side.RIGHT);
        else if (hInput < 0)
            hero.GetCoverComponent().UpdateComponent(hero.transform.right, CoverComponent.Side.LEFT);

        if (hInput > 0f && hero.GetCoverComponent().CanKeepMoving())
            hero.transform.position += hInput * -hero.transform.right * movSpeed * Time.deltaTime;
        else if (hInput < 0f && hero.GetCoverComponent().CanKeepMoving())
            hero.transform.position -= hInput * hero.transform.right * movSpeed * Time.deltaTime;
    }
}
