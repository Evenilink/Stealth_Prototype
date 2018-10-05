using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {

    private Slider suspiciousnessSlider;
    private RawImage crosshair;

	void Start () {
        suspiciousnessSlider = GetComponentInChildren<Slider>();
        crosshair = GetComponentInChildren<RawImage>();
        AlertState.OnSuspiciousnessChange += OnSuspiciousnessChange;
    }
	
    private void OnSuspiciousnessChange(float newValue) {
        suspiciousnessSlider.value = newValue;
    }
}
