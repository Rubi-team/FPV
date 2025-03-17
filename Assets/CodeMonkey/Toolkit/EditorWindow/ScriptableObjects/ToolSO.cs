#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace CodeMonkey.Toolkit {

    //[CreateAssetMenu()]
    public class ToolSO : ScriptableObject {


        public string nameId;
        public string nameString;

        [TextArea(10, 20)]
        public string description;
        public string tags;
        public Sprite iconSprite;
#if UNITY_EDITOR
        public SceneAsset demoScene;
#endif
        public YouTubeVideo walkthroughVideoYouTubeVideo;
        public YouTubeVideo tutorialVideoYouTubeVideo;
        public YouTubeVideo[] relatedVideoYouTubeVideoArray;


        public string[] GetTagArray() {
            return tags.Split(',');
        }

    }

}