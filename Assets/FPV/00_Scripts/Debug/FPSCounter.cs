using System;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    
    [SerializeField] Color badColor = Color.red;
    [SerializeField] Color neutralColor = Color.yellow;
    [SerializeField] Color goodColor = Color.cyan;
    
    [SerializeField] private float badValue = 50;
    [SerializeField] private float neutralValue = 60;

    [SerializeField] private float fps;
    
    float timeLeft;
    float fpsAccumulator;
    float framesCounter;

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        fpsAccumulator += Time.timeScale / Time.deltaTime;
        framesCounter++;
        
        if (timeLeft <= 0)
        {
            fps = fpsAccumulator / framesCounter;
            
            if (fps < badValue)
            {
                text.color = badColor;
            }
            else if (fps < neutralValue)
            {
                text.color = neutralColor;
            }
            else
            {
                text.color = goodColor;
            }
            
            text.text = $"{fps:0.0}";
        }
    }
}