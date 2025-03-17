using UnityEngine;
using TMPro;

namespace CodeMonkey.Toolkit.TChatBubble {

    /// <summary>
    /// ** Chat Bubble **
    /// 
    /// Spawn a nice Chat Bubble to display some message with an icon
    /// Great for simple messages from an NPC
    /// Or maybe chat messages in a multiplayer game
    /// </summary>
    public class ChatBubble : MonoBehaviour {

        public static ChatBubble Create(Transform parent, Vector3 localPosition, IconType iconType, string text, float scale = 1f, float destroyTimer = 6f) {
            ChatBubble chatBubblePrefab = Resources.Load<ChatBubble>(nameof(ChatBubble));
            if (chatBubblePrefab == null) {
                Debug.LogError("Could not find ChatBubble in Resources! Is the prefab inside a folder named exactly 'Resources'? And is the prefab named exactly '" + nameof(ChatBubble) + "'?");
                return null;
            }
            ChatBubble chatBubble = Instantiate(chatBubblePrefab, parent);
            chatBubble.transform.localPosition = localPosition;
            chatBubble.transform.localScale = Vector3.one * scale;

            chatBubble.Setup(iconType, text);

            Destroy(chatBubble.gameObject, destroyTimer);

            return chatBubble;
        }



        public enum IconType {
            Happy,
            Neutral,
            Angry,
        }


        [SerializeField] private SpriteRenderer backgroundSpriteRenderer;
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

            Vector3 offset = new Vector3(-3f, 0f);
            backgroundSpriteRenderer.transform.localPosition =
                new Vector3(backgroundSpriteRenderer.size.x / 2f, 0f) + offset;

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