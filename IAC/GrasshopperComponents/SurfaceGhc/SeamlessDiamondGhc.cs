using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace IAC.GrasshopperComponents.SurfaceGhc
{
    public class SeamlessDiamondGhc : GH_Component
    {
        public SeamlessDiamondGhc() : base("Seamless Diamond", "SDiamond", "Creates diamond panels on an open surface. for closed surfaces seamless diamond panels are considered.", "IAC", "Surface") { }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "S", "Surface to panelize.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("U", "U", "Divisions in the surface U direction.", GH_ParamAccess.item, 10);
            pManager.AddIntegerParameter("V", "V", "Divisions in the surface V direction.", GH_ParamAccess.item, 10);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGeometryParameter("Diamonds", "Diamonds", "Diamonds", GH_ParamAccess.list);
            pManager.AddGeometryParameter("Triangles", "Triangles", "Triangles", GH_ParamAccess.list);
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Surface srf = null;
            var u = 0;
            var v = 0;
            if (!DA.GetData(0, ref srf)) return;
            if (!DA.GetData(1, ref u)) return;
            if (!DA.GetData(2, ref v)) return;
            srf.SetDomain(0, new Interval(0.0, 1.0));
            srf.SetDomain(1, new Interval(0.0, 1.0));
            var uStep = 1.0 / u;
            var vStep = 1.0 / v;
            var uc = u % 2 == 0 && srf.IsClosed(0);
            var vc = v % 2 == 0 && srf.IsClosed(1);

            var diamonds = new List<NurbsSurface>();
            var triangles = new List<NurbsSurface>();
            switch (uc)
            {
                case true when vc:
                    for (var i = 0; i < u; i++)
                        for (var j = 0; j < v; j++)
                        {
                            if ((i + j) % 2 != 0) continue;
                            var pt1 = i == 0 ? srf.PointAt((u - 1) * uStep, j * vStep) : srf.PointAt((i - 1) * uStep, j * vStep);
                            var pt2 = j == 0 ? srf.PointAt(i * uStep, (v - 1) * vStep) : srf.PointAt(i * uStep, (j - 1) * vStep);
                            var pt3 = srf.PointAt((i + 1) * uStep, j * vStep);
                            var pt4 = j > (v - 1) ? srf.PointAt(i * uStep, j * vStep) : srf.PointAt(i * uStep, (j + 1) * vStep);
                            diamonds.Add(NurbsSurface.CreateFromCorners(pt1, pt2, pt3, pt4));
                        }
                    break;
                case false when !vc:
                    for (var i = 0; i <= u; i++)
                        for (var j = 0; j <= v; j++)
                        {
                            if ((i + j) % 2 != 0) continue;
                            var pt1 = i == 0 ? srf.PointAt(i * uStep, j * vStep) : srf.PointAt((i - 1) * uStep, j * vStep);
                            var pt2 = j == 0 ? srf.PointAt(i * uStep, j * vStep) : srf.PointAt(i * uStep, (j - 1) * vStep);
                            var pt3 = i >= u ? srf.PointAt(i * uStep, j * vStep) : srf.PointAt((i + 1) * uStep, j * vStep);
                            var pt4 = j > (v - 1) ? srf.PointAt(i * uStep, j * vStep) : srf.PointAt(i * uStep, (j + 1) * vStep);
                            if (i > 0 & j > 0 & i < u & j <= (v - 1))
                                diamonds.Add(NurbsSurface.CreateFromCorners(pt1, pt2, pt3, pt4));
                            else
                            {
                                if (i == 0 & j == 0)
                                    triangles.Add(NurbsSurface.CreateFromCorners(pt1, pt2, pt3, pt4));
                                if (i == 0 & j == v)
                                    triangles.Add(NurbsSurface.CreateFromCorners(pt1, pt2, pt3, pt4));
                                if (i == u & j == 0)
                                    triangles.Add(NurbsSurface.CreateFromCorners(pt1, pt2, pt3, pt4));
                                if (i == u & j == v)
                                    triangles.Add(NurbsSurface.CreateFromCorners(pt1, pt2, pt3, pt4));
                                if (i == 0 & j > 0 & j < v)
                                    triangles.Add(NurbsSurface.CreateFromCorners(pt2, pt3, pt4, pt2));
                                if (i == u & j > 0 & j < v)
                                    triangles.Add(NurbsSurface.CreateFromCorners(pt1, pt2, pt4, pt1));
                                if (j == 0 & i > 0 & i < u)
                                    triangles.Add(NurbsSurface.CreateFromCorners(pt1, pt3, pt4, pt1));
                                if (j == v & i > 0 & i < u)
                                    triangles.Add(NurbsSurface.CreateFromCorners(pt1, pt2, pt3, pt1));
                            }
                        }
                    break;
                case true:
                    for (var i = 0; i < u; i++)
                        for (var j = 0; j <= v; j++)
                        {
                            if ((i + j) % 2 != 0) continue;
                            var pt1 = i == 0 ? srf.PointAt((u - 1) * uStep, j * vStep) : srf.PointAt((i - 1) * uStep, j * vStep);
                            var pt2 = j <= 0 ? srf.PointAt(i * uStep, j * vStep) : srf.PointAt(i * uStep, (j - 1) * vStep);
                            var pt3 = srf.PointAt((i + 1) * uStep, j * vStep);
                            var pt4 = j > (v - 1) ? srf.PointAt(i * uStep, j * vStep) : srf.PointAt(i * uStep, (j + 1) * vStep);
                            if (j > 0 & j <= (v - 1))
                                diamonds.Add(NurbsSurface.CreateFromCorners(pt1, pt2, pt3, pt4));
                            else
                            {
                                if (i == 0 & j == 0)
                                    triangles.Add(NurbsSurface.CreateFromCorners(pt1, pt2, pt3, pt4));
                                if (i == 0 & j == v)
                                    triangles.Add(NurbsSurface.CreateFromCorners(pt1, pt2, pt3, pt4));
                                if (j == 0 & i > 0 & i < u)
                                    triangles.Add(NurbsSurface.CreateFromCorners(pt1, pt3, pt4, pt1));
                                if (j == v & i > 0 & i < u)
                                    triangles.Add(NurbsSurface.CreateFromCorners(pt1, pt2, pt3, pt1));
                            }
                        }
                    break;
                case false:
                    for (var i = 0; i <= u; i++)
                        for (var j = 0; j < v; j++)
                        {
                            if ((i + j) % 2 != 0) continue;
                            var pt1 = i <= 0 ? srf.PointAt(i * uStep, j * vStep) : srf.PointAt((i - 1) * uStep, j * vStep);
                            var pt2 = j == 0 ? srf.PointAt(i * uStep, (v - 1) * vStep) : srf.PointAt(i * uStep, (j - 1) * vStep);
                            var pt3 = i >= u ? srf.PointAt(i * uStep, j * vStep) : srf.PointAt((i + 1) * uStep, j * vStep);
                            var pt4 = j > (v - 1) ? srf.PointAt(i * uStep, j * vStep) : srf.PointAt(i * uStep, (j + 1) * vStep);
                            if (i > 0 & i < u)
                                diamonds.Add(NurbsSurface.CreateFromCorners(pt1, pt2, pt3, pt4));
                            else
                            {
                                if (i == 0 & j == 0)
                                    triangles.Add(NurbsSurface.CreateFromCorners(pt1, pt2, pt3, pt4));
                                if (i == u & j == 0)
                                    triangles.Add(NurbsSurface.CreateFromCorners(pt1, pt2, pt3, pt4));
                                if (i == 0 & j > 0 & j < v)
                                    triangles.Add(NurbsSurface.CreateFromCorners(pt2, pt3, pt4, pt2));
                                if (i == u & j > 0 & j < v)
                                    triangles.Add(NurbsSurface.CreateFromCorners(pt1, pt2, pt4, pt1));
                            }
                        }
                    break;
            }
            DA.SetDataList(0, diamonds);
            DA.SetDataList(1, triangles);
        }
        protected override System.Drawing.Bitmap Icon => Properties.Resources.seamlessDiamondPanel;
        public override Guid ComponentGuid => new Guid("1df548a9-5e91-4a4e-b784-49ed348d8d78");
    }
}