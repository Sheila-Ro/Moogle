public class Matrix
{
    protected float[,] A;

    public Matrix(int m, int n) // Crea una instancia de la clase Matrix desde cero 
    {
        this.A = new float[m, n];
    }

    public Matrix(float[,] matrix) // Crea una instancia de la clase Matrix desde una matriz existente 
    {
        this.A = new float[matrix.GetLength(0), matrix.GetLength(1)];

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                this.A[i, j] = matrix[i, j];
            }
        }
    }

    public override string ToString()
    {
        string str = "";
        for (int i = 0; i < this.GetRows; i++)
        {
            for (int j = 0; j < this.GetColumns; j++)
            {
                str += string.Format("[{0}]", this[i, j]);
            }
            str += "\n";
        }
        return str;
    }

    public float this[int i, int j] 
    {
        get 
        {
            if (i >= 0 && i < this.GetRows && j >= 0 && j < this.GetColumns) 
                return A[i, j];
            else 
                throw new IndexOutOfRangeException();
        }
        set 
        {
            if (i >= 0 && i < this.GetRows && j >= 0 && j < this.GetColumns) 
                A[i, j] = value;
            else 
                throw new IndexOutOfRangeException();
        }
    }

    public int GetRows 
    { 
        get { return A.GetLength(0); } 
    }

    public int GetColumns 
    { 
        get { return A.GetLength(1); } 
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

    public Matrix Sub(Matrix other) // Devuelve la resta de dos matrices 
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

    public Matrix MulEsc(float k) // Devuelve la multiplicación de una matriz por un escalar 
    {
        Matrix result = new Matrix(this.GetRows, this.GetColumns);

        for (int i = 0; i < this.GetRows; i++)
        {
            for (int j = 0; j < this.GetColumns; j++)
            {
                result[i, j] = this[i, j] * k;
            }
        }

        return result;
    }

    public Matrix Mul(Matrix other) // Devuelve la multiplicación de dos matrices 
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
}

public class SquareMatrix : Matrix
{
    // Crea una matriz cuadrada desde cero
    public SquareMatrix(int n) : base(n, n) { } 

    // Crea una matriz cuadrada a partir de una matriz ya existente. Si la matriz existente no es cuadrada lanza una excepción
    public SquareMatrix(float[,] matrix) : base(matrix)
    {
        if (matrix.GetLength(0) != matrix.GetLength(1))
            throw new IndexOutOfRangeException();
    }

    public int N // Orden de la matriz cuadrada (A e M_n(K))
    {
        get { return this.GetRows; }
    }

    public float Det // Det(A), A e M_n(K)
    {
        get 
        {
            float det = 0;

            if (this.N == 2)
            {
                det += this[0, 0]*this[1, 1] - this[1, 0]*this[0, 1]; // Det(A) = a_11*a_22 - a_12*a_21 
            }
            else if (this.N >= 3)
            {
                for (int i = 0; i < this.N; i++)
                {
                    det += this[0, i] * this.Cofactor(0, i); // Det(A) = a_i1*A_i1 + a_i2*A_i2 + ... + a_in*A_in
                }
            }

            return det;
        }
    }

    public float Menor(int k, int l) // Devuelve el menor relacionado a la k-ésima fila y la l-ésima columna de la matriz dada 
    {
        SquareMatrix menor = new SquareMatrix(this.N - 1);

        int ni = 0;
        int nj = 0;

        for (int i = 0; i < this.N; i++)
        {
            for (int j = 0; j < this.N; j++)
            {
                if (i != k && j != l)
                {
                    menor[ni, nj++] += this[i, j];

                    if (nj == menor.N)
                    {
                        ni++;
                        nj = 0;
                        
                        if (ni == menor.N) break;
                    }
                }
            }
        }

        return menor.Det;
    }

    public float Cofactor(int i, int j) // Devuelve el cofactor asociado al elemento (i,j) de una matriz 
    {
        return (Menor(i, j) == 0) ? 0 : (MathF.Pow(-1, i + j) * Menor(i, j)); // A_ij = (-1)^(i+j) * M_ij
    }

    public SquareMatrix CofactorMatrix() // Devuelve la matriz de cofactores de una matriz 
    {
        SquareMatrix result = new SquareMatrix(this.N);

        for (int i = 0; i < this.N; i++)
        {
            for (int j = 0; j < this.N; j++)
            {
                result[i, j] = this.Cofactor(i, j);
            }
        }

        return result;
    }

    public SquareMatrix Inverse() // Devuelve la inversa de una matriz 
    {
        if (this.Det == 0)
            throw new Exception("Esta matriz no tiene inversa");
        
        Matrix adjunta = this.CofactorMatrix().Transpose();
        SquareMatrix inverse = new SquareMatrix(this.N);

        for (int i = 0; i < this.N; i++)
        {
            for (int j = 0; j < this.N; j++)
            {
                inverse[i, j] = adjunta[i, j] == 0 ? 0 : adjunta[i, j] / this.Det;
            }
        }

        return inverse;
    }
}