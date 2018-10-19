using UnityEngine;

public class Hero : MonoBehaviour {

    [Header("Camera")]
    [SerializeField] private BaseCamera FPCam;
    [SerializeField] private BaseCamera TPCam;
    private BaseCamera activeCam;

    [Header("Default")]
    private CoverComponent coverComp;
    private PawnNoiseEmitterComponent noiseEmitter;
    private InteractComponent interactComp;
    //private Weapon weaponComp;
    private IHeroState heroState;

    private struct InventoryWeapons {
        BaseWeapon weaponStats;
        int currAmmo;
    }

    private void Start () {
        activeCam = FPCam;
        coverComp = GetComponent<CoverComponent>();
        noiseEmitter = GetComponent<PawnNoiseEmitterComponent>();
        interactComp = GetComponent<InteractComponent>();
        //weaponComp = GetComponentInChildren<Weapon>();
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

        /*if (Input.GetButton("Fire1"))
            weaponComp.Fire();*/

        //WeaponSelectionInput();

        // Debug only.
        if (Input.GetKeyDown(KeyCode.Q))
            noiseEmitter.MakeNoise(this.gameObject, 1, transform.position);
    }

    // TODO.
    private void WeaponSelectionInput() {
        
    }

    public BaseCamera GetFPCamera() {
        return FPCam;
    }

    public BaseCamera GetTPCamera() {
        return TPCam;
    }

    public BaseCamera GetActiveCamera() {
        return activeCam;
    }

    public void SetActiveCamera(BaseCamera newCamera) {
        if (activeCam == newCamera)
            return;
        newCamera.SetLastCameraRotation(activeCam.GetRotation());
        EnableCameraComponents(activeCam, false);
        EnableCameraComponents(newCamera, true);
        activeCam = newCamera;
    }

    private void EnableCameraComponents(BaseCamera cam, bool enable) {
        cam.GetComponent<Camera>().enabled = enable;
        cam.GetComponent<AudioListener>().enabled = enable;
        if (cam == FPCam)
            cam.GetComponent<FirstPersonCameraComponent>().enabled = enable;
        else cam.GetComponent<ThirdPersonCameraComponent>().enabled = enable;
    }

    public CoverComponent GetCoverComponent() {
        return coverComp;
    }
}
