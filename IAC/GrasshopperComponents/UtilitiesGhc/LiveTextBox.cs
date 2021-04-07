using Grasshopper.Kernel;
using System;
using System.Windows.Forms;

namespace IAC.GrasshopperComponents.UtilitiesGhc
{
    public class LiveTextBoxGhc : GH_Component
    {
        private string _text;
        public LiveTextBoxGhc() : base("Live Text Box", "LTB", "Live Text Box", "IAC", "Utilities")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Text", "T", "Text", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
            da.SetData(0, _text);
        }
        public override bool AppendMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendTextItem(menu, _text, Menu_KeyDown, Menu_TextChanged, true);
            return true;
        }
        private void Menu_TextChanged(object sender, string inputText)
        {
            _text = inputText;
            ExpireSolution(true);
        }
        private static void Menu_KeyDown(object sender, EventArgs e)
        {
        }
        protected override System.Drawing.Bitmap Icon => Properties.Resources.LiveTextBox;
        public override Guid ComponentGuid => new Guid("382D2692-0E56-45BA-9644-5812B6D26701");
    }
}