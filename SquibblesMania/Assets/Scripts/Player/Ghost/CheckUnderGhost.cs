using UnityEngine;

public class CheckUnderGhost : MonoBehaviour
{
    public Transform currentBlockGhostOn;
   
    private void Start()
    {
        currentBlockGhostOn = GameManager.Instance.currentPlayerTurn.currentBlockPlayerOn;
    }
    
    public void GhostMoved()
    {
        var t = transform.position;

        if (Physics.Raycast(new Vector3(t.x, t.y + 0.5f, t.z),-transform.up, out var hit, 1.1f))
        {
            if (hit.collider.gameObject.GetComponent<Node>() != null)
            {
                currentBlockGhostOn = hit.transform;
            }
        } 
    }
}
