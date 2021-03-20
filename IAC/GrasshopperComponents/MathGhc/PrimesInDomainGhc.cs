using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IAC.GrasshopperComponents.MathGhc
{
    public class PrimesInDomainGhc : GH_Component
    {
        public PrimesInDomainGhc() : base("Primes In Domain", "PInDom", "Finds all prime numbers in a given domain.", "IAC", "Math") { }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntervalParameter("Domain", "D", "Interval to find primes number in.", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("PrimeNumbers", "P", "All prime numbers in the given interval",
                GH_ParamAccess.list);
        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            var interval = Interval.Unset;
            if (!da.GetData(0, ref interval) || !interval.IsValid) return;
            da.SetDataList(0, GeneratePrimes(interval));
        }
        public static List<GH_Integer> GeneratePrimes(Interval interval)
        {
            var min = interval.Min > 2 ? (int)interval.Min : 2;
            var count = (int)interval.Max - min + 1;
            var r = from i in Enumerable.Range(min, count).AsParallel()
                    where Enumerable.Range(1, (int)System.Math.Sqrt(i)).All(j => j == 1 || i % j != 0)
                    select i;
            return interval.IsIncreasing ? r.OrderBy(p => p).Select(p => new GH_Integer(p)).ToList() : r.OrderByDescending(p => p).Select(p => new GH_Integer(p)).ToList();
        }
        protected override System.Drawing.Bitmap Icon => Properties.Resources.primeInDomain;
        public override Guid ComponentGuid => new Guid("0eeb1f90-862d-40e4-b8ba-b2b39c459f68");
    }
}