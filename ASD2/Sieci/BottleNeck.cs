using System.Collections.Generic;
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
            flowValue = 0;                     
            cost = 0;                         
            flow = g.IsolatedVerticesGraph(); 
            ext = new Edge[0];                
            //return 2;     
            IGraph mincostFlow;

            int balans = 0;
            int minv = 0;
            int maxv = 0;      


            int n = g.VerticesCount;
            IGraph gnew = new AdjacencyMatrixGraph(true, 2 * n + 2);
            IGraph cnew = new AdjacencyMatrixGraph(true, 2 * n + 2);
            // <0;n-1> wierzcholki
            // n, n+1 to zrodlo i ujscie
            // <n+2;2n+1> to zdublowane wierzcholki
            //Krawedzie
            for (int i = 0; i < n; i++)
            {
                foreach (Edge e in g.OutEdges(i))
                {
                    gnew.AddEdge(e.From, e.To, e.Weight);
                    gnew.AddEdge(n + 2 + e.From, e.To, int.MaxValue);
                    gnew.AddEdge(e.From, n + 2 + e.From, int.MaxValue);
                }

                foreach (Edge e in c.OutEdges(i))
                {
                    cnew.AddEdge(e.From, e.To, 0);
                    cnew.AddEdge(e.From + n + 2, e.To, e.Weight);
                    cnew.AddEdge(e.From, e.From + n + 2, 0);
                }
            }
            //Jedno zrodlo i ujscie
            for (int i = 0; i < n; i++)
            {
                if (p[i] > 0)
                {
                    gnew.AddEdge(n, i, p[i]);
                    cnew.AddEdge(n, i, 0);
                }
                else if (p[i] < 0)
                {
                    gnew.AddEdge(i, n + 1, -p[i]);
                    cnew.AddEdge(i, n + 1, 0);
                }
            }

            cost = gnew.MinCostFlow(cnew, n, n + 1, out mincostFlow);

            flow = new AdjacencyMatrixGraph(true, n);

            for (int i = 0; i < n; i++)
                foreach (Edge e in mincostFlow.OutEdges(i))
                {
                    if (e.From == n || e.To == n + 1 || e.To == e.From + n + 2 ) continue;
                    flow.AddEdge(e.From, e.To, e.Weight + (int)mincostFlow.GetEdgeWeight(e.From + n + 2, e.To));
                }

            List<Edge> doPoprawy = new List<Edge>();

            for (int i = n + 2; i <= 2 * n + 1; i++)
                foreach (Edge e in mincostFlow.OutEdges(i))
                {
                    if (e.Weight == 0 || e.To == e.From + n + 2 || e.From == n || e.To == n + 1) continue;
                    doPoprawy.Add(new Edge(e.From - n - 2, e.To, e.Weight));
                    flow.ModifyEdgeWeight(e.From - n - 2, e.To, e.Weight);
                }

            ext = doPoprawy.ToArray();

            foreach (Edge e in mincostFlow.OutEdges(n))
                flowValue += e.Weight;

            for (int i = 0; i < p.Length; i++)
            {
                balans += p[i];
                if (p[i] > 0)
                    maxv += p[i];
                if (p[i] < 0)
                    minv += p[i];
            }

            if (balans < 0)       //wiecej wypl niz wplyw
                return 2;

            if (cost == 0 && flowValue == maxv) //nie poprawilismy, jest maxflow
                return 0;

            if (flowValue == maxv && cost != 0) //poprawilismy, udalo sie
                return 1;

            return 2;

        }

    }

}