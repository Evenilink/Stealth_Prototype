using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnNoiseEmitterComponent : MonoBehaviour {

    public delegate void PawnMakeNoise(GameObject noiseMaker, float Loudness, Vector3 noisePosition);
    public static event PawnMakeNoise OnPawnMakeNoise;

    public void MakeNoise(GameObject noiseMaker, float loudness, Vector3 noisePosition) {
        if (OnPawnMakeNoise != null)
            OnPawnMakeNoise(noiseMaker, loudness, noisePosition);
    }
}
