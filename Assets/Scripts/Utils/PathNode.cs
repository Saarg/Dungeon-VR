using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour {

    [SerializeField]
    List<PathNode> nextNodes = new List<PathNode>();

    [SerializeField]
    Vector2 zone;

    public PathNode GetNextPathNodes()
    {
        if (nextNodes.Count == 0)
            return null;
        else
            return nextNodes[Random.Range(0, nextNodes.Count)];
    }

    public bool ContainsNode(PathNode node)
    {
        return nextNodes.Contains(node);
    }

    public Vector3 GetOffsetPosition()
    {
        return new Vector3(transform.position.x - zone.x / 2 + Random.Range(0f, zone.x), transform.position.y, transform.position.z - zone.y/2 + Random.Range(0f, zone.y));
    }

    void OnDrawGizmosSelected()
    {
        // Display the explosion radius when selected
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(gameObject.transform.position, new Vector3(zone.x, 5f, zone.y));
    }
}
