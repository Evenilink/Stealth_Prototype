using UnityEngine;

public class CrouchState : IHeroState {

    private PlayerController pc;
    private float movSpeed = 5f;

    public void Enter(PlayerController pc) {
        Debug.Log("IpcState: Entered 'StandingState'.");
        this.pc = pc;

        // Lowering height of the collider.
        pc.GetComponent<CapsuleCollider>().height = 1.5f;

        pc.SetActiveCamera(pc.GetFPCamera());
    }

    public void Exit() {
        Debug.Log("IpcState: Exited 'StandingState'.");
        // Increasing height of the collider.
        pc.GetComponent<CapsuleCollider>().height = 2.1f;

    }

    public IHeroState Update() {
        UpdateMovement();

        if (Input.GetButtonDown("Cover")) {
            bool enterCover = pc.GetCoverComponent().ToogleCover();
            if (enterCover)
                return new CoverState();
        }
        if (Input.GetButtonDown("Crouch"))
            return new StandingState();
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
