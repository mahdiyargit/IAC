using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace IAC.GrasshopperComponents.SubDGhc
{
    public class SubDSweep2Ghc : GH_Component
    {
        public SubDSweep2Ghc() : base("SubD Sweep2", "SubDSweep2", "Fits a SubD through a series of profile curves that define the SubD cross-sections and two curves that defines SubD edges",
              "IAC", "SubD")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Rail 1", "R¹", "The first curve to sweep along",
                GH_ParamAccess.item);
            pManager.AddCurveParameter("Rail 2", "R²", "The second curve to sweep along",
                GH_ParamAccess.item);
            pManager.AddCurveParameter("Sections", "S", "Section curves", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Closed", "Cl",
                "Set to true in order Creates a SubD that is closed in the rail curve direction", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Corners", "Co",
                "Set to true to adds creased vertices to the SubD at both ends of the first and last curves",
                GH_ParamAccess.item, false);

        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddSubDParameter("SubD", "S", "Resulting subD", GH_ParamAccess.item);
        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            Curve rail1 = null;
            Curve rail2 = null;
            var shapes = new List<Curve>();
            var closed = false;
            var corners = false;
            if (!da.GetData(0, ref rail1)) return;
            if (!da.GetData(1, ref rail2)) return;
            if (!da.GetDataList(2, shapes)) return;
            if (!da.GetData(3, ref closed)) return;
            if (!da.GetData(4, ref corners)) return;
            var nurbs = new List<NurbsCurve>();
            if (!rail1.IsSubDFriendly || !rail2.IsSubDFriendly)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Rail curves must be subD-friendly");
                return;
            }
            foreach (var shape in shapes)
            {
                if (shape == null)
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "A null section was removed from the list.");
                else if (!shape.IsSubDFriendly)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "All sections curves must be subD-friendly");
                    return;
                }
                else
                    nurbs.Add(shape.ToNurbsCurve());
            }
            var result = SubD.CreateFromSweep(rail1.ToNurbsCurve(), rail2.ToNurbsCurve(), nurbs, closed, corners);
            if (result == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Sweep could not be created");
                return;
            }
            da.SetData(0, result);
        }
        protected override System.Drawing.Bitmap Icon => Properties.Resources.subDSweep2;
        public override GH_Exposure Exposure => GH_Exposure.senary;
        public override Guid ComponentGuid => new Guid("1d3d99f8-2d46-4bdb-83c5-5352e27f85be");
    }
}