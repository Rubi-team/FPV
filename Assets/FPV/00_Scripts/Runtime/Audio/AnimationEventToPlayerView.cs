using UnityEngine;

namespace FPV.Runtime
{
    public class AnimationEventToPlayerView : MonoBehaviour
    {
        [SerializeField] private PlayerView playerView;

        private void Footsteps()
        {
            playerView.AudioFootsteps();
        }
    }
}