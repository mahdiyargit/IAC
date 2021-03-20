using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;

namespace IAC.GrasshopperComponents.SubDGhc
{
    public class SubDToNURBSGhc : GH_Component
    {
        public SubDToNURBSGhc() : base("SubD To NURBS", "ToNURBS", "Converts SubD objects to NURBS.", "IAC", "SubD")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddSubDParameter("SubD", "S", "SubD geometry", GH_ParamAccess.item);
            pManager.AddBooleanParameter("PackFaces", "P", "Specifies if faces will be merged when converting a SubD into a polysurface.", GH_ParamAccess.item, false);
            pManager.AddIntegerParameter("VertexProcess", "V",
                "Continuity on a SubD at extraordinary vertices is G2. When converting a SubD into a polysurface,\n" +
                "extraordinary vertices can only be approximated. This option controls the continuity of the polysurface near extraordinary vertices.", GH_ParamAccess.item, 1);
            var vertexProcess = (Param_Integer)pManager[2];
            vertexProcess.AddNamedValue("None", 0);
            vertexProcess.AddNamedValue("LocalG1", 1);
            vertexProcess.AddNamedValue("LocalG2", 2);
            vertexProcess.AddNamedValue("LocalG1x", 3);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "Brep", GH_ParamAccess.item);
        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            SubD subd = null;
            var packFaces = false;
            var vertexProcess = 1;
            if (!da.GetData(0, ref subd)) return;
            if (!da.GetData(1, ref packFaces)) return;
            if (!da.GetData(2, ref vertexProcess)) return;
            var result = subd.ToBrep(new SubDToBrepOptions(packFaces,
                (SubDToBrepOptions.ExtraordinaryVertexProcessOption)vertexProcess));
            if (result is null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "SubD could not be constructed.");
                return;
            }
            da.SetData(0, result);
        }
        protected override System.Drawing.Bitmap Icon => Properties.Resources.subDToNurbs;
        public override GH_Exposure Exposure => GH_Exposure.senary;
        public override Guid ComponentGuid => new Guid("a2c36254-9c3b-48dd-a0dc-16cd575d2799");
    }
}