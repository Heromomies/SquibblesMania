using UnityEngine;

public class CheckUnderGhost : MonoBehaviour
{
    [HideInInspector] public Transform currentBlockGhostOn;
    public LayerMask layerBloc;
    private void Start()
    {
        currentBlockGhostOn = GameManager.Instance.currentPlayerTurn.currentBlocPlayerOn;
    }
    
    public void GhostMoved()
    {
        var t = transform.position;

        if (Physics.Raycast(new Vector3(t.x, t.y + 0.5f, t.z),-transform.up, out var hit, 1.1f, layerBloc))
        {
            if (hit.collider.gameObject.GetComponent<Node>() != null)
            {
                currentBlockGhostOn = hit.transform;
            }
        } 
    }
}
