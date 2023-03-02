using UnityEngine;

public class Init : MonoBehaviour
{
    private void Awake()
    {
        Input.multiTouchEnabled = false;
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 120;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}
