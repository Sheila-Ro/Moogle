namespace MoogleEngine;


public class Terms
{
    public static Dictionary<int, string>[] DocumentText(FileInfo[] files) // Devuelve todos los textos de cada documento en forma de diccionarios 
    {
        Dictionary<int, string>[] documentText = new Dictionary<int, string>[files.Length];

        for (int i = 0; i < files.Length; i++)
        {
            documentText[i] = Text(files[i]);
        }

        return documentText;
    }

    private static Dictionary<int, string> Text(FileInfo file) // Devuelve el texto del documento dado en forma de diccionario 
    {
        Dictionary<int, string> text = new Dictionary<int, string>();

        using (StreamReader sr = new StreamReader(file.FullName))
        {
            string line = sr.ReadLine();
            int i = 0;

            while (line != null)
            {
                foreach (string term in GetTerms(line)) // Obteniendo todos los términos de cada línea
                {
                    text[i++] = term; // Cada término del texto se guardará en la i-ésima posición del diccionario
                }

                text[i++] = "/n"; // Cada salto de línea se representa como un "/n" (no deja guardarlo de la forma "\n")
                line = sr.ReadLine();
            }
        }

        return text;
    }

    public static Dictionary<string, int> GetAllTerms(Dictionary<int, string>[] documentText) // Genera "nuestro diccionario" de términos 
    {
        Dictionary<string, int> terms = new Dictionary<string, int>();

        foreach (Dictionary<int, string> text in documentText)  // Recorriendo todos los documentos
        {
            foreach (string term in text.Values)                // Por cada término que aparece en el texto
            {
                if (term != "/n")                               // Si el término no es un salto de línea (no queremos guardar esto)
                {
                    if (!terms.ContainsKey(term.ToLower()))     // Y si el término no se ha guardado aún en el diccionario
                    {
                        terms.Add(term.ToLower(), 0);           // Se añade el término al diccionario 
                                                                // (el .ToLower() lo utilizamos porque los términos del texto no siempre estarán en minúsculas)
                                                                // (el valor de int lo dejamos para más adelante)
                    }
                }
            }
        }

        int count = 0;
        foreach (KeyValuePair<string, int> term in terms.OrderBy(x => x.Key)) // Luego ordenamos alfabéticamente nuestro diccionario
        {
            terms[term.Key] = count++;                                        // Y a cada término le damos el valor i-ésimo
                                                                              // Este valor será la fila del término en la matriz resultante
        }

        return terms; // Y finalmente devolvemos nuestro diccionario resultante
    }
    
    public static List<string> GetTerms(string expression) // Devuelve todos los términos que aparecen en un string dado
    {
        List<string> terms = new List<string>();
        string term;

        for (int i = 0; i < expression.Length; i++) // Recorriendo cada carácter del string
        {
            (term, i) = GetTerm(expression, i); // Se intenta formar un nuevo término iterando cada carácter del string
            if (term != "") terms.Add(term); // Si se ha logrado formar un término, se añade este a la lista de términos
        }

        return terms;
    }

    public static (string, int) GetTerm(string expression, int i) // Devuelve un "término" y actualiza el valor del "i" dado
    {
        string term = "";

        while (term == "" && i < expression.Length)     // Mientras el término no tenga una primera letra, 
        {                                               // seguir iterando por i
            term = AddChar(term, expression, i);
            i++;
        }

        // Una vez que el término ha comenzado a formarse:
        while (i < expression.Length && char.IsLetter(expression[i]))   // Mientras que se siga encontrando una letra, 
        {                                                               // seguir añadiendo caracteres al término
            term = AddChar(term, expression, i);
            i++;
        }

        return (term, i);
    }

    private static string AddChar(string term, string expression, int i) // Actualiza un "término" dado añadiéndole el siguiente carácter 
    {
        if (char.IsLetter(expression[i])) // Si el i-ésimo carácter de la expresión es una letra, añadírsela al término
        {
            term += expression[i];
        }

        return term;
    }
}