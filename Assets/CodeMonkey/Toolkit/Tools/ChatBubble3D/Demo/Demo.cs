using CodeMonkey.Toolkit.TBlockerUI;
using CodeMonkey.Toolkit.TFirstPersonController;
using CodeMonkey.Toolkit.TFunctionPeriodic;
using CodeMonkey.Toolkit.TInputWindow;
using CodeMonkey.Toolkit.TLookAtCamera;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.Toolkit.TChatBubble3D {

    public class Demo : MonoBehaviour {


        [SerializeField] private Transform[] npcTransformArray;
        [SerializeField] private FirstPersonController firstPersonController;


        private int npcIndex = 2;
        private Dictionary<Transform, ChatBubble3D> transformChatBubble3DDictionary;


        private void Awake() {
            transformChatBubble3DDictionary = new Dictionary<Transform, ChatBubble3D>();
        }

        private void Start() {
            // Spawn first Chat Bubble
            transformChatBubble3DDictionary[npcTransformArray[0]] =
                ChatBubble3D.Create(npcTransformArray[0], new Vector3(.2f, .9f, -.5f), ChatBubble3D.IconType.Neutral, "Hello and Welcome, I'm your Code Monkey!", scale: .07f);

            // Optional: Add the LookAtCamera component
            transformChatBubble3DDictionary[npcTransformArray[0]]
                .gameObject.AddLookAtCamera(TLookAtCamera.LookAtCamera.Method.LookAtInverted);

            // Periodically spawn messages
            FunctionPeriodic.Create(() => {
                Transform npcTransform = npcTransformArray[npcIndex];
                npcIndex = (npcIndex + 1) % npcTransformArray.Length;
                string message = GetRandomMessage();

                ChatBubble3D.IconType[] iconArray =
                    new ChatBubble3D.IconType[] { ChatBubble3D.IconType.Happy, ChatBubble3D.IconType.Neutral, ChatBubble3D.IconType.Angry };
                ChatBubble3D.IconType icon = iconArray[Random.Range(0, iconArray.Length)];


                if (transformChatBubble3DDictionary.ContainsKey(npcTransform)) {
                    transformChatBubble3DDictionary[npcTransform]?.DestroySelf();
                }

                transformChatBubble3DDictionary[npcTransform] =
                    ChatBubble3D.Create(npcTransform, new Vector3(.2f, .9f, -.5f), icon, message, scale: .07f);
                transformChatBubble3DDictionary[npcTransform].
                    gameObject.AddLookAtCamera(TLookAtCamera.LookAtCamera.Method.LookAtInverted);

            }, 2f);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.T)) {
                if (InputWindowUI.IsVisible()) {
                    // Already visible
                    return;
                }
                firstPersonController.Disable();
                firstPersonController.UnlockMouse();
                BlockerUI.Show();
                InputWindowUI.Show(
                    "Say what?",
                    "",
                    "abcdefghijklmnopqrstuvxywz! ABCDEFGHIJKLMNOPQRSTUVXYWZ,.?",
                    50,
                    () => {
                        BlockerUI.Hide();
                        firstPersonController.Enable();
                        firstPersonController.LockMouse();
                    },
                    (string inputText) => {
                        BlockerUI.Hide();

                        transformChatBubble3DDictionary[npcTransformArray[0]]?.DestroySelf();

                        transformChatBubble3DDictionary[npcTransformArray[0]] =
                            ChatBubble3D.Create(npcTransformArray[0], new Vector3(.2f, .9f, -.5f), ChatBubble3D.IconType.Happy, inputText, scale: .07f);
                        transformChatBubble3DDictionary[npcTransformArray[0]].
                            gameObject.AddLookAtCamera(TLookAtCamera.LookAtCamera.Method.LookAtInverted);

                        firstPersonController.Enable();
                        firstPersonController.LockMouse();
                    }
                );
            }
        }

        private string GetRandomMessage() {
            string[] messageArray = new string[] {
                "Hello World!",
                "Good morning!",
                "Subscribe to Code Monkey!",
                "Check out Code Monkey on Steam!",
                "This is a really excellent place!",
                "I'm having so much fun walking around!",
                "I'm really sad about something",
                "I heard someone said something!",
                "I was wondering why the ball was getting bigger, then it hit me",
                "Did you hear about the guy whose whole left side was cut off? He’s all right now",
                "I'm reading a book about anti-gravity. It's impossible to put down!",
                "Don't trust atoms. They make up everything!",
                "What did the pirate say on his 80th birthday? AYE MATEY",
                "What’s Forrest Gump’s password? 1forrest1",
                "Two guys walk into a bar, the third one ducks.",
                "How many tickles does it take to make an octopus laugh? Ten-tickles",
                "Our wedding was so beautiful, even the cake was in tiers.",
                "What do you call a dinosaur with a extensive vocabulary? A thesaurus."
            };

            return messageArray[Random.Range(0, messageArray.Length)];
        }

    }

}