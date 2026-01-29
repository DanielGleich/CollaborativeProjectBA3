using UnityEngine;
using UnityEngine.Events;

public class DetachObjectOnDestroy : MonoBehaviour
{
    [SerializeField] GameObject objectToDetach;
    public UnityEvent OnObjectDetached;

    private void OnDestroy()
    {
        objectToDetach.transform.parent = null;
        OnObjectDetached?.Invoke();
    }
}
