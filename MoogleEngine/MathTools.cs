namespace MoogleEngine;

public static class MathTools
{
    public static Matrix TF_IDF(Dictionary<string, int> terms, Dictionary<int, string>[] documentText) // Calcula el TF-IDF de una matriz de términos por documentos
    {   // TF-IDF(t, d, D) = TF(t, d)*IDF(t, D)
        Matrix D = new Matrix(terms.Count, documentText.Length);

        // TF (Term Frequency): TF(t, d) = f(t, d)/T(d)
        // * f(t, d): cantidad de veces que se repite un término en un documento 
        // * T(d): cantidad total de términos en el documento

        for (int j = 0; j < D.GetColumns; j++){                         // Por cada documento de la colección (esta será nuestra columna j)
            foreach (string term in documentText[j].Values)             // Por cada término que aparezca en el documento
                if (term != "/n") D[terms[term.ToLower()], j] += 1;     // Aumentar su valor en uno en la matriz de términos
                                                                        // (el .ToLower() lo utilizamos porque los términos del texto no siempre estarán en minúsculas)

            float count = 0;                                            // Guardar la cantidad total de términos del documento j
            for (int i = 0; i < D.GetRows; i++) count += D[i, j];

            for (int i = 0; i < D.GetRows; i++) D[i, j] /= count;       // Y dividir cada uno de los téminos por la cantidad total de términos
        }

        // IDF (Inverse Document Frequency): idf(t, D) = log(n/DF(t, D)) + 1
        // * n: cantidad de documentos
        // * DF: document frequency

        for (int j = 0; j < D.GetColumns; j++)                          // Por cada columna j de documentos
            for (int i = 0; i < D.GetRows; i++)                         // Iterar por cada término i
                D[i, j] *= MathF.Log(D.GetColumns / DF(D, i)) + 1;      // Y multiplicar el TF guardado por el IDF calculado
        
        return D;
    }

    private static int DF(Matrix D, int i) // Calcula el Document Frequency (la cantidad de documentos en los que aparece cierto término)
    {        
        int df = 0;

        for (int j = 0; j < D.GetColumns; j++)  // Iterando por todas las columnas de documentos en una misma fila
            if (D[i, j] != 0)                   // Si el valor es cero es que el término no existe en este documento
                df++;                           // Si no, entonces aumentar la cantidad de documentos que poseen este término

        return df;
    }
}