using UnityEngine;
using UnityEngine.UI;

namespace CodeMonkey.Toolkit.TTemplate.Demo {

    public class HealthBarListSingleUI : MonoBehaviour {


        [SerializeField] private Image image;


        private float targetAmount;
        private float currentAmount;


        private void Start() {
            GenerateTargetAmount();
        }

        private void Update() {
            currentAmount = Mathf.Lerp(currentAmount, targetAmount, Time.deltaTime);
            image.fillAmount = currentAmount;
            image.color = GetRedGreenColor(currentAmount);

            if (Mathf.Abs(targetAmount - currentAmount) < .02f) {
                GenerateTargetAmount();
            }
        }

        private void GenerateTargetAmount() {
            targetAmount = Random.Range(0f, 1f);
        }

        public Color GetRedGreenColor(float value) {
            float r = 0f;
            float g = 0f;
            if (value <= .5f) {
                r = 1f;
                g = value * 2f;
            } else {
                g = 1f;
                r = 1f - (value - .5f) * 2f;
            }
            return new Color(r, g, 0f, 1f);
        }

    }

}