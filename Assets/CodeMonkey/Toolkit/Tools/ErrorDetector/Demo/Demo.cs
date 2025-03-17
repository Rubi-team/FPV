using System;
using UnityEngine;
using UnityEngine.UI;

namespace CodeMonkey.Toolkit.TErrorDetector.Demo {

    public class Demo : MonoBehaviour {


        [SerializeField] private Button spawnErrorButton;
        [SerializeField] private Button spawnDifferentErrorButton;



        private void Awake() {
            spawnErrorButton.onClick.AddListener(() => {
                object obj = null;
                obj.ToString();
            });
            spawnDifferentErrorButton.onClick.AddListener(() => {
                throw new DivideByZeroException();
            });
        }

    }

}