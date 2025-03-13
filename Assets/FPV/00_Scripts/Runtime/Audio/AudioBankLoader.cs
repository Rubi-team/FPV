using System;

public static class AudioBankLoader
{
    /// <summary>
    ///     Checks if a given bank is loaded.
    /// </summary>
    public static bool HasBankLoaded(string bankName)
    {
        // For now, assume the bank is always loaded.
        // In a real implementation, check FMOD’s bank management.
        return true;
    }

    /// <summary>
    ///     Loads an FMOD bank and then calls the provided callback.
    /// </summary>
    public static void LoadBank(string bankName, bool loadSampleData, Action callback)
    {
        // In a real implementation, load the bank asynchronously.
        // Once the bank is loaded, call the callback.
        // For now, just call the callback immediately.
        callback?.Invoke();
    }
}