using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace ConsoleApp8
{

    struct pair
    {
        public int ID_Parent;
        public int ID_Child;
        pair(int ID_P, int ID_C)
        {
            ID_Parent = ID_P;
            ID_Child = ID_C;
        }
    }
    
    class Program
    {

        



        static string constring = ConfigurationManager.ConnectionStrings["ConsoleApp8.Properties.Settings.setting"].ConnectionString;
        static int n = 3083;
        static double[][] MatrixALambda = new double[n][];
        static double[][] MatrixAF = new double[n][];
        static double[] N = new double[n];


        static Tnode FindNode(Tnode Tn, int num)
        {

            
            if (Tn.number == num)
            {
                return Tn;
            }
            Tnode outNode;
            for (int i = 0; i < Tn.son.Count; ++i)
            {
                outNode = FindNode(Tn.son[i], num);
                if (outNode.number != -1 && outNode.number == num)
                {
                    return outNode;
                }
            }
            return new Tnode(-1);
        }

        static void WriteTree(Tnode Tn,int depth = 0,string str = "", int outputMode = 0)
        {
            // "├" "└" "|"
            for (int i = 0; i < str.Length; ++i)
            {
                if (str[i] == '0' && str.Length - 1 == i)
                {
                    Console.Write("└");
                }
                else if (str[i] == '0')
                {
                    Console.Write(" ");
                }
                else if (str[i] == '1' && str.Length - 1 == i)
                {
                    Console.Write("├");
                }
                else if (str[i] == '1')
                {
                    Console.Write("|");
                }
            }
            //if( == Tn.son.Count)
            if(outputMode == 0)
            {
                Console.WriteLine("" + Tn.number);
            }
            if(outputMode == 1)
            {
                Console.Write("" + Tn.number);
                for (int i = 0; i < Tn.son.Count; ++i)
                {
                    string result2 = String.Format("{0:C5}", Tn.decayConstant[i]);
                    Console.Write(" " + result2);
                }
                Console.WriteLine("");
            }
            for (int i = 0; i < Tn.son.Count; ++i)
            {
                if (i < Tn.son.Count - 1)
                {
                    WriteTree(Tn.son[i], depth + 1, str + "1", outputMode);
                }
                else if (i == Tn.son.Count - 1)
                {
                    WriteTree(Tn.son[i], depth + 1, str + "0", outputMode);
                }

            }
        }

        static List<int> ReduceChain(Tnode Tn, double time)
        {
            List<int> reduced = new List<int>();
            for(int i = 0; i < Tn.son.Count; ++i)
            {
                if(Tn.son[i].son.Count != 0 && 0.693/Tn.decayConstant[i] < time)
                {
                    for(int j = 0; j < Tn.son[i].son.Count; ++j)
                    {
                        double a1 = Tn.decayConstant[i];
                        double a2 = Tn.son[i].decayConstant[j];
                        double b2 = Tn.branching[i];
                        double newA2 = a2*a1*b2*(Math.Exp(-a1)- Math.Exp(-a2))/((a2-a1)*Math.Exp(-a1));
                        Tn.branching.Add(1);//rework
                        Tn.decayConstant.Add(newA2);
                        Tn.son.Add(Tn.son[i].son[j]);
                    }
                    reduced.Add(Tn.son[i].number);
                    Tn.branching.RemoveAt(i);//rework
                    Tn.son.RemoveAt(i);
                    Tn.decayConstant.RemoveAt(i);
                    --i;
                }
            }
            return reduced;
        }
        public static Vector function(double t, Vector x0)
        {
            return new Vector(Math.Sin(t)-x0.var[0]);
        }
        static void Main(string[] args)
        {
            Integrate integrate = new Integrate();
            Integrate.func func = function;
            double dt = 0.05;
            double t0 = 0;
            double tEnd = 5;
            double[] mass = new double[1] {1};
            Vector ver = new Vector(mass);
            Vector[] RK         = integrate.RungeKutta(func, ver, t0, tEnd, dt);
            Vector[] PEC        = integrate.PEC       (func, ver, t0, tEnd, dt);
            Vector[] PECE       = integrate.PECE      (func, ver, t0, tEnd, dt);
            int n = (int)((tEnd - t0) / dt);

            Console.WriteLine("{0,-18} {1,-18} {2,-18}", "RK","RK1","PEc");
            for (int i = 0; i < n; ++i)
            {
                Console.WriteLine("{0,-18} {1,-18} {2,-18}", RK[i].var[0], PEC[i].var[0], PECE[i].var[0]);
            }
            Console.ReadLine();
            for (int i = 0; i < n; ++i)
            {
                N[i] = 1;
                MatrixALambda[i] = new double[n];
                MatrixAF[i] = new double[n];
                for (int j = 0; j < n; ++j)
                {
                    MatrixALambda[i][j] = 0;
                    MatrixAF[i][j] = 0;
                }
            }
            using (SqlConnection con = new SqlConnection(constring))
            {
                // calculate decay constant
                con.Open();
                Dictionary<int, int> Isotop = new Dictionary<int, int>();
                List<Double> DecayConstant = new List<Double>();
                SqlCommand cmd = new SqlCommand();
                string sql = "SELECT [ID_Isotope],[DecayConstant] FROM [Nuclear].[dbo].[Isotopes]";
                cmd.Connection = con;
                cmd.CommandText = sql;
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int i = 0;
                        while (reader.Read())
                        {
                            
                            Isotop.Add(reader.GetInt32(0), i);
                            if(reader.GetValue(1) != DBNull.Value)
                            {
                                DecayConstant.Add(reader.GetDouble(1));

                            }
                            else
                            {
                                DecayConstant.Add(0);
                            }
                            ++i;
                        }
                    }
                }

                // read isotope chain
                //sql = "SELECT [ID_Isotop],[ID_Parent],[ID_Child] FROM[Nuclear].[dbo].[IsotopeChain] ";
                //cmd.Connection = con;
                //cmd.CommandText = sql;
                //Dictionary<int,List<pair>> chain = new Dictionary<int, List<pair>>();
                //int g = 0;
                //using (DbDataReader reader = cmd.ExecuteReader())
                //{
                //    pair pairIN;
                //    List<pair> chainPair = new List<pair>();
                //    int lastIsotope = -1;
                //    if (reader.HasRows)
                //    {
                //        while (reader.Read())
                //        {
                //            g++;
                //            if(lastIsotope != reader.GetInt32(0) && lastIsotope != -1)
                //            {
                //                chain.Add(lastIsotope, new List<pair>( chainPair));
                //                lastIsotope = reader.GetInt32(0);
                //                chainPair.Clear();
                //            }
                //            else
                //            {
                //                lastIsotope = reader.GetInt32(0);
                //            }
                //            pairIN.ID_Parent = reader.GetInt32(1);
                //            pairIN.ID_Child = reader.GetInt32(2);
                //            chainPair.Add(pairIN);
                //        }
                //        chain.Add(lastIsotope, new List<pair>(chainPair));
                //    }
                //}
                //Console.WriteLine("transaction count" + g);




                //Read isotop decay
                int countOfDecay = 0;

                sql = "SELECT [ID_Parent],[ID_Child],[Branching] FROM [Nuclear].[dbo].[IsotopeDecay]";
                cmd.Connection = con;
                cmd.CommandText = sql;
                List<Tnode> simpleNode = new List<Tnode>();
                Dictionary<int, int> simpleNodePosition = new Dictionary<int, int>();
                List<Tnode> simpleNodeTrees = new List<Tnode>();
                Dictionary<int, List<int>> isotopDecay = new Dictionary<int, List<int>>();
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int g = 0;
                        while (reader.Read())
                        {
                            if (Isotop.ContainsKey(reader.GetInt32(1)) && Isotop.ContainsKey(reader.GetInt32(0)))
                            {
                                int parent = reader.GetInt32(0);
                                int child = reader.GetInt32(1);
                                if (!simpleNodePosition.ContainsKey(parent))
                                {
                                    simpleNode.Add(new Tnode(parent));
                                    simpleNodePosition.Add(parent, g);
                                    ++g;
                                }
                                if (!simpleNodePosition.ContainsKey(child))
                                {
                                    simpleNode.Add(new Tnode(child));
                                    simpleNodePosition.Add(reader.GetInt32(1), g);
                                    ++g;
                                }
                                simpleNode[simpleNodePosition[parent]].son.Add(simpleNode[simpleNodePosition[child]]);
                                simpleNode[simpleNodePosition[child]].hasParent = true;

                                simpleNode[simpleNodePosition[parent]].branching.Add(1);//rework need include branching ratio
                                simpleNode[simpleNodePosition[parent]].decayConstant.Add(DecayConstant[Isotop[parent]]);
                                countOfDecay++;
                                //if (isotopDecay.ContainsKey(reader.GetInt32(0)))
                                //{
                                //    isotopDecay[reader.GetInt32(0)].Add(reader.GetInt32(1));
                                //}
                                //else
                                //{
                                //    isotopDecay.Add(reader.GetInt32(0),new List<int>());
                                //    isotopDecay[reader.GetInt32(0)].Add(reader.GetInt32(1));
                                //}
                            }
                        }
                        Console.WriteLine("simple node count" + g);
                        for(int i = 0; i< simpleNode.Count; ++i)
                        {
                            if(simpleNode[i].hasParent == false)
                            {
                                simpleNodeTrees.Add(simpleNode[i]);
                            }
                        }
                    }
                }

                //Chain Tree 
                //List<Tnode> trees = new List<Tnode>();
                //foreach (KeyValuePair<int, List<pair>> keyValue in chain)
                //{
                //    Tnode tree;
                //    Tnode localNode;
                //    tree = new Tnode(Isotop[chain[keyValue.Key][0].ID_Parent]);
                //    for (int j = 0; j < chain[keyValue.Key].Count; ++j)
                //    {
                //        localNode = FindNode(tree, Isotop[chain[keyValue.Key][j].ID_Parent]);
                //        localNode.branching.Add(1);//rework need include branching ratio
                //        localNode.decayConstant.Add(DecayConstant[Isotop[chain[keyValue.Key][j].ID_Parent]]);
                //        localNode.son.Add(new Tnode(Isotop[chain[keyValue.Key][j].ID_Child]));
                //    }
                //    trees.Add(tree);
                //}
                //Console.WriteLine("end");


                //WRITE TREE
                //for (int i = 0; i < simpleNodeTrees.Count; ++i)
                //{
                //    Console.WriteLine(" " + i + " ");
                //    WriteTree(simpleNodeTrees[i]);
                //}

                WriteTree(simpleNodeTrees[0], outputMode: 1);
                //reduce tree
                double time = 24;
                List<int> reduced = new List<int>();
                for (int i = 0; i < simpleNodeTrees.Count; ++i)
                {
                    reduced = reduced.Union(ReduceChain(simpleNodeTrees[i], time)).ToList();
                }
                for (int i = 0; i < simpleNodeTrees.Count; ++i)
                {
                    for (int j = 0; j < simpleNodeTrees[i].son.Count; ++j)
                    {
                        for (int g = j + 1; g < simpleNodeTrees[i].son.Count; ++g)
                        {
                            if(simpleNodeTrees[i].son[j] == simpleNodeTrees[i].son[g])
                            {
                                simpleNodeTrees[i].branching[j] += simpleNodeTrees[i].branching[g];
                                simpleNodeTrees[i].decayConstant[j] += simpleNodeTrees[i].decayConstant[j];
                                simpleNodeTrees[i].son.RemoveAt(g);
                                --g;
                            }
                        }
                    }
                }

                List<int> completelyReduced = new List<int>(reduced);
                for (int i = 0; i < simpleNodeTrees.Count; ++i)
                {
                    for (int j = 0; j < completelyReduced.Count; ++j)
                    {
                        if(FindNode(simpleNodeTrees[i], completelyReduced[j]).number != -1)
                        {
                            completelyReduced.RemoveAt(j);
                        }
                    }
                }
                WriteTree(simpleNodeTrees[0], outputMode: 1);
                //for (int i = 0; i < simpleNodeTrees.Count; ++i)
                //{
                //    Console.WriteLine(" " + i + " ");
                //    WriteTree(simpleNodeTrees[i], outputMode: 1);
                //}





                Console.WriteLine("");
                //for (int i = 0; i < reduced.Count; ++i)
                //{
                //    Console.WriteLine(reduced[i]);
                //}
                Console.WriteLine("count of decay" + countOfDecay);
                Console.WriteLine("count of del Tnode" + reduced.Count);
                Console.WriteLine("count of del Tnode" + completelyReduced.Count);

                //double time = 1;
                //List<int> reduced = new List<int>();
                //for (int i = 0; i < trees.Count; ++i)
                //{
                //    reduced = reduced.Union(ReduceChain(trees[i], time)).ToList();
                //}
                //Console.WriteLine("Reduce::" + reduced.Count);
                //for (int i = 0; i < reduced.Count; ++i)
                //{
                //    Console.WriteLine(reduced[i]);
                //}


                //reduce test 50
                //int k = 50;
                //Console.WriteLine("NoReduce:");


                //WriteTree(trees[k], outputMode: 1);


                //ReduceChain(trees[k], 10.0);
                //Console.WriteLine("Reduce:");
                //WriteTree(trees[k], outputMode: 1);




                //foreach (KeyValuePair<int, List<pair>> keyValue in chain)
                //{
                //    List<int> tree = new List<int>();
                //    for (int j = 0; j < chain[keyValue.Key].Count; ++j)
                //    {
                //        if (tree.Contains(Isotop[chain[keyValue.Key][j].ID_Parent]) == true)
                //        {
                //            while (tree.LastIndexOf(Isotop[chain[keyValue.Key][j].ID_Parent]) != tree.Count - 1)
                //            {
                //                tree.RemoveAt(tree.Count - 1);
                //            }
                //        }
                //        else
                //        {
                //            tree.Add(Isotop[chain[keyValue.Key][j].ID_Parent]);
                //        }
                //        for (int i = 0; i < tree.Count; ++i)
                //        {
                //            Console.Write(" ");
                //        }
                //        Console.WriteLine(chain[keyValue.Key][j].ID_Parent + " " + chain[keyValue.Key][j].ID_Child);
                //    }
                //    Console.WriteLine("");
                //}

                //chains reduction
                /*
                int maxTime = 1;

                foreach (KeyValuePair<int, List<pair>> keyValue in chain)
                {
                    double timeChain = 0;
                    int lastIsotopID = 0;
                    int lastIndex = 0;
                    for (int j = 0; j < chain[keyValue.Key].Count; ++j)
                    {
                        if (Isotop.ContainsKey(chain[keyValue.Key][j].ID_Parent))
                        {
                            if (DecayConstant[Isotop[chain[keyValue.Key][j].ID_Parent]] + timeChain < maxTime)
                            {
                                timeChain += DecayConstant[Isotop[chain[keyValue.Key][j].ID_Parent]];
                                lastIsotopID = Isotop[chain[keyValue.Key][j].ID_Parent];
                                lastIndex = j;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if(lastIsotopID != keyValue.Key)
                    {
                        double sum = 0;
                        for (int i = 0; i < lastIndex; ++i)
                        {
                            double alpha = 1;
                            for (int j = 0; j < lastIndex; ++j)
                            {
                                if (i != j)
                                    if(Isotop.ContainsKey(chain[keyValue.Key][j].ID_Parent))
                                        alpha *= DecayConstant[Isotop[chain[keyValue.Key][j].ID_Parent]] / (DecayConstant[Isotop[chain[keyValue.Key][j].ID_Parent]] - DecayConstant[Isotop[chain[keyValue.Key][i].ID_Parent]]);
                            }
                            sum = DecayConstant[Isotop[chain[keyValue.Key][i].ID_Parent]]*alpha*Math.Exp(-DecayConstant[Isotop[chain[keyValue.Key][i].ID_Parent]]*timeChain) ;
                        }
                        Console.WriteLine(Isotop[keyValue.Key]);
                        Console.WriteLine(N[lastIsotopID] + " " + N[Isotop[keyValue.Key]] / DecayConstant[lastIsotopID] * sum);
                        N[lastIsotopID] =  N[Isotop[keyValue.Key]]/DecayConstant[lastIsotopID]*sum;
                    }
                }
                */
                // calculate Matrix AF (not end)
                sql = "SELECT [ID_Parent],[ID_Child],[Energy],[Yield] FROM[Nuclear].[dbo].[FissionProductsYield] ";
                cmd.Connection = con;
                cmd.CommandText = sql;
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int i = 0;
                        while (reader.Read())
                        {
                            ++i;
                            if (Isotop.ContainsKey(reader.GetInt32(1)) && Isotop.ContainsKey(reader.GetInt32(0)))
                            {
                               
                                MatrixAF[Isotop[reader.GetInt32(1)]][Isotop[reader.GetInt32(0)]] = reader.GetDouble(2) + reader.GetDouble(3);
                                MatrixAF[Isotop[reader.GetInt32(0)]][Isotop[reader.GetInt32(0)]] += reader.GetDouble(3);
                            }
                            else
                            {
                                //Console.WriteLine(reader.GetInt32(0) + " " + reader.GetInt32(1));
                            }
                        }
                    }
                }

                // Calculate matrix A Lambda 
                sql = "SELECT [ID_Parent],[ID_Child],[Branching] FROM [Nuclear].[dbo].[IsotopeDecay]";
                cmd.Connection = con;
                cmd.CommandText = sql;
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int i = 0;
                        while (reader.Read())
                        {
                            ++i;
                            if (Isotop.ContainsKey(reader.GetInt32(1)) && Isotop.ContainsKey(reader.GetInt32(0)))
                            {
                                MatrixALambda[Isotop[reader.GetInt32(0)]][Isotop[reader.GetInt32(1)]] = reader.GetDouble(2) * DecayConstant[Isotop[reader.GetInt32(1)]];
                                
                            }
                            if (Isotop.ContainsKey(reader.GetInt32(1)) && Isotop.ContainsKey(reader.GetInt32(0)))
                                MatrixALambda[Isotop[reader.GetInt32(1)]][Isotop[reader.GetInt32(1)]] -= DecayConstant[Isotop[reader.GetInt32(1)]];

                        }
                    }
                }

                
                //int k = 10;
                //string s;
                //for (int i = 0; i < k; ++i)
                //{
                //    for (int j = 0; j < k; ++j)
                //    {
                //        s = string.format("{0,14:f2}", matrixa[i][j]);
                //        console.write(s);
                //    }
                //    console.write("\n");
                //}
                //con.close();

            }
            Console.Read();
            
        }

        void ReduseMatrix(double[][] matrixA, Dictionary<int, List<pair>> chain, Dictionary<int, int> Isotop, List<Double> DecayConstant, int reduseTime)
        {
            Dictionary<int, int> reduseChain = new Dictionary<int, int>();
            List<double> lambda = new List<double>();
            foreach (KeyValuePair<int, List<pair>> keyValue in chain)
            {
                for (int j = 0; j < chain[keyValue.Key].Count; ++j)
                {
                    
                }
                
            }

            
        }
    }
}
