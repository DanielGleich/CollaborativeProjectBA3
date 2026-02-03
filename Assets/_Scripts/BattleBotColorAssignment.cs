using FishNet.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TeamMaterials
{
    public int teamId;
    public List<Material> materials;
}
public class BattleBotColorAssignment : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MeshRenderer targetMeshRenderer;

    [Header("Settings")]
    [SerializeField] private List<TeamMaterials> teamMaterials;
    int ownerId = -1;

    private void OnEnable()
    {
        TeamManager.OnTeamReady += AssignMaterials;
    }

    private void OnDisable()
    {
        TeamManager.OnTeamReady -= AssignMaterials;
    }

    private void AssignMaterials(Team team)
    {
        if (ownerId == -1)
            ownerId = gameObject.transform.root.GetComponent<NetworkObject>().OwnerId;

        if (ownerId == -1) return;

        if (team.scientistPlayerClientId == ownerId || team.ratPlayerClientId == ownerId)
        {
            foreach (TeamMaterials tm in teamMaterials)
            {
                if (tm.teamId == team.id)
                { 
                    targetMeshRenderer?.SetMaterials(tm.materials);
                    return;
                }
            }
        }
    }
}
