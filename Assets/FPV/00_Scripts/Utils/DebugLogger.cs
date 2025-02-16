using UnityEngine;
using Logger = Utils.Logger;

public class DebugLogger : MonoBehaviour
{
    [SerializeField] private Logger _logger;

    // Update is called once per frame
    private void Update()
    {
        if (_logger == null)
            return;

        _logger.Log("This is a log message");
        _logger.WarningLog("This is a warning message");
        _logger.ErrorLog("This is an error message");
    }
}