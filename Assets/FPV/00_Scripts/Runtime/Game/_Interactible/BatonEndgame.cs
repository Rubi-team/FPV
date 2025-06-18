using System;
using System.Collections;
using FPV.Runtime;
using Unity.Netcode;
using UnityEngine;

public class BatonEndgame : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerApplication>(out var playerApp))
        {
            //FIN DU JEU (ui gg vous avez gagné)
            Debug.Log($"gg");

            //QUIT GAME
            StartCoroutine(ResetToMainMenu());
        }
    }

    [Rpc(SendTo.Everyone)]
    private IEnumerator ResetToMainMenu()
    {
        yield return new WaitForSeconds(10f);
        Application.Quit();
    }
}
