using System;
using System.Runtime.InteropServices;

namespace FPV
{
    public static class SteamAudioWrapper
    {
        [DllImport("phonon", CallingConvention = CallingConvention.Cdecl)]
        public static extern void iplGetDirectSoundPath(
            IntPtr simulator,
            ref IPLDirectSoundPathSettings settings,
            ref IPLVector3 source,
            ref IPLVector3 listener,
            out IPLDirectSoundPath path);
    }

// Structures nécessaires (à adapter selon la définition dans l'API C)
    [StructLayout(LayoutKind.Sequential)]
    public struct IPLVector3
    {
        public float x;
        public float y;
        public float z;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct IPLDirectSoundPathSettings
    {
        public int maxPathLength;
        public int maxOcclusion;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct IPLDirectSoundPath
    {
        public float distance;

        public float occlusion;
        // Autres champs selon l'API C
    }
}