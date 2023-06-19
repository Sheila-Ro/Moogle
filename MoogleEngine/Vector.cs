public class Vector
{
    private float[] vector;

    public Vector(int n) // Crea una instancia de la clase Vector desde cero 
    {
        this.vector = new float[n];
    }

    public Vector(float[] vector) // Crea una instancia de la clase Vector a partir de un array de float dado 
    {
        this.vector = new float[vector.Length];

        for (int i = 0; i < vector.Length; i++)
        {
            this.vector[i] = vector[i];
        }
    }

    public Vector(Matrix matrix) // Crea una instancia de la clase Vector a partir de una matriz de 1xn o nx1 
    {
        if (matrix.GetRows != 1 && matrix.GetColumns != 1)
            throw new IndexOutOfRangeException();
        
        if (matrix.GetRows == 1)
        {
            this.vector = new float[matrix.GetColumns];
            for (int i = 0; i < matrix.GetColumns; i++) vector[i] = matrix[0, i];
        }
        if (matrix.GetColumns == 1)
        {
            this.vector = new float[matrix.GetRows];
            for (int i = 0; i < matrix.GetRows; i++) vector[i] = matrix[i, 0];
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

    private float MulM(Vector other) // Calcula el producto escalar de dos vectores a través de la multiplicación por matrices 
    {
        if (this.Dimension != other.Dimension) 
            throw new IndexOutOfRangeException();
        
        Matrix a = new Matrix(1, this.Dimension);
        Matrix b = new Matrix(other.Dimension, 1);

        for (int i = 0; i < this.Dimension; i++)
        {
            a[0, i] = this[i];
            b[i, 0] = other[i];
        }

        Matrix result = a.Mul(b);   // <a, b> = a1*b1 + a2*b2 + ... + an*bn

        return result[0, 0];
    }
    
    private float Norm() // Calcula la norma de un vector 
    {
        return MathF.Sqrt(Mul(this));   // ||x|| = Sqrt(<x,x>)
    }

    public Vector SimCos(Matrix D) // Calcula la similitud del coseno o similitud vectorial
    {
        // Calcula el coseno del ángulo de el vector fila del query por un vector columna de la matriz de términos
        // cos(q, dj) = <q, dj>/||q||*||dj||

        Vector simCos = new Vector(D.GetColumns);

        for (int j = 0; j < D.GetColumns; j++)
        {
            Vector dj = new Vector(D.GetRows); // Tomamos la j-ésima columna de la matriz para poder multiplicarla por el vector

            for (int i = 0; i < dj.Dimension; i++) dj[i] = D[i, j];

            simCos[j] = (this.Norm() == 0) ? 0 : this.Mul(dj) / (this.Norm() * dj.Norm()); // Si la norma del vector query es 0 ocurriría una multiplicación por 0
        }

        return simCos;
    }
}