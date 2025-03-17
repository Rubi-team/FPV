using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.Toolkit.TSaveFileScreenshot {

    public class Demo : MonoBehaviour {


        public static Demo Instance { get; private set; }


        public static readonly string FILE_PATH = Application.dataPath + "/CodeMonkey/Toolkit/Tools/SaveFileScreenshot/Demo/SaveFile.bytesave";


        [SerializeField] private GameObject weapon1;
        [SerializeField] private GameObject weapon2;
        [SerializeField] private GameObject weapon3;
        [SerializeField] private GameObject hat2;
        [SerializeField] private GameObject hat3;
        [SerializeField] private GameObject armor2;
        [SerializeField] private GameObject armor3;
        [SerializeField] private Camera screenshotCamera;


        private void Awake() {
            Instance = this;
        }


        // Game Data you want to save
        public class SaveData {

            public int hat;
            public int armor;
            public int weapon;

        }



        public string GetSaveDataJSON() {
            SaveData saveData = new SaveData();

            if (weapon1.activeSelf) saveData.weapon = 1;
            if (weapon2.activeSelf) saveData.weapon = 2;
            if (weapon3.activeSelf) saveData.weapon = 3;

            saveData.hat = 1;
            if (hat2.activeSelf) saveData.hat = 2;
            if (hat3.activeSelf) saveData.hat = 3;

            saveData.armor = 1;
            if (armor2.activeSelf) saveData.armor = 2;
            if (armor3.activeSelf) saveData.armor = 3;

            string json = JsonUtility.ToJson(saveData);
            return json;
        }

        public void Save() {
            string SAVE_FOLDER = Application.dataPath;

            string json = GetSaveDataJSON();
            byte[] jsonByteArray = Encoding.Unicode.GetBytes(json);

            // Save GameData only
            //File.WriteAllText(Application.dataPath + "/CodeMonkey/Toolkit/Tools/SaveFileScreenshot/Demo/SaveFileGameData.save", json);

            CodeMonkey.Toolkit.TTakeScreenshot.TakeScreenshot.TakeScreenshotTexture(
                screenshotCamera,
                new Rect(0, 0, Screen.width, Screen.height),
                (Texture2D screenshotTexture) => {
                    byte[] screenshotByteArray = screenshotTexture.EncodeToPNG();

                    List<byte> byteList = new List<byte>(jsonByteArray);
                    byteList.AddRange(screenshotByteArray);

                    Debug.Log("Saving Save File with Screenshot, Game Data: " + json);

                    SaveFileImage.Save(json, screenshotTexture, FILE_PATH);
                    //File.WriteAllBytes(Application.dataPath + "/CodeMonkey/Toolkit/Tools/SaveFileScreenshot/Demo/SaveFileScreenshot.png", screenshotByteArray);
                },
                null,
                false
            );
        }

        public void Load() {
            SaveFileImage.LoadJson(
                FILE_PATH,
                out SaveData saveData, 
                out Texture2D screenshotTexture2D
            );

            Debug.Log("Loaded Save File with Screenshot, Game Data: " + JsonUtility.ToJson(saveData));

            weapon1.SetActive(saveData.weapon == 1);
            weapon2.SetActive(saveData.weapon == 2);
            weapon3.SetActive(saveData.weapon == 3);

            hat2.SetActive(saveData.hat == 2);
            hat3.SetActive(saveData.hat == 3);

            armor2.SetActive(saveData.armor == 2);
            armor3.SetActive(saveData.armor == 3);
        }

    }

}