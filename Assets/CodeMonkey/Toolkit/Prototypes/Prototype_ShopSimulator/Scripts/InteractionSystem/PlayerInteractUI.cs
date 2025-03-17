using UnityEngine;
using System.Collections.Generic;

namespace CodeMonkey.Toolkit.ShopSimulatorDemo {

    public class PlayerInteractUI : MonoBehaviour {


        [SerializeField] private GameObject containerGameObject;
        [SerializeField] private PlayerInteractLookAt playerInteract;
        [SerializeField] private Transform playerInteractSingleUITemplate;


        private void Start() {
            playerInteractSingleUITemplate.gameObject.SetActive(false);
        }

        private void Update() {
            if (playerInteract.GetInteractableObject() != null) {
                Show(playerInteract.GetInteractableObject());
            } else {
                if (PlayerShopSimulator.Instance.IsCarryingContainerBox()) {
                    // Player carrying something, show that
                    Show(null);
                } else {
                    // Player not carrying anything, hide all interaction UI
                    Hide();
                }
            }
        }

        private void Show(IInteractable interactable) {
            containerGameObject.SetActive(true);

            // Clear previous objects
            foreach (Transform child in containerGameObject.transform) {
                if (child == playerInteractSingleUITemplate) {
                    // Don't destroy Template
                    continue;
                }
                Destroy(child.gameObject);
            }

            if (interactable != null) {
                Dictionary<IInteractable.InteractAction, string> interactTextDictionary = interactable.GetInteractTextDictionary();
                if (interactTextDictionary != null) {
                    foreach (IInteractable.InteractAction interactAction in interactTextDictionary.Keys) {
                        Transform playerInteractSingleUITransform = Instantiate(playerInteractSingleUITemplate, containerGameObject.transform);

                        playerInteractSingleUITransform.gameObject.SetActive(true);

                        PlayerInteractSingleUI playerInteractSingleUI = playerInteractSingleUITransform.GetComponent<PlayerInteractSingleUI>();
                        playerInteractSingleUI.Setup(interactAction, interactTextDictionary[interactAction]);
                    }
                }
            }

            if (PlayerShopSimulator.Instance.IsCarryingContainerBox()) {
                Transform playerInteractSingleUITransform = Instantiate(playerInteractSingleUITemplate, containerGameObject.transform);

                playerInteractSingleUITransform.gameObject.SetActive(true);

                PlayerInteractSingleUI playerInteractSingleUI = playerInteractSingleUITransform.GetComponent<PlayerInteractSingleUI>();
                playerInteractSingleUI.Setup(IInteractable.InteractAction.DropBox, "Drop Box");
            }
        }

        private void Hide() {
            containerGameObject.SetActive(false);

        }

    }

}