using UnityEngine;

namespace FPV
{
    public class PlayerApplication : BaseApplication<PlayerModel, PlayerView, PlayerController>
    {
        protected override void Awake()
        {
            base.Awake();
        }
    }
}