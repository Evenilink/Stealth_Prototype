using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AI {

    public class PawnSensingComponent : MonoBehaviour {

        // See variables.
        [Header("Sight")]
        [SerializeField] private bool seePawns = true;
        [SerializeField, Range(0, 360)] private float fovAngle = 110f;
        [SerializeField] private float fovRadious = 10f;
        [SerializeField] private bool drawFov = false;
        [SerializeField] private int numRays = 30;
        private Mesh fovMesh;
        private MeshFilter fovMeshFilter;

        // Hear variables.
        [Header("Hear")]
        [SerializeField] private bool hearNoises = true;
        [SerializeField] private float hearRadious = 10f;

        [Header("Detection Layers")]
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private LayerMask obstaclesMask;

        // Delegates.
        public delegate void SeePawn(GameObject gameObject);
        public static SeePawn OnSeePawn;
        public delegate void HearNoise(GameObject instigator, float loudness, Vector3 noisePosition);
        public static HearNoise OnHearNoise;

        private float sensingInterval = 0.25f;

        private void Awake() {
            if (seePawns) {
                // Creating field of view and hear colliders.
                /*SphereCollider fovCollider = gameObject.AddComponent<SphereCollider>();
                fovCollider.isTrigger = true;
                fovCollider.radius = fovRadious;*/
                if (drawFov) {
                    fovMesh = new Mesh();
                    fovMesh.name = "Field of View Mesh";
                }
            }
        }

        private void Start() {
            if (drawFov) {
                GameObject fovMeshHolder = Instantiate(new GameObject(), transform);
                fovMeshHolder.name = "Fov Mesh Holder";
                fovMeshHolder.AddComponent<MeshRenderer>();
                fovMeshHolder.AddComponent<MeshFilter>().mesh = fovMesh;
            }
            if (hearNoises)
                PawnNoiseEmitterComponent.OnPawnMakeNoise += OnPawnMakeNoiseHandler;
            if (seePawns)
                StartCoroutine("SensePlayer");
        }

        private void Update() {
            if (drawFov)
                DrawFov();
        }

        // Senses the player presence periodically.
        // Dispatches an event to all subscribers when the player is seen.
        // TODO: Right now it only works with the "Player" tag.
        IEnumerator SensePlayer() {
            while (true) {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null && OnSeePawn != null && CanSeeOther(player.transform))
                    OnSeePawn(player);
                yield return new WaitForSeconds(sensingInterval);
            }
        }

        // TODO: Right now it only works with the "Player" tag.
        public bool CanSeeOther(Transform other) {
            // Checks if there's a line of sight to the player.
            Vector3 direction = other.transform.position - transform.position;
            RaycastHit hit;
            bool hasLineOfSight = Physics.Raycast(transform.position, direction.normalized, out hit, fovRadious);

            if (hasLineOfSight && hit.collider.gameObject.tag == "Player") {
                // If there's a line of sight to the player, we calculate if the pawn can see the player, based on their positions and view angle.
                float radians = Mathf.Acos(Vector3.Dot(direction, transform.forward) / (direction.magnitude * transform.forward.magnitude));
                float angle = radians * 180 / Mathf.PI;

                if (angle <= (fovAngle / 2))
                    return true;
            }
            return false;
        }

        /*private void OnTriggerStay(Collider other) {
            Debug.Log(other.gameObject);
            if (OnSeePawn != null && (other.gameObject.layer == playerLayer || other.gameObject.layer == enemyLayer)) {
                Debug.Log("Cenas bro!");
                Vector3 pawnToOtherVector = other.transform.position - transform.position;
                float radians = Mathf.Acos(Vector3.Dot(pawnToOtherVector, transform.forward) / (pawnToOtherVector.magnitude * transform.forward.magnitude));
                float angle = radians * 180 / Mathf.PI;
                if (angle <= (fovAngle / 2)) {
                    OnSeePawn(other.gameObject);
                    Debug.Log("Pawn saw something!");
                }
            }
        }*/

        private void OnPawnMakeNoiseHandler(GameObject noiseMaker, float loudness, Vector3 noisePosition) {
            if (OnHearNoise != null) {
                float noiseDistanceFromPawn = Vector3.Distance(transform.position, noisePosition);
                if (noiseDistanceFromPawn <= hearRadious)
                    OnHearNoise(noiseMaker, loudness, noisePosition);
            }
        }

        private Vector3 DirFromAngle(float angleInDegrees, bool isGlobalAngle) {
            if (!isGlobalAngle)
                angleInDegrees += transform.eulerAngles.y;
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        // Field of view mesh raycast info structure.
        private struct FovCastInfo {

            public bool hit;
            public Vector3 hitPosition;

            public FovCastInfo(bool hit, Vector3 hitPosition) {
                this.hit = hit;
                this.hitPosition = hitPosition;
            }
        }

        // Calculates the field of view mesh to draw.
        private void DrawFov() {
            List<Vector3> castPoints = new List<Vector3>();
            float angleStep = fovAngle / (numRays - 1);
            float currAngle = -fovAngle / 2;

            // Calculates the raycast info for every ray that defines the mesh.
            for (int i = 0; i < numRays; i++) {
                Vector3 currLocalDir = DirFromAngle(currAngle, false);
                FovCastInfo currCastInfo = CastFov(currLocalDir);
                castPoints.Add(currCastInfo.hitPosition);
                // Debug.DrawLine(transform.position, transform.position + currLocalDir * fieldOfViewRadious);
                currAngle += angleStep;
            }

            int numVertex = numRays + 1;    // Or castInfos + 1.
            Vector3[] vertices = new Vector3[numVertex];
            int[] triangles = new int[(numVertex - 2) * 3];

            vertices[0] = Vector3.zero;
            for (int i = 0; i < numVertex - 1; i++) {
                // Vertices need to be handled in local space!
                // The first cast point has the result of the second vertex, since the first vertex is the origin (transform.position).
                vertices[i + 1] = transform.InverseTransformPoint(castPoints[i]);


                if (i < numVertex - 2) {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
            }

            fovMesh.Clear();
            fovMesh.vertices = vertices;
            fovMesh.triangles = triangles;
            fovMesh.RecalculateNormals();
        }

        // Casts a ray based on a local direction.
        private FovCastInfo CastFov(Vector3 localDir) {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, localDir, out hit, fovRadious, obstaclesMask))
                return new FovCastInfo(true, hit.point);
            else return new FovCastInfo(false, transform.position + localDir * fovRadious);
        }

        // Draws both the sight and hear radious spheres.
        // DEBUG only.
        private void OnDrawGizmosSelected() {
            if (seePawns) {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(transform.position, fovRadious);
            }
            if (hearNoises) {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, hearRadious);
            }
        }

        // Allows editor field of view debugging.
        [CustomEditor(typeof(PawnSensingComponent))]
        class FovEditor : Editor {

            private void OnSceneGUI() {
                PawnSensingComponent sensingComp = (PawnSensingComponent)target;
                if (sensingComp.seePawns) {
                    Handles.color = Color.white;
                    Handles.DrawLine(sensingComp.transform.position, sensingComp.transform.position + sensingComp.DirFromAngle(sensingComp.fovAngle / 2, false) * sensingComp.fovRadious);
                    Handles.DrawLine(sensingComp.transform.position, sensingComp.transform.position + sensingComp.DirFromAngle(-sensingComp.fovAngle / 2, false) * sensingComp.fovRadious);
                }
            }
        }
    }
}