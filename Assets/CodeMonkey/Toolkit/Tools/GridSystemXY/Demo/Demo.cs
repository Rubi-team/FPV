using CodeMonkey.Toolkit.TMousePosition;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CodeMonkey.Toolkit.TGridSystemXY {

    public class Demo : MonoBehaviour {


        [SerializeField] private Button blueButton;
        [SerializeField] private Button redButton;
        [SerializeField] private Button whiteButton;


        private GridSystemVisual.GridVisualType selectedGridVisualType;
        private int selectedValue;


        private void Awake() {
            blueButton.onClick.AddListener(() => {
                selectedValue = 56;
                selectedGridVisualType = GridSystemVisual.GridVisualType.Blue;
            });
            redButton.onClick.AddListener(() => {
                selectedValue = 12;
                selectedGridVisualType = GridSystemVisual.GridVisualType.Red;
            });
            whiteButton.onClick.AddListener(() => {
                selectedValue = 0;
                selectedGridVisualType = GridSystemVisual.GridVisualType.White;
            });
        }

        private void Update() {
            if (Input.GetMouseButtonDown(0) && !IsPointerOverUI()) {
                Vector3 mouseWorldPosition = MousePosition2D.GetPosition();

                GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(mouseWorldPosition);

                if (!LevelGrid.Instance.IsValidGridPosition(gridPosition)) {
                    return;
                }

                LevelGrid.Instance.GetGridObject(gridPosition).SetValue(selectedValue);
                GridSystemVisual.Instance.ShowGridPosition(gridPosition, selectedGridVisualType);
            }
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

    }

}