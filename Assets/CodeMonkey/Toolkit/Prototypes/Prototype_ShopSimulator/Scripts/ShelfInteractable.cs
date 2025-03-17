using CodeMonkey.Toolkit.TBlockerUI;
using CodeMonkey.Toolkit.TInputWindow;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.Toolkit.ShopSimulatorDemo {

    public class ShelfInteractable : MonoBehaviour, IInteractable {


        [SerializeField] private ShelfHeight shelfHeight;


        public Dictionary<IInteractable.InteractAction, string> GetInteractTextDictionary() {
            return new Dictionary<IInteractable.InteractAction, string>{
                { IInteractable.InteractAction.Stock, "Stock" },
                { IInteractable.InteractAction.Unstock, "Unstock" },
                { IInteractable.InteractAction.ChangePrice, "Change Price" },
            };
        }

        public Transform GetTransform() {
            return transform;
        }

        public bool CanDoInteractAction(IInteractable.InteractAction interactAction) {
            return
                interactAction == IInteractable.InteractAction.Stock ||
                interactAction == IInteractable.InteractAction.Unstock ||
                interactAction == IInteractable.InteractAction.ChangePrice;
        }

        public void Interact(IInteractable.InteractAction interactAction, Transform interactorTransform) {
            switch (interactAction) {
                case IInteractable.InteractAction.Stock:
                    if (PlayerShopSimulator.Instance.IsCarryingContainerBox()) {
                        // Player is carrying something, try to stock it
                        ContainerBox containerBox = PlayerShopSimulator.Instance.GetCarryingContainerBox();
                        if (containerBox.CanRemoveAmount()) {
                            // Container box has amount
                            if (shelfHeight.TryAddObjectType(containerBox.GetObjectType())) {
                                // Stocked!
                                containerBox.RemoveAmount();
                            } else {
                                // Failed to stock! Probably different object type already there
                            }
                        }
                    }
                    break;
                case IInteractable.InteractAction.Unstock:
                    if (PlayerShopSimulator.Instance.IsCarryingContainerBox()) {
                        // Player is carrying something, try to unstock it
                        ContainerBox containerBox = PlayerShopSimulator.Instance.GetCarryingContainerBox();
                        if (containerBox.CanAddAmount(shelfHeight.GetObjectType())) {
                            // Container box can store more amount of this type
                            if (shelfHeight.TryRemoveObjectType(containerBox.GetObjectType())) {
                                // Unstocked!
                                containerBox.AddAmount();
                            } else {
                                // Failed to unstock!
                            }
                        }
                    }
                    break;
                case IInteractable.InteractAction.ChangePrice:
                    if (!shelfHeight.IsEmpty()) {
                        // Not empty, change price
                        PlayerShopSimulator.Instance.Freeze();
                        BlockerUI.Show();
                        InputWindowUI.Show(
                            "New Price for " + shelfHeight.GetObjectType(),
                            PriceManager.Instance.GetPrice(shelfHeight.GetObjectType()),
                            () => {
                                BlockerUI.Hide();
                                PlayerShopSimulator.Instance.Unfreeze();
                            },
                            (int newPrice) => {
                                PriceManager.Instance.SetPrice(shelfHeight.GetObjectType(), newPrice);
                                BlockerUI.Hide();
                                PlayerShopSimulator.Instance.Unfreeze();
                            });
                    } else {
                        // It's empty, cannot change price
                    }
                    break;
            }
        }
    }

}