﻿
using System.Collections.Generic;
using System;
namespace ASD.Graph
{

    public static class BottleNeckExtender
    {

        /// <summary>
        /// Wyszukiwanie "wąskich gardeł" w sieci przesyłowej
        /// </summary>
        /// <param name="g">Graf przepustowości krawędzi</param>
        /// <param name="c">Graf kosztów rozbudowy sieci (kosztów zwiększenia przepustowości)</param>
        /// <param name="p">Tablica mocy produkcyjnych/zapotrzebowania w poszczególnych węzłach</param>
        /// <param name="flowValue">Maksymalna osiągalna produkcja (parametr wyjściowy)</param>
        /// <param name="cost">Koszt rozbudowy sieci, aby możliwe było osiągnięcie produkcji flowValue (parametr wyjściowy)</param>
        /// <param name="flow">Graf przepływu dla produkcji flowValue (parametr wyjściowy)</param>
        /// <param name="ext">Tablica rozbudowywanych krawędzi (parametr wyjściowy)</param>
        /// <returns>
        /// 0 - zapotrzebowanie można zaspokoić bez konieczności zwiększania przepustowości krawędzi<br/>
        /// 1 - zapotrzebowanie można zaspokoić, ale trzeba zwiększyć przepustowość (niektórych) krawędzi<br/>
        /// 2 - zapotrzebowania nie można zaspokoić (zbyt małe moce produkcyjne lub nieodpowiednia struktura sieci
        ///     - można jedynie zwiększać przepustowości istniejących krawędzi, nie wolno dodawać nowych)
        /// </returns>
        /// <remarks>
        /// Każdy element tablicy p opisuje odpowiadający mu wierzchołek<br/>
        ///    wartość dodatnia oznacza moce produkcyjne (wierzchołek jest źródłem)<br/>
        ///    wartość ujemna oznacza zapotrzebowanie (wierzchołek jest ujściem),
        ///       oczywiście "możliwości pochłaniające" ujścia to moduł wartości elementu<br/>
        ///    "zwykłym" wierzchołkom odpowiada wartość 0 w tablicy p<br/>
        /// <br/>
        /// Jeśli funkcja zwraca 0, to<br/>
        ///    parametr flowValue jest równy modułowi sumy zapotrzebowań<br/>
        ///    parametr cost jest równy 0<br/>
        ///    parametr ext jest pustą (zeroelementową) tablicą<br/>
        /// Jeśli funkcja zwraca 1, to<br/>
        ///    parametr flowValue jest równy modułowi sumy zapotrzebowań<br/>
        ///    parametr cost jest równy sumarycznemu kosztowi rozbudowy sieci (zwiększenia przepustowości krawędzi)<br/>
        ///    parametr ext jest tablicą zawierającą informację o tym o ile należy zwiększyć przepustowości krawędzi<br/>
        /// Jeśli funkcja zwraca 2, to<br/>
        ///    parametr flowValue jest równy maksymalnej możliwej do osiągnięcia produkcji
        ///      (z uwzględnieniem zwiększenia przepustowości)<br/>
        ///    parametr cost jest równy sumarycznemu kosztowi rozbudowy sieci (zwiększenia przepustowości krawędzi)<br/>
        ///    parametr ext jest tablicą zawierającą informację o tym o ile należy zwiększyć przepustowości krawędzi<br/>
        /// Uwaga: parametr ext zawiera informacje jedynie o krawędziach, których przepustowości trzeba zwiększyć
        ///     (każdy element tablicy to opis jednej takiej krawędzi)
        /// </remarks>
        public static int BottleNeck(this IGraph g, IGraph c, int[] p, out int flowValue, out int cost, out IGraph flow, out Edge[] ext)
        {
            flowValue = 0;                    // ZMIENIĆ 
            cost = 0;                          // ZMIENIĆ
            flow = new AdjacencyMatrixGraph(true, g.VerticesCount); 
            ext = new Edge[0];                  // ZMIENIĆ


            IGraph g1 = new AdjacencyMatrixGraph(true, g.VerticesCount * 2 + 2);
            IGraph c1 = new AdjacencyMatrixGraph(true, g.VerticesCount * 2 + 2);
            IGraph flow2; 
            List<Edge> ext1 = new List<Edge>();
            int wplywy = 0, minw = 0, maxw= 0;
              
            
            for (int i = 0; i < g.VerticesCount; i++)
            {
                foreach (var e in g.OutEdges(i))
                {
                    g1.AddEdge(i, e.To, e.Weight);
                    g1.AddEdge(i + g.VerticesCount + 2, e.To, int.MaxValue);
                    g1.AddEdge(i, i + g.VerticesCount + 2, int.MaxValue);
                } 
                foreach (var e in c.OutEdges(i))
                {
                    c1.AddEdge(i, e.To, 0);  
                    c1.AddEdge(i + g.VerticesCount + 2, e.To, e.Weight);
                    c1.AddEdge(i, i + g.VerticesCount + 2, 0);
                }
            }


            for (int i = 0; i < g.VerticesCount; i++)
                if (p[i] > 0)
                {
                    g1.AddEdge(g.VerticesCount, i, Math.Abs(p[i]));
                    c1.AddEdge(g.VerticesCount, i,  0);
                }
                else if (p[i] < 0)
                {
                    g1.AddEdge(i, g.VerticesCount+ 1, Math.Abs(p[i]));
                    c1.AddEdge(i, g.VerticesCount + 1, 0);
                }
                 
            cost  = g1.MinCostFlow(c1, g.VerticesCount, g.VerticesCount + 1, out flow2);
              
            for (int i = 0; i < g.VerticesCount;  i++)
                foreach (var e in flow2.OutEdges(i))
                    if (i != g.VerticesCount && e.To != g.VerticesCount + 1 && e.To != i + g.VerticesCount + 2)
                        flow.AddEdge(i, e.To, e.Weight +(int)flow2.GetEdgeWeight(i + g.VerticesCount + 2, e.To));
               
            for (int i = g.VerticesCount + 2; i<= 2 * g.VerticesCount +1; i++)
                foreach (var e in flow2.OutEdges(i))
                    if (e.Weight != 0 && e.To != i+ g.VerticesCount + 2 && i != g.VerticesCount && e.To != g.VerticesCount + 1)
                    {
                        ext1.Add(new Edge(i - g.VerticesCount - 2, e.To, e.Weight));
                        flow.ModifyEdgeWeight(i -g.VerticesCount - 2, e.To, e.Weight);
                    }
             
            ext = ext1.ToArray();

            foreach (var e in flow2.OutEdges(g.VerticesCount))
                flowValue+=e.Weight;

            for (int i = 0; i < p.Length; i++)
            {
                wplywy += p[i];
                if (p[i] > 0)
                    maxw += Math.Abs(p[i]); 
                if (p[i] < 0)
                    minw -= Math.Abs(p[i]);
            }

            return (wplywy < 0) ? 2 : ((flowValue == maxw) ? ((cost == 0) ? 0 : 1) : 2);

        }

    }

}