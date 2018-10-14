using UnityEngine;

public class Hero : MonoBehaviour {

    [Header("Camera")]
    [SerializeField] private Camera FPCam;
    [SerializeField] private Camera TPCam;
    private Camera activeCam;

    [Header("Default")]
    private CoverComponent coverComp;
    private PawnNoiseEmitterComponent noiseEmitter;
    private InteractComponent interactComp;
    private Weapon weaponComp;
    private IHeroState heroState;

    private struct InventoryWeapons {
        BaseWeapon weaponStats;
        int currAmmo;
    }

    private void Start () {
        coverComp = GetComponent<CoverComponent>();
        noiseEmitter = GetComponent<PawnNoiseEmitterComponent>();
        interactComp = GetComponent<InteractComponent>();
        weaponComp = GetComponentInChildren<Weapon>();
        heroState = new StandingState();
        heroState.Enter(this);
    }

    private void Update () {
        IHeroState newState = heroState.Update();
        if (newState != null) {
            heroState.Exit();
            heroState = newState;
            heroState.Enter(this);
        }

        if (Input.GetButtonDown("Interact"))
            interactComp.Interact();

        if (Input.GetButton("Fire1"))
            weaponComp.Fire();

        WeaponSelectionInput();

        // Debug only.
        if (Input.GetKeyDown(KeyCode.Q))
            noiseEmitter.MakeNoise(this.gameObject, 1, transform.position);
    }

    // TODO.
    private void WeaponSelectionInput() {
        
    }

    public Camera GetFPCamera() {
        return FPCam;
    }

    public Camera GetTPCamera() {
        return TPCam;
    }

    public Camera GetActiveCamera() {
        return activeCam;
    }

    public void SetActiveCamera(Camera newCamera) {
        if (activeCam == newCamera)
            return;

        activeCam = newCamera;
        if (newCamera == FPCam) {
            TPCam.GetComponent<Camera>().enabled = false;
            TPCam.GetComponent<ThirdPersonCameraComponent>().enabled = false;
            FPCam.GetComponent<Camera>().enabled = true;
            FPCam.GetComponent<FirstPersonCameraComponent>().enabled = true;
        } else {
            FPCam.GetComponent<Camera>().enabled = false;
            FPCam.GetComponent<FirstPersonCameraComponent>().enabled = false;
            TPCam.GetComponent<Camera>().enabled = true;
            TPCam.GetComponent<ThirdPersonCameraComponent>().enabled = true;
        }
    }

    public CoverComponent GetCoverComponent() {
        return coverComp;
    }
}
