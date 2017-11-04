using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library
{
    public class Polynomial
    {
        private List<double> coefficients = new List<double>();

        public Polynomial()
        {
            coefficients.Add(0.0d);
        }
        public Polynomial(params double[] coef)
        {
            for (int i = 0; i < coef.Length; i++)
            {
                coefficients.Add(coef[i]);
            }
            DeleteZero();
        }

        public Polynomial(Polynomial polyCopy)
        {
            for (int i = 0; i <= polyCopy.Power; i++)
            {
                coefficients.Add(polyCopy.GetCoefficient(i));
            }
            DeleteZero();
        }

        public double this[int i]
        {
            get
            {
                return GetCoefficient(i);
            }
        }
        public int Power
        {
            get
            {
                return coefficients.Count - 1;
            }
        }

        public double GetCoefficient(int i)
        {
            if (i < 0)
                throw new IndexOutOfRangeException("Многочлен не имеет членов с отрицательными степенями!");
            else if (i > Power)
                throw new IndexOutOfRangeException(string.Format("Степень многочлена меньше указанного числа! Step = {0}", Power));
            return coefficients[i];
        }
        public double GetValue(double x)
        {
            double result = 0;
            for (int i = 0; i <= Power; i++)
            {
                result += coefficients[i] * Math.Pow(x, i);
            }
            return result;
        }

        public static Polynomial operator+(Polynomial poly1, Polynomial poly2)
        {
            double[] coef = new double[Math.Max(poly1.Power, poly2.Power) + 1];

            int minIndex = Math.Min(poly1.Power, poly2.Power) + 1;

            for (int i = 0; i < minIndex; i++)
            {
                coef[i] = poly1[i] + poly2[i];
            }
            for (int i = minIndex; i < coef.Length; i++)
            {
                if (poly1.Power >= i)
                    coef[i] = poly1[i];
                else
                    coef[i] = poly2[i];
            }

            return new Polynomial(coef);
        }
        public static Polynomial operator-(Polynomial poly)
        {
            double[] coef = new double[poly.Power + 1];
           
            for (int i = 0; i <= poly.Power; i++)
            {
                coef[i] = -poly[i];
            }

            return new Polynomial(coef);
        }
        public static Polynomial operator-(Polynomial poly1, Polynomial poly2)
        {
            return poly1 + (-poly2);
        }
        public static Polynomial operator*(Polynomial poly1, Polynomial poly2)
        {
            double[] coef = new double[poly1.Power + poly2.Power + 1];

            for (int i = 0; i <= poly1.Power; i++)
            {
                for (int j = 0; j <= poly2.Power; j++)
                {
                    coef[i + j] += poly1[i] * poly2[j];
                }
            }

            return new Polynomial(coef);
        }
        public static Polynomial operator*(Polynomial poly, double coef)
        {
            return coef * poly;
        }
        public static Polynomial operator *(double coef, Polynomial poly)
        {
            double[] coeff = poly.coefficients.ToArray();

            for (int i = 0; i < coeff.Length; i++)
                coeff[i] *= coef;

            return new Polynomial(coeff);
        }

        public override string ToString()
        {
            return ToString("Polynomial \n P (x) ");
        }
        public string ToString(string polyName)
        {
            StringBuilder str = new StringBuilder();

            str.Insert(0, polyName + " = ");

            for (int i = 0; i < Power; i++)
            {
                str.Insert(str.Length - 1,
                           string.Format(" {0} * x ^ {1} {2} ", Math.Abs(coefficients[i]),
                                                               i,
                                                               coefficients[i + 1] > 0 ? '+' : '-'));
            }

            str.Insert(str.Length - 1, Math.Abs(coefficients[Power]) + string.Format(" * x ^ {0} \n", Power));

            return str.ToString();
        }

        private void DeleteZero()
        {
            int i = Power;
            while (coefficients[i] == 0 && i > 0)
            {
                coefficients.Remove(coefficients[i]);
                i--;
            }
        }

        public Polynomial Derivative(int order)
        {
            if (order < 0)
                throw new ArgumentOutOfRangeException("Order cannot be less than 0!");
            if (order == 0)
                return new Polynomial(this);
            if (order == -1)
            {
                double[] coef = new double[Power];

                for (int i = 0; i < Power; i++)
                    coef[i] = coefficients[i + 1] * (i + 1);

                return new Polynomial(coef);
            }

            return Derivative(order - 1);
        }
    }
}
