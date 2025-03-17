using UnityEngine;

namespace CodeMonkey.Toolkit.TKeyDoorSystem {

    [CreateAssetMenu(fileName = "Key", menuName = "Code Monkey/Toolkit/Key Door System/Create Key", order = 1)]
    public class Key : ScriptableObject {


        [Tooltip("Color of the Key")]
        public Color keyColor;


    }

}