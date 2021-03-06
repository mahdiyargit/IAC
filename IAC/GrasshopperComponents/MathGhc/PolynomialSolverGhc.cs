using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace IAC.GrasshopperComponents.MathGhc
{
    public class PolynomialSolverGhc : GH_Component
    {
        public PolynomialSolverGhc() : base("Polynomial Solver", "Solver",
            "Find all roots of a single-variable polynomial with real-valued coefficients and non-negative exponents by calculating the characteristic polynomial of the companion matrix",
            "IAC", "Math")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Coefficients", "C",
                "The coefficients of the polynomial in ascending order,\ne.g. {5, 0, 2} = \"5 + 0x¹ + 2x²\"",
                GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Polynomial", "P",
                "The polynomial in ascending order, e.g. \"4.3 + 2.0x ^ 2 - x ^ 3\".", GH_ParamAccess.item);
            pManager.AddNumberParameter("Real Roots", "R", "The real roots of the polynomial", GH_ParamAccess.list);
            pManager.AddComplexNumberParameter("Complex Roots", "C", "The Complex Roots of the polynomial",
                GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
            var coefficients = new List<double>();
            if (!da.GetDataList(0, coefficients)) return;
            var p = new Polynomial(coefficients);
            var roots = p.Roots();
            da.SetData(0, ToExpression(p));
            da.SetDataList(1, from root in roots where root.Imaginary == 0 select root.Real);
            da.SetDataList(2,
                from root in roots where root.Imaginary != 0 select new Complex(root.Real, root.Imaginary));
        }

        private static string ToExpression(Polynomial p)
        {
            var co = p.Coefficients;
            var expr = string.Empty;
            for (var i = co.Length - 1; i >= 0; i--)
            {
                if (co[i] == 0) continue;
                if (expr.Length == 0)
                {
                    expr += co[i].ToString(CultureInfo.InvariantCulture);
                }
                else if (co[i] > 0.0)
                    expr += "+" + co[i].ToString(CultureInfo.InvariantCulture);
                else
                    expr += co[i].ToString(CultureInfo.InvariantCulture);
                if (i > 1)
                    expr += "*x^" + i;
                else if (i == 1)
                    expr += "*x";
            }
            return expr;
        }
        protected override System.Drawing.Bitmap Icon => Properties.Resources.polynominalSolver;
        public override Guid ComponentGuid => new Guid("22f4a640-9f3a-4a72-b7a3-97ce5fc16412");
    }
}