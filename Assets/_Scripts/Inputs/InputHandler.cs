using UnityEngine;

public abstract class InputHandler : MonoBehaviour {
    protected PlayerInputs playerInputs;
    private void Awake() {
        playerInputs = new PlayerInputs();
    }
}