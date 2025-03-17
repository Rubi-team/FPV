using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using System.IO;

namespace CodeMonkey.Toolkit.TSaveFileScreenshot {

    /// <summary>
    /// ** Save File with Screenshot **
    /// 
    /// Game Data plus a Screenshot (or any Image) in a single file.
    /// Call SaveFileImage.Save(); or SaveFileImage.Load();
    /// with both a Screenshot and your Game Data which it a type you create
    /// 
    /// You can use the TakeScreenshot tool, or use any Texture2D
    /// The Game Data is saved as JSON, so make sure your
    /// custom type is marked [Serializable]
    /// 
    /// It's always nice to have a visual so the player can 
    /// easily identify the game they were playing
    /// or what level/world they were on.
    /// </summary>
    public class SaveFileImage {

        [Serializable]
        public class Header {

            public int jsonByteSize;

        }

        public static List<byte> Save<T>(T saveGameData, Texture2D screenshot, string savePath = null) {
            return Save(JsonUtility.ToJson(saveGameData), screenshot, savePath);
        }

        public static List<byte> Save(string json, Texture2D screenshot, string savePath = null) {
            byte[] jsonByteArray = Encoding.Unicode.GetBytes(json);
            byte[] screenshotByteArray = screenshot.EncodeToPNG();


            Header header = new Header {
                jsonByteSize = jsonByteArray.Length
            };
            string headerJson = JsonUtility.ToJson(header);
            byte[] headerJsonByteArray = Encoding.Unicode.GetBytes(headerJson);

            ushort headerSize = (ushort)headerJsonByteArray.Length;
            byte[] headerSizeByteArray = BitConverter.GetBytes(headerSize);

            List<byte> byteList = new List<byte>();
            byteList.AddRange(headerSizeByteArray);
            byteList.AddRange(headerJsonByteArray);
            byteList.AddRange(jsonByteArray);
            byteList.AddRange(screenshotByteArray);

            if (savePath != null) {
                File.WriteAllBytes(savePath, byteList.ToArray());
            }

            return byteList;
        }

        public static void Load(string path, out string gameDataJson, out Texture2D screenshotTexture2D) {
            byte[] byteArray = File.ReadAllBytes(path);
            Load(byteArray, out gameDataJson, out screenshotTexture2D);
        }

        public static void Load(byte[] byteArray, out string gameDataJson, out Texture2D screenshotTexture2D) {
            List<byte> byteList = new List<byte>(byteArray);

            ushort headerSize = BitConverter.ToUInt16(new byte[] { byteArray[0], byteArray[1] }, 0);
            List<byte> headerByteList = byteList.GetRange(2, headerSize);
            string headerJson = Encoding.Unicode.GetString(headerByteList.ToArray());
            Header header = JsonUtility.FromJson<Header>(headerJson);

            List<byte> jsonByteList = byteList.GetRange(2 + headerSize, header.jsonByteSize);
            gameDataJson = Encoding.Unicode.GetString(jsonByteList.ToArray());

            int startIndex = 2 + headerSize + header.jsonByteSize;
            int endIndex = byteArray.Length - startIndex;
            List<byte> screenshotByteList = byteList.GetRange(startIndex, endIndex);
            screenshotTexture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            screenshotTexture2D.LoadImage(screenshotByteList.ToArray());
        }

        public static void LoadJson<TSaveType>(string path, out TSaveType saveData, out Texture2D screenshotTexture2D) {
            byte[] byteArray = File.ReadAllBytes(path);
            LoadJson(byteArray, out saveData, out screenshotTexture2D);
        }

        public static void LoadJson<TSaveType>(byte[] byteArray, out TSaveType saveData, out Texture2D screenshotTexture2D) {
            Load(byteArray, out string gameDataJson, out screenshotTexture2D);
            saveData = JsonUtility.FromJson<TSaveType>(gameDataJson);
        }

    }

}