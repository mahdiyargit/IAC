using Grasshopper.Kernel;
using Rhino.Geometry;
using System;

namespace IAC.GrasshopperComponents.SurfaceGhc
{
    public class ExtendSurfaceGhc : GH_Component
    {
        public ExtendSurfaceGhc() : base("Extend Surface", "Extend", "Extends an untrimmed surface along edges.", "IAC", "Surface")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "S", "Surface to extend", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Smooth", "S",
                "True for smooth (C-infinity) extension. false for a C1- ruled extension.", GH_ParamAccess.item, true);
            pManager.AddNumberParameter("West", "W", "Distance along the west side of the surface's domain.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("South", "S", "Distance along the west side of the surface's domain.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("East", "E", "Distance along the west side of the surface's domain.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("North", "N", "Distance along the west side of the surface's domain.", GH_ParamAccess.item, 0.0);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "S", "Extended surface", GH_ParamAccess.item);
        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            Surface srf = null;
            var smooth = true;
            var west = 0.0;
            var south = 0.0;
            var east = 0.0;
            var north = 0.0;
            if (!da.GetData(0, ref srf)) return;
            if (!da.GetData(1, ref smooth)) return;
            if (!da.GetData(2, ref west)) return;
            if (!da.GetData(3, ref south)) return;
            if (!da.GetData(4, ref east)) return;
            if (!da.GetData(4, ref north)) return;
            if (west > 0.0) srf.Extend(IsoStatus.West, west, smooth);
            if (south > 0.0) srf.Extend(IsoStatus.South, south, smooth);
            if (east > 0.0) srf.Extend(IsoStatus.East, east, smooth);
            if (north > 0.0) srf.Extend(IsoStatus.North, north, smooth);
            da.SetData(0, srf);
        }
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("0dcd654f-d51c-4ead-8b10-9fdf8d5ddf29");
    }
}