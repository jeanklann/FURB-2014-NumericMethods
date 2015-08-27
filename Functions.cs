using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace FractionedNumber {

    public class Functions {
        private static string result = "";
        public static string Result {
            get { return result; }
        }
        public static void clearResult() {
            result = "";
        }
        public static void writeFile(string path) {
            System.IO.StreamWriter file = new System.IO.StreamWriter(path);
            file.WriteLine(result);
            file.Close();
        }
        public static Number calculateBisectEquation(Number[] list, Number error){
            return calculateBisectEquation(list,error,-10000,10000);
        }
        public static Number calculateBisectEquation(Number[] list, Number error, BigInteger begin, BigInteger end) {
            BisectTablePart lastTable = null;
            lastTable = calculateBisectEquationBisectTablePart(list, lastTable, begin, end);
            result += "Intervalo entre "+lastTable.BisectInterval.Begin;
            result += " e " + lastTable.BisectInterval.End;
            result += "\n";

            while(lastTable.Error>error) {
                result+=lastTable.ToString()+"\n";
                lastTable = calculateBisectEquationBisectTablePart(list, lastTable, begin, end);
            }
            result += lastTable.ToString() + "\n";
            result += "\n";
            result += "Resultado: "+lastTable.BisectInterval.Average;
            return lastTable.BisectInterval.Average;
        }
        public static string calculateInterval(Number[] list, BigInteger begin, BigInteger end) {
            string res = "";
            BigInteger begin2 = begin;
            while(begin2 < end - 1) {
                BisectInterval bi = calculateBisectEquationBisectIntervals(list, begin2, end);
                begin2 = ((int) bi.End);
                if(begin2 >= end - 1) break;
                res += "a: " + bi.Begin;
                res += "    b: " + bi.End;
                res += "\n";
                
            }
            return res;
        }
        public static string calculateIntervalExtended(Number[] list, BigInteger begin, BigInteger end) {
            string res = "";
            for(BigInteger i = begin;i < end;i++) {
                res += i + ": \t";
                res += calculateEquationPart(list, new Number(i));
                res += "\n";
            }
            return res;
        }
        private static BisectInterval calculateBisectEquationBisectIntervals(Number[] list, BigInteger begin, BigInteger end) {
            BisectInterval BisectInterval = new BisectInterval();
            BisectInterval.Begin = new Number(begin);
            BisectInterval.End = new Number(end);

            Number tmp = calculateEquationPart(list, new Number(begin));
            if(tmp.Numerator == 0) {
                BisectInterval.Begin = new Number(begin);
                BisectInterval.End = new Number(end);
            } else if(tmp.Numerator < 0) {
                for(BigInteger i = begin + 1;i < end;i++) {
                    Number n = calculateEquationPart(list, new Number(i));
                    if(n.Numerator < 0) {
                        BisectInterval.Begin = new Number(i);
                    } else if(n.Numerator == 0) {
                        BisectInterval.Begin = new Number(i);
                        BisectInterval.End = new Number(i);
                        break;
                    } else {
                        BisectInterval.End = new Number(i);
                        break;
                    }
                }
            } else {
                for(BigInteger i = begin+1;i < end;i++) {
                    Number n = calculateEquationPart(list, new Number(i));
                    if(n.Numerator > 0) {
                        BisectInterval.Begin = new Number(i);
                    } else if(n.Numerator == 0) {
                        BisectInterval.Begin = new Number(i);
                        BisectInterval.End = new Number(i);
                        break;
                    } else {
                        BisectInterval.End = new Number(i);
                        break;
                    }
                }
            }

            return BisectInterval;
        }
        private static BisectTablePart calculateBisectEquationBisectTablePart(Number[] list, BisectTablePart lastTable, BigInteger begin, BigInteger end) {
            BisectTablePart table = new BisectTablePart();
            if(lastTable != null) {
                if(lastTable.ResInterval.Begin.Numerator * lastTable.ResInterval.Average.Numerator < 0) {
                    table.BisectInterval.Begin = lastTable.BisectInterval.Begin;
                    table.BisectInterval.End = lastTable.BisectInterval.Average;
                    table.ResInterval.Begin  = lastTable.ResInterval.Begin;
                    table.ResInterval.End = lastTable.ResInterval.Average;
                } else {
                    table.BisectInterval.Begin = lastTable.BisectInterval.Average;
                    table.BisectInterval.End = lastTable.BisectInterval.End;
                    table.ResInterval.Begin = lastTable.ResInterval.Average;
                    table.ResInterval.End = lastTable.ResInterval.End;
                }
            } else {
                table.BisectInterval = calculateBisectEquationBisectIntervals(list,begin,end);
                table.ResInterval.Begin = calculateEquationPart(list, table.BisectInterval.Begin);
                table.ResInterval.End = calculateEquationPart(list, table.BisectInterval.End);
            }
            table.BisectInterval.DoAverage();
            table.ResInterval.Average = calculateEquationPart(list, table.BisectInterval.Average);
            if(lastTable != null) {
                if(table.BisectInterval.Average.Numerator == 0)
                    table.Error = new Number(0);
                else
                    table.Error =  (( table.BisectInterval.Average -  lastTable.BisectInterval.Average) /  table.BisectInterval.Average);
                if(table.Error.Numerator < 0)
                    table.Error.Numerator = -table.Error.Numerator;
            }
            return table;
        }
        private static Number calculateEquationPart(Number[] list, Number number) {
            Number res = new Number(0);
            for(int i = 0;i < list.Length;i++) {
                Number resTemp = new Number(1);
                for(int j = 0;j < i;j++) {
                    resTemp *= number;
                }
                resTemp *= list[i];
                res += resTemp;
            }
            return res;
        }
        public static Number calculateNewtonEquation(Number[] list, Number error) {
            return calculateNewtonEquation(list, error, -10000, 10000);
        }
        public static Number calculateNewtonEquation(Number[] list, Number error, BigInteger begin, BigInteger end) {
            NewtonTablePart lastTable = null;
            Number[] list2 = calculateNewtonList2(list);
            
            lastTable = calculateNewtonEquationTablePart(list, list2, lastTable, begin, end);
            result += "Número próximo a " + lastTable.X0;
            result += "\n";
            while(lastTable.Error > error) {
                result += lastTable.ToString();
                result += "\n";
                lastTable = calculateNewtonEquationTablePart(list,list2, lastTable, begin, end);
            }
            result += lastTable.ToString();
            result += "\n";
            result += "Resultado: " + lastTable.X1;
            return lastTable.X1;
        }
        private static Number[] calculateNewtonList2(Number[] list) {
            Number[] list2 = new Number[list.Length - 1];
            for(int i = 1;i < list.Length;i++) {
                list2[i - 1] = list[i] * new Number(i);
            }
            return list2;
        }
        private static NewtonTablePart calculateNewtonEquationTablePart(Number[] list1, Number[] list2, NewtonTablePart lastTable, BigInteger begin, BigInteger end) {
                NewtonTablePart table = new NewtonTablePart();
            if(lastTable == null) {
                table.X0 = calculateNewtonEquationMin(list1, begin, end);
            } else {
                table.X0 = lastTable.X1;
            }
            table.FX = calculateEquationPart(list1, table.X0);
            table.FLX = calculateEquationPart(list2, table.X0);
            table.X1 = table.X0 - (table.FX/table.FLX);
            table.Error = (table.X1 - table.X0) / table.X1;
            if(table.Error < new Number(0)) {
                table.Error = table.Error * new Number(-1);
            }
            return table;
        }
        public static Number calculateNewtonEquationMax(Number[] list, BigInteger begin, BigInteger end) {
            Number n = new Number(end);

            Number tmp = calculateEquationPart(list, new Number(begin));
            if(tmp.Numerator == 0) {
                n = new Number(end);
            } else if(tmp.Numerator < 0) {
                for(BigInteger i = begin + 1;i < end;i++) {
                    Number n2 = calculateEquationPart(list, new Number(i));
                    if(n2.Numerator < 0) {
                    } else if(n2.Numerator == 0) {
                        n = new Number(i);
                        break;
                    } else {
                        n = new Number(i);
                        break;
                    }
                }
            } else {
                for(BigInteger i = begin + 1;i < end;i++) {
                    Number n2 = calculateEquationPart(list, new Number(i));
                    if(n2.Numerator > 0) {
                    } else if(n2.Numerator == 0) {
                        n = new Number(i);
                        break;
                    } else {
                        n = new Number(i);
                        break;
                    }
                }
            }

            return n;
        }
        public static Number calculateNewtonEquationMin(Number[] list, BigInteger begin, BigInteger end) {
            Number n = calculateNewtonEquationMax(list, begin, end);
            n--;
            return n;
        }

        public static Number[] lagrangeInterpolation(Number[] x, Number[] y) {
            Number[] coefficients = new Number[x.Length];
            
            int n = x.Length;
            coefficients = new Number[n];
            for (int i = 0; i < n; i++) {
                coefficients[i] = new Number(0);
            }

            // c[] are the coefficients of P(x) = (x-x[0])(x-x[1])...(x-x[n-1])
            Number[] c = new Number[n + 1];
            c[0] = new Number(1);
            for (int i = 0; i < n; i++) {
                for (int j = i; j > 0; j--) {
                    c[j] = c[j-1] - c[j] * x[i];
                }
                c[0] *= new Number(0)-x[i];
                c[i + 1] = new Number(1);
            }
            /*for(int i = 0;i < c.Length;i++) {
                System.Console.WriteLine("c[" + i + "] = " + c[i]);
            }*/
            Number[] tc = new Number[n];
            for (int i = 0; i < n; i++) {
                // d = (x[i]-x[0])...(x[i]-x[i-1])(x[i]-x[i+1])...(x[i]-x[n-1])
                Number d = new Number(1);
                for (int j = 0; j < n; j++) {
                    if (i != j) {
                        d *= x[i] - x[j];
                        
                    }
                    
                }
                System.Console.WriteLine("Fracao "+i+" = " + d);

                Number t = y[i] / d;
                // Lagrange polynomial is the sum of n terms, each of which is a
                // polynomial of degree n-1. tc[] are the coefficients of the i-th
                // numerator Pi(x) = (x-x[0])...(x-x[i-1])(x-x[i+1])...(x-x[n-1]).
                tc[n-1] = c[n];     // actually c[n] = 1
                coefficients[n-1] += t * tc[n-1];
                for (int j = n-2; j >= 0; j--) {
                    tc[j] = c[j+1] + tc[j+1] * x[i];
                    coefficients[j] += t * tc[j];
                }
            }
            /*for(int i = 0;i < tc.Length;i++) {
                System.Console.WriteLine("tc[" + i + "] = " + tc[i]);
            }*/
            System.Console.WriteLine("================");
            return coefficients;
        }
        
        public static Number permutation(Number[] n, int qt, int ignore) {
            Number sum = permutationR(n, qt, 0,new Number(0), new Number(1), ignore);
            return sum;
            

        }
        private static Number permutationR(Number[] n, int qt, int i, Number sum, Number sumPart, int ignore) {
            for(int j = i;j < n.Length-(qt-i);j++) {
                sumPart *= permutationR(n, qt-1, j, sum, sumPart, ignore);
            }

            return sum+sumPart;
        }
        
        public static Number[] newtonInterpolation(Number[] x, Number[] y) {
            Number[] a = computeDividedDifference(x, y);
            Number[] c = new Number[x.Length-1];
            System.Array.Copy(x, 0, c, 0, c.Length);
            
            int n = c.Length;

            Number[] coefficients = new Number[n + 1];
            for (int i = 0; i <= n; i++) {
                coefficients[i] = new Number(0);
            }

            coefficients[0] = a[n];
            for (int i = n-1; i >= 0; i--) {
                for (int j = n-i; j > 0; j--) {
                    coefficients[j] = coefficients[j-1] - (c[i] * coefficients[j]);
                }
                coefficients[0] = a[i] - (c[i] * coefficients[0]);
            }
            return coefficients;
        }
        private static Number[] computeDividedDifference(Number[] x, Number[] y){
            Number[] divdiff = new Number[y.Length];
            y.CopyTo(divdiff, 0);
            int n = x.Length;
            Number[] a = new Number [n];
            a[0] = divdiff[0];
            for (int i = 1; i < n; i++) {
                for (int j = 0; j < n-i; j++) {
                    Number denominator = x[j+i] - x[j];
                    divdiff[j] = (divdiff[j+1] - divdiff[j]) / denominator;
                }
                a[i] = divdiff[0];
            }
            return a;
        }
        private static void verifyVogel(Number[] demand, Number[] supply, Number[,] matrix){
            if(matrix.GetLength(1) != demand.Length || matrix.GetLength(0) != supply.Length) {
                string msg = "The demand and supply array doesn't have the same size of the matix\n";
                msg += "demand["+demand.Length+"], supply["+supply.Length+"], costs["+matrix.GetLength(0)+","+matrix.GetLength(1)+"]";
                throw new Exception(msg);
            }

            Number sumSupply = SumNumbers(supply);
            Number sumDemand = SumNumbers(demand);
            if(sumSupply != sumDemand) {
                string msg = "The sums of the supply and demand are differents!\n";
                msg += "Supply sum: " + sumSupply + ", Demand sum: " + sumDemand+"\n";
                msg += "You need to insert a new ";
                if(sumSupply > sumDemand)
                    msg += "demand (row bellow)";
                else
                    msg += "supply (colum on right)";
                msg += " with the value " + (Number.Max(sumDemand, sumSupply) - Number.Min(sumDemand, sumSupply));
                throw new Exception(msg);
            }

        }

        private static Number SumNumbers(Number[] array) {
            Number res = 0;
            for(int i = 0;i < array.Length;i++) {
                res += array[i];
            }
            return res;
        }
        
        private static Number[,] VogelFirstPart(Number[,] costs, Number[] demand, Number[] supply) {
            verifyVogel(demand, supply, costs);
            Functions.result += "Matriz de custos de entrada:\n";
            Functions.result += printMatrix(costs);
            Functions.result += "Supply de entrada:\n";
            for(int i = 0;i < supply.Length;i++) {
                Functions.result += supply[i] + (i+1<supply.Length?", ":"");
            }
            Functions.result += "\nDemand de entrada:\n";
            for(int i = 0;i < demand.Length;i++) {
                Functions.result += demand[i] + (i + 1 < demand.Length ? ", " : "");
            }
            Functions.result += "\n----------------------------\n";
            Number[] demandRemaining = new Number[demand.Length];
            Number[] supplyRemaining = new Number[supply.Length];
            Number[,] result = new Number[costs.GetLength(0), costs.GetLength(1)];
            List<VogelPos> taxaDegeneracao = new List<VogelPos>();

            Array.Copy(demand, demandRemaining, demand.Length);
            Array.Copy(supply, supplyRemaining, supply.Length);

            List<VogelPos> temp = new List<VogelPos>();

            for (int l = 0; l < costs.GetLength(0); l++) {
                temp.Clear();
                for (int c = 0; c < costs.GetLength(1); c++) {
                    temp.Add(new VogelPos(l,c, costs[l, c],true));
                }
                var n = temp.OrderBy(x => x.number);

                if(costs[n.ElementAt(0).x, n.ElementAt(0).y] < costs[n.ElementAt(1).x, n.ElementAt(1).y]) {
                    taxaDegeneracao.Add(new VogelPos(n.ElementAt(0).x, n.ElementAt(0).y, n.ElementAt(1).number - n.ElementAt(0).number,true));
                } else {
                    taxaDegeneracao.Add(new VogelPos(n.ElementAt(1).x, n.ElementAt(1).y, n.ElementAt(0).number - n.ElementAt(1).number,true));
                }
            }

            for(int c = 0;c < costs.GetLength(1);c++) {
                temp.Clear();
                for(int l = 0;l < costs.GetLength(0);l++) {
                    temp.Add(new VogelPos(l, c, costs[l, c],false));
                }
                temp = temp.OrderBy(x => x.number).ToList();

                if(costs[temp.ElementAt(0).x, temp.ElementAt(0).y] < costs[temp.ElementAt(1).x, temp.ElementAt(1).y]) {
                    taxaDegeneracao.Add(new VogelPos(temp.ElementAt(0).x, temp.ElementAt(0).y,  temp.ElementAt(1).number - temp.ElementAt(0).number,false));
                } else {
                    taxaDegeneracao.Add(new VogelPos(temp.ElementAt(1).x, temp.ElementAt(1).y, temp.ElementAt(0).number - temp.ElementAt(1).number,false));
                }
            }
            Functions.result += "Taxa de degeneração: \n";
            for(int i = 0;i < taxaDegeneracao.Count;i++) {
                Functions.result += taxaDegeneracao[i].number + " @ " + (taxaDegeneracao[i].x + 1) + "," + (taxaDegeneracao[i].y + 1) + (taxaDegeneracao[i].line?" linha ":" coluna ") + "\n";
            }
            Functions.result += "---------- \n";

            taxaDegeneracao = taxaDegeneracao.OrderBy(x => 0-x.number).ThenBy(x=>costs[x.x,x.y]).ToList();

            Functions.result += "Taxa de degeneração ordenada: \n";
            for(int i = 0;i < taxaDegeneracao.Count;i++) {
                Functions.result += taxaDegeneracao[i].number + " @ " + (taxaDegeneracao[i].x + 1) + "," + (taxaDegeneracao[i].y + 1) + (taxaDegeneracao[i].line ? " linha " : " coluna ") + "\n";
            }
            Functions.result += "---------- \n";

            //Respeita a ordem da taxa de degeneracao
            for(int i = 0;i < taxaDegeneracao.Count;i++) {
                int l = taxaDegeneracao[i].x;
                int c = taxaDegeneracao[i].y;
                Number min = long.MaxValue;
                Number minSupplyDemand;
                
                if(taxaDegeneracao[i].line) {
                    for(int j = 0;j < costs.GetLength(1);j++) {
                        if(costs[l, j] < min) {
                            if(supply[l] > 0 && demand[j] > 0) {
                                min = costs[l, j];
                                c = j;
                            }
                        }
                    }
                } else {
                    for(int j = 0;j < costs.GetLength(0);j++) {
                        if(costs[j, c] < min) {
                            if(supply[j] > 0 && demand[c] > 0) {
                                min = costs[j, c];
                                l = j;
                            }
                        }
                    }
                }
                if(supply[l] == 0 || demand[c] == 0)
                    continue;
                minSupplyDemand = Number.Min(supply[l], demand[c]);
                supply.SetValue(supply.ElementAt(l) - minSupplyDemand, l);
                demand.SetValue(demand.ElementAt(c) - minSupplyDemand, c);
                result[l, c] = minSupplyDemand;
            }

            temp.Clear();
            for(int l = 0;l < costs.GetLength(0);l++) {
                for(int c = 0;c < costs.GetLength(1);c++) {
                    temp.Add(new VogelPos(l, c, costs[l, c],false));
                }
            }
            temp = temp.OrderBy(x => x.number).ToList();
            
            for(int i = 0;i < temp.Count;i++) {
                if(supply[temp[i].x] > 0) {
                    if(demand[temp[i].y] > 0){
                        Number min = Number.Min(supply[temp[i].x], demand[temp[i].y]);
                            result[temp[i].x, temp[i].y] = min;
                            supply[temp[i].x] -= min;
                            demand[temp[i].y] -= min;
                    }
                }
            }
            Functions.result += "Matriz antes de fazer a segunda parte:\n";
            Functions.result += printMatrix(result);
            Functions.result += "-------------------------------\n";


            return result;
        }
        private static Number[,] CalculateVogelInternalMatrix(Number[,] matrix) {
            return null;
        }

        private static Number[,] VogelSecondPart(Number[,] costs, Number[] demand, Number[] supply, Number[,] matrix) {

            VogelPos min;
            
            int count = 1;

            do {
                min = new VogelPos(-1, -1, long.MaxValue, false);
                Number[,] res = new Number[matrix.GetLength(0), matrix.GetLength(1)];
                bool[,] usedMatrix = GetMatrixInsideVogel(matrix);
                bool[,] usedMatrix2 = new bool[usedMatrix.GetLength(0), usedMatrix.GetLength(1)];
                System.Array.Copy(usedMatrix, usedMatrix2, usedMatrix.Length);
                Number[,] tempMatrix = new Number[matrix.GetLength(0), matrix.GetLength(1)];

                Functions.result += "=============================\n";
                Functions.result += "Iteração "+count+"\n";
                Functions.result += "=============================\n";

                Number[] lines = new Number[matrix.GetLength(0)];
                Number[] colums = new Number[matrix.GetLength(1)];

                goLine(-1, -1, 0, tempMatrix, costs, usedMatrix2, lines, colums);

                bool erroL = false;
                bool erroC = false;
                for(int i = 0;i < lines.Length;i++) {
                    if(lines[i] == null)
                        erroL = true;
                }
                for(int i = 0;i < colums.Length;i++) {
                    if(colums[i] == null)
                        erroC = true;
                }
                if(erroL || erroC) {
                    min.number = -1;
                    Functions.result += "=============================\n";
                    Functions.result += "Não foi possível colocar valores em todas as linhas e colunas.\n";
                    Functions.result += "Deve-se criar uma variável artificial.\n";
                    int posX = -1;
                    int posY = -1;

                    if(erroL){
                        for(int i = 0;i < lines.Length;i++) {
                            if(lines[i] == null) {
                                for(int j = 0;j < matrix.GetLength(1);j++) {
                                    if(matrix[i, j] == null) {
                                        posX = i;
                                        posY = j;
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    } else {
                        for(int i = 0;i < colums.Length;i++) {
                            if(colums[i] == null) {
                                for(int j = 0;j < matrix.GetLength(0);j++) {
                                    if(matrix[j, i] == null) {
                                        posX = i;
                                        posY = j;
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    }
                    matrix[posX, posY] = 0;
                    Functions.result += "Ela foi colocada em: L"+(posX+1)+", C"+(posY+1)+"\n";
                    Functions.result += "=============================\n";
                    continue;
                }

                for(int i = 0;i < lines.Length;i++) {
                    Functions.result += "L" + (i + 1) + ":" + lines[i] + ", \t";
                }
                Functions.result += "\n";
                for(int i = 0;i < colums.Length;i++) {
                    Functions.result += "C" + (i + 1) + ":" + colums[i] + ", \t"; ;
                }
                Functions.result += "\n\n";

                Functions.result += "Matriz dentro:\n";
                Functions.result += printMatrix(tempMatrix);
                Functions.result += "-------------------------\n";



                sumOut(lines, colums, usedMatrix, costs, tempMatrix);

                Functions.result += "Matriz fora:\n";
                Functions.result += printMatrix(tempMatrix);
                Functions.result += "-------------------------\n";

            
                for(int l = 0;l < tempMatrix.GetLength(0);l++) {
                    for(int c = 0;c < tempMatrix.GetLength(1);c++) {
                        if(tempMatrix[l, c] != null) {
                            if(tempMatrix[l, c] < min.number) {
                                min.y = c;
                                min.x = l;
                                min.number = tempMatrix[l, c];
                            }
                        }
                    }
                }
                Functions.result += "Valor mínimo: "+min.number+" na linha "+(min.x+1)+" e coluna "+(min.y+1)+"\n";


                if(min.number < 0) {
                    List<VogelPos> list = VogelFindWay(min, usedMatrix, matrix);
                    if(list == null)
                        break;
                    
                    Number min2 = Number.Min(list[0].number, list[list.Count - 1].number);
                    Functions.result += "Minimo: " + min2 + "\n";
                    
                    bool soma = false;
                    for(int i = 0;i < list.Count;i++) {
                        if(matrix[list[i].x, list[i].y] == null)
                            matrix[list[i].x, list[i].y] = 0;

                        if(soma) {
                            matrix[list[i].x, list[i].y] += min2;
                        } else {
                            matrix[list[i].x, list[i].y] -= min2;
                        }

                        if(matrix[list[i].x, list[i].y] == 0)
                            matrix[list[i].x, list[i].y] = null;
                        soma = !soma;
                    }
                    matrix[min.x, min.y] += min2;
                    Functions.result += "Fez a troca\n";
                }
                Functions.result += "-------------------------\n";
                Functions.result += "Matriz: \n";
                Functions.result += "-------------------------\n";
                Functions.result += printMatrix(matrix);
                Functions.result += "-------------------------\n";
                count++;
            } while(min.number<0 && count <= 25);
            if(count > 20) {
                Functions.result += "***********************\n";
                Functions.result += "Houve muitas iterações, algo está errado!\n";
                Functions.result += "***********************\n";
            }
            bool tirouArt = false;
            for(int l = 0;l < matrix.GetLength(0);l++) {
                for(int c = 0;c < matrix.GetLength(1);c++) {
                    if(matrix[l, c] == 0) {
                        matrix[l, c] = null;
                        tirouArt = true;
                    }
                }
            }
            if(tirouArt) {
                Functions.result += "Foram tiradas as variáveis artificiais, ficando assim:\n";
                Functions.result += printMatrix(matrix);
                Functions.result += "-------------------------\n";
            }
            return matrix;
        }

        private static List<VogelPos> VogelFindWay(VogelPos destiny, bool[,] usedMatrix, Number[,] matrix) {
            bool[,] usedMatrixTemp = new bool[usedMatrix.GetLength(0),usedMatrix.GetLength(1)];
            Array.Copy(usedMatrix, usedMatrixTemp, usedMatrix.Length);
            List<VogelPos> list = new List<VogelPos>();
            if(VogelFindWayLine(list, destiny, usedMatrixTemp, destiny, matrix)) {
                Functions.result += "Caminho encontrado:\n";
                for(int i = 0;i < list.Count;i++) {
                    Functions.result += "[" + (list[i].x + 1) + ", " + (list[i].y + 1) + "] ";
                }
                Functions.result += "\n";

                return list;
            } else {
                Functions.result += "++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n";
                Functions.result += "Não encontrou caminho. ERRO!\n";
                Functions.result += "++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n";

            }
            return null;
        }
        private static bool VogelFindWayLine(List<VogelPos> list, VogelPos destiny, bool[,] usedMatrix, VogelPos now, Number[,] matrix) {
            for(int y = 0;y < usedMatrix.GetLength(1);y++) {
                if(y == destiny.y && usedMatrix[now.x,y]) {
                    VogelPos v = new VogelPos(now.x, y, matrix[now.x, y], true);
                    usedMatrix[v.x, v.y] = false;
                    list.Add(v);
                    return true;
                }
                if(usedMatrix[now.x, y]) {
                    VogelPos v = new VogelPos(now.x, y, matrix[now.x,y], true);
                    usedMatrix[v.x, v.y] = false;
                    list.Add(v);
                    if(VogelFindWayColum(list,destiny,usedMatrix,v,matrix)){
                        return true;
                    }
                    list.Remove(v);
                }
            }
            return false;
        }
        private static bool VogelFindWayColum(List<VogelPos> list, VogelPos destiny, bool[,] usedMatrix, VogelPos now, Number[,] matrix) {
            for(int x = 0;x < usedMatrix.GetLength(0);x++) {
                if(x == destiny.x && usedMatrix[x, now.y]) {
                    return true;
                }
                if(usedMatrix[x, now.y]) {
                    VogelPos v = new VogelPos(x, now.y, matrix[x, now.y], true);
                    usedMatrix[v.x, v.y] = false;
                    list.Add(v);
                    if(VogelFindWayLine(list, destiny, usedMatrix, v, matrix)) {
                        return true;
                    }
                    list.Remove(v);
                }
            }
            return false;
        }

        private static void sumOut(Number[] lines, Number[] colums, bool[,] usedMatrix, Number[,] costs, Number[,] tempMatrix) {
            for(int l = 0;l < tempMatrix.GetLength(0);l++) {
                for(int c = 0;c < tempMatrix.GetLength(1);c++) {
                    if(!usedMatrix[l, c]) {
                        tempMatrix[l, c] = costs[l, c] - lines[l] - colums[c];
                    } else {
                        tempMatrix[l, c] = null;
                    }
                }
            }
        }

        private static void goLine(int x, int y, Number v, Number[,] m, Number[,] costs, bool[,] usedMatrix, Number[] lines, Number[] colums) {
            if(x == -1 && y == -1) {
                lines[0] = 0;
            }
            for(int i = 0;i < m.GetLength(1);i++) {
                if(i == y)
                    continue;
                
                if((x == -1 && y == -1) || usedMatrix[x,i]) {
                    if(x == -1 && y == -1) {
                        if(usedMatrix[0, i]) {
                            m[0, i] = 0 - v + costs[0, i];
                            if(colums[i] == null)
                                colums[i] = m[0, i];
                            usedMatrix[0, i] = false;
                            goColum(0, i, m[0, i], m, costs, usedMatrix, lines, colums);
                        }
                    } else {
                        m[x, i] = 0 - v + costs[x, i];
                        if(colums[i] == null)
                            colums[i] = m[x, i];
                        usedMatrix[x, i] = false;
                        goColum(x, i, m[x, i], m, costs, usedMatrix, lines, colums);
                    }
                }
            }
        }
        private static void goColum(int x, int y, Number v, Number[,] m, Number[,] costs, bool[,] usedMatrix, Number[] lines, Number[] colums) {
            for(int i = 0;i < m.GetLength(0);i++) {
                if(i == x)
                    continue;
                if(usedMatrix[i,y]) {
                    m[i,y] = 0 - v + costs[i,y];
                    if(lines[i] == null)
                        lines[i] = m[i, y];
                    usedMatrix[i,y] = false;
                    goLine(i,y, m[i,y], m, costs, usedMatrix, lines, colums);
                }
            }
        }

        private static bool[,] GetMatrixInsideVogel(Number[,] matrix) {
            bool[,] res = new bool[matrix.GetLength(0), matrix.GetLength(1)];
            for (int l = 0; l < matrix.GetLength(0); l++) {
                for (int c = 0; c < matrix.GetLength(1); c++) {
                    if (matrix[l, c] != null)
                        res[l, c] = true;
                    else
                        res[l, c] = false;
                }
            }
            return res;
        }
        public static Number[,] VogelApproximationMethod(Number[,] costs, Number[] demand, Number[] supply) {
            clearResult();
            Number[,] res = VogelFirstPart(costs, demand, supply);
            res = VogelSecondPart(costs, demand, supply, res);
            Functions.result += "\n-------------------\n";
            VogelCalculateTotalCost(res, costs);
            System.Console.WriteLine(Result);
            return res;
        }
        public static Number[,] VogelApproximationMethod(Number[,] costs, Number[] demand, Number[] supply, string path) {
            clearResult();
            Number[,] res = VogelFirstPart(costs, demand, supply);
            res = VogelSecondPart(costs, demand, supply, res);
            Functions.result += "\n-------------------\n";
            VogelCalculateTotalCost(res, costs);
            writeFile(path);
            System.Console.WriteLine("Output to "+path);
            return res;
        }
        public static Number[,] NorthWestMethod(Number[,] costs, Number[] demand, Number[] supply, string path) {
            clearResult();
            Number[,] res = NorthWestFirstPart(costs, demand, supply);
            res = VogelSecondPart(costs, demand, supply, res);
            Functions.result += "\n-------------------\n";
            VogelCalculateTotalCost(res, costs);
            writeFile(path);
            System.Console.WriteLine("Output to " + path);
            return res;
        }
        public static Number[,] NorthWestMethod(Number[,] costs, Number[] demand, Number[] supply) {
            clearResult();
            Number[,] res = NorthWestFirstPart(costs, demand, supply);
            res = VogelSecondPart(costs, demand, supply, res);
            Functions.result += "\n-------------------\n";
            VogelCalculateTotalCost(res, costs);
            System.Console.WriteLine(Result);
            return res;
        }
        private static Number[,] NorthWestFirstPart(Number[,] costs, Number[] demand, Number[] supply) {
            verifyVogel(demand, supply, costs);
            Functions.result += "Matriz de custos de entrada:\n";
            Functions.result += printMatrix(costs);
            Functions.result += "Supply de entrada:\n";
            for(int i = 0;i < supply.Length;i++) {
                Functions.result += supply[i] + (i + 1 < supply.Length ? ", " : "");
            }
            Functions.result += "\nDemand de entrada:\n";
            for(int i = 0;i < demand.Length;i++) {
                Functions.result += demand[i] + (i + 1 < demand.Length ? ", " : "");
            }
            Functions.result += "\n----------------------------\n";

            Number[] demandRemaining = new Number[demand.Length];
            Number[] supplyRemaining = new Number[supply.Length];
            Number[,] result = new Number[costs.GetLength(0), costs.GetLength(1)];

            Array.Copy(demand, demandRemaining, demand.Length);
            Array.Copy(supply, supplyRemaining, supply.Length);

            int posL = 0;
            int posC = 0;
            while(demandRemaining[posC]!=0 && supplyRemaining[posL]!=0) {
                Number min = Number.Min(demandRemaining[posC], supplyRemaining[posL]);
                result[posL, posC] = min;
                demandRemaining[posC] -= min;
                supplyRemaining[posL] -= min;
                if(supplyRemaining[posL] == 0) {
                    if(supplyRemaining.Length!=posL+1)
                        posL++;
                }
                if(demandRemaining[posC] == 0) {
                    if(demandRemaining.Length != posC + 1)
                        posC++;
                }

            }
            Functions.result += "Matriz antes de fazer a segunda parte:\n";
            Functions.result += printMatrix(result);
            Functions.result += "-------------------------------\n";
            return result;

        }
        public static Number[,] VogelInverseMatrix(Number[,] matrix) {
            Number max = long.MinValue;
            Number[,] newMatrix = new Number[matrix.GetLength(0), matrix.GetLength(1)];
            for(int l = 0;l < matrix.GetLength(0);l++) {
                for(int c = 0;c < matrix.GetLength(1);c++) {
                    max = Number.Max(max, matrix[l, c]);
                }
            }
            for(int l = 0;l < matrix.GetLength(0);l++) {
                for(int c = 0;c < matrix.GetLength(1);c++) {
                    newMatrix[l, c] = max - matrix[l, c];
                }
            }
            return newMatrix;
        }
        
        private static Number VogelCalculateTotalCost(Number[,] matrix, Number[,] costs) {
            result += "Total cost: ";
            Number sum = 0;
            for(int l = 0;l < matrix.GetLength(0);l++) {
                for(int c = 0;c < matrix.GetLength(1);c++) {
                    sum += matrix[l, c] * costs[l, c];
                }
            }
            result += sum;
            result += "\n";
            return sum;
        }
        private static string printMatrix(Number[,] m) {
            string s = "";
            for(int l = 0;l < m.GetLength(0);l++) {
                for(int c = 0;c < m.GetLength(1);c++) {
                    s += (m[l, c] + "\t|");
                }
                s += "\n";
            }
            return s;
        }
    }

    class Function {
        public Function(Number[] l) {
            list = l;
        }
        private Number[] list;
        public Number[] List{
            get { return list; }
            set { list = value; }
        }
        public static Function operator *(Function n1, Function n2) {
            Number[] res = new Number[n1.list.Length + n2.list.Length];
            for(int i = 0;i < res.Length;i++) {
                res[i] = new Number(0);
            }
            for(int i = 0;i < n1.list.Length;i++) {
                for(int j = 0;j < n2.list.Length;j++) {
                    res[i + j] += n1.list[i] * n2.list[j];
                }
            }
            return new Function(res);
        }
        public static Function operator /(Function n1, Function n2) {
            Number[] res = new Number[n1.list.Length + n2.list.Length];
            for(int i = 0;i < res.Length;i++) {
                res[i] = new Number(0);
            }
            for(int i = 0;i < n1.list.Length;i++) {
                for(int j = 0;j < n2.list.Length;j++) {
                    if(i - j >= 0)
                        res[i - j] += n1.list[i] / n2.list[j];
                    else {
                        //Implementeção com índice negativo (x^-1)
                    }
                }
            }
            return new Function(res);
        }
        public static Function operator +(Function n1, Function n2) {
            Number[] res;
            if(n1.List.Length> n2.List.Length)
                res = new Number[n1.list.Length];
            else
                res = new Number[n2.list.Length];
            for(int i = 0;i < res.Length;i++) {
                res[i] = new Number(0);
            }
            for(int i = 0;i < res.Length;i++) {
                if(n2.list.Length>i)
                    res[i] += n1.list[i] + n2.list[i];
            }
            return new Function(res);
        }
        public static Function operator -(Function n1, Function n2) {
            Number[] res;
            if(n1.List.Length > n2.List.Length)
                res = new Number[n1.list.Length];
            else
                res = new Number[n2.list.Length];
            for(int i = 0;i < res.Length;i++) {
                res[i] = new Number(0);
            }
            for(int i = 0;i < res.Length;i++) {
                if(n2.list.Length > i)
                    res[i] += n1.list[i] - n2.list[i];
            }
            return new Function(res);
        }
        public override String ToString() {
            string res = "";
            for(int i = 0;i < list.Length;i++) {
                if(list[i] != new Number(0))
                res += "("+list[i]+")x^"+i+" ";
            }
            return res;
        }
    }

    class BisectInterval {
        private Number begin = new Number(0);
        private Number end = new Number(1);
        private Number average = new Number(1, 2);
        public Number Begin {
            set { begin = value; }
            get { return begin; }
        }
        public Number End {
            set { end = value; }
            get { return end; }
        }
        public Number Average {
            set { average = value; }
            get { return average; }
        }
        public Number DoAverage() {
            BigInteger num = begin.Numerator * end.Denominator + end.Numerator * begin.Denominator;
            BigInteger den = begin.Denominator * end.Denominator * 2;
            Average = Number.simplify(new Number(num, den));
            return Average;
        }
        
        
    }

    class BisectTablePart {
        private BisectInterval interval = new BisectInterval();
        private BisectInterval resInterval = new BisectInterval();
        private Number error = new Number(1);

        public BisectInterval BisectInterval {
            set { interval = value; }
            get { return interval; }
        }
        public BisectInterval ResInterval {
            set { resInterval = value; }
            get { return resInterval; }
        }
        public Number Error {
            set { error = value; }
            get { return error; }
        }
        public override String ToString() {
            string res = "";
            res += "a: " + interval.Begin;
            res += "    m: " + interval.Average;
            res += "    b: " + interval.End;
            res += "    f(a): " + resInterval.Begin;
            res += "    f(m): " + resInterval.Average;
            res += "    f(b): " + resInterval.End;
            res += "    erro: " + error;
            return res;
        }
    }
    class NewtonTablePart {
        private Number x0 = new Number(0);
        private Number x1 = new Number(0);
        private Number fx = new Number(0);
        private Number flx = new Number(0);
        private Number error = new Number(1);

        public Number X0 {
            get { return x0; }
            set { x0 = value; }
        }
        public Number X1 {
            get { return x1; }
            set { x1 = value; }
        }
        public Number FX {
            get { return fx; }
            set { fx = value; }
        }
        public Number FLX {
            get { return flx; }
            set { flx = value; }
        }
        public Number Error {
            get { return error; }
            set { error = value; }
        }
        public override String ToString() {
            string res = "";
            res += "x0: " + x0;
            res += "    f(x0): " + fx;
            res += "    f('x0): " + flx;
            res += "    x1: " + x1;
            res += "    erro: " + error;
            return res;
        }
    }

    class VogelPos {
        public int x;
        public int y;
        public bool line;

        public Number number;
        public VogelPos(int x, int y, Number number, bool line) {
            this.x = x;
            this.y = y;
            this.number = number;
            this.line = line;
        }
    }
}
