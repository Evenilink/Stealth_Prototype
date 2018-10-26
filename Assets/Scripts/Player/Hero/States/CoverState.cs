using UnityEngine;

public class CoverState : IHeroState {

    private PlayerController pc;
    private float movSpeed = 3f;
    private float currSwapTriggerTime = 0f;

    private float jumpSwapKeyPressTime = 0.2f;

    public void Enter(PlayerController pc) {
        Debug.Log("IpcState: Entered 'CoverState'.");
        this.pc = pc;
        pc.SetActiveCamera(pc.GetTPCamera());
        pc.GetAnimController().SetBool("inCover", true);
    }

    public void Exit() {
        Debug.Log("IpcState: Exited 'CoverState'.");
        pc.GetAnimController().SetBool("inCover", false);
    }

    public IHeroState Update() {
        UpdateMovement();
        
        if (Input.GetButtonDown("Cover")) {
            bool exitCover = pc.GetCoverComponent().ToogleCover();
            if (exitCover)
                return new StandingState();
        }

        // Store the cumulative time the cover interaction button is being pressed, only if there's a swap type available.
        if (Input.GetButton("Cover Interaction") && (pc.GetCoverComponent().IsSwapAvailable() || pc.GetCoverComponent().IsJumpSwapAvailable())) {
            currSwapTriggerTime += Time.deltaTime;
            if (currSwapTriggerTime >= pc.GetCoverComponent().GetSwapTriggerTime() && pc.GetCoverComponent().IsSwapAvailable()) {
                pc.GetCoverComponent().Swap();
                currSwapTriggerTime = 0f;
            }
        }
        // Only jump swap if the player already pressed the swap button, but not for the necessary time it's required for a swap, so do a jump swap instead.
        else if (Input.GetButtonUp("Cover Interaction") && currSwapTriggerTime > 0 && pc.GetCoverComponent().IsJumpSwapAvailable()) {
            currSwapTriggerTime = 0f;
            pc.GetCoverComponent().JumpSwap();
        }
        else if (currSwapTriggerTime != 0)
            currSwapTriggerTime = 0;

        return null;
    }

    private void UpdateMovement() {
        float hInput = Input.GetAxis("Horizontal");
        if (hInput > 0)
            pc.GetCoverComponent().UpdateComponent(-pc.transform.right, CoverComponent.Side.RIGHT);
        else if (hInput < 0)
            pc.GetCoverComponent().UpdateComponent(pc.transform.right, CoverComponent.Side.LEFT);

        if (hInput > 0f && pc.GetCoverComponent().CanKeepMoving())
            pc.transform.position += hInput * -pc.transform.right * movSpeed * Time.deltaTime;
        else if (hInput < 0f && pc.GetCoverComponent().CanKeepMoving())
            pc.transform.position -= hInput * pc.transform.right * movSpeed * Time.deltaTime;

        // Anim Controller.
        if (pc.GetCoverComponent().CanKeepMoving())
            pc.GetAnimController().SetFloat("hInput", hInput);
        else pc.GetAnimController().SetFloat("hInput", 0);
    }
}
