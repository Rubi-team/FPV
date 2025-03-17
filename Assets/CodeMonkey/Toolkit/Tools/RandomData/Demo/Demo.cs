using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeMonkey.Toolkit.TRandomData.Demo {

    public class Demo : MonoBehaviour {


        [SerializeField] private Button generateColorButton;
        [SerializeField] private Button sequentialColorButton;
        [SerializeField] private Button randomPositionButton;
        [SerializeField] private Button randomDirectionButton;
        [SerializeField] private Button randomMaleNameButton;
        [SerializeField] private Button randomFemaleNameButton;
        [SerializeField] private Button randomCityNameButton;
        [SerializeField] private Button randomIdButton;
        [SerializeField] private TextMeshProUGUI randomIdTextMesh;
        [SerializeField] private TextMeshProUGUI randomCityNameTextMesh;
        [SerializeField] private TextMeshProUGUI randomFemaleNameTextMesh;
        [SerializeField] private TextMeshProUGUI randomMaleNameTextMesh;
        [SerializeField] private TextMeshProUGUI randomDirectionTextMesh;
        [SerializeField] private TextMeshProUGUI randomPositionTextMesh;
        [SerializeField] private Image randomColorImage;
        [SerializeField] private Image sequentialColorImage;
        [SerializeField] private RectTransform codeMonkeyRectTransform;
        [SerializeField] private RectTransform arrowRectTransform;


        private void Awake() {
            generateColorButton.onClick.AddListener(() => {
                randomColorImage.color = RandomData.GetRandomColor();
            });
            sequentialColorButton.onClick.AddListener(() => {
                sequentialColorImage.color = RandomData.GetSequencialColor();
            });
            randomPositionButton.onClick.AddListener(() => {
                Vector3 randomPosition = RandomData.GetRandomPositionWithinRectangle(50, 326, -200, -110);
                randomPositionTextMesh.text = randomPosition.ToString("F2");
                codeMonkeyRectTransform.anchoredPosition = randomPosition;
            });
            randomDirectionButton.onClick.AddListener(() => {
                Vector3 randomDir = RandomData.GetRandomDir();
                randomDirectionTextMesh.text = randomDir.ToString("F2");
                arrowRectTransform.eulerAngles = new (0, 0, RandomData.GetAngleFromVectorFloat(randomDir));
            });
            randomMaleNameButton.onClick.AddListener(() => {
                randomMaleNameTextMesh.text = RandomData.GetRandomMaleName(true);
            });
            randomFemaleNameButton.onClick.AddListener(() => {
                randomFemaleNameTextMesh.text = RandomData.GetRandomFemaleName(false);
            });
            randomCityNameButton.onClick.AddListener(() => {
                randomCityNameTextMesh.text = RandomData.GetRandomCityName();
            });
            randomIdButton.onClick.AddListener(() => {
                randomIdTextMesh.text = RandomData.GetIdStringLong();
            });
        }
    }

}