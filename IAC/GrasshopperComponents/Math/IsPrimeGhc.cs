using Grasshopper.Kernel;
using System;

namespace IAC
{
    public class IsPrimeGhc : GH_Component
    {
        public IsPrimeGhc()
          : base("Is Prime", "IsPrime", "Tests primality of a given number.", "IAC", "Math") { }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Number", "N", "Number to test for primality.", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Prime", "P", "True if number is prime.", GH_ParamAccess.item);
        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            var n = 0;
            if (!da.GetData(0, ref n)) return;
            da.SetData(0, IsPrime(n));
        }
        private static bool IsPrime(int n)
        {
            if (n < 2) return false;
            if (n % 2 == 0) return n == 2;
            var root = (int)Math.Sqrt(n);
            for (var i = 3; i <= root; i += 2)
                if (n % i == 0) return false;
            return true;
        }
        protected override System.Drawing.Bitmap Icon => Properties.Resources.isPrime;
        public override Guid ComponentGuid => new Guid("abdce8a2-b48a-486f-a161-9be309bfdfc1");
    }
}
