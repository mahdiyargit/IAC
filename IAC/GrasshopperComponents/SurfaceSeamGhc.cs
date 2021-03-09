using Grasshopper.Kernel;
using Rhino.Geometry;
using System;

namespace IAC.GrasshopperComponents
{
    public class SurfaceSeamGhc : GH_Component
    {
        public SurfaceSeamGhc() : base("Surface Seam", "SrfSeam", "Adjust the seam of a closed surface.", "IAC", "Surface") { }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "S", "Surface to adjust", GH_ParamAccess.item);
            pManager.AddPointParameter("Point", "uv", "uv of new seams", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "S", "Adjusted surface", GH_ParamAccess.item);
        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            Surface srf = null;
            if (!da.GetData(0, ref srf)) return;
            var uv = Point3d.Unset;
            if (!da.GetData(1, ref uv)) return;
            if (srf.IsClosed(0) || srf.IsClosed(1))
            {
                var brep = srf.ToBrep();
                if (srf.IsClosed(0))
                    brep = Brep.ChangeSeam(brep.Faces[0], 0, uv.X, DocumentTolerance()) ?? brep;
                if (srf.IsClosed(1))
                    brep = Brep.ChangeSeam(brep.Faces[0], 1, uv.Y, DocumentTolerance()) ?? brep;
                da.SetData(0, brep.Faces[0].ToNurbsSurface());
                return;
            }
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Surface is not closed and the seam cannot be adjusted");
            da.SetData(0, srf);
        }
        protected override System.Drawing.Bitmap Icon => Properties.Resources.srfSeam;
        public override Guid ComponentGuid => new Guid("9d5d88a1-ed91-4cca-99ea-e0f51a1ef1a0");
    }
}