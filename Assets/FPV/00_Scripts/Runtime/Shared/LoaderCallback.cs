using FPV.Shared;
using UnityEngine;

namespace FPV._00_Scripts.Runtime.Shared
{
    public class LoaderCallback : MonoBehaviour
    {
        private bool isFirstUpdate = true;

        private void Update()
        {
            if (isFirstUpdate)
            {
                isFirstUpdate = false;
                // This is the first update, so we can safely call the callback
                SceneLoader.OnLoaderCallback();
            }
        }
    }
}