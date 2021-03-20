using Grasshopper.Kernel;
using Rhino.Geometry;
using System;

namespace IAC.GrasshopperComponents.CurveGhc
{
    public class CircleBy2Spheres : GH_Component
    {
        public CircleBy2Spheres() : base("Circle by 2 Spheres", "Circle", "The intersection circle of two sphere.", "IAC", "Curve")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("FirstCenter", "C¹", "Center of first circle", GH_ParamAccess.item);
            pManager.AddPointParameter("SecondCenter", "C²", "Center of second circle", GH_ParamAccess.item);
            pManager.AddNumberParameter("FirstRadius", "R¹", "Radius of first circle", GH_ParamAccess.item);
            pManager.AddNumberParameter("SecondRadius", "R²", "Radius of second circle", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCircleParameter("Circle", "C", "Resulting circle", GH_ParamAccess.item);
        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            var pt1 = Point3d.Unset;
            var pt2 = Point3d.Unset;
            var r1 = double.NaN;
            var r2 = double.NaN;
            if (!da.GetData(0, ref pt1)) return;
            if (!da.GetData(1, ref pt2)) return;
            if (!da.GetData(2, ref r1)) return;
            if (!da.GetData(3, ref r2)) return;

            var a = Math.Min(r1, r2);
            var c = Math.Max(r1, r2);
            if (r1 > r2)
            {
                var temp = pt1;
                pt1 = pt2;
                pt2 = temp;
            }
            var b = pt1.DistanceTo(pt2);
            //Area of triangle Heron’s formula
            var s = (a + b + c) / 2;
            var area = Math.Sqrt(s * (s - a) * (s - b) * (s - c));
            var h = 2 * area / b;
            var x = Math.Sqrt((a * a) - (h * h));

            if ((a * a + b * b) < (c * c))
                x = -x;

            var vector = new Vector3d(pt2 - pt1);
            vector.Unitize();
            var center = pt1 + vector * x;
            var circle = new Circle(new Plane(center, vector), h);
            if (!circle.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Spheres are not intersected");
                return;
            }
            da.SetData(0, circle);
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("a139b206-10d6-4144-b2ff-e868a6b3859c");
    }
}