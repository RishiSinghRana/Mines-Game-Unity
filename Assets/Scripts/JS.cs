using System.Runtime.InteropServices;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void GetScore(int value);
#endif

    // Call this function where you need to send data after
    public void SendScore(int score) 
    {
#if UNITY_WEBGL && !UNITY_EDITOR
            GetScore(score); // JavaScript function
#endif
        return;
    }
