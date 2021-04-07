using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace IAC.GrasshopperComponents.SurfaceGhc
{
    public class SplitKinkyFacesGhc : GH_Component
    {
        private bool _degreesTol;
        public SplitKinkyFacesGhc() : base("Split Kinky Faces", "SplitKinkyFaces", "Splits any faces with creases into G1 pieces.", "IAC", "Surface")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "Brep to split", GH_ParamAccess.item);
            pManager.AddIntegerParameter("FaceIndices", "I", "Optional indices of the faces to split.", GH_ParamAccess.list);
            pManager.AddAngleParameter("KinkTolerance", "T", "Optional angle tolerance to use for crease detection.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "Resulting Brep", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Splitted", "S", "Splitted", GH_ParamAccess.item);
        }
        protected override void BeforeSolveInstance()
        {
            _degreesTol = ((Param_Number)Params.Input[2]).UseDegrees;
        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            Brep brp = null;
            var indices = new List<int>();
            var tol = DocumentAngleTolerance();
            if (!da.GetData(0, ref brp)) return;
            var withIndices = da.GetDataList(1, indices);
            var withTol = da.GetData(2, ref tol);
            if (withTol && _degreesTol) tol = RhinoMath.ToRadians(tol);
            var faceCount = brp.Faces.Count;
            if (withIndices)
            {
                foreach (var i in indices)
                {
                    if (i >= faceCount || i < 0)
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                            "Face index exceeds the edge count of the brep.");
                    else brp.Faces.SplitKinkyFace(i, tol);
                }
            }
            else
            {
                if (withTol) brp.Faces.SplitKinkyFaces();
                else brp.Faces.SplitKinkyFaces(tol, true);
            }
            da.SetData(0, brp);
            da.SetData(1, brp.Faces.Count != faceCount);
        }
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("d41745ef-5713-49eb-871e-eb7ec4170e51");
    }
}