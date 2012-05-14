namespace ASD.Text
{
    using System.Collections.Generic;

    /// <summary>
    /// Klasa statyczna zawierająca implementację różnych metod wyszukiwania wzorców w tekście
    /// </summary>
    public static partial class StringMatching
    {
        /// <summary>
        /// Wyszukiwanie wzorca w tekście algorytmem Karpa-Rabina
        /// </summary>
        /// <param name="y">Badany tekst</param>
        /// <param name="x">Szukany wzorzec</param>
        /// <returns>Lista zawierająca początkowe indeksy wystąpień wzorca x w tekście y</returns>
        public static List<int> RK(string y, string x)
        {
            int n, m, i;
            n = y.Length;
            m = x.Length;

            int hsub = x.GetHashCode();
            int hs = y.Substring(0, m).GetHashCode();

            List<int> ml = new List<int>();
            for (i = 0; i <= n - m ; i++)
            {
                if (hs == hsub)
                    if (y.Substring(i, m) == x)
                        ml.Add(i);
                if (i < n - m)
                    hs = y.Substring(i + 1, m).GetHashCode();
            }

            return ml;
        }

        /*
        function RabinKarpSet(string s[1..n], set of string subs, m):
     set hsubs := emptySet
     for each sub in subs
         insert hash(sub[1..m]) into hsubs
     hs := hash(s[1..m])
     for i from 1 to n-m+1
         if hs ∈ hsubs and s[i..i+m-1] ∈ subs
             return i
         hs := hash(s[i+1..i+m])
     return not found
        */

        public static int RKS(string y, List<string> x, int m)
        {
            int n = y.Length;
            List<int> hsubs = new List<int>();
            foreach (string sub in x)
                hsubs.Add(sub.GetHashCode());
            int hs = y.Substring(0, m).GetHashCode();

            for (int i = 0; i < n - m; i++)
            {
                if (hsubs.Contains(hs) && x.Contains(y.Substring(i, m)))
                    return i;
                hs = y.Substring(i + 1, m).GetHashCode();
            }

            return -1;
        }
    }
}