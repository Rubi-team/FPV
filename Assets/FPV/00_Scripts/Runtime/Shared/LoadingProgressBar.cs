using System;
using FPV.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace FPV
{
    public class LoadingProgressBar : MonoBehaviour
    {
        private Image image;
        private float currentProgress;
        private float targetProgress;
        private float smoothSpeed = 5f;

        private void Awake()
        {
            image = GetComponent<Image>();
            ResetProgress();
        }

        private void Update()
        {
            if (Math.Abs(currentProgress - targetProgress) > 0.01f)
            {
                currentProgress = Mathf.Lerp(currentProgress, targetProgress, Time.deltaTime * smoothSpeed);
                image.fillAmount = currentProgress;
            }

            SetProgress(SceneLoader.GetLoadingProgress());
        }

        public void SetProgress(float progress)
        {
            targetProgress = Mathf.Clamp01(progress);
        }

        public void ResetProgress()
        {
            currentProgress = 0f;
            targetProgress = 0f;
            image.fillAmount = 0f;
        }
    }
}