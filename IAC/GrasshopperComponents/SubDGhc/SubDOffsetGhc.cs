using Grasshopper.Kernel;
using Rhino.Geometry;
using System;

namespace IAC.GrasshopperComponents.SubDGhc
{
    public class SubDOffsetGhc : GH_Component
    {
        public SubDOffsetGhc() : base("SubD Offset", "Offset", "Makes a new SubD with vertices offset at distance in the direction of the control net vertex normals.\nOptionally, based on the value of solidify, adds the input SubD and a ribbon of faces along any naked edges.", "IAC", "SubD")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddSubDParameter("SubD", "S", "Base subD", GH_ParamAccess.item);
            pManager.AddNumberParameter("Distance", "D", "The distance to offset", GH_ParamAccess.item, 1.0);
            pManager.AddBooleanParameter("Solidify", "S", "True if the output SubD should be turned into a closed SubD",
                GH_ParamAccess.item, false);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddSubDParameter("SubD", "S", "Offset result", GH_ParamAccess.item);
        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            SubD subd = null;
            var d = 1.0;
            var solid = false;
            if (!da.GetData(0, ref subd)) return;
            if (!da.GetData(1, ref d)) return;
            if (!da.GetData(2, ref solid)) return;
            var result = subd.Offset(d, solid);
            if (result is null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "SubD could not be constructed.");
                return;
            }
            da.SetData(0, result);
        }
        protected override System.Drawing.Bitmap Icon => Properties.Resources.subDOffset;

        public override Guid ComponentGuid => new Guid("6c5cf8b4-ad54-4756-9b19-1d99a6b92032");
    }
}