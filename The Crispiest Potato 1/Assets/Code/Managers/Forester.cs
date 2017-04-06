using System.Collections.Generic;
using UnityEngine;

class Forester
{
    List<ResourceNodeBody> nodes = new List<ResourceNodeBody>();

    public Forester()
    {
    }

    public void NewDepleted(ResourceNodeBody body)
    {
        nodes.Add(body);
    }

    public void RegrowthTick()
    {
        int doneCount = 0;
        foreach (var node in nodes)
        {
            node.RegrowingTimer -= Time.deltaTime;
            if (node.RegrowingTimer <= 0)
            {
                doneCount++;
                node.Regrown();
            }
        }

        if (doneCount > 0)
            nodes.RemoveAll(n => n.RegrowingTimer <= 0);
    }
}