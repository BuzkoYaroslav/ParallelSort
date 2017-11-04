using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library
{
    public enum MathFunctionType { Sum, Multiplication, Division }

    class CosFunction: MathFunction
    {
        public CosFunction(double coef, MathFunction foundation): base()
        {
            this.coef = coef;

            functions.Add(foundation);
        }

        public override double Calculate(double x)
        {
            return coef * Math.Cos(functions[0].Calculate(x));
        }
        public override MathFunction Derivative(int order)
        {
            if (order < 0)
                throw new Exception("Incorrect derivative order!");

            if (order == 0)
                return new CosFunction(coef, functions[0]);

            return (new SinFunction(-coef, functions[0]) * functions[0].Derivative(1)).Derivative(order - 1);
        }

        public override string ToString()
        {
            return string.Format("{0}cos[{1}]", coef != 1 ? Math.Round(coef, 2).ToString() + " * " : "", functions[0].ToString());
        }

        public override bool IsZero()
        {
            return coef == 0;
        }

        protected override MathFunction MinusFunction()
        {
            return new CosFunction(-coef, functions[0]);
        }
        protected override MathFunction Multiply(double coef)
        {
            return new CosFunction(coef * this.coef, functions[0]);
        }
    }
    class SinFunction: MathFunction
    {
        public SinFunction(double coef, MathFunction foundation): base()
        {
            this.coef = coef;

            functions.Add(foundation);
        }

        public override double Calculate(double x)
        {
            return coef * Math.Sin(functions[0].Calculate(x));
        }
        public override MathFunction Derivative(int order)
        {
            if (order < 0)
                throw new Exception("Incorrect derivative order!");

            if (order == 0)
                return new SinFunction(coef, functions[0]);

            return (new CosFunction(coef, functions[0]) * functions[0].Derivative(1)).Derivative(order - 1);
        }

        public override string ToString()
        {
            return string.Format("{0}sin[{1}]", coef != 1 ? Math.Round(coef, 2).ToString() + " * " : "", functions[0].ToString());
        }

        public override bool IsZero()
        {
            return coef == 0;
        }

        protected override MathFunction MinusFunction()
        {
            return new SinFunction(-coef, functions[0]);
        }
        protected override MathFunction Multiply(double coef)
        {
            return new SinFunction(coef * this.coef, functions[0]);
        }
    }
    class PowerFunction: MathFunction
    {
        public PowerFunction(double coef, MathFunction foundation, MathFunction power): base()
        {
            this.coef = coef;
            functions.Add(foundation);
            functions.Add(power);
        }

        public override double Calculate(double x)
        {
            return coef * Math.Pow(functions[0].Calculate(x), functions[1].Calculate(x));
        }
        public override MathFunction Derivative(int order)
        {
            MathFunction f = functions[0],
                          g = functions[1];

            return this * (f.Derivative(1) * g / f + new LnFunction(1.0d, f) * g.Derivative(1));
        }

        public override string ToString()
        {
            return string.Format("{0}({1}) ^ [{2}]", coef != 1 ? Math.Round(coef, 2).ToString() + " * " : "", functions[0], functions[1]);
        }

        public override bool IsZero()
        {
            return functions[0].IsZero() && coef == 0;
        }

        protected override MathFunction MinusFunction()
        {
            return new PowerFunction(-coef, functions[0], functions[1]);
        }
        protected override MathFunction Multiply(double coef)
        {
            return new PowerFunction(coef * this.coef, functions[0], functions[1]);
        }
    }
    class LnFunction: MathFunction
    {
        public LnFunction(double coef, MathFunction foundation): base()
        {
            this.coef = coef;

            functions.Add(foundation);
        }

        public override double Calculate(double x)
        {
            return coef * Math.Log(functions[0].Calculate(x));
        }
        public override MathFunction Derivative(int order)
        {
            if (order < 0)
                throw new Exception("Incorrect derivative order!");

            if (order == 0)
                return new LnFunction(coef, functions[0]);

            return (coef * functions[0].Derivative(1) / functions[0]).Derivative(order - 1);
        }

        public override string ToString()
        {
            return string.Format("{0}ln[{1}]", coef != 1 ? Math.Round(coef, 2).ToString() + " * " : "", functions[0].ToString());
        }

        public override bool IsZero()
        {
            return coef == 0;
        }
        protected override MathFunction MinusFunction()
        {
            return new LnFunction(-coef, functions[0]);
        }
        protected override MathFunction Multiply(double coef)
        {
            return new LnFunction(coef * this.coef, functions[0]);
        }
    }
    class ConstFunction: MathFunction
    {
        public ConstFunction(double coef): base()
        {
            this.coef = coef;
        }

        public override double Calculate(double x)
        {
            return coef;
        }
        public override MathFunction Derivative(int order)
        {
            if (order < 0)
                throw new Exception("Incorrect order!");

            return order == 0 ? coef : 0;
        }

        public override string ToString()
        {
            return Math.Round(coef, 2).ToString();
        }

        public override bool IsZero()
        {
            return coef == 0;
        }
        protected override MathFunction MinusFunction()
        {
            return new ConstFunction(-coef);
        }
        protected override MathFunction Multiply(double coef)
        {
            return new ConstFunction(coef * this.coef);
        }

    }
    class XFunction: MathFunction
    {
        public XFunction(double coef): base()
        {
            this.coef = coef;
        }

        public override double Calculate(double x)
        {
            return coef * x;
        }
        public override MathFunction Derivative(int order)
        {
            if (order < 0)
                throw new Exception("Incorrect derivative order!");

            return order == 0 ? new XFunction(coef) : ((MathFunction)coef).Derivative(order - 1);
        }

        public override string ToString()
        {
            return string.Format("{0}x", coef != 1 ? Math.Round(coef, 2).ToString() + " * " : "");
        }

        public override bool IsZero()
        {
            return coef == 0;
        }

        protected override MathFunction MinusFunction()
        {
            return new XFunction(-coef);
        }
        protected override MathFunction Multiply(double coef)
        {
            return new XFunction(coef * this.coef);
        }
    }

    public class MathFunction
    {
        private const double epsilan = 0.0001;

        private delegate bool Condition(double f, double best);
        private delegate bool SingleCondition(double f);

        protected double coef;
        protected MathFunctionType type;
        protected List<MathFunction> functions;

        #region Constructor
        public MathFunction()
        {
            functions = new List<MathFunction>();
            type = MathFunctionType.Sum;
            coef = 0.0d;
        }
        public MathFunction(double coef, MathFunctionType type, params MathFunction[] functions)
        {
            this.functions = new List<MathFunction>();

            this.coef = coef;
            this.type = type;

            if (functions.Length == 0)
                this.functions.Add(1.0d);

            switch(type)
            {
                case MathFunctionType.Sum:
                    foreach (MathFunction func in functions)
                        if (!func.IsZero())
                            this.functions.Add(func);
                    break;
                case MathFunctionType.Multiplication:
                    for (int i = 0; i < functions.Length; i++)
                        if (functions[i].IsZero())
                        {
                            coef = 1;
                            this.functions.Add(0.0d);
                            break;
                        }

                    foreach (MathFunction func in functions)
                            this.functions.Add(func);
                    break;
                case MathFunctionType.Division:
                    InitializeDivision(functions);

                    if (functions[0].IsZero())
                    {
                        coef = 1;
                        this.functions.Clear();
                        this.functions.Add(0.0d);
                        break;
                    }

                    break;
            }
        }
        #endregion

        #region Bool features
        public virtual bool IsZero()
        {
            foreach (MathFunction func in functions)
                if (func.IsZero())
                    return true;

            return false;
        }

        private bool IsGood(SingleCondition cond, double a, double b, double step = epsilan)
        {
            try
            {
                for (double i = a; i <= b; i += epsilan)
                {
                    if (!cond(Calculate(i)))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (NotFiniteNumberException)
            {
                return false;
            }
            catch (DivideByZeroException)
            {
                return false;
            }
        }

        public bool IsGreaterThanZero(double a, double b)
        {
            return IsGood((double func) => { return func > 0; }, a, b);
        }
        public bool IsSmallerThanZero(double a, double b)
        {
            return IsGood((double f) => { return f < 0; }, a, b);
        }
        public bool IsContinuous(double a, double b)
        {
            return IsGood((double func) => { return !double.IsInfinity(func) && !double.IsNaN(func); },
                          a, b);
        }
        public bool IsContinuous(double a, double b, double downYBound, double upYBound, double eps = epsilan)
        {
            return IsGood((double func) => { return func > downYBound && func < upYBound; },
                          a, b, eps);
        }
        public bool IsWithConstSign(double a, double b)
        {
            return IsSmallerThanZero(a, b) || IsGreaterThanZero(a, b);
        }

        public Tuple<double, double>[] ContinuousRegions(double a, double b, double downYBound = double.NegativeInfinity, double upYBound = double.PositiveInfinity)
        {
            List<Tuple<double, double>> regions = new List<Tuple<double, double>>();
            double epsilan = Math.Pow(10.0, -2);

            double left = a, right = a + epsilan;

            while (true)
            {
                if (right > b)
                {
                    if (IsContinuous(left, right - epsilan, downYBound, upYBound, epsilan))
                        regions.Add(Tuple.Create(left, right - epsilan));

                    break;
                }
                if (IsContinuous(left, right, downYBound, upYBound, epsilan))
                {
                    right += epsilan;
                    continue;
                }

                if (left < right - epsilan &&
                    IsContinuous(left, right - epsilan, downYBound, upYBound, epsilan))
                    regions.Add(Tuple.Create(left, right - epsilan));

                left = right;
                right += epsilan;
            }

            return regions.ToArray();
        }

        #endregion

        #region Public methods
        private void InitializeDivision(MathFunction[] functions)
        {
            MathFunction up = new MathFunction(1.0d, MathFunctionType.Multiplication);
            MathFunction low = new MathFunction(1.0d, MathFunctionType.Multiplication);

            for (int i = 0; i < functions.Length; i++)
            {
                if (i % 2 == 0)
                    up *= functions[i];
                else
                    low *= functions[i];
            }

            this.functions.Add(up);
            this.functions.Add(low);
        }

        public virtual double Calculate(double x)
        {
            if (functions.Count == 0)
                return coef;

            double result = functions[0].Calculate(x);

            for (int i = 1; i < functions.Count; i++)
                switch(type)
                {
                    case MathFunctionType.Sum:
                        result += functions[i].Calculate(x);
                        break;
                    case MathFunctionType.Multiplication:
                        result *= functions[i].Calculate(x);
                        break;
                    case MathFunctionType.Division:
                        result /= functions[i].Calculate(x);
                        break;
                }

            result *= coef;

            return result;
        }
        public virtual MathFunction Derivative(int order)
        {
            if (functions.Count == 0)
                throw new Exception("Function is empty!");

            if (order < 0)
                throw new Exception("Incorrect derivative order!");

            if (order == 0)
                return new MathFunction(coef, type, functions.ToArray());

            MathFunction result = 0.0d;

            switch (type)
            {
                case MathFunctionType.Sum:
                    result = functions[0].Derivative(order);

                    for (int i = 1; i < functions.Count; i++)
                        result += functions[i].Derivative(order);

                    break;
                case MathFunctionType.Multiplication:
                    result = new ConstFunction(0.0d);

                    for (int i = 0; i < functions.Count; i++)
                    {
                        MathFunction func = functions[i].Derivative(1);
                        for (int j = 0; j < functions.Count; j++)
                            if (i != j)
                                func *= functions[j];

                        result += func;
                    }

                    result = Derivative(order - 1);

                    break;
                case MathFunctionType.Division:
                    MathFunction f = functions[0],
                                 g = functions[1];

                    result = (f.Derivative(1) * g - g.Derivative(1) * f) / (g ^ 2);

                    result = Derivative(order - 1);
                    break;
            }

            return result;
        }
        
        public override string ToString()
        {
            string result = coef != 1 ? Math.Round(coef, 2) + " * " : "" + "(";
            string splitter = type == MathFunctionType.Sum ? " + " : type == MathFunctionType.Multiplication ? " * " : " : ";

            for (int i = 0; i < functions.Count; i++)
                result += functions[i].ToString() + (i != (functions.Count - 1) ? splitter : "");

            result += ")";

            return result;
        }
        #endregion

        #region Overrided operators
        protected virtual MathFunction MinusFunction()
        {
            return new MathFunction(-coef, type, functions.ToArray());
        }
        protected virtual MathFunction Multiply(double coef)
        {
            return new MathFunction(coef * this.coef, type, functions.ToArray());
        }

        public static MathFunction operator -(MathFunction func)
        {
            return func.MinusFunction();
        }
        public static MathFunction operator +(MathFunction func1, MathFunction func2)
        {
            return new MathFunction(1.0d, MathFunctionType.Sum, func1, func2);
        }
        public static MathFunction operator -(MathFunction func1, MathFunction func2)
        {
            return func1 + (-func2);
        }
        public static MathFunction operator *(MathFunction func1, MathFunction func2)
        {
            return new MathFunction(1.0d, MathFunctionType.Multiplication, func1, func2);
        }
        public static MathFunction operator /(MathFunction func1, MathFunction func2)
        {
            return new MathFunction(1.0d, MathFunctionType.Division, func1, func2);
        }
        public static MathFunction operator *(MathFunction func, double coef)
        {
            return coef * func;
        }
        public static MathFunction operator *(double coef, MathFunction func)
        {
            return func.Multiply(coef);
        }
        public static MathFunction operator /(MathFunction func, double coef)
        {
            return (1.0d / coef) * func;
        }
        public static MathFunction operator ^(MathFunction func, double coef)
        {
            return new PowerFunction(1.0d, func, coef);
        }
        public static MathFunction operator ^(double coef, MathFunction func)
        {
            return new PowerFunction(1.0d, coef, func);
        }

        public static implicit operator MathFunction(double number)
        {
            return new ConstFunction(number);
        } 
        public static implicit operator MathFunction(Polynomial poly)
        {
            MathFunction func = poly[0];

            for (int i = 1; i <= poly.Power; i++)
                func += new PowerFunction(poly[i], new XFunction(1.0d), i);

            return func;
        }
        public static explicit operator Polynomial(MathFunction func)
        {
            if (func.type != MathFunctionType.Sum)
                throw new InvalidCastException();

            double[] coef = new double[func.functions.Count];
            int pointer = 0;
            bool incremented = false;

            for (int i = 0; i < func.functions.Count; i++)
            {
                if (i == 0)
                {
                    ConstFunction num;
                    incremented = false;
                    for (int j = 0; j < func.functions.Count; j++)
                        if ((num = func.functions[j] as ConstFunction) != null)
                        {
                            coef[0] += num.coef;
                            incremented = true;
                        }
                }
                else
                {
                    PowerFunction num;
                    ConstFunction step;
                    incremented = false;
                    for (int j = 0; j < func.functions.Count; j++)
                        if ((num = func.functions[j] as PowerFunction) != null && 
                            (step = num.functions[1] as ConstFunction) != null &&
                            step.coef == pointer)
                        {
                            coef[pointer] += num.coef;
                            incremented = true;
                        }
                }

                if (incremented)
                    pointer += 1;
            }

            for (int i = 0; i < coef.Length; i++)
                coef[i] *= func.coef;

            return new Polynomial(coef);
        }
        #endregion

        #region Global extremum

        public double MaxValue(double a, double b, bool isAbsolute = true)
        {
            return FindValue(a, b, (double f, double best) => { return f > best; }, double.MinValue, isAbsolute);
        }
        public double MinValue(double a, double b, bool isAbsolute = true)
        {
            return FindValue(a, b, (double f, double best) => { return f < best; }, double.MaxValue, isAbsolute);
        }

        private double FindValue(double a, double b, Condition cond, double startVal, bool isAbsolute)
        {
            double result = startVal;

            for (double x = a; x <= b; x += epsilan)
            {
                double f = Calculate(x);
                f = isAbsolute ? Math.Abs(f) : f;

                if (cond(f, result)) result = f;
            }

            return result;
        }

        #endregion
    }
}
