using UnityEngine;

public class RadarEntity : MonoBehaviour {

    private void Start() {
        Radar.Instance().AddEntity(gameObject);
    }

    private void OnDestroy() {
        Radar.Instance().RemoveEntity(gameObject);
    }
}
