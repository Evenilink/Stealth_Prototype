using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {

    private Slider suspiciousnessSlider;

	void Start () {
        suspiciousnessSlider = GetComponentInChildren<Slider>();
        AlertState.OnSuspiciousnessChange += OnSuspiciousnessChange;
    }
	
    private void OnSuspiciousnessChange(float newValue) {
        suspiciousnessSlider.value = newValue;
    }
}
