public class Matrix
{
    private float[,] matrix;

    public Matrix(int m, int n) // Crea una instancia de la clase Matrix desde cero 
    {
        this.matrix = new float[m, n];
    }

    public Matrix(int[,] matrix) // Crea una instancia de la clase Matrix desde una matriz existente 
    {
        this.matrix = new float[matrix.GetLength(0), matrix.GetLength(1)];

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                this.matrix[i, j] = matrix[i, j];
            }
        }
    }

    public float this[int i, int j] 
    {
        get 
        {
            if (i >= 0 && i < this.GetRows && j >= 0 && j < this.GetColumns) 
                return matrix[i, j];
            else 
                throw new IndexOutOfRangeException();
        }
        set 
        {
            if (i >= 0 && i < this.GetRows && j >= 0 && j < this.GetColumns) 
                matrix[i, j] = value;
            else 
                throw new IndexOutOfRangeException();
        }
    }

    public int GetRows 
    { 
        get 
        { 
            return matrix.GetLength(0);
        }
    }

    public int GetColumns 
    {
        get 
        {
            return matrix.GetLength(1);
        }
    }

    public Matrix Sum(Matrix other) // Devuelve la suma de dos matrices 
    {
        if (this.GetRows != other.GetRows || this.GetColumns != other.GetColumns)
            throw new IndexOutOfRangeException();

        Matrix result = new Matrix(this.GetRows, this.GetColumns);

        for (int i = 0; i < this.GetRows; i++)
        {
            for (int j = 0; j < this.GetColumns; j++)
            {
                result[i, j] = this[i, j] + other[i, j];
            }
        }

        return result;
    }

    public Matrix Sub(Matrix other) // Devuelsve la resta de dos matrices 
    {
        if (this.GetRows != other.GetRows || this.GetColumns != other.GetColumns)
            throw new IndexOutOfRangeException();

        Matrix result = new Matrix(this.GetRows, this.GetColumns);

        for (int i = 0; i < this.GetRows; i++)
        {
            for (int j = 0; j < this.GetColumns; j++)
            {
                result[i, j] = this[i, j] - other[i, j];
            }
        }

        return result;
    }

    public Matrix Mul(Matrix other) // Devuelve la multiplicación de matrices 
    {
        if (this.GetColumns != other.GetRows)
            throw new IndexOutOfRangeException();

        Matrix result = new Matrix(this.GetRows, other.GetColumns);

        for (int i = 0; i < this.GetRows; i++)
        {
            for (int j = 0; j < other.GetColumns; j++)
            {
                for (int k = 0; k < this.GetColumns; k++)
                {
                    result[i, j] += this[i, k] * other[k, j];
                }
            }
        }

        return result;
    }

    public Matrix Transpose() // Devuelve la transpuesta de una matriz 
    {
        Matrix transpose = new Matrix(this.GetColumns, this.GetRows);

        for (int i = 0; i < this.GetColumns; i++)
        {
            for (int j = 0; j < this.GetRows; j++)
            {
                transpose[i, j] = this[j, i];
            }
        }

        return transpose;
    }

    // public int Det 
    // {
    //     get 
    //     {
    //         if (this.GetRows != this.GetColumns) 
    //             return 0;

    //         int det = 0;
    //         int n = this.GetRows;

    //         throw new NotImplementedException();
    //     }
    // }

    // public int Cofactor() 
    // {
    //     throw new NotImplementedException();
    // }

    // public int Menor()
    // {
    //     throw new NotImplementedException();
    // }

    // public Matrix Inverse() 
    // {
    //     if (this.GetRows != this.GetColumns) 
    //         throw new IndexOutOfRangeException();

    //     int n = this.GetRows;
    //     Matrix inverse = new Matrix(n, n);

    //     throw new NotImplementedException();
    // }
}