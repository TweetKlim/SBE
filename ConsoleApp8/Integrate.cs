
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApp8
{
    using System;
    using System.Numerics;

    class Integrate
    {
        public delegate Vector func(double t, Vector x0);

        
        public Vector[] RungeKutta(func f, Vector x0,double t0, double tEnd, double dt)
        {
            int n = (int)((tEnd - t0) / dt);
            Vector[] xOut = new Vector[n];
            for (int i = 0; i < n; ++i)
            {
                x0 = x0 + increment(f, x0, t0, dt);
                xOut[i] = x0;
                t0 += dt;
            }
            return xOut;
        }

        private Vector increment(func f, Vector x0, double t0, double dt)
        {
            Vector k1 = f(t0, x0) * dt;
            Vector k2 = f(t0 + dt / 2f, x0 + k1 / 2f) * dt;
            Vector k3 = f(t0 + dt / 2f, x0 + k2 / 2f) * dt;
            Vector k4 = f(t0 + dt, x0 + k3) * dt;
            Vector x = new Vector( (k1 + k2 * 2 + k3 * 2 + k4) / 6f);
            return x;
        }
        public Vector Corrector(func f, Vector x0, Vector xPre, double t0, double dt)
        {
            return (increment(f, x0, t0, dt) + increment(f, xPre, t0 + dt, dt)) * dt / 2; ;
        }
        public Vector[] PEC(func f, Vector x0, double t0, double tEnd, double dt)// Predict – Evaluate – Correct
        {
            int n = (int)((tEnd - t0) / dt);
            Vector[] xOut = new Vector[n];
            Vector xPre = x0;
            for (int i = 0; i < n; ++i)
            {
                xPre = x0 + increment(f, xPre, t0, dt) * dt;
                x0 = x0 + Corrector(f, xPre, xPre, t0, dt);
                Console.WriteLine(n + " " + i);
                xOut[i] = x0;
                t0 += dt;
            }
            return xOut;
        }
        public Vector[] PECE(func f, Vector x0, double t0, double tEnd, double dt)// Predict – Evaluate – Correct – Evaluate
        {
            int n = (int)((tEnd - t0) / dt);
            Vector[] xOut = new Vector[n];
            Vector xPre;
            for (int i = 0; i < n; ++i)
            {
                xPre = x0 + increment(f, x0, t0, dt) * dt;
                x0 = x0 + Corrector(f, x0, xPre, t0, dt);
                xOut[i] = x0;
                t0 = t0 + dt;
            }
            return xOut;
        }
        public double det(Matrix A, int ii, int jj)
        {
            int detA = 0;
            for(int i = 0; i < A.size; ++i)
            {
                for (int j = 0; j < A.size; ++j)
                {

                }
            }
            return detA;
        }
        public Matrix Invertible(Matrix A)
        {
            Matrix B;
            return A;
        }
        /*public Vector ChebyshevRA(func f, Vector x0, double t0, double tEnd, double dt, Matrix A)
        {
            Complex[] alpha = new Complex[] { new Complex(1, 1), new Complex(1, 1) };
            Complex[] theta = new Complex[] { new Complex(1, 1), new Complex(1, 1) };
            Matrix identity = new Matrix(A.size, 1);
            for(int i = 0; i < 8; ++i)
            {
                Matrix B = A * (tEnd - t0) - identity * theta;
            }
            return x0;
        }*/
        public Vector[] Exponent(func f, Vector x0, double t0, double tEnd, double dt, Matrix A)
        {
            int n = 10;
            Vector[] xOut = new Vector[n];
            Matrix mOut = new Matrix(A.size, 0);
            A *= (tEnd - t0);
            for(int i = 0; i < n;++i)
            {
                Matrix mLocal = new Matrix(A);
                for (int j = 0; j < i; ++j)
                {
                    mLocal *= A;
                }
                mLocal *= Math.Pow(tEnd - t0, i);
                double factorial = 1;
                for (int j = 1; j < i; ++j)
                {
                    factorial *= j;
                }
                mLocal /= factorial;
                mOut += mLocal;
                xOut[i] = new Vector(mOut * x0);
            }
            return xOut;
        }
    }
}
