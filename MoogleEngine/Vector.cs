public class Vector : Matrix
{
    // Crea una instancia de la clase Vector desde cero, a partir de una matriz de 1xn 
    public Vector(int n) : base(1, n) { } 

    // Crea una instancia de la clase Vector a partir de un array de float dado 
    public Vector(float[] vector) : base(1, vector.Length) 
    {
        for (int i = 0; i < vector.Length; i++)
        {
            this[0, i] = vector[i];
        }
    }

    public float this[int index]
    {
        get 
        {
            if (index >= 0 && index < this.Dimension) 
                return this.A[0, index];
            else 
                throw new IndexOutOfRangeException();
        }
        set 
        {  
            if (index >= 0 && index < this.Dimension) 
                this.A[0, index] = value;
            else 
                throw new IndexOutOfRangeException();
        }
    }

    public int Dimension 
    {
        get { return A.GetLength(1); }
    }
    
    public float Mul(Vector other) // Calcula el producto escalar de dos vectores 
    {
        if (this.Dimension != other.Dimension) 
            throw new IndexOutOfRangeException();
        
        float result = 0;

        for (int i = 0; i < this.Dimension; i++)
            result += this[i] * other[i];   // <a, b> = a1*b1 + a2*b2 + ... + an*bn

        return result;
    }

    public float MulM(Vector other) // Calcula el producto escalar de dos vectores a través de la multiplicación por matrices 
    {
        if (this.Dimension != other.Dimension) 
            throw new IndexOutOfRangeException();

        Matrix result = this.Mul(other.Transpose());   // <a, b> = a1*b1 + a2*b2 + ... + an*bn

        return result[0, 0];
    }
    
    public float Norm() // Calcula la norma de un vector 
    {
        return MathF.Sqrt(this.Mul(this));   // ||x|| = Sqrt(<x,x>)
    }

    public Vector SimCos(Matrix D) // Calcula la similitud del coseno o similitud vectorial
    {
        // Calcula un vector que recopila todos los cosenos de los ángulos de un vector por todas las columnas de una matriz
        // cos(v, dj) = <v, dj>/||v||*||dj||

        Vector simCos = new Vector(D.GetColumns);

        for (int j = 0; j < D.GetColumns; j++)
        {
            Vector dj = new Vector(D.GetRows); // Tomamos la j-ésima columna de la matriz para poder multiplicarla por el vector

            for (int i = 0; i < dj.Dimension; i++) 
                dj[i] = D[i, j];

            simCos[j] = (this.Norm() == 0) ? 0 : this.Mul(dj) / (this.Norm() * dj.Norm()); // Si la norma del vector es 0 ocurriría una división por 0
        }

        return simCos;
    }
}