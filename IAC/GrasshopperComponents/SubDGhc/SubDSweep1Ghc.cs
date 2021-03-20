using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace IAC.GrasshopperComponents.SubDGhc
{
    public class SubDSweep1Ghc : GH_Component
    {
        public SubDSweep1Ghc() : base("SubD Sweep1", "SubDSweep2", "Fits a SubD through a series of profile curves that define the SubD cross-sections and one curve that defines a SubD edge.", "IAC", "SubD")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Rail", "R", "A subD-friendly curve to sweep along",
                GH_ParamAccess.item);
            pManager.AddCurveParameter("Sections", "S", "Section subD-friendly curves", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Closed", "Cl",
                "Set to true in order Creates a SubD that is closed in the rail curve direction", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Corners", "Co",
                "Set to true to adds creased vertices to the SubD at both ends of the first and last curves",
                GH_ParamAccess.item, false);
            pManager.AddVectorParameter("RoadlikeNormal", "N",
                "Optional 3D vector used to calculate the frame rotations for sweep shapes.\n" +
                "if empty, frame will be propagated based on a reference direction taken from the rail curve curvature direction.",
                GH_ParamAccess.item);
            pManager[4].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddSubDParameter("SubD", "S", "Resulting subD", GH_ParamAccess.item);
        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            Curve rail = null;
            var shapes = new List<Curve>();
            var closed = false;
            var corners = false;
            var normal = Vector3d.Unset;
            if (!da.GetData(0, ref rail)) return;
            if (!da.GetDataList(1, shapes)) return;
            if (!da.GetData(2, ref closed)) return;
            if (!da.GetData(3, ref corners)) return;
            da.GetData(4, ref normal);
            var nurbs = new List<NurbsCurve>();
            if (!rail.IsSubDFriendly)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Rail curve must be subD-friendly");
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
            SubD result;
            if (!normal.IsValid || normal.IsZero)
                result = SubD.CreateFromSweep(rail.ToNurbsCurve(), nurbs, closed, corners, false, Vector3d.Unset);
            else
                result = SubD.CreateFromSweep(rail.ToNurbsCurve(), nurbs, closed, corners, true, normal);
            if (result == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Sweep could not be created");
                return;
            }
            da.SetData(0, result);
        }
        protected override System.Drawing.Bitmap Icon => Properties.Resources.subDSweep1;
        public override GH_Exposure Exposure => GH_Exposure.senary;
        public override Guid ComponentGuid => new Guid("205555d3-15a0-4a8b-904f-7cccdb14bc84");
    }
}