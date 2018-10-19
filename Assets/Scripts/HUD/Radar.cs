using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityInfo {

    public GameObject entity;
    public Image icon;

    public EntityInfo(GameObject entity, Image icon) {
        this.entity = entity;
        this.icon = icon;
    }
}

public class Radar : MonoBehaviour {

    [Header("Radar Settings")]
    [SerializeField] private float radarWidth = 200f;
    [SerializeField] private float radarHeight = 200f;
    [SerializeField] private float mapZoom = 5.0f;
    [SerializeField] private Image image;
    [SerializeField] Sprite enemyNotAware;
    [SerializeField] Sprite enemySuspicious;
    [SerializeField] Sprite enemyAlerted;
    [SerializeField] Transform playerTransform;

    private List<EntityInfo> entitiesInfo;
    private static Radar instance;  // Singleton.

    private void Awake() {
        // Singleton.
        if (instance != null && instance != this)
            Destroy(gameObject);
        else instance = this;

        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(radarWidth, radarHeight);
        rect.position = new Vector3(radarWidth / 2f + 50f, radarHeight / 2f + 50f, 0);
        entitiesInfo = new List<EntityInfo>();
    }

    private void Update () {
        DrawRadar();
	}

    // Iterates through all the entities info, calculating the direction from the player to the entity, its distance and angle.
    // Then, it clamps the x and y values to fit inside the radar screen.
    private void DrawRadar() {
        foreach (EntityInfo entityInfo in entitiesInfo) {
            Vector3 dirPlayerToEntity = entityInfo.entity.transform.position - playerTransform.position;
            float distToPlayer = dirPlayerToEntity.magnitude;
            float angle = Vector3.SignedAngle(playerTransform.forward, dirPlayerToEntity, Vector3.up);
            // TODO: improve this calculation efficiency, by saving to a var the value of the division by 2.
            float x = Mathf.Clamp(Mathf.Sin(angle * Mathf.Deg2Rad) * distToPlayer * mapZoom, -radarWidth / 2f, radarWidth / 2f);
            float y = Mathf.Clamp(Mathf.Cos(angle * Mathf.Deg2Rad) * distToPlayer * mapZoom, -radarHeight / 2f, radarHeight / 2f);
            entityInfo.icon.transform.localPosition = new Vector3(x, y, 0);

            // Setting the entity radar rotation.
            // The minus here is due to how the angle is implemented in the canvas.
            // Instead of growing clockwise, it grows anti-clockwise.
            float entityOrientation = -Vector3.SignedAngle(playerTransform.forward, entityInfo.entity.transform.forward, Vector3.up);
            entityInfo.icon.transform.eulerAngles = new Vector3(0, 0, entityOrientation);
        }
    }

    // Adds a new entity to the radar system.
    // Creates a new entity info and stores it.
    public void AddEntity(GameObject entity) {
        Image icon = Instantiate(image, transform);
        icon.GetComponent<Image>().sprite = enemyNotAware;
        entitiesInfo.Add(new EntityInfo(entity, icon));
    }

    // Removes the specified entity from the radar system.
    public void RemoveEntity(GameObject entity) {
        for (int i = 0; i < entitiesInfo.Count; i++) {
            if (entitiesInfo[i].entity == entity) {
                entitiesInfo.RemoveAt(i);
                break;
            }
        }
    }

    // Singleton method to retrieve the instance.
    public static Radar Instance() {
        return instance;
    }
}
