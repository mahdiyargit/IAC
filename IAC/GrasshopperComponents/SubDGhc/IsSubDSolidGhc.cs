using Grasshopper.Kernel;
using Rhino.Geometry;
using System;

namespace IAC.GrasshopperComponents.SubDGhc
{
    public class IsSubDSolidGhc : GH_Component
    {
        public IsSubDSolidGhc() : base("Is SubD Solid", "IsSubDSolid", "Test SubD to see if the active level is a solid.\n" +
                                                                    "A \"solid\" is a closed oriented manifold, or a closed oriented manifold.", "IAC", "SubD")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddSubDParameter("SubD", "S", "SubD to evaluate", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Solid", "S", "True if the active level is a solid", GH_ParamAccess.item);
        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            SubD subd = null;
            if (!da.GetData(0, ref subd)) return;
            da.SetData(0, subd.IsSolid);
        }
        protected override System.Drawing.Bitmap Icon => null;
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("ce374c52-ce57-4849-953d-0e51e9c6fe8a");
    }
}