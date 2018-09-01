using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Condition
{
    public class Field:IField
    {
        public Field(object seed)
        {
            this.Seed = seed;
        }

        #region properties
        public object Seed
        {
            get;
            set;
        }

      

        #endregion

        #region internals

        internal Condition EqualTo(Field other)
        {
            return new Condition() { Left = this, Rigth = other, Operator= Operator.Equal };
        }

        internal Condition LessThanTo(Field other)
        {
            return new Condition(this,other,Operator.Less);
        }

        internal Condition LessAndEqualTo(Field other)
        {
            return new Condition(this, other, Operator.LessAndEqual);
        }

        internal Condition GreaterThanTo(Field other)
        {
            return new Condition(this, other, Operator.Greater);
        }

        internal Condition GreaterAndEqualTo(Field other)
        {
            return new Condition(this, other, Operator.GreaterAndEqual);
        }

        internal Condition NotEqualTo(Field other)
        {
            return new Condition(this, other, Operator.NotEqual);
        }

        internal Condition Likes(Field other)
        {
            return new Condition(this, other, Operator.Like);
        }

        internal Condition Betweens(Field left, Field right)
        {
            if(left.Seed.GetType().FullName != right.Seed.GetType().FullName)
                throw new Exception("between两边的类型不相同");
            var mark = left.Seed.GetType().In(typeof(int), typeof(double), typeof(float), typeof(Int64)) ? "" : "'";
            if (left.Seed.GetType().FullName == typeof (string).FullName && left.Seed.ToString().StartsWith(":", StringComparison.Ordinal))
            {
                mark = "";
            }
            return new Condition(this, new Field(string.Format("{0}{1}{0} and {0}{2}{0}", mark, left.Seed, right.Seed)), Operator.Between);
        }
        internal Condition Betweennew(Field left, Field right)
        {
            if (left.Seed.GetType().FullName != right.Seed.GetType().FullName)
                throw new Exception("between两边的类型不相同");
            var mark = left.Seed.GetType().In(typeof(int), typeof(double), typeof(float), typeof(Int64)) ? "" : "";
            if (left.Seed.GetType().FullName == typeof(string).FullName && left.Seed.ToString().StartsWith(":", StringComparison.Ordinal))
            {
                mark = "";
            }
            return new Condition(this, new Field(string.Format("{0}{1}{0} and {0}{2}{0}", mark, left.Seed, right.Seed)), Operator.Between);
        }
        #endregion

        #region statics
        public static Field Val(object seed,DbType dbType = DbType.String)
        {
            return new Field(seed);
        }
        #endregion       


        public Condition In<T>(params T[] array) 
        {            
            var mark = typeof(T).In(typeof(int), typeof(double), typeof(float), typeof(Int64)) ? "" : "'";
            string rSeed = "(" + array.Aggregate("", (a, b) => a + mark + b.ToString() + mark+ ",").TrimEnd(',') + ")";
            return new Condition() { Left = this, Operator= Operator.In, Rigth = new Field(rSeed) };
        }


        public Condition NotIn<T>(params T[] array)
        {
            var mark = typeof(T).In(typeof(int), typeof(double), typeof(float), typeof(Int64)) ? "" : "'";
            string rSeed = "(" + array.Aggregate("", (a, b) => a + mark + b.ToString() + mark + ",").TrimEnd(',') + ")";
            return new Condition() { Left = this, Operator = Operator.NotIn, Rigth = new Field(rSeed) };
        }

        public Condition IsNull()
        {
            
            return new Condition() { Left= this, Operator = Operator.IsNull , IsUnitary= true };
        }

        public Condition IsNotNull()
        {

            return new Condition() { Left = this, Operator = Operator.IsNotNull, IsUnitary = true };
        }

        #region operator overrides

        public static implicit operator Field(string value)
        {
            return new Field("'" + value + "'");
        }

        public static implicit operator Field(double value)
        {
            return new Field(value);
        }

        public static implicit operator Field(int value)
        {
            return new Field(value);
        }

        public static implicit operator Field(float value)
        {
            return new Field(value);
        }

        public static implicit operator Field(DateTime value)
        {
            
            return new Field("cast('" +value.ToString("yyyy-MM-dd H:m:s") + "' as datetime)");
        }
         
        public static Condition operator ==(Field left, Field right)
        {
            return left.EqualTo(right);
        }

        public static Condition operator !=(Field left, Field right)
        {
            return left.NotEqualTo(right);
        }

        public static Condition operator >(Field left, Field right)
        {
            return left.GreaterThanTo(right);
        }


        public static Condition operator >=(Field left, Field right)
        {
            return left.GreaterAndEqualTo(right);
        }

        public static Condition operator <=(Field left, Field right)
        {
            return left.LessAndEqualTo(right);
        }

        public static Condition operator <(Field left, Field right)
        {
            return left.LessThanTo(right);
        }

        #endregion

        #region extends operator

        public Condition Like(Field value)
        {
            return this.Likes(value);
        }

        public Condition BetweenNew(Field left, Field right)
        {
            return this.Betweens(left, right);
        }
        public Condition Between(Field left, Field right)
        {
            return this.Betweennew(left, right);
        }
        
        #endregion

        #region overrides

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return true;
        }

        public override string ToString()
        {
            return base.ToString();
        }
        #endregion
              
       
    }
}
