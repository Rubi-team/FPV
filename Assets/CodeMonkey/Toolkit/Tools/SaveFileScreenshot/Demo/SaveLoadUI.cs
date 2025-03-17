using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Toolkit.TFunctionTimer;

namespace CodeMonkey.Toolkit.TSaveFileScreenshot {

    public class SaveLoadUI : MonoBehaviour {


        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button weapon1Button;
        [SerializeField] private Button weapon2Button;
        [SerializeField] private Button weapon3Button;
        [SerializeField] private Button armor1Button;
        [SerializeField] private Button armor2Button;
        [SerializeField] private Button armor3Button;
        [SerializeField] private Button hat1Button;
        [SerializeField] private Button hat2Button;
        [SerializeField] private Button hat3Button;
        [SerializeField] private Transform weapon1;
        [SerializeField] private Transform weapon2;
        [SerializeField] private Transform weapon3;
        [SerializeField] private Transform armor2;
        [SerializeField] private Transform armor3;
        [SerializeField] private Transform hat2;
        [SerializeField] private Transform hat3;
        [SerializeField] private RawImage rawImage;



        private void Awake() {
            LoadSaveImage();

            saveButton.onClick.AddListener(SaveData);
            loadButton.onClick.AddListener(LoadData);
            weapon1Button.onClick.AddListener(SetWeapon1);
            weapon2Button.onClick.AddListener(SetWeapon2);
            weapon3Button.onClick.AddListener(SetWeapon3);
            armor1Button.onClick.AddListener(SetArmor1);
            armor2Button.onClick.AddListener(SetArmor2);
            armor3Button.onClick.AddListener(SetArmor3);
            hat1Button.onClick.AddListener(SetHat1);
            hat2Button.onClick.AddListener(SetHat2);
            hat3Button.onClick.AddListener(SetHat3);
        }

        public void SetWeapon1() {
            weapon1.gameObject.SetActive(true);
            weapon2.gameObject.SetActive(false);
            weapon3.gameObject.SetActive(false);
        }

        public void SetWeapon2() {
            weapon1.gameObject.SetActive(false);
            weapon2.gameObject.SetActive(true);
            weapon3.gameObject.SetActive(false);
        }

        public void SetWeapon3() {
            weapon1.gameObject.SetActive(false);
            weapon2.gameObject.SetActive(false);
            weapon3.gameObject.SetActive(true);
        }

        public void SetArmor1() {
            armor2.gameObject.SetActive(false);
            armor3.gameObject.SetActive(false);
        }

        public void SetArmor2() {
            armor2.gameObject.SetActive(true);
            armor3.gameObject.SetActive(false);
        }

        public void SetArmor3() {
            armor2.gameObject.SetActive(false);
            armor3.gameObject.SetActive(true);
        }

        public void SetHat1() {
            hat2.gameObject.SetActive(false);
            hat3.gameObject.SetActive(false);
        }

        public void SetHat2() {
            hat2.gameObject.SetActive(true);
            hat3.gameObject.SetActive(false);
        }

        public void SetHat3() {
            hat2.gameObject.SetActive(false);
            hat3.gameObject.SetActive(true);
        }

        public void SaveData() {
            Demo.Instance.Save();
            FunctionTimer.Create(LoadSaveImage, .1f);
        }

        public void LoadData() {
            Demo.Instance.Load();
        }

        private void LoadSaveImage() {
            SaveFileImage.LoadJson(
                Demo.FILE_PATH, 
                out Demo.SaveData saveData, 
                out Texture2D screenshotTexture2D
            );

            rawImage.texture = screenshotTexture2D;
        }

    }

}