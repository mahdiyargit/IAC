using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IAC.GrasshopperComponents.SurfaceGhc
{
    public class SurfaceDiscontinuityGhc : GH_Component
    {
        public SurfaceDiscontinuityGhc() : base("Surface Discontinuity", "SrfDisc", "Find all discontinuity along a surface.", "IAC", "Surface")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "S", "Surface  to analyze", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Level", "L", "Level of discontinuity to test for (1=C1, 2=C2, 3=Cinfinite)", GH_ParamAccess.item, 1);
            var level = (Param_Integer)pManager[1];
            level.AddNamedValue("C1 (tangency)", 1);
            level.AddNamedValue("C2 (curvature)", 2);
            level.AddNamedValue("C∞ (analytic)", 3);

        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("U Curves", "uC", "Curves at discontinuities along {u} direction", GH_ParamAccess.list);
            pManager.AddCurveParameter("V Curves", "vC", "Curves at discontinuities along {v} direction", GH_ParamAccess.list);
            pManager.AddPointParameter("U Parameters", "u", "uv Parameters at the discontinuities along {u} direction.", GH_ParamAccess.list);
            pManager.AddPointParameter("V Parameters", "v", "uv Parameters at the discontinuities along {v} direction.", GH_ParamAccess.list);
            pManager.HideParameter(2);
            pManager.HideParameter(3);
        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            Surface srf = null;
            var level = 1;
            if (!da.GetData(0, ref srf)) return;
            if (!da.GetData(1, ref level)) return;
            Continuity continuity;
            switch (level)
            {
                case 1:
                    continuity = (Continuity)7;
                    break;
                case 2:
                    continuity = (Continuity)8;
                    break;
                case 3:
                    continuity = (Continuity)11;
                    break;
                default:
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid discontinuity level. Allowed values are: 1, 2, 3");
                    return;
            }

            var uParameters = new List<double>();
            var vParameters = new List<double>();
            var ud = srf.Domain(0);
            var vd = srf.Domain(1);
            var tol = DocumentAngleTolerance();
            #region uDiscontinuities
            switch ((int)continuity)
            {
                case 7: 
                    if (srf.IsClosed(0))
                    {
                        var crv = srf.IsoCurve(1, ud.Min);
                        var vec1 = crv.TangentAt(ud.Min);
                        var vec2 = crv.TangentAt(ud.Max);
                        if (vec1.IsParallelTo(vec2, tol) != 1)
                            uParameters.Add(ud.Min);
                        break;
                    }
                    uParameters.Add(ud.Min);
                    break;
                case 8: 
                    if (srf.IsClosed(0))
                    {
                        var crv = srf.IsoCurve(1, ud.Min);
                        var vec1 = crv.CurvatureAt(ud.Min);
                        var vec2 = crv.CurvatureAt(ud.Max);
                        if (Math.Abs(vec1.Length - vec2.Length) > 1E-06)
                            uParameters.Add(ud.Min);
                        break;
                    }
                    uParameters.Add(ud.Min);
                    break;
                case 11: 
                    uParameters.Add(ud.Min);
                    break;
            }
            var t0 = ud.Min;
            while (srf.GetNextDiscontinuity(0, continuity, t0, ud.Max, out var u) && Math.Abs(ud.Max - t0) >= double.Epsilon)
            {
                t0 = u;
                uParameters.Add(u);
            }
            if (!srf.IsClosed(0) && !uParameters.Contains(ud.Max))
                uParameters.Add(ud.Max);
            #endregion
            #region vDiscontinuities
            switch ((int)continuity)
            {
                case 7: 
                    if (srf.IsClosed(1))
                    {
                        var crv = srf.IsoCurve(0, vd.Min);
                        var vec1 = crv.TangentAt(vd.Min);
                        var vec2 = crv.TangentAt(vd.Max);
                        if (vec1.IsParallelTo(vec2, tol) != 1)
                            vParameters.Add(vd.Min);
                        break;
                    }
                    vParameters.Add(vd.Min);
                    break;
                case 8: 
                    if (srf.IsClosed(1))
                    {
                        var crv = srf.IsoCurve(0, vd.Min);
                        var vec1 = crv.CurvatureAt(vd.Min);
                        var vec2 = crv.CurvatureAt(vd.Max);
                        if (Math.Abs(vec1.Length - vec2.Length) > 1E-06)
                            vParameters.Add(vd.Min);
                        break;
                    }
                    vParameters.Add(vd.Min);
                    break;
                case 11: 
                    vParameters.Add(vd.Min);
                    break;
            }
            t0 = vd.Min;
            while (srf.GetNextDiscontinuity(1, continuity, t0, vd.Max, out var v) && Math.Abs(vd.Max - t0) >= double.Epsilon)
            {
                t0 = v;
                vParameters.Add(v);
            }
            if (!srf.IsClosed(1) && !vParameters.Contains(vd.Max))
                vParameters.Add(vd.Max);
            #endregion
            da.SetDataList(0, uParameters.Select(u => srf.IsoCurve(1, u)));
            da.SetDataList(1, vParameters.Select(v => srf.IsoCurve(0, v)));
            da.SetDataList(2, uParameters.Select(u => new Point3d(u, vd.Min, 0.0)));
            da.SetDataList(3, vParameters.Select(v => new Point3d(ud.Min, v, 0.0)));
        }
        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("1c0dcacd-c407-4f88-8cd8-f0b22718f662");
    }
}