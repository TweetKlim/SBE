namespace ConsoleApp8
{
    public class Vector
    {
        public double[] var;
        public Vector(double[] var)
        {
            this.var = new double[var.Length];
            for(int i = 0; i < var.Length;++i)
            {
                this.var[i] = var[i];
            }
            
        }
        public Vector(double var)
        {
            this.var = new double[1];
            this.var[0] = var;
        }
        public Vector(Vector var)
        {
            this.var = new double[var.var.Length];
            for (int i = 0; i < var.var.Length; ++i)
            {
                this.var[i] = var.var[i];
            }
        }
        public int size()
        {
            return this.var.Length;
        }
        public static Vector operator +(Vector v, double b)
        {
            Vector p = new Vector(v);
            for (int i = 0; i < v.var.Length; ++i)
            {
                p.var[i] += b;
            }
            return p;
        }
        
        public static Vector operator +(Vector v, Vector b)
        {
            Vector p = new Vector(v);
            for (int i = 0; i < v.var.Length; ++i)
            {
                p.var[i] += b.var[i];
            }
            return p;
        }
        public static Vector operator *(Vector v, double b)
        {
            Vector p = new Vector(v);
            for (int i = 0; i < v.var.Length; ++i)
            {
                p.var[i] *= b;
            }
            return p;
        }
        public static Vector operator /(Vector v, double b)
        {
            Vector p = new Vector(v);
            for (int i = 0; i < v.var.Length; ++i)
            {
                p.var[i] /= b;
            }
            return p;
        }

        public double this[int index]
        {
            get
            {
                return var[index];
            }

            set
            {
                var[index] = value;
            }
        }
    }
}