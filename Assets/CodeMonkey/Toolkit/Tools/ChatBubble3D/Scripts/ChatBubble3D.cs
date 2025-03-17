using UnityEngine;
using TMPro;

namespace CodeMonkey.Toolkit.TChatBubble3D {

    /// <summary>
    /// ** Chat Bubble 3D **
    /// 
    /// Spawn a nice Chat Bubble to display some message with an icon, in 3D
    /// Great for simple messages from an NPC
    /// Or maybe chat messages in a multiplayer game
    /// </summary>
    public class ChatBubble3D : MonoBehaviour {

        public static ChatBubble3D Create(Transform parent, Vector3 localPosition, IconType iconType, string text, float scale = 1f, float destroyTimer = 6f) {
            ChatBubble3D chatBubble3DPrefab = Resources.Load<ChatBubble3D>(nameof(ChatBubble3D));
            if (chatBubble3DPrefab == null) {
                Debug.LogError("Could not find ChatBubble3D in Resources! Is the prefab inside a folder named exactly 'Resources'? And is the prefab named exactly '" + nameof(ChatBubble3D) + "'?");
                return null;
            }
            ChatBubble3D chatBubble3D = Instantiate(chatBubble3DPrefab, parent);
            chatBubble3D.transform.localPosition = localPosition;
            chatBubble3D.transform.localScale = Vector3.one * scale;

            chatBubble3D.Setup(iconType, text);

            Destroy(chatBubble3D.gameObject, destroyTimer);

            return chatBubble3D;
        }



        public enum IconType {
            Happy,
            Neutral,
            Angry,
        }

        [SerializeField] private SpriteRenderer backgroundSpriteRenderer;
        [SerializeField] private Transform backgroundCube;
        [SerializeField] private SpriteRenderer iconSpriteRenderer;
        [SerializeField] private TextMeshPro textMesh;
        [SerializeField] private Sprite happyIconSprite;
        [SerializeField] private Sprite neutralIconSprite;
        [SerializeField] private Sprite angryIconSprite;


        private void Setup(IconType iconType, string text) {
            textMesh.SetText(text);
            textMesh.ForceMeshUpdate();
            Vector2 textSize = textMesh.GetRenderedValues(false);

            Vector2 padding = new Vector2(7f, 3f);
            backgroundSpriteRenderer.size = textSize + padding;
            Vector3 backgroundCubeScale = textSize + padding * .5f;
            backgroundCubeScale.z = .1f;
            backgroundCube.localScale = backgroundCubeScale;

            Vector3 offset = new Vector3(-3f, 0f);
            backgroundSpriteRenderer.transform.localPosition =
                new Vector3(backgroundSpriteRenderer.size.x / 2f, 0f) + offset;
            backgroundCube.localPosition =
                new Vector3(backgroundSpriteRenderer.size.x / 2f, 0f, +.1f) + offset;

            iconSpriteRenderer.sprite = GetIconSprite(iconType);

            CodeMonkey.Toolkit.TTextWriter.TextWriter.AddWriter(textMesh, text, .03f, true, true, () => { });
        }

        private Sprite GetIconSprite(IconType iconType) {
            switch (iconType) {
                default:
                case IconType.Happy:    return happyIconSprite;
                case IconType.Neutral:  return neutralIconSprite;
                case IconType.Angry:    return angryIconSprite;
            }
        }

        public void DestroySelf() {
            if (this != null && gameObject != null) {
                Destroy(gameObject);
            }
        }

    }

}