using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace IAC.GrasshopperComponents.SubDGhc
{
    public class SubDLoftGhc : GH_Component
    {
        public SubDLoftGhc() : base("SubD Loft", "SubDLoft", "Creates a SubD lofted through shape curves", "IAC", "SubD")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "Section curves", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Closed", "Cl",
                "Closed loft. Must have three or more shape curves.", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("AddCorners", "Co",
                "With open curves, adds creased vertices to the SubD at both ends of the first and last curves.", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("AddCreases", "Cr",
                "With kinked curves, adds creased edges to the SubD along the kinks.", GH_ParamAccess.item, false);
            pManager.AddIntegerParameter("Divisions", "D", "The segment number between adjacent input curves.", GH_ParamAccess.item, 1);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddSubDParameter("SubD", "S", "SubD lofted", GH_ParamAccess.item);
        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            var curves = new List<Curve>();
            var closed = false;
            var corners = false;
            var creases = false;
            var divisions = 1;
            if (!da.GetDataList(0, curves)) return;
            if (!da.GetData(1, ref closed)) return;
            if (!da.GetData(2, ref corners)) return;
            if (!da.GetData(3, ref creases)) return;
            if (!da.GetData(4, ref divisions)) return;
            if (divisions < 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The division count must be at least 1.");
                return;
            }
            var nurbs = new List<NurbsCurve>();
            var isClosed = curves[0].IsClosed;
            foreach (var curve in curves)
            {
                if (curve == null)
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "A null profile curve was removed");
                else if (!curve.IsValid)
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Invalid profile curve in loft, this may cause problems.");
                else if (isClosed != curve.IsClosed)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Loft only works if all curves are open, or all curves are closed.");
                    return;
                }
                else
                    nurbs.Add(curve.ToNurbsCurve());
            }
            if (nurbs.Count < 2)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "You need at least two curves for a loft.");
                return;
            }
            var result = SubD.CreateFromLoft(nurbs, closed, corners, creases, divisions);
            if (result is null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "SubD could not be constructed.");
                return;
            }
            da.SetData(0, result);
        }
        protected override System.Drawing.Bitmap Icon => Properties.Resources.subDLoft;
        public override GH_Exposure Exposure => GH_Exposure.senary;
        public override Guid ComponentGuid => new Guid("edb3e043-978b-4d36-9b70-69201cfa8870");
    }
}