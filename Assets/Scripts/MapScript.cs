using Unity.VisualScripting;
using UnityEngine;

public class MapScript : MonoBehaviour
{

    public Transform MapTransform;





#if UNITY_EDITOR
    [ContextMenu("Add Mesh Renderer and Layer")]
    void SetLayerAndColliderAllChildren()
    {
        var children = MapTransform.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (var child in children)
        {
            //            Debug.Log(child.name);
            child.gameObject.layer = LayerMask.NameToLayer("Ground");
            if (child.GetComponent<MeshRenderer>() != null && !child.GetComponent<MeshCollider>())
            {
                child.AddComponent<MeshCollider>();
            }
        }
    }



#endif
}
