using Grasshopper.Kernel;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IAC.GrasshopperComponents.MathGhc
{
    public class LCMGCDGhc : GH_Component
    {
        public LCMGCDGhc() : base("LCM GCD", "LCM/GCD", "LCM/GCD", "IAC", "Math") { }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Numbers", "N", "Numbers to find LCM and GCD.", GH_ParamAccess.list);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("LCM", "LCM", "The Least Common Multiple", GH_ParamAccess.item);
            pManager.AddIntegerParameter("GCD", "GCD", "The Greatest Common Divisor", GH_ParamAccess.item);

        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            var input = new List<int>();
            if (!da.GetDataList(0, input)) return;
            var numbers = input.Select(x => (long)x).ToArray();
            da.SetData(0, Euclid.LeastCommonMultiple(numbers));
            da.SetData(1, Euclid.GreatestCommonDivisor(numbers));
        }
        protected override System.Drawing.Bitmap Icon => Properties.Resources.LCMGCD;

        public override Guid ComponentGuid => new Guid("fd52fb86-33d3-41f4-b7e2-4b8db7e7c217");
    }
}