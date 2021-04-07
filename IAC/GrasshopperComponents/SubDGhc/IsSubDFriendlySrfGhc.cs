using Grasshopper.Kernel;
using System;

namespace IAC.GrasshopperComponents.SubDGhc
{
    public class IsSubDFriendlySrfGhc : GH_Component
    {
        public IsSubDFriendlySrfGhc() : base("Is SubD Friendly Surface", "IsSubDSrf", "Test if a surface is subD Friendly",
              "IAC", "SubD")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "S", "Surface", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("SubD Friendly", "S", "True if the surface is a non-rational,\n" +
                                                               "uniform, natural or periodic, cubic NURBS surface. Otherwise, false is returned.", GH_ParamAccess.item);
        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            Rhino.Geometry.Surface srf = null;
            if (!da.GetData(0, ref srf)) return;
            da.SetData(0, srf.IsSubDFriendly);
        }
        protected override System.Drawing.Bitmap Icon => Properties.Resources.IsSubDFriendlySurf;
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("c691b0bd-7ccb-4fb1-bb35-4a863d1ce437");
    }
}