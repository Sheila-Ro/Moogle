namespace MoogleEngine;


public static class Operators
{
    private static Dictionary<char, int> operators = new Dictionary<char, int>() // Los operadores disponibles y el valor por el que se multiplicarán por el valor numérico del término 
    {
        {'!', 0},
        {'^', 10},
        {'~', 1},
        {'*', 2}
    };

    private static List<string>? bannedTerms; // Lista de términos baneados, que no queremos que aparezcan en el resultado de búsqueda

    private static Dictionary<string, string>? relatedTerms; // Lista de términos relacionados para mejorar la búsqueda por su cercanía

    // Las funciones con las que se trabaja en el query
    #region Operators in query
    public static void FindOperators(string query, Dictionary<string, float> queryTerms) // Revisa si en el query dado aparecen uno o más operadores y realiza las operaciones debidas 
    {
        bannedTerms = new List<string>();                       // Inicializando nuestras listas 
        relatedTerms = new Dictionary<string, string>();        // de términos

        for (int i = 0; i < query.Length; i++)                  // Recorriendo todo el query
        {
            if (operators.ContainsKey(query[i]))                // Si aparece un operador válido
            {                
                int j = RegisterOperator(query, queryTerms, i); // Registrar el operador con el término relacionado
                                                                // Y realizar las operciones debidas del operador
                i = j - 1;                                      // Luego, actualizar la iteración
            }
        }
    }
    
    private static int RegisterOperator(string query, Dictionary<string, float> queryTerms, int i) // Realiza las operaciones del operador que esté en la posición i del string
    {
        (string term, int j) = Terms.GetTerm(query, i + 1);         // Obteniendo el término relacionado con el operador

        switch (query[i])
        {
            case '!':                                               // Si es '!':
                bannedTerms.Add(term);                              // "baneamos" el término correspondiente 
                queryTerms.Remove(term);                            // y lo eliminamos de la lista de términos de búsqueda
                break;

            case '^':                                               // Si es '^':
                queryTerms[term] *= operators['^'];                 // Solamente se le aumenta su valor proporcionalmente al valor del operador
                break;

            case '~':                                               // Si es '~':
                relatedTerms.Add(term, FindRelatedTerm(query, i));  // Añadir a la lista de términos relacionados los dos términos que utilizan al operador
                break;

            case '*':                                               // Si es '*':
                int k = 1;                                          // Como este es acumulable, primero revisamos 
                for (; query[i + k] == '*'; k++) ;                  // la cantidad de veces que se repite 
                queryTerms[term] *= operators['*'] * k;             // Para luego calcular su valor y aumentárselo al término correspondiente
                break;
        }

        return j;                                                   // Y por último se actualiza la iteración
    }

    private static string FindRelatedTerm(string query, int i) // Busca el término relacionado antepuesto al operador '~' 
    {
        for (; i > 0 ; i--)
        {
            if (char.IsWhiteSpace(query[i - 1])) break;     // Buscamos hasta el principio del término
        }

        (string term, i) = Terms.GetTerm(query, i);         // Para después poder devolver el término
        
        return term;
    }
    #endregion

    // Las funciones con las que se trabaja en los documentos ya teniendo las listas de términos baneados y relacionados preparadas
    #region Operators in documents
    public static float CheckOperators(float score, Dictionary<int, string> documentText) // Revisa si en un documento dado es necesario hacer cambios en el score con relación a los términos que utilizan operadores 
    {
        if (CheckBannedTerms(documentText))
        {
            return 0;                           // Si el texto contiene algún término "baneado", no queremos devolverlo, así que su score será 0
        }

        if (CheckRelatedTerms(documentText))    // Si el texto posee una pareja o más de términos que se relacionen por el operador '~'
        {
            float distances = 0;

            foreach (KeyValuePair<string, string> terms in FoundInText(documentText))   // Se recorren todas las parejas de términos
            {
                distances += FindDistance(documentText, terms);                         // Y se calcula la distancia mínima de cada par
            }
            
            return score + distances;
        }

        return score;       // Si no ocurre ninguna de estas condiciones, el score no se modifica y se puede devolver sin cambios
    }

