
namespace ASD.Text
{
    using System.Collections.Generic;

    /// <summary>
    /// Klasa statyczna zawierająca implementację różnych metod wyszukiwania wzorców w tekście
    /// </summary>
    public static partial class StringMatching
    {

        /// <summary>
        /// Wyszukiwanie wzorca w tekście algorytmem naiwnym
        /// </summary>
        /// <param name="y">Badany tekst</param>
        /// <param name="x">Szukany wzorzec</param>
        /// <returns>Lista zawierająca początkowe indeksy wystąpień wzorca x w tekście y</returns>
        public static List<int> Simple(string y, string x)
        {
            int n, m, i, j;
            n = y.Length;
            m = x.Length;
            List<int> ml = new List<int>();
            for (i = 0; i <= n - m; ++i)
            {
                for (j = 0; j < m && y[i + j] == x[j]; ++j) ;
                if (j == m) ml.Add(i);
            }
            return ml;
        }

        /// <summary>
        /// Wyszukiwanie wzorca w tekście algorytmem "naiwnym wstecz"
        /// </summary>
        /// <param name="y">Badany tekst</param>
        /// <param name="x">Szukany wzorzec</param>
        /// <returns>Lista zawierająca początkowe indeksy wystąpień wzorca x w tekście y</returns>
        public static List<int> SimpleBackward(string y, string x)
        {
            int n, m, i, j;
            n = y.Length;
            m = x.Length;
            List<int> ml = new List<int>();
            for (i = 0; i <= n - m; ++i)
            {
                for (j = m - 1; j >= 0 && y[i + j] == x[j]; --j) ;
                if (j == -1) ml.Add(i);
            }
            return ml;
        }

    } // class StringMatching

} // namespace ASD.Text