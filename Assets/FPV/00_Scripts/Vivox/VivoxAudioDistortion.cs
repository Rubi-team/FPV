using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VivoxAudioDistortion : MonoBehaviour
{
    [Header("Paramètres de distorsion")] [Range(0.0f, 10.0f)]
    public float drive = 5.0f; // Amplification d'entrée (plus c'est élevé, plus le signal est saturé)

    [Range(0.0f, 1.0f)] public float mix = 1.0f; // Mix Dry/Wet (1 = effet complet)

    // Cette méthode est appelée automatiquement par Unity sur l'AudioSource.
    private void OnAudioFilterRead(float[] data, int channels)
    {
        // Pour chaque échantillon du buffer audio, on applique une distorsion non linéaire.
        for (var i = 0; i < data.Length; i++)
        {
            var cleanSample = data[i];
            // Appliquer un gain, puis saturer avec la fonction tanh pour obtenir une distorsion "douce"
            var distortedSample = Mathf.Tan(cleanSample * drive);
            // Mélanger le signal original et le signal traité selon le paramètre mix.
            data[i] = Mathf.Lerp(cleanSample, distortedSample, mix);
        }
    }
}