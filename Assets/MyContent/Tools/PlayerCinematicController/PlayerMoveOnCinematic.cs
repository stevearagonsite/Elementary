using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMoveOnCinematic : MonoBehaviour
{
    [SerializeField]
    public List<CinematicNode> nodes;

    public CinematicNode AddNewNode(Vector3 pos , float radius = 0)
    {
        if(nodes == null)
        {
            nodes = new List<CinematicNode>();
        }
        var newNode = new CinematicNode(pos, radius);
        nodes.Add(newNode);

        return newNode;
    }

    public void DeleteNode(CinematicNode node)
    {
        if (nodes.Contains(node))
        {
            nodes.Remove(node);
        }
    }
}
[Serializable]
public class CinematicNode
{
    public Vector3 position;
    public float radius;

    public CinematicNode(Vector3 pos, float r)
    {
        position = pos;
        radius = r;
    }

}
