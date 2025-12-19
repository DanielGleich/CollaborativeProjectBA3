using UnityEngine;

public class Test : MonoBehaviour
{
    void Update()
    {
        Debug.Log($"Team 0 => {TeamManager.Instance?.IsTeamReady(0)}");
        Debug.Log($"Team 1 => {TeamManager.Instance?.IsTeamReady(1)}");
    }
}
