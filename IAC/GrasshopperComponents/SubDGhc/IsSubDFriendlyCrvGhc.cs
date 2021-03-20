using Grasshopper.Kernel;
using Rhino.Geometry;
using System;

namespace IAC.GrasshopperComponents.SubDGhc
{
    public class IsSubDFriendlyCrvGhc : GH_Component
    {
        public IsSubDFriendlyCrvGhc()
          : base("Is SubD Friendly Curve", "IsSubDCrv", "Test if a curve is subD Friendly", "IAC", "SubD")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("SubD Friendly", "S", "Returns true if the curve is a cubic, non-rational,\n" +
                                                               "uniform NURBS curve that is either periodic or has natural end conditions. Otherwise, false is returned.", GH_ParamAccess.item);
        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            Curve crv = null;
            if (!da.GetData(0, ref crv)) return;
            da.SetData(0, crv.IsSubDFriendly);
        }
        protected override System.Drawing.Bitmap Icon => Properties.Resources.isSubDFriendly;
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("33ccd8b4-5e65-4d50-91f0-0fc7d36d9045");
    }
}