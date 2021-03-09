using Grasshopper.Kernel;
using System;
using System.Collections.Generic;

namespace IAC
{
    public class PrimeFactorizationGhc : GH_Component
    {
        public PrimeFactorizationGhc() : base("Prime Factorization", "PFactors",
              "Decomposes the number into its constituent prime factors.", "IAC", "Math")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Number", "N", "Number to factorize", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Prime Factors", "F", "All prime factors of the number", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Prime", "P", "True if number is prime.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Even", "E", "True if number is even.", GH_ParamAccess.item);
        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            var n = 0;
            if (!da.GetData(0, ref n)) return;
            if (n == 0)
            {
                da.SetDataList(0, null);
                da.SetData(1, false);
                da.SetData(2, true);
                return;
            }
            var factors = PrimeFactor(n);
            da.SetDataList(0, factors);
            da.SetData(1, factors.Count == 1);
            da.SetData(2, factors.Contains(2));
        }
        private static List<int> PrimeFactor(int n)
        {
            var factors = new List<int>();
            while (n % 2 == 0)
            {
                factors.Add(2);
                n /= 2;
            }
            for (var i = 3; i <= Math.Sqrt(n); i += 2)
            {
                while (n % i == 0)
                {
                    factors.Add(i);
                    n /= i;
                }
            }
            if (n > 2)
                factors.Add(n);
            return factors;
        }
        protected override System.Drawing.Bitmap Icon => Properties.Resources.primeFactorization;
        public override Guid ComponentGuid => new Guid("02638df3-3e0f-4945-bc3a-34ce4b4407e6");
    }
}