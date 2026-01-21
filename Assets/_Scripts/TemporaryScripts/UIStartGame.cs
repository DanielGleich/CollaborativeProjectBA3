using UnityEngine;

public class UIStartGame : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            NetworkSceneManager.LoadNetworkScene("Game", new string[] { "Tutorial" });
        }
    }
}
