using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Entity {

    public GameObject entity;
    public Image icon;

    public Entity(GameObject entity, Image icon) {
        this.entity = entity;
        this.icon = icon;
    }
}

public class Radar : MonoBehaviour {

    [Header("Radar Settings")]
    [SerializeField] private float radarWidth = 200f;
    [SerializeField] private float radarHeight = 200f;
    [SerializeField] private float mapZoom = 5.0f;
    [SerializeField] Image enemyNotAware;
    [SerializeField] Image enemySuspicious;
    [SerializeField] Image enemyAlerted;
    [SerializeField] Transform playerTransform;

    private List<Entity> entities;
    private static Radar instance;  // Singleton.

    private void Awake() {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else instance = this;

        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(radarWidth, radarHeight);
        rect.position = new Vector3(radarWidth / 2f + 50f, radarHeight / 2f + 50f, 0);
        entities = new List<Entity>();
    }

    private void Update () {
        DrawRadar();
	}

    private void DrawRadar() {
        foreach (Entity entity in entities) {
            Vector3 dirPlayerToEntity = entity.entity.transform.position - playerTransform.position;
            float distToPlayer = dirPlayerToEntity.magnitude;
            float angle = Vector3.SignedAngle(playerTransform.forward, dirPlayerToEntity, Vector3.up);
            // TODO: improve this calculation efficiency, by saving to a var the value of the division by 2.
            float x = Mathf.Clamp(Mathf.Sin(angle * Mathf.Deg2Rad) * distToPlayer * mapZoom, -radarWidth / 2f, radarWidth / 2f);
            float y = Mathf.Clamp(Mathf.Cos(angle * Mathf.Deg2Rad) * distToPlayer * mapZoom, -radarHeight / 2f, radarHeight / 2f);
            entity.icon.transform.localPosition = new Vector3(x, y, 0);
        }
    }

    public void AddEntity(GameObject entity) {
        Image icon = Instantiate(enemyNotAware, transform);
        entities.Add(new Entity(entity, icon));
    }

    public void RemoveEntity(GameObject entity) {
        // entities.Remove()
    }

    // Singleton method to retrieve the instance.
    public static Radar Instance() {
        return instance;
    }
}
