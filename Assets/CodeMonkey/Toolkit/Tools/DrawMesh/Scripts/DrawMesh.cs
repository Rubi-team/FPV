using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace CodeMonkey.Toolkit.TDrawMesh {

    /// <summary>
    /// ** Draw Mesh **
    /// Draw a mesh with the mouse
    /// 
    /// You can use this to let the player draw anything
    /// It could be a logo, their player icon
    /// Or since it's a mesh you can also add a MeshCollider
    /// and do something like a Level Builder
    /// </summary>
    public class DrawMesh : MonoBehaviour {


        public static DrawMesh Instance { get; private set; }


        [SerializeField] private Material drawMeshMaterial;


        private GameObject lastGameObject;
        private int lastSortingOrder;
        private Mesh mesh;
        private Vector3 lastMouseWorldPosition;
        private float lineThickness = 0.6f;
        private Color lineColor = Color.green;


        private void Awake() {
            Instance = this;
        }

        private void Update() {
            if (!IsPointerOverUI()) {
                // Only run logic if not over UI
                Vector3 mouseWorldPosition = GetMouseWorldPosition();
                if (Input.GetMouseButtonDown(0)) {
                    // Mouse Down
                    CreateMeshObject();
                    mesh = MeshUtils.CreateMesh(mouseWorldPosition, mouseWorldPosition, mouseWorldPosition, mouseWorldPosition);
                    mesh.MarkDynamic();
                    lastGameObject.GetComponent<MeshFilter>().mesh = mesh;

                    Material material = new Material(drawMeshMaterial);
                    material.color = lineColor;
                    lastGameObject.GetComponent<MeshRenderer>().material = material;
                }

                if (Input.GetMouseButton(0)) {
                    // Mouse Held Down
                    float minDistance = .1f;
                    if (Vector2.Distance(lastMouseWorldPosition, mouseWorldPosition) > minDistance) {
                        // Far enough from last point
                        Vector2 forwardVector = (mouseWorldPosition - lastMouseWorldPosition).normalized;

                        lastMouseWorldPosition = mouseWorldPosition;

                        MeshUtils.AddLinePoint(mesh, mouseWorldPosition, lineThickness);
                    }
                }

                if (Input.GetMouseButtonUp(0)) {
                    // Mouse Up
                    MeshUtils.AddLinePoint(mesh, mouseWorldPosition, 0f);
                }
            }
        }

        private void CreateMeshObject() {
            lastGameObject = new GameObject("DrawMeshSingle", typeof(MeshFilter), typeof(MeshRenderer));
            lastSortingOrder++;
            lastGameObject.GetComponent<MeshRenderer>().sortingOrder = lastSortingOrder;
        }

        private Vector3 GetMouseWorldPosition() {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0f;
            return worldPosition;
        }

        private bool IsPointerOverUI() {
            if (EventSystem.current.IsPointerOverGameObject()) {
                return true;
            } else {
                PointerEventData pe = new PointerEventData(EventSystem.current);
                pe.position = Input.mousePosition;
                List<RaycastResult> hits = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pe, hits);
                return hits.Count > 0;
            }
        }

        public void SetThickness(float lineThickness) {
            this.lineThickness = lineThickness;
        }

        public void SetColor(Color lineColor) {
            this.lineColor = lineColor;
        }

    }

}