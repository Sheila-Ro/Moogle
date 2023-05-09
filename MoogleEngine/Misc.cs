namespace MoogleEngine;


public static class Misc
{
    // Realiza las operaciones para calcular un pedazo de texto relacionado con la búsqueda
    #region Snippet
    public static string Snippet(Dictionary<int, string>[] documentText, int i, Dictionary<string, float> queryTerms) // Devuelve un pedazo del texto con respecto a los términos mostrados en el query 
    {
        Dictionary<int, string> text = documentText[i]; // Tomando el texto del i-ésimo documento

        foreach (KeyValuePair<string, float> qi in queryTerms.OrderByDescending(x => x.Value)) // Por cada término que aparezca en el query, 
        {                                                                                      // (ordenado por mayor importancia)
            foreach (string term in text.Values)                                               // Por cada término que aparece en el texto
            {
                if (qi.Key == term.ToLower())                                                  // Si el término del query coincide con uno del texto
                {                                                                              // (el .ToLower() lo utilizamos porque los términos del texto no siempre estarán en minúsculas)
                    return CreateSnippet(text, qi.Key);                                        // Comenzar a crear el snippet basados en ese término
                }
            }
        }

        return "";
    }

    private static string CreateSnippet(Dictionary<int, string> text, string term) // Crea un pedazo de texto basado en la primera instancia de un término dado
    {
        string snippet = "";
        int j = 0; // Utilizaremos esta j globalmente
        
        // Una vez que se haya encontrado la primera instancia del término...
        for (; j < text.Count; j++) { if (text[j].ToLower() == term) break; }    

        // ...Buscar dónde comienza la línea en donde aparece el término...
        for (; j > 0; j--) { if (text[j - 1] == "/n") break; }

        // ...Y comenzar a formar el snippet desde ese punto
        for (; j < text.Count; j++)
        {
            if (snippet.Length >= 300)                      // Una vez que el snippet haya pasado una cierta cantidad de caracteres, devolver
                break;
            
            if (text[j] == "/n")                            // Si el término encontrado es un salto de línea, ignorarlo y continuar
                continue;
            else                                            // Sino, añadir el término al snippet
                snippet += string.Format("{0} ", text[j]);
        }

        if (snippet.Length > 1) snippet = snippet.Remove(snippet.Length - 1);
        return snippet;
    }
    #endregion

    // Realiza las operaciones para calcular una sugerencia que tenga relación con la búsqueda original
    #region Suggestion
    public static string Suggestion(string query, Dictionary<string, int> terms, Dictionary<string, float> queryTerms) // Devuelve una sugerencia basada en los términos del query
    {
        string[] t = terms.Keys.ToArray(); // Guarda todos los términos del diccionario (no nos importa su posición i en estos momentos)
        string suggestion = "";

        foreach (string qi in queryTerms.Keys) // Recorriendo cada término del query
        {
            int[] lev = new int[t.Length];

            for (int i = 0; i < t.Length; i++)
            {
                lev[i] = LevenshteinDistance(qi, t[i]); // Calcular la distancia de Levenshtein para todos los términos del diccionario
            }

            suggestion += FindMostSimilar(lev, t); // Añadir el término más semejante encontrado
        }

        if (suggestion != "") suggestion = suggestion.Remove(suggestion.Length - 1); // Remover el espacio sobrante al final

        return suggestion; // Y finalmente devolver la sugerencia
    }

    private static int LevenshteinDistance(string a, string b) // Calcula la distancia de Levenshtein de un término "orígen" a otro "destino" 
    {
        int cost = 0;
        int m = a.Length;
        int n = b.Length;
        
        if (n == 0) return m;
        if (m == 0) return n;

        int[,] d = new int[m + 1, n + 1]; 

        // Llenar la primera columna y la primera fila.
        for (int i = 0; i <= m; i++) d[i, 0] = i;
        for (int j = 0; j <= n; j++) d[0, j] = j;
        
        /// Recorrer la matriz llenando cada uno de los pesos.
        for (int i = 1; i <= m; i++)
        {
            for (int j = 1; j <= n; j++)
            {       
                /// Si son iguales en posiciones equidistantes el peso es 0
                /// de lo contrario el peso suma a uno.
                cost = (a[i - 1] == b[j - 1]) ? 0 : 1;  
                
                d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
            }
        }

        return d[m, n]; 
    }
    
    private static string FindMostSimilar(int[] lev, string[] t) // Busca el término más similar al término introducido por la distancia de Levenshtein 
    {
        for (int i = 0; i <= int.MaxValue; i++) // Recorriendo por cada valor posible
        {
            for (int j = 0; j < lev.Length; j++) // Recorriendo el array de distancias de Levenshtein
            {
                if (lev[j] == i)
                {
                    return string.Format("{0} ", t[j]); // Devolver el primer término que más se asemeje al original
                }
            }
        }

        return "";
    }
    #endregion    
}