using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASD.Graph
{
    public static class GraphComparerExtender
    {
        public static bool Compare(this IGraph g, IGraph h)
        {
            bool comparision = true;
            for (int i = 0; i < g.VerticesCount; i++)
                foreach (Edge e in g.OutEdges(i))
                    comparision &= e.Weight == h.GetEdgeWeight(e.From, e.To);
            return comparision;
        }
    }
}
