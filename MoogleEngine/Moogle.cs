namespace MoogleEngine;


public static class Moogle
{
    // Aquí guardaremos nuestras propiedades estáticas
    #region Properties
    private static FileInfo[]? files; // Conjunto de documentos del Content

    private static Dictionary<int, string>[]? documentText; // Aquí se guardarán todos los términos de cada documento 
                                                            // en el orden en que aparecen en el texto

    private static Dictionary<string, int>? terms;  // Nuestro "diccionario" de términos
                                                    // Aquí se guardarán todos los términos de manera ordenada y enumerada

    private static Matrix D; // Nuestra matriz de apoyo para el modelo vectorial
                                // En ella se implementa todo el modelo vectorial utilizando TF-IDF

    private static Dictionary<string, float>? queryTerms; // Los términos de nuestro query
    
    private static Vector q;// El vector de búsqueda o query
    #endregion

    public static SearchResult Query(string query) // Función principal, donde se empieza a computar el resultado de búsqueda 
    {        
        if (D == null) LoadCache();

        QueryTerms(query);

        Q(terms, query.ToLower());

        SearchItem[] items = Items();

        if (items.Length < 5)               // Si hay menos de 5 resultados de búsqueda:
        {
            if (items.Length == 0)          // Si no hay resultados de búsqueda, devolver el resultado vacío
                return new SearchResult();
            else                            // Sino, devolver los resultados encontrados y una sugerencia de búsqueda
                return new SearchResult(items, Misc.Suggestion(query, terms, queryTerms));
        }
        else                                // Sino, devolver solo los items
            return new SearchResult(items);
    }

    // Funciones que fundamentalmente utilizan las propiedades estáticas de la clase
    #region AuxiliaryFunctions
    private static void LoadCache() // En esta función se cargan todas las propiedades que luego no cambiarán 
    {
        // * Todas las descripciones de las propiedades se encuentran en la región Properties

        DirectoryInfo directory = new DirectoryInfo(Path.Join("..", "Content")); // Dirección de la carpeta Content

        files = directory.EnumerateFiles().ToArray();

        documentText = Terms.DocumentText(files);

        terms = Terms.GetAllTerms(documentText);

        D = MathTools.TF_IDF(terms, documentText);
    }

    private static void QueryTerms(string query) // Carga todos los términos del query con un valor para el vector de búsqueda 
    {
        Dictionary<string, float> queryTerms = new Dictionary<string, float>();

        foreach (string term in Terms.GetTerms(query))
        {
            if (terms.ContainsKey(term))
            {
                if (!queryTerms.ContainsKey(term))
                {
                    queryTerms.Add(term, 1); // Si el término no se ha guardado, guárdalo
                }
                else
                {
                    queryTerms[term] += 1; // Si el término ya ha sido guardado, aumentar su valor
                }
            }
        }

        Operators.FindOperators(query, queryTerms); // Modificar el valor de los términos con el uso de operadores

        Moogle.queryTerms = queryTerms;
    }

    private static void Q(Dictionary<string, int> terms, string query) // Prepara el vector de búsqueda 
    {
        Moogle.q = new Vector(terms.Count);

        foreach (KeyValuePair<string, float> term in queryTerms)
        {
            if (terms.ContainsKey(term.Key)) // Si el término es una palabra del "diccionario"
            {
                q[terms[term.Key]] = term.Value; // Guardar el valor del término en el i-ésimo elemento del vector
            }
        }
    }

    private static SearchItem[] Items() // Devuelve los resultados de búsqueda que más se asemejan al query 
    {
        Dictionary<int, float> searchItems = CalculateScore();
        SearchItem[] items = new SearchItem[files.Length];
        int count = 0;

        // Se ordenan los searchItems por el valor del score de mayor a menor
        foreach (KeyValuePair<int, float> si in searchItems.OrderByDescending(x => x.Value))
        {
            if (si.Value == 0) // Si llegamos a un score de 0, no se devolverán este ni los siguientes
                break;
            else
            {
                string name = files[si.Key].Name;
                string snippet = Misc.Snippet(documentText, si.Key, queryTerms);
                float score = si.Value;

                items[count++] = new SearchItem(name, snippet, score);
            }
        }

        // Para devolver un SearchItem[] con la cantidad de elementos justa, sin ningún valor vacío 
        // (para conocer el verdadero tamaño)
        SearchItem[] result = new SearchItem[count];
        Array.Copy(items, result, count);
        items = result;

        return items;
    }

    private static Dictionary<int, float> CalculateScore() // Se calcula el score relacionado con cada documento 
    {
        Dictionary<int, float> searchItems = new Dictionary<int, float>();

        // Cada documento se identifica con su posición "i" en el FileInfo[] files
        for (int i = 0; i < files.Length; i++)
        {
            // Se calcula la similitud vectorial del query (en fila) con cada columna de la matriz de términos
            searchItems[i] = Vector.SimCos(D, i, q);
        }

        for (int i = 0; i < searchItems.Count; i++)
        {
            if (searchItems[i] != 0)
            {
                // Si el documento es válido hasta ahora (el score es distinto de 0) 
                // Se modifica el score por el uso de los operadores en el query
                searchItems[i] = Operators.CheckOperators(searchItems[i], documentText[i]);
            }
        }

        return searchItems;
    }
    #endregion
}