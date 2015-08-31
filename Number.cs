using System;

namespace FractionedNumber {
	public struct Number {
		private static ulong OFFSET = ulong.MaxValue;
		private bool negative;
		private ulong multiplier;
		private ulong number;
		public Number(int n){
			multiplier = 0;
			if(n < 0) {
				number = (ulong)(-n);
				negative = true;
			} else {
				number = (ulong)n;
				negative = false;
			}
		}
		public Number(uint n){
			multiplier = 0;
			number = n;
			negative = false;

		}
		public Number(long n){
			multiplier = 0;
			if(n < 0) {
				number = (ulong)(-n);
				negative = true;
			} else {
				number = (ulong)n;
				negative = false;
			}
		}
		public Number(ulong n){
			multiplier = 0;
			number = n;
			negative = false;
		}
		public Number(string n){
			//TODO
			multiplier = 0;
			number = 0;
			negative = false;
		}


		public static implicit operator Number(int n) {
			return new Number(n);
		}
		public static implicit operator Number(long n) {
			return new Number(n);
		}
		public static implicit operator Number(uint n) {
			return new Number(n);
		}
		public static implicit operator Number(ulong n) {
			return new Number(n);
		}
		public static implicit operator Number(string n) {
			return new Number(n);
		}
		public static Number operator +(Number n1, Number n2) {
			//TODO
			Number n = new Number();
			if((!n1.negative && !n2.negative) || (n1.negative && n2.negative)) {
				n.negative = n1.negative;
				n.multiplier = n1.multiplier + n2.multiplier;
				n.number = n1.number + n2.number;
				if(n1.number != 0 && n2.number != 0) {
					if(n.number <= n1.number || n.number <= n2.number)
						n.multiplier++;
				}
			}
			return n;
		}
		public static Number operator -(Number n1, Number n2) {
			//TODO
			return new Number(0);
		}
		public static Number operator -(Number n) {
			//TODO
			return new Number(0);
		}
		public static Number operator ++(Number n) {
			//TODO
			return new Number(0);
		}
		public static Number operator --(Number n) {
			//TODO
			return new Number(0);
		}
		public static Number operator *(Number n1, Number n2) {
			//TODO
			return new Number(0);
		}
		public static Number operator /(Number n1, Number n2) {
			//TODO
			return new Number(0);
		}
		public static Number operator %(Number n1, Number n2) {
			//TODO
			return new Number(0);
		}
		public static bool operator ==(Number n1, Number n2) {
			//TODO
			return false;
		}
		public static bool operator !=(Number n1, Number n2) {
			//TODO
			return false;
		}
		public static bool operator >(Number n1, Number n2) {
			//TODO
			return false;
		}
		public static bool operator <(Number n1, Number n2) {
			//TODO
			return false;
		}
		public static bool operator >=(Number n1, Number n2) {
			//TODO
			return false;
		}
		public static bool operator <=(Number n1, Number n2) {
			//TODO
			return false;
		}
		public static implicit operator string(Number n) {
			//TODO
			return "";
		}
		public static bool TryParse(string str, out Number number){
			//TODO
			number = new Number(0);
			return false;
		}


	}
}

