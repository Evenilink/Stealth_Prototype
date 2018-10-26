using UnityEngine;

public class RadarEntityComponent : MonoBehaviour {

    private void Start() {
        Radar.Instance().AddEntity(gameObject);
    }

    private void OnDestroy() {
        Radar.Instance().RemoveEntity(gameObject);
    }
}
