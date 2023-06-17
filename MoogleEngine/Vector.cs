public class Vector
{
    private float[] vector;

    public Vector(int n) 
    {
        this.vector = new float[n];
    }

    public Vector(float[] vector) 
    {
        this.vector = new float[vector.Length];

        for (int i = 0; i < vector.Length; i++)
        {
            this.vector[i] = vector[i];
        }
    }

    public float this[int index]
    {
        get 
        {
            if (index >= 0 && index < this.Dimension) 
                return vector[index];
            else 
                throw new IndexOutOfRangeException();
        }
        set 
        {  
            if (index >= 0 && index < this.Dimension) 
                vector[index] = value;
            else 
                throw new IndexOutOfRangeException();
        }
    }

    public int Dimension 
    {
        get 
        {
            return vector.Length;
        }
    }
    
    private float Mul(Vector other) // Calcula el producto escalar de dos vectores 
    {
        if (this.Dimension != other.Dimension) 
            throw new IndexOutOfRangeException();
        
        float result = 0;

        for (int i = 0; i < this.Dimension; i++)
        {
            result += this[i] * other[i];   // <a, b> = a1*b1 + a2*b2 + ... + an*bn
        }

        return result;
    }
    
    private float Norm() // Calcula la norma de un vector 
    {
        return MathF.Sqrt(Mul(this));   // ||x|| = Sqrt(<x,x>)
    }

    public static float SimCos(Matrix D, int j, Vector q) // Calcula la similitud del coseno o similitud vectorial
    {
        // Calcula el coseno del ángulo de el vector fila del query por un vector columna de la matriz de términos
        // cos(q, dj) = <q, dj>/||q||*||dj||

        Vector dj = new Vector(D.GetRows); // Tomamos la j-ésima columna de la matriz para poder multiplicarla por el vector

        for (int i = 0; i < dj.Dimension; i++)
        {
            dj[i] = D[i, j];
        }

        return (q.Norm() == 0) ? 0 : q.Mul(dj) / (q.Norm() * dj.Norm()); // Si la norma del vector query es 0 ocurriría una multiplicación por 0
    }
}