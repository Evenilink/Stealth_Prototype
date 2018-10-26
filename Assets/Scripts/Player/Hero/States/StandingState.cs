using UnityEngine;

public class StandingState : IHeroState {

    private PlayerController pc;
    private float movSpeed = 5f;

    public void Enter(PlayerController pc) {
        Debug.Log("IHeroState: Entered 'StandingState'.");
        this.pc = pc;
        pc.SetActiveCamera(pc.GetFPCamera());
    }

    public void Exit() {
        Debug.Log("IHeroState: Exited 'StandingState'.");
    }

    public IHeroState Update() {
        UpdateMovement();

        if (Input.GetButtonDown("Cover")) {
            bool enterCover = pc.GetCoverComponent().ToogleCover();
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

        Vector3 forwardMovement = pc.GetActiveCamera().transform.forward;
        forwardMovement.y = 0;
        forwardMovement *= vInput;

        Vector3 rightMovement = pc.GetActiveCamera().transform.right;
        rightMovement.y = 0;
        rightMovement *= hInput;

        pc.transform.position += (forwardMovement + rightMovement) * movSpeed * Time.deltaTime;
    }
}