    private static bool CheckBannedTerms(Dictionary<int, string> documentText) // Revisa si un texto posee al menos un término "baneado" 
    {
        foreach (string bannedTerm in bannedTerms)              // Por cada término de la lista
        {
            foreach (string term in documentText.Values)        // Iterando por cada término del texto
            {
                if (term.ToLower() == bannedTerm)               // Revisar si un coincide con un término de la lista de "baneados"
                {
                    return true;
                }
            }
        }

        return false;                                           // Si hemos llegado aquí es porque no se han encontrado dichos términos en el texto
    }

    private static bool CheckRelatedTerms(Dictionary<int, string> documentText) // Revisa si un texto posee al menos un par de téminos relacionados 
    {
        foreach (KeyValuePair<string, string> terms in relatedTerms)    // Por cada par de términos de la lista
        {
            foreach (string term1 in documentText.Values)               // Iterando por todos los términos del texto
            {
                if (term1.ToLower() == terms.Key)                       // Si el primer término guardado aparece, ahora hay que buscar el otro
                {
                    foreach (string term2 in documentText.Values)       // Volvemos a buscar por cada término del texto
                    {
                        if (term2.ToLower() == terms.Value)             // Y si aparece el segundo, devolver verdadero (al menos aparece un par)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;                                                   // Si llegamos aquí es que ningún par se encontró en el texto
    }

    private static Dictionary<string, string> FoundInText(Dictionary<int, string> documentText) // Crea una lista con los pares de términos relacionados que se encuentren en el texto 
    {
        Dictionary<string, string> foundInText = new Dictionary<string, string>();

        foreach (KeyValuePair<string, string> terms in relatedTerms)        // Por cada par de términos de la lista
        {
            foreach (string term1 in documentText.Values)                   // Iterando por todos los términos del texto
            {
                if (term1.ToLower() == terms.Key)                           // Si el primer término guardado aparece, ahora hay que buscar el otro
                {
                    foreach (string term2 in documentText.Values)           // Volvemos a buscar por cada término del texto
                    {
                        if (term2.ToLower() == terms.Value)                 // Y si aparece el segundo, se añaden a la nueva lista
                        {
                            if (!foundInText.ContainsKey(terms.Key))        // Los términos pueden aparecer más de una vez, y no queremos repetirlos
                            {
                                foundInText.Add(terms.Key, terms.Value);
                            }
                        }
                    }
                }
            }
        }

        return foundInText;
    }

    private static float FindDistance(Dictionary<int, string> documentText, KeyValuePair<string, string> terms) // Calcula la mínima distancia en un texto de un par de términos relacionados 
    {
        int minDis = int.MaxValue;

        for (int i = 0; i < documentText.Count; i++)
        {
            if (documentText[i].ToLower() == terms.Key)                     // Buscando dónde se encuentra el primer término guardado
            {
                for (int j = i + 1; j < documentText.Count; j++)
                {
                    if (documentText[j].ToLower() == terms.Value)           // Buscando el segundo término por delante del primero
                    {
                        if (j - i < minDis) { minDis = j - i; break; }      // Si ya se encontró una vez, esta es la distancia mínima encontrada
                    }                                                       // Cualquier aparición del segundo término más adelante no cambiará esta distancia
                }
                for (int j = i - 1; j >= 0; j--)
                {
                    if (documentText[j].ToLower() == terms.Value)           // Buscando el segundo término por detrás del primero
                    {
                        if (i - j < minDis) { minDis = i - j; break; }      // Si ya se encontró una vez, esta es la distancia mínima encontrada
                    }                                                       // Cualquier aparición del segundo término más adelante no cambiará esta distancia
                }
            }
        }

        return (float)1/minDis;  // Finalmente queremos devolver una proporcionalidad inversa a la distancia mínima encontrada (a menor distancia, mayor valor)
    }
    #endregion
}