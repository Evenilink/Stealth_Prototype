using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {

    [SerializeField] private Slider suspiciousnessSlider;
    [SerializeField] private RawImage crosshair;

    [Header("Interactables")]
    [SerializeField] private Image interactable;
    [SerializeField] private Text interactableText;

    [Header("Weapon")]
    [SerializeField] private Image currWeapon;
    [SerializeField] private Text currMagazine;
    [SerializeField] private Text fullMagazine;

	private void Awake () {
        AlertState.OnSuspiciousnessChange += OnSuspiciousnessChange;

        // TODO: not receiving these events!!!!!!!!!!!!!!!
        CoverComponent.OnCornerEnter += OnCornerEnterHandler;
        CoverComponent.OnCornerExit += OnCornerExitHandler;

        InteractComponent.OnSeeInteractable += OnSeeInteractableHandler;
        InteractComponent.OnStopSeeInteractable += OnStopSeeInteractableHandler;

        // WeaponComponent.Shoot += OnShoot;
        Weapon.OnClipAmmoChange += OnClipAmmoChange;
    }

    private void OnSuspiciousnessChange(float newValue) {
        suspiciousnessSlider.value = newValue;
    }

    private void OnCornerEnterHandler(CoverComponent.Side fromSide) {
        // Displays crosshair.
        Debug.Log("Entered!");
        crosshair.gameObject.SetActive(false);
    }

    private void OnCornerExitHandler() {
        // Hides crosshair.
        Debug.Log("Exited!");
        crosshair.gameObject.SetActive(true);
    }

    private void OnSeeInteractableHandler(GameObject gameObject) {
        interactable.gameObject.SetActive(true);
        interactableText.gameObject.SetActive(true);
    }

    private void OnStopSeeInteractableHandler() {
        interactable.gameObject.SetActive(false);
        interactableText.gameObject.SetActive(false);
    }

    private void OnClipAmmoChange(int currClip) {
        currMagazine.text = currClip.ToString();
    }

    private void OnDestroy() {
        AlertState.OnSuspiciousnessChange -= OnSuspiciousnessChange;
        CoverComponent.OnCornerEnter -= OnCornerEnterHandler;
        CoverComponent.OnCornerExit -= OnCornerExitHandler;
        InteractComponent.OnSeeInteractable -= OnSeeInteractableHandler;
        InteractComponent.OnStopSeeInteractable -= OnStopSeeInteractableHandler;
        Weapon.OnClipAmmoChange -= OnClipAmmoChange;
    }
}
