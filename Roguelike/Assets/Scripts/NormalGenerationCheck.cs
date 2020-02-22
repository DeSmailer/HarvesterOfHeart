using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NormalGenerationCheck : MonoBehaviour
{
    public List<Vector2> PathToTarget;
    List<Node> ChechedNodes = new List<Node>();
    List<Node> WaitingNodes = new List<Node>();
    public LayerMask SolidLayer;

    public int GetAvailable(Vector2 start, Vector2 target)
    {
        PathToTarget = new List<Vector2>();
        ChechedNodes = new List<Node>();
        WaitingNodes = new List<Node>();

        Vector2 StartPosition = new Vector2(Mathf.Round(start.x), Mathf.Round(start.y));
        Vector2 TargetPosition = new Vector2(Mathf.Round(target.x), Mathf.Round(target.y));

        if (StartPosition == TargetPosition)
            return 1;

        Node startNode = new Node(0, StartPosition, TargetPosition, null);
        ChechedNodes.Add(startNode);
        WaitingNodes.AddRange(GetNeighbourNodes(startNode));

        while (WaitingNodes.Count > 0)
        {
            Node nodeCheck = WaitingNodes.Where(x => x.F == WaitingNodes.Min(y => y.F)).FirstOrDefault();

            if (nodeCheck.Position == TargetPosition)
            {
                return 1;
            }
            var walkable = !Physics2D.OverlapCircle(nodeCheck.Position, 0.1f, SolidLayer);
            if (!walkable)
            {
                WaitingNodes.Remove(nodeCheck);
                ChechedNodes.Add(nodeCheck);
            }
            else
            {
                WaitingNodes.Remove(nodeCheck);
                if (!ChechedNodes.Where(x => x.Position == nodeCheck.Position).Any())
                {
                    ChechedNodes.Add(nodeCheck);
                    WaitingNodes.AddRange(GetNeighbourNodes(nodeCheck));
                }
            }
        }
        return 0;
    }
    
    List<Node> GetNeighbourNodes(Node node)
    {
        var Neighbour = new List<Node>();

        Neighbour.Add(new Node(node.G + 1, new Vector2(
            node.Position.x - 1, node.Position.y),
            node.targetPosition,
            node));
        Neighbour.Add(new Node(node.G + 1, new Vector2(
             node.Position.x + 1, node.Position.y),
             node.targetPosition,
             node));
        Neighbour.Add(new Node(node.G + 1, new Vector2(
             node.Position.x, node.Position.y - 1),
             node.targetPosition,
             node));
        Neighbour.Add(new Node(node.G + 1, new Vector2(
             node.Position.x, node.Position.y + 1),
             node.targetPosition,
             node));

        return Neighbour;
    }    
}
