using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

// TODO: Replace the following version attributes by creating AssemblyInfo.cs. You can do this in the properties of the Visual Studio project.
[assembly: AssemblyVersion("1.0.0.1")]
[assembly: AssemblyFileVersion("1.0.0.1")]
[assembly: AssemblyInformationalVersion("1.0")]

// TODO: Uncomment the following line if the script requires write access.
// [assembly: ESAPIScript(IsWriteable = true)]

namespace VMS.TPS
{
  public class Script
  {
    public Script()
    {
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void Execute(ScriptContext context , System.Windows.Window window/*, ScriptEnvironment environment*/)
    {
            var userControl = new EQD2CalculatorWindow.UserControl1(context, window);

            userControl.context1 = context;
            userControl.window1 = window;

            window.Width = 1600;
            window.Height = 1150;

            window.Left = 5;
            window.Top = 5;

            window.Content = userControl;
            window.VerticalAlignment = VerticalAlignment.Top;
            window.HorizontalAlignment = HorizontalAlignment.Left;

            Patient current_patient = context.Patient;



        }
    }
}
