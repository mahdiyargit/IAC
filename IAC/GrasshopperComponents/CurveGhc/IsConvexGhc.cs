using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using System;

namespace IAC.GrasshopperComponents.CurveGhc
{
    public class IsConvexGhc : GH_Component
    {
        public IsConvexGhc()
          : base("Is Convex", "Convex", "Test if a polygon is convex.", "IAC", "Curve")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Polygon", "P", "Polygon", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Convex", "C", "True if polygon is convex", GH_ParamAccess.item);
        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            Curve crv = null;
            if (!da.GetData(0, ref crv)) return;
            var tol = DocumentTolerance();
            if (crv.IsClosed && crv.IsPlanar(tol))
            {
                crv = crv.Simplify(CurveSimplifyOptions.All, tol, DocumentAngleTolerance());
                if (crv.TryGetPolyline(out var pl))
                    if (Intersection.CurveSelf(crv, tol).Count == 0)
                        da.SetData(0, IsConvex(pl));
                    else
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Polygon must be simple (no self intersections)");
                else
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Curve can't be represented as a polyline");
            }
            else
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Curve must be closed and planar polyline");
        }
        private static bool IsConvex(Polyline pl)
        {
            pl.ToNurbsCurve().TryGetPlane(out var plane);
            pl.Transform(Transform.PlaneToPlane(plane, Plane.WorldXY));
            if (pl.ToNurbsCurve().ClosedCurveOrientation() != CurveOrientation.CounterClockwise)
                pl.Reverse();
            var n = pl.SegmentCount;
            for (var i = 0; i < n; i++)
            {
                if (Vector3d.CrossProduct(pl.SegmentAt(i).Direction, pl.SegmentAt((i + 1) % n).Direction).Z < 0)
                    return false;
            }
            return true;
        }
        protected override System.Drawing.Bitmap Icon => Properties.Resources.IsConvex;
        public override Guid ComponentGuid => new Guid("2665e016-0ab6-42a9-8485-f201c4a0d9ab");
    }
}