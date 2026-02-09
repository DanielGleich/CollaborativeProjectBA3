using FishNet.Object;

public class GameObjectOwnershipHandler : NetworkBehaviour {
    public override void OnStartClient()
    {
        base.OnStartClient();
        gameObject.SetActive(IsOwner);
    }
}