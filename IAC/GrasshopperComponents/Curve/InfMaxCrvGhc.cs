using Grasshopper.Kernel;
using Rhino.Display;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace IAC.GrasshopperComponents
{
    public class InfMaxCrvGhc : GH_Component
    {
        private readonly List<Point3d> _inflections = new List<Point3d>();
        private readonly List<Point3d> _mcp = new List<Point3d>();
        private BoundingBox _box;
        public InfMaxCrvGhc() : base("Inflection and max curvature points", "InfMaxCrvPts", "Find inflection and maximum curvature points of a given curve.", "IAC", "Curve")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve to evaluate", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Inflection points", "I",
                "An inflection point is a location on a curve at which the sign of the curvature (i.e., the concavity) changes.\nThe curvature at these locations is always 0.", GH_ParamAccess.list);
            pManager.AddPointParameter("Max curvature points", "M",
                "Curve's maximum curvature points.\nWhere the curvature starts to decrease in both directions from the points.",
                GH_ParamAccess.list);
        }
        protected override void BeforeSolveInstance()
        {
            _inflections.Clear();
            _mcp.Clear();
        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            Curve crv = null;
            if (!da.GetData(0, ref crv)) return;
            var inflections = crv.InflectionPoints();
            if (inflections != null)
                _inflections.AddRange(inflections);
            var mcp = crv.MaxCurvaturePoints();
            if (mcp != null)
                _mcp.AddRange(mcp);
            da.SetDataList(0, inflections);
            da.SetDataList(1, mcp);
        }
        protected override void AfterSolveInstance()
        {
            _box = new BoundingBox(_inflections.Union(_mcp));
        }
        public override BoundingBox ClippingBox => _box;
        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            if (_inflections != null && _inflections.Any())
                args.Display.DrawPoints(_inflections, PointStyle.Square, 6, Color.Black);
            if (_mcp != null && _mcp.Any())
                args.Display.DrawPoints(_mcp, PointStyle.Square, 6, Color.White);
        }
        protected override Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("aefc361d-70bb-4a29-8489-89dbfd0fe91c");
    }
}