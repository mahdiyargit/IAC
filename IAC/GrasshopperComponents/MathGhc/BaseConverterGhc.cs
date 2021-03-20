using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using System;
using System.Linq;

namespace IAC.GrasshopperComponents.MathGhc
{
    public class BaseConverterGhc : GH_Component
    {
        private const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public BaseConverterGhc() : base("Base Converter", "BConverter", "Converts a given value from one base to another.", "IAC", "Math") { }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Value", "V", "Value to convert to another base.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("From Base", "F", "Base for the given value to convert from.\nmust be within Range 2 to 36.", GH_ParamAccess.item, 10);
            pManager.AddIntegerParameter("To Base", "T", "New base for the given value to convert to.\nmust be within Range 2 to 36.", GH_ParamAccess.item, 2);
            var from = (Param_Integer)pManager[1];
            from.AddNamedValue("2 Binary", 2);
            from.AddNamedValue("8 Octal", 8);
            from.AddNamedValue("10 Decimal", 10);
            from.AddNamedValue("16 Hex", 16);
            var to = (Param_Integer)pManager[2];
            to.AddNamedValue("2 Binary", 2);
            to.AddNamedValue("8 Octal", 8);
            to.AddNamedValue("10 Decimal", 10);
            to.AddNamedValue("16 Hex", 16);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Result", "R", "The value which its base has been converted.", GH_ParamAccess.item);
        }
        protected override void SolveInstance(IGH_DataAccess da)
        {
            var str = string.Empty;
            var from = 10;
            var to = 2;
            if (!da.GetData(0, ref str)) return;
            if (!da.GetData(1, ref from)) return;
            if (!da.GetData(2, ref to)) return;
            if (from < 2 || from > 36 || to < 2 || to > 36)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Base must be >=2 and <= 36");
                return;
            }
            if (double.TryParse(str, out var d) && d == 0)
            {
                da.SetData(0, 0);
                return;
            }
            str = str.ToUpper();
            if (string.IsNullOrEmpty(str) || !IsValid(str, from))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Invalid number in base {from}");
                return;
            }
            if (from == to)
            {
                da.SetData(0, str);
                return;
            }

            if (to == 10)
            {
                da.SetData(0, ToDecimal(str, from));
                return;
            }

            if (from == 10)
            {
                da.SetData(0, FromDecimal(str, to));
                return;
            }
            str = ToDecimal(str, from).ToString("f20").Trim('0').Trim('.');
            da.SetData(0, FromDecimal(str, to));
        }
        private static bool IsValid(string str, int radix)
        {
            if (str[0] == '-')
                str = str.Substring(1);
            if (str.IndexOf('.') != -1)
                str = str.Remove(str.IndexOf('.'), 1);
            var digits = Digits.Substring(0, radix);
            return str.All(c => digits.IndexOf(c) != -1);
        }

        public static double ToDecimal(string str, int radix)
        {
            var negative = str[0] == '-';
            if (negative)
                str = str.Substring(1);
            var factor = str.Length - 1;
            var ptIndex = str.IndexOf('.');
            if (ptIndex != -1)
            {
                factor = ptIndex - 1;
                str = str.Remove(ptIndex, 1);
            }
            var result = 0.0;
            foreach (var c in str)
            {
                result += Digits.IndexOf(c) * System.Math.Pow(radix, factor);
                factor--;
            }

            return negative ? -result : result;
        }

        public static string FromDecimal(string str, int radix)
        {
            var negative = str[0] == '-';
            if (negative)
                str = str.Substring(1);
            var intPart = str;
            var decPart = string.Empty;
            var ptIndex = str.IndexOf('.');
            if (ptIndex != -1)
            {
                intPart = str.Remove(ptIndex);
                decPart = str.Remove(0, ptIndex);
            }

            var result = string.Empty;
            if (string.IsNullOrEmpty(intPart))
                result += "0";
            else
            {
                var n = Convert.ToUInt64(intPart);
                while (n != 0)
                {
                    result += Digits[(int)(n % (ulong)radix)];
                    n /= (ulong)radix;
                }
            }
            if (negative)
                result += "-";
            result = new string(result.Reverse().ToArray());
            if (string.IsNullOrEmpty(decPart)) return result;
            var d = Convert.ToDecimal(decPart);
            result += '.';
            for (var i = 0; i < 20; i++)
            {
                var r = d * radix;
                result += Digits[(int)System.Math.Truncate(r)];
                d = r - System.Math.Truncate(r);
                if (d == 0) break;
            }
            return result;
        }
        protected override System.Drawing.Bitmap Icon => Properties.Resources.baseConverter;
        public override Guid ComponentGuid => new Guid("1a59c965-fd3c-4fa5-9848-5ae9b6755a82");
    }
}