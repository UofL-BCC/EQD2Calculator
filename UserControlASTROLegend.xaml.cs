using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EQD2CalculatorWindow
{
    /// <summary>
    /// Interaction logic for UserControlASTROLegend.xaml
    /// </summary>
    public partial class UserControlASTROLegend : UserControl
    {
        public UserControlASTROLegend()
        {

            

            InitializeComponent();

            try
            {
                //string filePath = @"//10.47.8.26/argus/Max/HDR EQD2 Scripting/Projects/EQD2CalculatorWindow3/ASTROGuidelines.png";

                string filePath1 = @"\\umcp-vardb\ProgramData\Vision\images\ASTROGuidelines.png";
                string fullPath = Assembly.GetExecutingAssembly().Location;
                string theDirectory = System.IO.Path.GetDirectoryName(fullPath);
                string fullPath2 = theDirectory + @"\ASTROGuidelines.png";

                AstroGuidelinesImage.Source = new BitmapImage(new Uri(fullPath2, UriKind.Absolute));

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }

        }
    }
}
