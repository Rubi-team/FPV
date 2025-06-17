using UnityEngine;

public class AnimationEventToMenace : MonoBehaviour
{
    [SerializeField] private Menace menace;

    private void Footsteps()
    {
        menace.AudioFootsteps();
    }
}
