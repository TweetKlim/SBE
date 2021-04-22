using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp8
{
    class Matrix // only square
    {
        public int size;
        double[][] M;
        public Matrix(int s, double[][] inMatrix)
        {
            size = s;
            M = new double[size][];
            for(int i = 0;i < size; ++i)
            {
                M[i] = new double[size];
                for (int j = 0; j < size; ++j)
                {
                    M[i][j] = inMatrix[i][j];
                }
            }
        }
        public Matrix(int s, double diag)
        {
            size = s;
            M = new double[size][];
            for (int i = 0; i < size; ++i)
            {
                M[i] = new double[size];
                for (int j = 0; j < size; ++j)
                {
                    if (j == i)
                    {
                        M[i][j] = diag;
                    }
                    else
                    {
                        M[i][j] = 0;
                    }
                }
            }
        }
        public Matrix(Matrix A)
        {
            this.size = A.size;
            M = new double[this.size][];
            for (int i = 0; i < size; ++i)
            {
                M[i] = new double[this.size];
                for (int j = 0; j < size; ++j)
                {
                    M[i][j] = A[i,j];
                }
            }
        }
        void exp()
        {
            
        }
        double[] product(double[] vector)
        {
            double[] vectorOut = new double[size];
            for (int i = 0; i < size; ++i)
            {
                vectorOut[i] = 0;
                for (int j = 0; j < size; ++j)
                {
                    vectorOut[i] += vector[j] * M[i][j];
                }
            }
            return vectorOut;
        }
        public double this[int indexI, int indexJ]
        {
            get
            {
                return M[indexI][indexJ];
            }

            set
            {
                M[indexI][indexJ] = value;
            }
        }
        public static Vector operator *(Matrix A, Vector B)
        {
            double[] mass = new double[B.size()];
            for(int i = 0; i < B.size();++i)
            {
                for (int j = 0; j < B.size(); ++j)
                {
                    mass[i] += A[i,j] * B[j];
                }
            }
            Vector vOut = new Vector(mass);
            
            return vOut;
        }
        public static Matrix operator *(Matrix A, Matrix B)
        {
            Matrix mass = new Matrix(B.size,0);
            for (int i = 0; i < B.size; ++i)
            {
                for (int j = 0; j < B.size; ++j)
                {
                    for (int p = 0; p < B.size; ++p)
                    {
                        mass[i, j] += A[i, p] * B[p, j];
                    }
                }
            }

            return mass;
        }
        public static Matrix operator +(Matrix A, Matrix B)
        {
            Matrix mass = new Matrix(B.size,0);
            for (int i = 0; i < B.size; ++i)
            {
                for (int j = 0; j < B.size; ++j)
                {
                    mass[i, j] = A[i, j] + B[j, i];
                }
            }

            return mass;
        }
        public static Matrix operator /(Matrix A, double B)
        {
            Matrix mass = new Matrix(A.size,0);
            for (int i = 0; i < A.size; ++i)
            {
                for (int j = 0; j < A.size; ++j)
                {
                    mass[i, j] += A[i, j] / B;
                }
            }

            return mass;
        }
        public static Matrix operator *(Matrix A, double B)
        {
            Matrix mass = new Matrix(A.size,0);
            for (int i = 0; i < A.size; ++i)
            {
                for (int j = 0; j < A.size; ++j)
                {
                    mass[i, j] += A[i, j] * B;
                }
            }

            return mass;
        }
    }
}
