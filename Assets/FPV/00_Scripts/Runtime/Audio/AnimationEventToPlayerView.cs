using FPV;
using UnityEngine;

public class AnimationEventToPlayerView : MonoBehaviour
{
    [SerializeField] private PlayerView playerView;

    private void Footsteps()
    {
        playerView.AudioFootsteps();
    }
}
