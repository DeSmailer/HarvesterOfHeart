using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class PathFinder : MonoBehaviour
{
    public List<Vector2> PathToTarget;

    List<Node> ChechedNodes = new List<Node>();
    List<Node> WaitingNodes = new List<Node>();

    private GameObject Target;

    public LayerMask SolidLayer;

    public List<Vector2> GetPath(Vector2 target)
    {        
        PathToTarget = new List<Vector2>();
        ChechedNodes = new List<Node>();
        WaitingNodes = new List<Node>();

        Vector2 StartPosition = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
        Vector2 TargetPosition = new Vector2(Mathf.Round(target.x), Mathf.Round(target.y));

        if (StartPosition == TargetPosition)
            return PathToTarget;

        Node startNode = new Node(0, StartPosition, TargetPosition, null);
        ChechedNodes.Add(startNode);
        WaitingNodes.AddRange(GetNeighbourNodes(startNode));

        while (WaitingNodes.Count > 0)
        {
            Node nodeCheck = WaitingNodes.Where(x => x.F == WaitingNodes.Min(y => y.F)).FirstOrDefault();

            if (nodeCheck.Position == TargetPosition)
            {
                return CalculatePathFromNode(nodeCheck);
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
                //else
                //{
                //    var sameNode = ChechedNodes.Where(x => x.Position == nodeCheck.Position).ToList();
                //    for (int i = 0; i < sameNode.Count; i++)
                //    {
                //        if(sameNode[i].F>nodeCheck.F)
                //    }
                //}
            }
        }

        return PathToTarget;
    }

    public List<Vector2> CalculatePathFromNode(Node node)
    {
        var path = new List<Vector2>();
        Node currentNode = node;

        while(currentNode.previousNode != null)
        {
            path.Add(new Vector2(currentNode.Position.x, currentNode.Position.y));
            currentNode = currentNode.previousNode;
        }
        return path;
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


        //эксперимент
        //Neighbour.Add(new Node(node.G + 1, new Vector2(
        //    node.Position.x - 1, node.Position.y-1),
        //    node.targetPosition,
        //    node));
        //Neighbour.Add(new Node(node.G + 1, new Vector2(
        //     node.Position.x + 1, node.Position.y-1),
        //     node.targetPosition,
        //     node));
        //Neighbour.Add(new Node(node.G + 1, new Vector2(
        //     node.Position.x+1, node.Position.y + 1),
        //     node.targetPosition,
        //     node));
        //Neighbour.Add(new Node(node.G + 1, new Vector2(
        //     node.Position.x-1, node.Position.y + 1),
        //     node.targetPosition,
        //     node));
        return Neighbour;
    }
    private void OnDrawGizmos()
    {
        foreach(var item in ChechedNodes)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(new Vector2(item.Position.x, item.Position.y), 0.1f);
        }
        if(PathToTarget != null)
        foreach(var item in PathToTarget)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(new Vector2(item.x, item.y), 0.2f);
            }
    }
    private void Update()
    {
       // PathToTarget = GetPath(Target.transform.position);
    }
}


public class Node
{
    public Vector2 Position;
    public Vector2 targetPosition;
    public Node previousNode;
    public int F; //F=G+H
    public int G; //расстояние от старта до ноды
    public int H; //расстояние от ноды до цели

    public Node(int g, Vector2 nodePosition, Vector2 targetPosition, Node previousNode)
    {
        Position = nodePosition;
        this.targetPosition = targetPosition;
        this.previousNode = previousNode;
        G = g;
        H = (int)Mathf.Abs(targetPosition.x - Position.x) + (int)Mathf.Abs(targetPosition.y - Position.y);
        F = G + H;
    }

}
