using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace FractionedNumber{
    public class Number : IComparable, IComparable<Number>, IEquatable<Number> {
        private BigInteger numerator = 0;
        public BigInteger Numerator {
            get { return numerator; }
            set {
                if(value == 0)
                    Denominator = 1;
                numerator = value; 
            }
        }
        private BigInteger denominator = 1;
        public BigInteger Denominator {
            get { return denominator; }
            set { 
                if(value == 0) 
                    throw new System.ArgumentException("The denominator cannot be 0 (zero)");
                denominator = value; 
            }
        }

        private Number() { }
        private void Validate(){
            if(Denominator < 0) {
                Numerator = -Numerator;
                Denominator = -Denominator;
            }
        }
        public Number(BigInteger n, BigInteger d) {
            Numerator = n;
            Denominator = d;
            Validate();
        }
        public Number(BigInteger n) {
            Numerator = n;
            Denominator = 1;
        }
        public Number(string value) {
            bool found = false;
            for(int i = 0; i < value.Length; i++) {
                if(value.ElementAt(i) == '/') {
                    found = true;

                    BigInteger num;
                    bool parsed1 = BigInteger.TryParse(value.Substring(0, i), out num);
                    BigInteger den;
                    bool parsed2 = BigInteger.TryParse(value.Substring(i + 1, value.Length - i - 1), out den);
                    if(parsed1 && parsed2) {
                        Numerator = num;
                        Denominator = den;
                    } else {
                        throw new System.ArgumentException("Invalid string fraction ({0})",value);
                    }
                    
                }
            }
            if(!found) {
                BigInteger number;
                bool parsed = BigInteger.TryParse(value, out number);
                if(parsed) {
                    Numerator = number;
                    Denominator = 1;
                } else {
                    throw new System.ArgumentException("Invalid value ({0})",value);
                }
            }
            Validate();
        }
        public static Number operator +(Number n1, Number n2) {
            if(n1 == null && n2 == null)
                return null;
            if(n1 == null)
                return n2;
            if(n2 == null)
                return n1;
            Number n = new Number();
            if(n1.Numerator == 0) {
                n = n2;
            } else if(n2.Numerator == 0){
                n = n1;
            } else if(n1.Denominator == n2.Denominator) {
                n.Numerator = n1.Numerator + n2.Numerator;
                n.Denominator = n1.Denominator;
                n = simplify(n);
            } else {
                n.Numerator = n2.Denominator * n1.Numerator +
                                n1.Denominator * n2.Numerator;
                n.Denominator = n1.Denominator * n2.Denominator;
                n = simplify(n);
            }
            if(n.Denominator < 0) {
                n.Numerator = -n.Numerator;
                n.Denominator = -n.Denominator;
            }
            n.Validate();
            return n;
        }
        public static Number operator -(Number n1, Number n2) {
            Number n = new Number();
            n.Numerator = -n2.Numerator;
            n.Denominator = n2.Denominator;
            return n1+n;
        }
        public static Number operator *(Number n1, Number n2) {
            if(n1 == null || n2 == null)
                return null;
            Number n = new Number();
            n.Numerator = n1.Numerator * n2.Numerator;
            n.Denominator = n1.Denominator * n2.Denominator;
            n = simplify(n);

            if(n.Denominator < 0) {
                n.Numerator = -n.Numerator;
                n.Denominator = -n.Denominator;
            }
            n.Validate();
            return n;
        }
        public static Number operator /(Number n1, Number n2) {
            Number n = new Number();
            n.Numerator = n2.Denominator;
            n.Denominator = n2.Numerator;
            return n1*n;
        }
        public static Number operator ++(Number n1) {
            if(n1 == null)
                return null;
            Number n = new Number(1);
            return n1 + n;
        }
        public static Number operator --(Number n1) {
            if(n1 == null)
                return null;
            Number n = new Number(-1);
            return n1 + n;
        }
        public static bool operator ==(Number n1, Number n2){
            if((object) n1 == (object) n2)
                return true;
            if((object)n1==null || (object)n2 == null)
                return false;

            Number n1tmp = n1;
            Number n2tmp = n2;
            n1tmp = Number.simplify(n1tmp);
            n2tmp = Number.simplify(n2tmp);
            return ((n1tmp.Numerator == n2tmp.Numerator) &&
                    (n1tmp.Denominator == n2tmp.Denominator));
        }
        public override bool Equals(object obj) {
            Number other = null;
            try {
                other = (Number) obj;
                return this == other;
            } catch(Exception) { }
            return false;
        }

        public static bool operator >=(Number n1, Number n2) {
            if((n1 - n2).Numerator >= 0)
                return true;
            return false; 
        }
        public static bool operator >(Number n1, Number n2) {
            if((n1 - n2).Numerator > 0)
                return true;
            return false;
        }
        public static bool operator <=(Number n1, Number n2) {
            if((n1 - n2).Numerator <= 0)
                return true;
            return false;
        }
        public static bool operator <(Number n1, Number n2) {
            if((n1 - n2).Numerator < 0)
                return true;
            return false;
        }
        public static bool operator !=(Number n1, Number n2) {
            return !(n1==n2);
        }
        public static explicit operator double(Number n) {
            double res = 0;
            double tmp2 = 1;
            if(n.Denominator == 0) {
                if(n.Numerator < 0)
                    return double.NegativeInfinity;
                else if(n.Numerator > 0)
                    return double.PositiveInfinity;
                else
                    return double.NaN;
            }
            Number n2;
            if(n.Numerator>=0)
                n2 = new Number(n.Numerator, n.Denominator);
            else
                n2 = new Number(-n.Numerator, n.Denominator);
            if(n.Denominator < 0)
                n2.Denominator = -n2.Denominator;
            for(int i = 0;i < 20;i++) {
                BigInteger tmp1 = 0;
                int num = 0;
                

                while(true) {
                    tmp1 += n2.Denominator;
                    if(tmp1 > n2.Numerator) {
                        tmp1 -= n2.Denominator;
                        n2.Numerator = (n2.Numerator - tmp1) * 10;
                        break;
                    } else if(tmp1 == n2.Numerator) {
                        num++;
                        res += (num * tmp2);
                        if(n.Numerator < 0)
                            res = res * -1;
                        if(n.Denominator < 0)
                            res = res * -1;
                        return res;
                    }
                    num++;
                }
                res += (num * tmp2);
                tmp2 = tmp2 / 10;
            }
            if(n.Numerator < 0)
                res = res * -1;
            if(n.Denominator < 0)
                res = res * -1;
            return res;
        }
        public static implicit operator Number(int n) {
            return new Number(n);
        }
        public static implicit operator Number(long n) {
            return new Number(n);
        }
        private static BigInteger mdc(Number inNumber) {
            BigInteger last;
            BigInteger rest;
            if(inNumber.Numerator>inNumber.Denominator){
                last = inNumber.Numerator;
                rest = inNumber.Denominator;
            }else{
                last = inNumber.Denominator;
                rest = inNumber.Numerator;
            }
            while(rest != 0) {
                BigInteger tmp = rest;
                rest = last % rest;
                last = tmp;
            }
            return last;
        }
        public static Number simplify(Number inNumber) {
            Number num = inNumber;
            BigInteger numMdc = mdc(num);
            num.Denominator = num.Denominator / numMdc;
            num.Numerator = num.Numerator / numMdc;
            num.Validate();
            return num;
        }

        public override string ToString() {
            string res = Numerator.ToString();
            if(Denominator != 1) {
                res += "/";
                res += Denominator.ToString();
            }
            return res;
        }

        public static Number Min(Number n1, Number n2) {
            return n1 > n2 ? n2 : n1;
        }
        public static Number Max(Number n1, Number n2) {
            return n1 > n2 ? n1 : n2;
        }



        public int CompareTo(object obj) {
            if (obj == null) return 1;
            Number other = obj as Number;
            if (other != null) {
                if (this < other) return -1;
                else if (this > other) return 1;
                return 0;
            } else
                throw new ArgumentException("Object is not a Number");
        }

        public bool Equals(Number other) {
            return this == other;
        }

        public int CompareTo(Number other) {
            if (this < other) return -1;
            else if (this > other) return 1;
            return 0;
        }

        public static bool Equals(Number n1, Number n2) {
            return n1 == n2;
        }

        public static int CompareTo(Number n1, Number n2) {
            if (n1<n2) return -1;
            else if (n1>n2) return 1;
            return 0;
        }
    }
}
