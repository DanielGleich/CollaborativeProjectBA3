using UnityEngine;
using FishNet.Object;

/// <summary>
/// Sets all rigibodies in children to kinematic if the object is not owned by the client 
/// </summary>
public class DisableRigibodiesIfNotOwner : NetworkBehaviour
{
     public override void OnStartClient()
     {
          base.OnStartClient();
          if (IsOwner) 
               return;
          var rbs = GetComponentsInChildren<Rigidbody>();
          foreach(var r in rbs)
          {
               r.isKinematic = true;
          }

     }
}
