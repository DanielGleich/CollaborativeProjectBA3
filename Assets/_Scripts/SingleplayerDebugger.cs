using UnityEngine;

public class SingleplayerDebugger : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
