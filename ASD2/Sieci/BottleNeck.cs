
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
        //     (każdy element tablicy to opis jednej takiej krawędzi)
        /// </remarks>
        public static int BottleNeck(this IGraph g, IGraph c, int[] p, out int flowValue, out int cost, out IGraph flow, out Edge[] ext)
        {
            flowValue = 0;                     // ZMIENIĆ 
            cost = 0;                          // ZMIENIĆ
            flow = g.IsolatedVerticesGraph(true, g.VerticesCount*2 + 5);
            ext = new Edge[0];                 // ZMIENIĆ
            IGraph c2 = new AdjacencyMatrixGraph(true, c.VerticesCount * 2 + 5);
            

            List<Edge> used = new List<Edge>();
            int x2 = g.VerticesCount - 1;

            for (int i = 0; i < g.VerticesCount; i++)
            {
                foreach (Edge e in g.OutEdges(i))
                {
                    if (used.Contains(e)) continue;
                    flow.AddEdge(e.From, e.To, e.Weight);
                    flow.AddEdge(e.From, ++x2, int.MaxValue);
                    flow.AddEdge(x2, e.To, int.MaxValue);
                    c2.AddEdge(e.From, e.To, 0);
                    c2.AddEdge(x2, e.To, (int)c.GetEdgeWeight(e.From, e.To));
                    c2.AddEdge(e.From, x2, 0);
                    used.Add(e);               
                }
            }

            int zrodlo = g.VerticesCount * 2 + 3;
            int ujscie = g.VerticesCount * 2 + 4;

            for (int i = 0; i < p.Length; i++)
            {
                if (p[i] < 0)
                {
                    flow.AddEdge(i, ujscie, p[i]);
                    c2.AddEdge(i, ujscie, 0);
                    flowValue += Math.Abs(p[i]);
                }
                if (p[i] > 0)
                {
                    flow.AddEdge(zrodlo, i, p[i]);
                    c2.AddEdge(zrodlo, i,  0);
                    cost += p[i];
                }
            }

            cost = flowValue - cost;

            /*
            used.Clear();
            x2 = c.VerticesCount - 1;
            
           
            for (int i = 0; i < c.VerticesCount; i++)
            {
                foreach (Edge e in c.OutEdges(i))
                {
                    if (used.Contains(e)) continue;
                    c2.AddEdge(e.From, e.To, 0);
                    c2.AddEdge(++x2, e.To, (int)c.GetEdgeWeight(e.From, e.To));
                    c2.AddEdge(e.From, x2, 0);
                    used.Add(e); 
                }
            }
            */
            

            IGraph flow2 = g.IsolatedVerticesGraph(true, g.VerticesCount * 2 + 5);

            flow.MinCostFlow(c2, zrodlo, ujscie, out flow2);

            flow = flow2.Clone();

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

            int pom=0;
            foreach (int i in p)
                if (i > 0)
                    pom += i;

            if (cost == 0 && ext.Length == 0)
                return 0;
            else if (cost > 0)
                return 1;

            return 2;
        }

    }

}