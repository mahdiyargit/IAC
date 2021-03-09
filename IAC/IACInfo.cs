using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace IAC
{
    public class IACInfo : GH_AssemblyInfo
    {
        public override string Name => "IAC";
        public override Bitmap Icon => Properties.Resources.icon24X24;
        public override string Description => "This plugin has been written by the participants of the \"Scripting & plugin development\" workshop in Iranian Architecture Center.";
        public override Guid Id => new Guid("cd54ee94-048a-43f1-b492-150024d0640c");
        public override string AuthorName => "IranianArchitectureCenter";
        public override string AuthorContact => "info@Mahdiyar.io";
    }
}
