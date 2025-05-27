using System;
using Audio;
using Unity.Cinemachine;
using UnityEngine;

namespace FPV
{
    public class PlayerView : NetworkView<PlayerApplication>
    {
        private PlayerModel Model => App.Model;
        private PlayerController Controller => App.Controller;

        public void AudioFootsteps()
        {
            //AudioManager.Instance.PlayOneShot(AudioManager.Instance.footStep, transform.position);
        }
    }
}