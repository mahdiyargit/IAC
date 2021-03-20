using Grasshopper.Kernel;
using Rhino.Geometry;
using System;

namespace IAC.GrasshopperComponents.CurveGhc
{
    public class RecByAreaPerimeterGhc : GH_Component
    {
        public RecByAreaPerimeterGhc() : base("Rectangle by Area Perimeter", "Rec", "Create a rectangle with certain area and/or perimeter",
              "IAC", "Curve")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Rectangle base plane", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddNumberParameter("Area", "A", "Optional area of rectangle", GH_ParamAccess.item, 12.0);
            pManager.AddNumberParameter("Perimeter", "P", "Optional perimeter of rectangle", GH_ParamAccess.item, 14.0);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddRectangleParameter("Rectangle", "R", "Rectangle defined by area and/or perimeter",
                GH_ParamAccess.item);
            pManager.AddNumberParameter("Length", "L", "Length of rectangle curve", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
            var plane = Plane.WorldXY;
            var s = double.NaN;
            var p = double.NaN;
            if (!da.GetData(0, ref plane)) return;
            var byS = da.GetData(1, ref s);
            var byP = da.GetData(2, ref p);
            double w, l;
            switch (byS)
            {
                case false when !byP:
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Both Area and Perimeter can't be undefined");
                    return;
                case true when !byP:
                    w = Math.Sqrt(s);
                    l = w;
                    break;
                case false:
                    w = p / 4.0;
                    l = w;
                    break;
                case true:
                    var delta = p * p - 16 * s;
                    if (delta < 0)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Rectangle can not be created");
                        return;
                    }
                    delta = Math.Sqrt(delta);
                    w = (p + delta) / 4;
                    l = (p - delta) / 4;
                    break;
            }
            var rec = new Rectangle3d(plane, w, l);
            da.SetData(0, rec);
            da.SetData(1, rec.Circumference);
        }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("292a06bd-f038-48a9-9a5a-854c534bc792");
    }
}