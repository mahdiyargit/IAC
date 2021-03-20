using Grasshopper.Kernel;
using Rhino.Geometry;
using System;

namespace IAC.GrasshopperComponents.SubDGhc
{
    public class SubdivideGhc : GH_Component
    {
        public SubdivideGhc() : base("Subdivide", "SubD", "Apply the Catmull-Clark subdivision algorithm", "IAC", "SubD")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddSubDParameter("SubD", "S", "Input subD", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Level", "L", "Number of times to subdivide", GH_ParamAccess.item, 1);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddSubDParameter("SubD", "S", "The subD after subdivision process", GH_ParamAccess.item);
        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            SubD subd = null;
            var level = 0;
            if (!da.GetData(0, ref subd)) return;
            if (!da.GetData(1, ref level)) return;
            if (level == 0)
            {
                da.SetData(0, subd);
                return;
            }

            if (level > 3)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, $"Subdivision level was lowered to 2 from {level}");
                level = 2;
            }

            var result = (SubD)subd.Duplicate();
            if (!result.Subdivide(level))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Subdivision failed");
                return;
            }
            da.SetData(0, result);
        }
        protected override System.Drawing.Bitmap Icon => Properties.Resources.subdivide;
        public override Guid ComponentGuid => new Guid("655e0c70-411b-438c-8393-25f507b536ea");
    }
}