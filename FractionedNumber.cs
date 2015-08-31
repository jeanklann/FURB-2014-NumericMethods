using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace FractionedNumber{
    public class FractionedNumber : IComparable, IComparable<FractionedNumber>, IEquatable<FractionedNumber> {
        private Number numerator = 0;
        public Number Numerator {
            get { return numerator; }
            set {
                if(value == 0)
                    Denominator = 1;
                numerator = value; 
            }
        }
        private Number denominator = 1;
        public Number Denominator {
            get { return denominator; }
            set { 
                if(value == 0) 
                    throw new System.ArgumentException("The denominator cannot be 0 (zero)");
                denominator = value; 
            }
        }

        private FractionedNumber() { }
        private void Validate(){
            if(Denominator < 0) {
                Numerator = -Numerator;
                Denominator = -Denominator;
            }
        }
        public FractionedNumber(Number n, Number d) {
            Numerator = n;
            Denominator = d;
            Validate();
        }
        public FractionedNumber(Number n) {
            Numerator = n;
            Denominator = 1;
        }
        public FractionedNumber(string value) {
            bool found = false;
            for(int i = 0; i < value.Length; i++) {
                if(value.ElementAt(i) == '/') {
                    found = true;

                    Number num;
                    bool parsed1 = Number.TryParse(value.Substring(0, i), out num);
                    Number den;
                    bool parsed2 = Number.TryParse(value.Substring(i + 1, value.Length - i - 1), out den);
                    if(parsed1 && parsed2) {
                        Numerator = num;
                        Denominator = den;
                    } else {
                        throw new System.ArgumentException("Invalid string fraction ({0})",value);
                    }
                    
                }
            }
            if(!found) {
                Number number;
                bool parsed = Number.TryParse(value, out number);
                if(parsed) {
                    Numerator = number;
                    Denominator = 1;
                } else {
                    throw new System.ArgumentException("Invalid value ({0})",value);
                }
            }
            Validate();
        }
        public static FractionedNumber operator +(FractionedNumber n1, FractionedNumber n2) {
            if(n1 == null && n2 == null)
                return null;
            if(n1 == null)
                return n2;
            if(n2 == null)
                return n1;
            FractionedNumber n = new FractionedNumber();
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
        public static FractionedNumber operator -(FractionedNumber n1, FractionedNumber n2) {
            FractionedNumber n = new FractionedNumber();
            n.Numerator = -n2.Numerator;
            n.Denominator = n2.Denominator;
            return n1+n;
        }
        public static FractionedNumber operator *(FractionedNumber n1, FractionedNumber n2) {
            if(n1 == null || n2 == null)
                return null;
            FractionedNumber n = new FractionedNumber();
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
        public static FractionedNumber operator /(FractionedNumber n1, FractionedNumber n2) {
            FractionedNumber n = new FractionedNumber();
            n.Numerator = n2.Denominator;
            n.Denominator = n2.Numerator;
            return n1*n;
        }
        public static FractionedNumber operator ++(FractionedNumber n1) {
            if(n1 == null)
                return null;
            FractionedNumber n = new FractionedNumber(1);
            return n1 + n;
        }
        public static FractionedNumber operator --(FractionedNumber n1) {
            if(n1 == null)
                return null;
            FractionedNumber n = new FractionedNumber(-1);
            return n1 + n;
        }
        public static bool operator ==(FractionedNumber n1, FractionedNumber n2){
            if((object) n1 == (object) n2)
                return true;
            if((object)n1==null || (object)n2 == null)
                return false;

            FractionedNumber n1tmp = n1;
            FractionedNumber n2tmp = n2;
            n1tmp = FractionedNumber.simplify(n1tmp);
            n2tmp = FractionedNumber.simplify(n2tmp);
            return ((n1tmp.Numerator == n2tmp.Numerator) &&
                    (n1tmp.Denominator == n2tmp.Denominator));
        }
        public override bool Equals(object obj) {
            FractionedNumber other = null;
            try {
                other = (FractionedNumber) obj;
                return this == other;
            } catch(Exception) { }
            return false;
        }

        public static bool operator >=(FractionedNumber n1, FractionedNumber n2) {
            if((n1 - n2).Numerator >= 0)
                return true;
            return false; 
        }
        public static bool operator >(FractionedNumber n1, FractionedNumber n2) {
            if((n1 - n2).Numerator > 0)
                return true;
            return false;
        }
        public static bool operator <=(FractionedNumber n1, FractionedNumber n2) {
            if((n1 - n2).Numerator <= 0)
                return true;
            return false;
        }
        public static bool operator <(FractionedNumber n1, FractionedNumber n2) {
            if((n1 - n2).Numerator < 0)
                return true;
            return false;
        }
        public static bool operator !=(FractionedNumber n1, FractionedNumber n2) {
            return !(n1==n2);
        }
        public static explicit operator double(FractionedNumber n) {
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
            FractionedNumber n2;
            if(n.Numerator>=0)
                n2 = new FractionedNumber(n.Numerator, n.Denominator);
            else
                n2 = new FractionedNumber(-n.Numerator, n.Denominator);
            if(n.Denominator < 0)
                n2.Denominator = -n2.Denominator;
            for(int i = 0;i < 20;i++) {
                Number tmp1 = 0;
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
        public static implicit operator FractionedNumber(int n) {
            return new FractionedNumber(n);
        }
        public static implicit operator FractionedNumber(long n) {
            return new FractionedNumber(n);
        }
        private static Number mdc(FractionedNumber inNumber) {
            Number last;
            Number rest;
            if(inNumber.Numerator>inNumber.Denominator){
                last = inNumber.Numerator;
                rest = inNumber.Denominator;
            }else{
                last = inNumber.Denominator;
                rest = inNumber.Numerator;
            }
            while(rest != 0) {
                Number tmp = rest;
                rest = last % rest;
                last = tmp;
            }
            return last;
        }
        public static FractionedNumber simplify(FractionedNumber inNumber) {
            FractionedNumber num = inNumber;
            Number numMdc = mdc(num);
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

        public static FractionedNumber Min(FractionedNumber n1, FractionedNumber n2) {
            return n1 > n2 ? n2 : n1;
        }
        public static FractionedNumber Max(FractionedNumber n1, FractionedNumber n2) {
            return n1 > n2 ? n1 : n2;
        }



        public int CompareTo(object obj) {
            if (obj == null) return 1;
            FractionedNumber other = obj as FractionedNumber;
            if (other != null) {
                if (this < other) return -1;
                else if (this > other) return 1;
                return 0;
            } else
                throw new ArgumentException("Object is not a Number");
        }

        public bool Equals(FractionedNumber other) {
            return this == other;
        }

        public int CompareTo(FractionedNumber other) {
            if (this < other) return -1;
            else if (this > other) return 1;
            return 0;
        }

        public static bool Equals(FractionedNumber n1, FractionedNumber n2) {
            return n1 == n2;
        }

        public static int CompareTo(FractionedNumber n1, FractionedNumber n2) {
            if (n1<n2) return -1;
            else if (n1>n2) return 1;
            return 0;
        }

		public FractionedNumber Sqrt(int iterations){
			FractionedNumber res = 2;
			for(int i = 0; i < iterations; i++) {
				res -= (((res * res) - this) / (res * 2));
			}
			return res;
		}
    }
}
