namespace MoogleEngine;

public static class MathTools
{
    // Realiza operaciones vectoriales como: Similitud vectorial, Producto escalar y Norma de vectores
    #region VectorialGeometry
    public static float SimCos(float[,] D, int j, float[] q) // Calcula la similitud del coseno o similitud vectorial
    {
        // Calcula el coseno del ángulo de el vector fila del query por un vector columna de la matriz de términos

        float[] dj = new float[D.GetLength(0)]; // Tomamos la j-ésima columna de la matriz para poder multiplicarla por el vector

        for (int i = 0; i < D.GetLength(0); i++)
        {
            dj[i] = D[i, j];
        }

        return (Norm(q) == 0) ? 0 : Mul(q, dj) / (Norm(q) * Norm(dj)); // Si la norma del vector query es 0 ocurriría una multiplicación por 0
    }

    private static float Mul(float[] a, float[] b) // Calcula el producto escalar de dos vectores 
    {
        // <a, b> = a1*b1 + a2*b2 + ... + an*bn
        
        float result = 0;

        for (int i = 0; i < a.Length; i++)
        {
            result += a[i] * b[i];
        }

        return result;
    }

    private static float Norm(float[] x) // Calcula la norma de un vector 
    {
        // ||x|| = Sqrt(<x,x>)

        return MathF.Sqrt(Mul(x, x));
    }
    #endregion

    // Realiza todas las operaciones para calcular el TF-IDF de una matriz de términos por documentos
    #region TF-IDF
    public static float[,] TF_IDF(Dictionary<string, int> terms, Dictionary<int, string>[] documentText) // Calcula el TF-IDF de una matriz de términos por documentos
    {
        // TF-IDF(t, d, D) = TF(t, d)*IDF(t, D)

        float[,] D = new float[terms.Count, documentText.Length];

        D = TF(D, documentText, terms); // Llenando nuestra matriz de valores por TF

        D = IDF(D); // Y luego aplicando el IDF
        
        return D;
    }

    private static float[,] TF(float[,] D, Dictionary<int, string>[] documentText, Dictionary<string, int> terms) // Calcula el Term Frequency de la matriz de términos
    {
        // TF (Term Frequency): TF(t, d) = f(t, d)/T(d)
        // * f(t, d): cantidad de veces que se repite un término en un documento 
        // * T(d): cantidad total de términos del del documento

        for (int j = 0; j < D.GetLength(1); j++)                        // Por cada documento de la colección (esta será nuestra columna j)
        {
            foreach (string term in documentText[j].Values)             // Por cada término que aparezca en el documento
            {
                if (term != "/n") D[terms[term.ToLower()], j] += 1;     // Aumentar su valor en uno en la matriz de términos
            }                                                           // (el .ToLower() lo utilizamos porque los términos del texto no siempre estarán en minúsculas)

            float count = 0;                                            // Guardar la cantidad total de términos del documento j
            for (int i = 0; i < D.GetLength(0); i++) count += D[i, j];
            
            for (int i = 0; i < D.GetLength(0); i++) D[i, j] /= count;  // Y dividir cada los téminos por la cantidad total de términos
        }

        return D;
    }

    private static float[,] IDF(float[,] D) // Calcula el Inverse Term Frequency de la matriz de términos
    {
        // IDF (Inverse Document Frequency): idf(t, D) = log(n/DF(t, D)) + 1
        // * n: cantidad de documentos
        // * DF: document frequency

        for (int j = 0; j < D.GetLength(1); j++)                        // Por cada columna j de documentos
        {
            for (int i = 0; i < D.GetLength(0); i++)                    // Iterar por cada término i
            {
                D[i, j] *= MathF.Log(D.GetLength(1) / DF(D, i)) + 1;    // Y multiplicar el TF guardado por el IDF calculado
            }
        }

        return D;
    }

    private static int DF(float[,] D, int i) // Calcula el Document Frequency (la cantidad de documentos en los que aparece cierto término)
    {        
        int df = 0;

        for (int j = 0; j < D.GetLength(1); j++)    // Iterando por todas las columnas de documentos en una misma fila
        {
            if (D[i, j] != 0)                       // Si el valor es cero es que el término no existe en este documento
            {
                df++;                               // Sino, entonces aumentar la cantidad de documentos que poseen este término
            }
        }

        return df;
    }
    #endregion
}