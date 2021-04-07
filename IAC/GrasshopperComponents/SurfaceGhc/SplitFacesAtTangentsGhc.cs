using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
namespace IAC.GrasshopperComponents.SurfaceGhc
{
    public class SplitFacesAtTangentsGhc : GH_Component
    {
        public SplitFacesAtTangentsGhc() : base("Split Brep Faces At Tangents", "SplitAtFaces", "Splits the face of a Brep at tangent locations.", "IAC", "Surface")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "Brep to split", GH_ParamAccess.item);
            pManager.AddIntegerParameter("FaceIndices", "I", "Optional indices of the faces to split.", GH_ParamAccess.list);
            pManager[1].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "Resulting Brep", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Splitted", "S", "Splitted", GH_ParamAccess.item);
        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            Brep brp = null;
            var indices = new List<int>();
            if (!da.GetData(0, ref brp)) return;
            var withIndices = da.GetDataList(1, indices);
            var faceCount = brp.Faces.Count;
            if (withIndices)
            {
                foreach (var i in indices)
                {
                    if (i >= faceCount || i < 0)
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                            "Face index exceeds the edge count of the brep.");
                    else
                        brp.Faces.SplitFaceAtTangents(i);
                }
            }
            else
                brp.Faces.SplitFacesAtTangents();
            da.SetData(0, brp);
            da.SetData(1, brp.Faces.Count != faceCount);
        }
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("512377c0-dbdb-4ee6-ac6e-e51207aef0ba");
    }
}