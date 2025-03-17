using System;
using UnityEngine;

namespace CodeMonkey.Toolkit.ShopSimulatorDemo {

    public class ShelfHeight : MonoBehaviour {


        public event EventHandler OnObjectTypeChanged;


        public enum HeightType {
            Top,
            Middle,
            Bottom,
        }


        [SerializeField] private HeightType heightType;
        [SerializeField] private Transform gridOriginTransform;
        [SerializeField] private Shelf shelf;


        private ObjectType objectType;


        public bool IsEmpty() {
            return objectType == ObjectType.None;
        }

        public ObjectType GetObjectType() {
            return objectType;
        }

        public void SetObjectType(ObjectType objectType) {
            this.objectType = objectType;

            OnObjectTypeChanged?.Invoke(this, EventArgs.Empty);
        }

        public Transform GetGridOriginTransform() {
            return gridOriginTransform;
        }

        public HeightType GetHeightType() {
            return heightType;
        }

        public bool TryAddObjectType(ObjectType objectType) {
            if (!IsEmpty() && GetObjectType() != objectType) {
                // This ShelfHeight already has an objectType and it doesn't match this one!
                return false;
            }
            if (IsEmpty()) {
                SetObjectType(objectType);
            }
            return shelf.TryAddObjectType(this, objectType);
        }

        public bool TryRemoveObjectType(ObjectType objectType) {
            if (shelf.IsFullyEmpty(this)) {
                // Shelf Empty, cannot remove anything
                return false;
            }
            if (GetObjectType() != objectType) {
                // Different object type, cannot remove
                return false;
            }

            bool ret = shelf.TryRemoveObjectType(this);

            if (shelf.IsFullyEmpty(this)) {
                SetObjectType(ObjectType.None);
            }

            return ret;
        }


    }

}