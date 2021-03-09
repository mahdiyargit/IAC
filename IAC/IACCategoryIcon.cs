using Grasshopper.Kernel;

namespace IAC
{
    public class IAC : GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            Grasshopper.Instances.ComponentServer.AddCategoryIcon("IAC", Properties.Resources.icon16X16);
            Grasshopper.Instances.ComponentServer.AddCategorySymbolName("IAC", 'I');
            return GH_LoadingInstruction.Proceed;
        }
    }
}