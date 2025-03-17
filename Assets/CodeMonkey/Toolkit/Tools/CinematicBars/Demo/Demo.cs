using UnityEngine;
using UnityEngine.UI;

namespace CodeMonkey.Toolkit.TCinematicBars.Demo {

    public class Demo : MonoBehaviour {


        [SerializeField] private Button showCinematicBarsButton;
        [SerializeField] private Button hideCinematicBarsButton;


        private void Start() {
            showCinematicBarsButton.onClick.AddListener(() => {
                // Show Cinematic Bars
                CinematicBars.Show(170f, .3f);
            });
            hideCinematicBarsButton.onClick.AddListener(() => {
                // Hide Cinematic Bars
                CinematicBars.Hide(.3f);
            });
        }
    }

}