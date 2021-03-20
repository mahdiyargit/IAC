using Grasshopper.Kernel;
using Rhino.Geometry;
using System;

namespace IAC.GrasshopperComponents.SubDGhc
{
    public class ToSubDFriendlyCrvGhc : GH_Component
    {
        public ToSubDFriendlyCrvGhc() : base("To SubD Friendly Curve", "ToSubDCrv", "Create a NURBS curve, that is suitable for calculations like lofting SubD objects, from an existing curve..", "IAC", "SubD")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Point count", "N",
                "Optional desired number of control points. If periodicClosedCurve is true,\nthe number must be >= 6, otherwise the number must be >= 4.", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Resulting subD friendly curve", GH_ParamAccess.item);
        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            Curve crv = null;
            var count = 0;
            if (!da.GetData(0, ref crv)) return;
            var output = da.GetData(1, ref count) ? NurbsCurve.CreateSubDFriendly(crv, count, crv.IsClosed) : NurbsCurve.CreateSubDFriendly(crv);
            if (output is null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Could not create subD friendly curve");
                return;
            }
            da.SetData(0, output);
        }
        protected override System.Drawing.Bitmap Icon => Properties.Resources.toSubDfrendly;
        public override Guid ComponentGuid => new Guid("032cb201-6472-4588-9a11-05cc77182025");
    }
}