using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace IAC.GrasshopperComponents.SubDGhc
{
    public class SubDFriendlyCrvGhc : GH_Component
    {
        public SubDFriendlyCrvGhc() : base("SubD Friendly Curve", "SubDCrv", "Create a NURBS curve, that is suitable for calculations like lofting SubD objects.", "IAC", "SubD")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Vertices", "V", "Curve control points", GH_ParamAccess.list);
            pManager.AddBooleanParameter("InterpolatePoints", "I",
                "Set to true if the curve should interpolate the points. or False if points specify control point locations.\n" +
                "In either case, the curve will begin at the first point and end at the last point.",
                GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Periodic", "P", "Periodic curve", GH_ParamAccess.item, false);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Resulting subD friendly curve", GH_ParamAccess.item);
            pManager.AddNumberParameter("Length", "L", "Curve length", GH_ParamAccess.item);
            pManager.AddIntervalParameter("Domain", "D", "Curve domain", GH_ParamAccess.item);
        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            var vertices = new List<Point3d>();
            var periodic = false;
            var interpolate = false;
            if (!da.GetDataList(0, vertices)) return;
            if (!da.GetData(1, ref interpolate)) return;
            if (!da.GetData(2, ref periodic)) return;
            if (vertices.Count < 2)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Insufficient vertices for a curve");
                return;
            }
            var crv = NurbsCurve.CreateSubDFriendly(vertices, interpolate, periodic);
            if (crv is null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Could not create subD friendly curve");
                return;
            }
            da.SetData(0, crv);
            da.SetData(1, crv.GetLength());
            da.SetData(2, crv.Domain);
        }
        protected override System.Drawing.Bitmap Icon => Properties.Resources.toSubDfrendly;
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("3e0e7011-00f4-493f-9d1b-f7c42e2afb98");
    }
}