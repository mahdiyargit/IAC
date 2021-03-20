using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using System;
using System.Collections;
using System.Reflection;

namespace IAC.GrasshopperComponents.UtilitiesGhc
{
    public class AutoValueListGhc : GH_Component
    {
        private bool _handled;
        private GH_Document _doc;
        public AutoValueListGhc() : base("Auto Value List", "AVL", "Connect to an input with named value options to create a ValueList",
              "IAC", "Utilities")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager) { }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("ToNamedValue", "NV", "Connect to an input with named-values to create a ValueList", GH_ParamAccess.item);
        }
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            _doc = OnPingDocument();
            if (_handled) return;
            _doc.SolutionStart += DocOnSolutionStart;
            _handled = true;
        }
        private void DocOnSolutionStart(object sender, GH_SolutionEventArgs e)
        {
            if (Params.Output[0].Recipients.Count > 0)
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This input doesn't have named-values");
            var c = -1;
            for (var i = 0; i < Params.Output[0].Recipients.Count; i++)
            {
                if (!(Params.Output[0].Recipients[i] is Param_Integer)) continue;
                c = i;
                break;
            }
            if (c == -1) return;
            var recipient = Params.Output[0].Recipients[c];
            var paramInt = (Param_Integer)recipient;
            if (!paramInt.HasNamedValues) return;
            var namedValues = (IList)typeof(Param_Integer).GetField("m_namedValues", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(paramInt);
            if (namedValues == null) return;
            var vl = new GH_ValueList();
            vl.ListItems.Clear();
            foreach (var nv in namedValues)
            {
                var name = (string)nv.GetType().GetField("Name", BindingFlags.Public | BindingFlags.Instance)?.GetValue(nv);
                var value = (int)nv.GetType().GetField("Value", BindingFlags.Public | BindingFlags.Instance)?.GetValue(nv);
                var vi = new GH_ValueListItem(name, value.ToString());
                vl.ListItems.Add(vi);
            }
            vl.NickName = recipient.NickName;

            var docIo = new GH_DocumentIO { Document = new GH_Document() };
            if (docIo.Document == null) return;
            docIo.Document.AddObject(vl, false, 1);
            vl.Attributes.Pivot = Attributes.Pivot;
            docIo.Document.SelectAll();
            docIo.Document.ExpireSolution();
            docIo.Document.MutateAllIds();
            var objs = docIo.Document.Objects;
            _doc.DeselectAll();
            _doc.UndoUtil.RecordAddObjectEvent("Create " + vl.NickName + " List", objs);
            _doc.MergeDocument(docIo.Document);
            recipient.AddSource(vl);
            _doc.DeselectAll();
            _doc.ScheduleSolution(1);
            _doc.SolutionStart -= DocOnSolutionStart;
            _doc.RemoveObject(this, true);
        }
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("57c3c434-fddc-47a9-aa87-67e5deb3958b");
    }
}