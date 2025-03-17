using UnityEngine;

namespace CodeMonkey.Toolkit.TTooltip.Demo {

    public class Demo : MonoBehaviour {


        [SerializeField] private TooltipOverOut randomTooltipOverOut;


        private float timer;
        

        private void Update() {
            timer -= Time.deltaTime;
            if (timer <= 0f) {
                timer = .5f;
                string abc = "qwertyuiopasdfghjklOQWEERTYSDVPIOSDFNMLM\n\n\n\n\n\n\n\n\n";
                string text = "Hello and Welcome, I'm your Code Monkey!\nRandom text:\n";
                for (int i = 0; i < Random.Range(5, 200); i++) {
                    text += abc[Random.Range(0, abc.Length)];
                }
                randomTooltipOverOut.SetTooltipMessage(text);
            }
        }

    }

}