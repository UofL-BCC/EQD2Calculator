using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
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
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;


namespace EQD2CalculatorWindow
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>

    public partial class UserControl1 : UserControl
    {
        public ScriptContext context1;
        public Window window1;

        public Dictionary<string, string> clipboardTableDict;

        public int ExtraMetrics = 0;

        public int FormatRowHeightInt = 0;

        //instantiate OAR dose constraint classes
        public OARDoseConstraints BladderDoseConstraints = new OARDoseConstraints();
        public OARDoseConstraints RectumDoseConstraints = new OARDoseConstraints();
        public OARDoseConstraints SigmoidDoseConstraints = new OARDoseConstraints();
        public OARDoseConstraints BowelDoseConstraints = new OARDoseConstraints();
        public TargetDoseConstraints HRCTVDoseConstraints = new TargetDoseConstraints();
        public TargetDoseConstraints IRCTVDoseConstraints = new TargetDoseConstraints();
        public TargetDoseConstraints GTVDoseConstraints = new TargetDoseConstraints();

        public int refreshCount = 0;


        public UserControl1(ScriptContext context, Window window)
        {
            InitializeComponent();



            var PlansInScope = context.PlansInScope;
            double NumberOfPLans = PlansInScope.Count();


            //throw if 2 plans with the same name are loaded in the context
            var identicalNameGroups = PlansInScope.GroupBy(c => c.Id).ToList();
            foreach (var group in identicalNameGroups)
            {
                if (group.Count() > 1)
                {
                    MessageBox.Show("There are plans in the scope which have the same Id.\nPlease close or rename the identical plans and run the script again.");
                    throw new Exception("script terminated.");
                }
            }
    

            FillAlphaBetaTextBoxes();


            if (NumberOfPLans > 5)
            {
                MessageBox.Show("You have more than 5 plans open. \n" +
                    "The script can only display information for 5 plans at a time.\n" +
                    "Please close some plans and re-run.");
            }


            for (int i = 1; i < NumberOfPLans + 1; i++)
            {
                FillComboBoxes(context, PlansInScope.ElementAt(i - 1), i);
 
                var fxTextBox = returnCurrentFractionTextBox(PlansInScope.ElementAt(i - 1));

                FillCurrentFractionTextBox(PlansInScope.ElementAt(i - 1), fxTextBox);

            }





        }


        public void FillComboBoxes(ScriptContext context1, PlanSetup planSetup, double planNumber)
        {

            FillPLanTextBoxes(planSetup, planNumber);


            List<ComboBox> ComboBoxList = new List<ComboBox>();
            List<ComboBox> ComboBoxListFirst = new List<ComboBox>
            {
                ComboBoxCTV, //*this is not the same order as in the UI. We used this order for nor eason, keeping it for consistency. See numbers below that match
                ComboBoxBladder,
                ComboBoxRectum,
                ComboBoxSigmoid,
                ComboBoxSmallBowel, 
                ComboBoxIRCTV,
                ComboBoxGTV,


            };
            List<ComboBox> ComboBoxListSecond = new List<ComboBox>
            {
                ComboBoxCTV1,
                ComboBoxBladder1,
                ComboBoxRectum1,
                ComboBoxSigmoid1,
                ComboBoxSmallBowel1,  
                ComboBoxIRCTV1,
                ComboBoxGTV1,
            };
            List<ComboBox> ComboBoxListThird = new List<ComboBox>
            {
                ComboBoxCTV2,
                ComboBoxBladder2,
                ComboBoxRectum2,
                ComboBoxSigmoid2,
                ComboBoxSmallBowel2,
                   ComboBoxIRCTV2,
                ComboBoxGTV2,
            };
            List<ComboBox> ComboBoxListFourth = new List<ComboBox>
            {
                ComboBoxCTV3,
                ComboBoxBladder3,
                ComboBoxRectum3,
                ComboBoxSigmoid3,
                ComboBoxSmallBowel3,
                   ComboBoxIRCTV3,
                ComboBoxGTV3,
            };
            List<ComboBox> ComboBoxListFifth = new List<ComboBox>
            {
                ComboBoxCTV4,
                ComboBoxBladder4,
                ComboBoxRectum4,
                ComboBoxSigmoid4,
                ComboBoxSmallBowel4,
                    ComboBoxIRCTV4,
                ComboBoxGTV4,
            };


            if (planNumber == 1)
            {
                ComboBoxList.AddRange(ComboBoxListFirst);
            }
            if (planNumber == 2)
            {
                ComboBoxList.AddRange(ComboBoxListSecond);
            }
            if (planNumber == 3)
            {
                ComboBoxList.AddRange(ComboBoxListThird);
            }
            if (planNumber == 4)
            {
                ComboBoxList.AddRange(ComboBoxListFourth);
            }
            if (planNumber == 5)
            {
                ComboBoxList.AddRange(ComboBoxListFifth);
            }




            foreach (var box in ComboBoxList)
            {
                foreach (var structure in planSetup.StructureSet.Structures)
                {
                    box.Items.Add(structure.Id);


                }
                box.Items.Add("");

            }

            foreach (var comboBox in ComboBoxList)
            {
                foreach (var structureId in comboBox.Items)
                {
                    if (structureId.ToString().ToLower().Contains("ctv") & (structureId.ToString().ToLower().Contains("ir") == false) & (structureId.ToString().ToLower().Contains("-")
                        & (structureId.ToString().ToLower().Contains("ptv") || structureId.ToString().ToLower().Contains("0.3"))) == false)
                    {
                        ComboBoxList.ElementAt(0).SelectedItem = structureId;
                    }

                    if (structureId.ToString().ToLower().Contains("ir") & structureId.ToString().ToLower().Contains("ctv") & (structureId.ToString().ToLower().Contains("-")
                        & (structureId.ToString().ToLower().Contains("ptv") || structureId.ToString().ToLower().Contains("0.3"))) == false)
                    {
                        ComboBoxList.ElementAt(5).SelectedItem = structureId;
                    }

                    if (structureId.ToString().ToLower().Contains("gtv") & (structureId.ToString().ToLower().Contains("-")
                        & (structureId.ToString().ToLower().Contains("ptv") || structureId.ToString().ToLower().Contains("0.3"))) == false)
                    {
                        ComboBoxList.ElementAt(6).SelectedItem = structureId;
                    }


                    if (structureId.ToString().ToLower().Contains("blad") & (structureId.ToString().ToLower().Contains("-")
                        & (structureId.ToString().ToLower().Contains("ptv") || structureId.ToString().ToLower().Contains("0.3"))) == false)
                    {
                        ComboBoxList.ElementAt(1).SelectedItem = structureId;
                    }

                    if (structureId.ToString().ToLower().Contains("rect") & (structureId.ToString().ToLower().Contains("-")
                        & (structureId.ToString().ToLower().Contains("ptv") || structureId.ToString().ToLower().Contains("0.3"))) == false)
                    {
                        ComboBoxList.ElementAt(2).SelectedItem = structureId;
                    }

                    if (structureId.ToString().ToLower().Contains("small") & structureId.ToString().ToLower().Contains("bowel")
                        & (structureId.ToString().ToLower().Contains("ptv") == false) & (structureId.ToString().ToLower().Contains("sig") == false)
                        || structureId.ToString().ToLower().Contains("bowel")
                        & (structureId.ToString().ToLower().Contains("ptv") == false)
                        & (structureId.ToString().ToLower().Contains("sig") == false))
                    {
                        ComboBoxList.ElementAt(4).SelectedItem = structureId;
                    }

                    if (structureId.ToString().ToLower().Contains("sig") & (structureId.ToString().ToLower().Contains("-")
                        & (structureId.ToString().ToLower().Contains("ptv") || structureId.ToString().ToLower().Contains("0.3"))) == false)
                    {
                        ComboBoxList.ElementAt(3).SelectedItem = structureId;
                    }

                }

            }


        }
        public void FillPLanTextBoxes(PlanSetup planSetup, double planNumber)
        {

            if (planNumber == 1)
            {
                Plan1TextBox.Text = planSetup.Id;
            }
            if (planNumber == 2)
            {
                Plan2TextBox.Text = planSetup.Id;
            }
            if (planNumber == 3)
            {
                Plan3TextBox.Text = planSetup.Id;
            }
            if (planNumber == 4)
            {
                Plan4TextBox.Text = planSetup.Id;
            }
            if (planNumber == 5)
            {
                Plan5TextBox.Text = planSetup.Id;
            }

            if (Plan1TextBox.Text != "")
            {
                Plan1CheckBox.IsChecked = true;
            }
            if (Plan2TextBox.Text != "")
            {
                Plan2CheckBox.IsChecked = true;
            }
            if (Plan3TextBox.Text != "")
            {
                Plan3CheckBox.IsChecked = true;
            }
            if (Plan4TextBox.Text != "")
            {
                Plan4CheckBox.IsChecked = true;
            }
            if (Plan5TextBox.Text != "")
            {
                Plan5CheckBox.IsChecked = true;
            }

        }

        public void FillAlphaBetaTextBoxes()
        {
            BladderAlphaBetaTextBox.Text = "3";
            BowelAlphaBetaTextBox.Text = "3";
            RectumAlphaBetaTextBox.Text = "3";
            SigmoidAlphaBetaTextBox.Text = "3";
            HRCTVAlphaBetaTextBox.Text = "10";
        }


        private bool TriggerOneFractionDisplay(PlanSetup planSetup)
        {

            bool oneFxDisplay;
            var numberOfEclipseFractions = planSetup.NumberOfFractions;


            TextBox FxTextBox = returnCurrentFractionTextBox(planSetup);


            int UIPlanFxs = int.Parse(FxTextBox.Text);

            if (UIPlanFxs != numberOfEclipseFractions & numberOfEclipseFractions == 1)
            {
                oneFxDisplay = true;
            }
            else if (UIPlanFxs != numberOfEclipseFractions)
            {
                MessageBox.Show(String.Format("Warning** \n The fractions entered for plan {0} do not match the fractions in Eclipse." +
                    "\n Please verify the fractions are correct.", planSetup.Id));
                oneFxDisplay = false;
            }
            else
            {
                oneFxDisplay = false;
            }

            return oneFxDisplay;
        }

        private List<Tuple<Structure, string, DoseValue, string>> GetDVHStatistics(ScriptContext context1, PlanSetup planSetup, double planNumber)
        {

            //we are going to export DVH metrics
            Patient patient = context1.Patient;






            //define the structures


            var structures = planSetup.StructureSet.Structures.ToList();
            List<Structure> BladList = new List<Structure>();
            List<Structure> RectList = new List<Structure>();
            List<Structure> SigList = new List<Structure>();
            List<Structure> BowelList = new List<Structure>();
            List<Structure> CTVList = new List<Structure>();
            List<Structure> IRCTVList = new List<Structure>();
            List<Structure> GTVList = new List<Structure>();

            List<Structure> BodyList = new List<Structure>();


            List<ComboBox> ComboBoxList = new List<ComboBox>();
            List<ComboBox> ComboBoxListFirst = new List<ComboBox>
            {
                ComboBoxCTV,
                ComboBoxBladder,
                ComboBoxRectum,
                ComboBoxSigmoid,
                ComboBoxSmallBowel,
                ComboBoxIRCTV,
                ComboBoxGTV
            };
            List<ComboBox> ComboBoxListSecond = new List<ComboBox>
            {
                ComboBoxCTV1,
                ComboBoxBladder1,
                ComboBoxRectum1,
                ComboBoxSigmoid1,
                ComboBoxSmallBowel1,
                ComboBoxIRCTV1,
                ComboBoxGTV1
            };
            List<ComboBox> ComboBoxListThird = new List<ComboBox>
            {
                ComboBoxCTV2,
                ComboBoxBladder2,
                ComboBoxRectum2,
                ComboBoxSigmoid2,
                ComboBoxSmallBowel2,
                ComboBoxIRCTV2,
                ComboBoxGTV2
            };
            List<ComboBox> ComboBoxListFourth = new List<ComboBox>
            {
                ComboBoxCTV3,
                ComboBoxBladder3,
                ComboBoxRectum3,
                ComboBoxSigmoid3,
                ComboBoxSmallBowel3,
                 ComboBoxIRCTV3,
                ComboBoxGTV3
            };
            List<ComboBox> ComboBoxListFifth = new List<ComboBox>
            {
                ComboBoxCTV4,
                ComboBoxBladder4,
                ComboBoxRectum4,
                ComboBoxSigmoid4,
                ComboBoxSmallBowel4,
                ComboBoxIRCTV4,
                ComboBoxGTV4
            };


            if (planNumber == 1)
            {
                ComboBoxList.AddRange(ComboBoxListFirst);
            }
            if (planNumber == 2)
            {
                ComboBoxList.AddRange(ComboBoxListSecond);
            }
            if (planNumber == 3)
            {
                ComboBoxList.AddRange(ComboBoxListThird);
            }
            if (planNumber == 4)
            {
                ComboBoxList.AddRange(ComboBoxListFourth);
            }
            if (planNumber == 5)
            {
                ComboBoxList.AddRange(ComboBoxListFifth);
            }


            string structInfoMessage = "";

            ComboBox ComboBoxCTVLocal = ComboBoxList.ElementAt(0);
            ComboBox ComboBoxBladderLocal = ComboBoxList.ElementAt(1);
            ComboBox ComboBoxRectumLocal = ComboBoxList.ElementAt(2);
            ComboBox ComboBoxSigmoidLocal = ComboBoxList.ElementAt(3);
            ComboBox ComboBoxSmallBowelLocal = ComboBoxList.ElementAt(4);
            ComboBox ComboBoxIRCTVLocal = ComboBoxList.ElementAt(5);
            ComboBox ComboBoxGTVLocal = ComboBoxList.ElementAt(6);





            try
            {
                BladList.Add(structures.Where(c => c.Id == (string)ComboBoxBladderLocal.SelectedItem).First());
            }
            catch (Exception)
            {
                structInfoMessage += String.Format("No bladder statistics exported for plan {0}.\n", planSetup.Id);

            }

            try
            {
                RectList.Add(structures.Where(c => c.Id == (string)ComboBoxRectumLocal.SelectedItem).First());
            }
            catch (Exception)
            {
                structInfoMessage += String.Format("No rectum statistics exported for plan {0}\n", planSetup.Id );

            }

            try
            {
                SigList.Add(structures.Where(c => c.Id == (string)ComboBoxSigmoidLocal.SelectedItem).First());
            }
            catch (Exception)
            {
                structInfoMessage += String.Format("No sigmoid statistics exported  for plan {0}.\n", planSetup.Id);

            }

            try
            {
                BowelList.Add(structures.Where(c => c.Id == (string)ComboBoxSmallBowelLocal.SelectedItem).First());
            }
            catch (Exception)
            {
                structInfoMessage += String.Format("No bowel statistics exported  for plan {0}.\n", planSetup.Id );

            }

            try
            {
                CTVList.Add(structures.Where(c => c.Id == (string)ComboBoxCTVLocal.SelectedItem).First());
            }
            catch (Exception)
            {
                structInfoMessage += String.Format("No ctv statistics exported for plan {0}.\n", planSetup.Id);

            }

            try
            {
                IRCTVList.Add(structures.Where(c => c.Id == (string)ComboBoxIRCTVLocal.SelectedItem).First());
            }
            catch (Exception)
            {
                structInfoMessage += String.Format("No ir_ctv statistics exported  for plan {0}.\n", planSetup.Id);

            }

            try
            {
                GTVList.Add(structures.Where(c => c.Id == (string)ComboBoxGTVLocal.SelectedItem).First());
            }
            catch (Exception)
            {
                structInfoMessage += String.Format("No gtv statistics exported  for plan {0}.\n", planSetup.Id);

            }

            if (structInfoMessage != "")
            {
                MessageBox.Show(structInfoMessage);
            }




            List<Structure> AllStructuresFound = new List<Structure>();
            List<Tuple<Structure, string, DoseValue, string>> Tuples = new List<Tuple<Structure, string, DoseValue, string>>();




            DoseValue CTVD50 = default(DoseValue);
            DoseValue CTVD90 = default(DoseValue);
            DoseValue CTVD98 = default(DoseValue);
            double CTVVol = 0;
            if (CTVList.Any() == false)
            {
            }
            else
            {
                CTVD50 = planSetup.GetDoseAtVolume(CTVList.FirstOrDefault(), 50, VolumePresentation.Relative, DoseValuePresentation.Absolute);
                CTVD90 = planSetup.GetDoseAtVolume(CTVList.FirstOrDefault(), 90, VolumePresentation.Relative, DoseValuePresentation.Absolute);
                CTVD98 = planSetup.GetDoseAtVolume(CTVList.FirstOrDefault(), 98, VolumePresentation.Relative, DoseValuePresentation.Absolute);
                CTVVol = CTVList.FirstOrDefault().Volume;
                Structure ctv = CTVList.FirstOrDefault();
                Tuple<Structure, string, DoseValue, string> ctvpackage = new Tuple<Structure, string, DoseValue, string>(ctv, "D50%[cGy]", CTVD50, "hr_ctv");
                Tuple<Structure, string, DoseValue, string> ctvpackage1 = new Tuple<Structure, string, DoseValue, string>(ctv, "D90%[cGy]", CTVD90, "hr_ctv");
                Tuple<Structure, string, DoseValue, string> ctvpackage2 = new Tuple<Structure, string, DoseValue, string>(ctv, "D98%[cGy]", CTVD98, "hr_ctv");
                Tuples.Add(ctvpackage);
                Tuples.Add(ctvpackage1);
                Tuples.Add(ctvpackage2);

            }

            DoseValue IRCTVD50 = default(DoseValue);
            DoseValue IRCTVD90 = default(DoseValue);
            DoseValue IRCTVD98 = default(DoseValue);
            double IRCTVVol = 0;
            if (IRCTVList.Any() == false)
            {
            }
            else
            {
                IRCTVD50 = planSetup.GetDoseAtVolume(IRCTVList.FirstOrDefault(), 50, VolumePresentation.Relative, DoseValuePresentation.Absolute);
                IRCTVD90 = planSetup.GetDoseAtVolume(IRCTVList.FirstOrDefault(), 90, VolumePresentation.Relative, DoseValuePresentation.Absolute);
                IRCTVD98 = planSetup.GetDoseAtVolume(IRCTVList.FirstOrDefault(), 98, VolumePresentation.Relative, DoseValuePresentation.Absolute);
                IRCTVVol = IRCTVList.FirstOrDefault().Volume;
                Structure irctv = IRCTVList.FirstOrDefault();
                Tuple<Structure, string, DoseValue, string> irctvpackage = new Tuple<Structure, string, DoseValue, string>(irctv, "D50%[cGy] IRCTV", IRCTVD50, "ir_ctv");
                Tuple<Structure, string, DoseValue, string> irctvpackage1 = new Tuple<Structure, string, DoseValue, string>(irctv, "D90%[cGy] IRCTV", IRCTVD90, "ir_ctv");
                Tuple<Structure, string, DoseValue, string> irctvpackage2 = new Tuple<Structure, string, DoseValue, string>(irctv, "D98%[cGy] IRCTV", IRCTVD98, "ir_ctv");
                Tuples.Add(irctvpackage);
                Tuples.Add(irctvpackage1);
                Tuples.Add(irctvpackage2);

            }

            DoseValue GTVD50 = default(DoseValue);
            DoseValue GTVD90 = default(DoseValue);
            DoseValue GTVD98 = default(DoseValue);
            double GTVVol = 0;
            if (GTVList.Any() == false)
            {
            }
            else
            {
                GTVD50 = planSetup.GetDoseAtVolume(GTVList.FirstOrDefault(), 50, VolumePresentation.Relative, DoseValuePresentation.Absolute);
                GTVD90 = planSetup.GetDoseAtVolume(GTVList.FirstOrDefault(), 90, VolumePresentation.Relative, DoseValuePresentation.Absolute);
                GTVD98 = planSetup.GetDoseAtVolume(GTVList.FirstOrDefault(), 98, VolumePresentation.Relative, DoseValuePresentation.Absolute);
                GTVVol = GTVList.FirstOrDefault().Volume;
                Structure gtv = GTVList.FirstOrDefault();
                Tuple<Structure, string, DoseValue, string> gtvpackage = new Tuple<Structure, string, DoseValue, string>(gtv, "D50%[cGy] GTV", GTVD50, "gtv");
                Tuple<Structure, string, DoseValue, string> gtvpackage1 = new Tuple<Structure, string, DoseValue, string>(gtv, "D90%[cGy] GTV", GTVD90, "gtv");
                Tuple<Structure, string, DoseValue, string> gtvpackage2 = new Tuple<Structure, string, DoseValue, string>(gtv, "D98%[cGy] GTV", GTVD98, "gtv");
                Tuples.Add(gtvpackage);
                Tuples.Add(gtvpackage1);
                Tuples.Add(gtvpackage2);

            }


            DoseValue BladPoint1cc = default(DoseValue);
            DoseValue Blad1cc = default(DoseValue);
            DoseValue Blad2cc = default(DoseValue);
            double BladVol = 0;
            if (BladList.Any() == false)
            {
            }
            else
            {
                BladPoint1cc = planSetup.GetDoseAtVolume(BladList.FirstOrDefault(), 0.1, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                Blad1cc = planSetup.GetDoseAtVolume(BladList.FirstOrDefault(), 1, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                Blad2cc = planSetup.GetDoseAtVolume(BladList.FirstOrDefault(), 2, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                BladVol = BladList.FirstOrDefault().Volume;
                Structure bladder = BladList.FirstOrDefault();
                Tuple<Structure, string, DoseValue, string> bladderpackage = new Tuple<Structure, string, DoseValue, string>(bladder, "D0.1cc[cGy]", BladPoint1cc, "bladder");
                Tuple<Structure, string, DoseValue, string> bladderpackage1 = new Tuple<Structure, string, DoseValue, string>(bladder, "D1cc[cGy]", Blad1cc, "bladder");
                Tuple<Structure, string, DoseValue, string> bladderpackage2 = new Tuple<Structure, string, DoseValue, string>(bladder, "D2cc[cGy]", Blad2cc, "bladder");

                Tuples.Add(bladderpackage);
                Tuples.Add(bladderpackage1);
                Tuples.Add(bladderpackage2);

            }


            DoseValue BowelPoint1cc = default(DoseValue);
            DoseValue Bowel1cc = default(DoseValue);
            DoseValue Bowel2cc = default(DoseValue);
            double BowelVol = 0;

            if (BowelList.Any() == false)
            {
            }
            else
            {
                BowelPoint1cc = planSetup.GetDoseAtVolume(BowelList.FirstOrDefault(), 0.1, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                Bowel1cc = planSetup.GetDoseAtVolume(BowelList.FirstOrDefault(), 1, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                Bowel2cc = planSetup.GetDoseAtVolume(BowelList.FirstOrDefault(), 2, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                BowelVol = BowelList.FirstOrDefault().Volume;
                Structure bowel = BowelList.FirstOrDefault();
                Tuple<Structure, string, DoseValue, string> bowelpackage = new Tuple<Structure, string, DoseValue, string>(bowel, "D0.1cc[cGy]", BowelPoint1cc, "bowel");
                Tuple<Structure, string, DoseValue, string> bowelpackage1 = new Tuple<Structure, string, DoseValue, string>(bowel, "D1cc[cGy]", Bowel1cc, "bowel");
                Tuple<Structure, string, DoseValue, string> bowelpackage2 = new Tuple<Structure, string, DoseValue, string>(bowel, "D2cc[cGy]", Bowel2cc, "bowel");
                Tuples.Add(bowelpackage);
                Tuples.Add(bowelpackage1);
                Tuples.Add(bowelpackage2);

            }

            DoseValue SigPoint1cc = default(DoseValue);
            DoseValue Sig1cc = default(DoseValue);
            DoseValue Sig2cc = default(DoseValue);
            double SigVol = 0;
            if (SigList.Any() == false)
            {
            }
            else
            {
                SigPoint1cc = planSetup.GetDoseAtVolume(SigList.FirstOrDefault(), 0.1, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                Sig1cc = planSetup.GetDoseAtVolume(SigList.FirstOrDefault(), 1, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                Sig2cc = planSetup.GetDoseAtVolume(SigList.FirstOrDefault(), 2, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                SigVol = SigList.FirstOrDefault().Volume;
                Structure sig = SigList.FirstOrDefault();
                Tuple<Structure, string, DoseValue, string> sigpackage = new Tuple<Structure, string, DoseValue, string>(sig, "D0.1cc[cGy]", SigPoint1cc, "sigmoid");
                Tuple<Structure, string, DoseValue, string> sigpackage1 = new Tuple<Structure, string, DoseValue, string>(sig, "D1cc[cGy]", Sig1cc, "sigmoid");
                Tuple<Structure, string, DoseValue, string> sigpackage2 = new Tuple<Structure, string, DoseValue, string>(sig, "D2cc[cGy]", Sig2cc, "sigmoid");
                Tuples.Add(sigpackage);
                Tuples.Add(sigpackage1);
                Tuples.Add(sigpackage2);


            }

            DoseValue RectPoint1cc = default(DoseValue);
            DoseValue Rect1cc = default(DoseValue);
            DoseValue Rect2cc = default(DoseValue);
            double RectVol = 0;
            if (RectList.Any() == false)
            {
            }
            else
            {
                RectPoint1cc = planSetup.GetDoseAtVolume(RectList.FirstOrDefault(), 0.1, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                Rect1cc = planSetup.GetDoseAtVolume(RectList.FirstOrDefault(), 1, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);
                Rect2cc = planSetup.GetDoseAtVolume(RectList.FirstOrDefault(), 2, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute);

                RectVol = RectList.FirstOrDefault().Volume;
                Structure rectum = RectList.FirstOrDefault();
                Tuple<Structure, string, DoseValue, string> rectpackage = new Tuple<Structure, string, DoseValue, string>(rectum, "D0.1cc[cGy]", RectPoint1cc, "rectum");
                Tuple<Structure, string, DoseValue, string> rectpackage1 = new Tuple<Structure, string, DoseValue, string>(rectum, "D1cc[cGy]", Rect1cc, "rectum");
                Tuple<Structure, string, DoseValue, string> rectpackage2 = new Tuple<Structure, string, DoseValue, string>(rectum, "D2cc[cGy]", Rect2cc, "rectum");
                Tuples.Add(rectpackage);
                Tuples.Add(rectpackage1);
                Tuples.Add(rectpackage2);

            }



            return Tuples;


        }

        private List<Tuple<PlanSetup, Structure, string, double, string>> CalcEQD2Values(List<Tuple<Structure, string, DoseValue, string>> PlanTuples, PlanSetup planSetup)
        {
            //Additional bug fix for future. If plan fxs != the user plan fractions, and this is CORRECT. We need to scale the dvh

            IDictionary<string, double> alphaBetaDict = new Dictionary<string, double>();
            alphaBetaDict.Add("bladder", double.Parse(BladderAlphaBetaTextBox.Text));
            alphaBetaDict.Add("rectum", double.Parse(RectumAlphaBetaTextBox.Text));
            alphaBetaDict.Add("bowel", double.Parse(BowelAlphaBetaTextBox.Text));
            alphaBetaDict.Add("sigmoid", double.Parse(SigmoidAlphaBetaTextBox.Text));
            alphaBetaDict.Add("hr_ctv", double.Parse(HRCTVAlphaBetaTextBox.Text));
            alphaBetaDict.Add("ir_ctv", double.Parse(HRCTVAlphaBetaTextBox.Text));
            alphaBetaDict.Add("gtv", double.Parse(HRCTVAlphaBetaTextBox.Text));


            List<Tuple<PlanSetup, Structure, string, double, string>> EQD2Tuples = new List<Tuple<PlanSetup, Structure, string, double, string>>();


            bool oneFxDisplay = TriggerOneFractionDisplay(planSetup);

            //always do 1 fraction?
            

            var UIPlanFxsBox = returnCurrentFractionTextBox(planSetup);
            double userPlanFxs;
            double.TryParse(UIPlanFxsBox.Text, out userPlanFxs);

            if (oneFxDisplay)
            {
                foreach (var tuple in PlanTuples)
                {
                    double alphaBetaValue = alphaBetaDict.FirstOrDefault(c => c.Key == tuple.Item4).Value;
                    double OrganDosePerFx = tuple.Item3.Dose / 100;
                    double OrganEQD2Fractional = OrganDosePerFx * (1 + OrganDosePerFx / alphaBetaValue) / (1 + 2 / alphaBetaValue); 
                    double OrganEQD2Total = OrganEQD2Fractional * userPlanFxs;
                    Tuple<PlanSetup, Structure, string, double, string> EQD2Tuple = new Tuple<PlanSetup, Structure, string, double, string>(planSetup, tuple.Item1, tuple.Item2, OrganEQD2Total, tuple.Item4);
                    EQD2Tuples.Add(EQD2Tuple);


                }
            }
            else
            {
                foreach (var tuple in PlanTuples)
                {
                    double alphaBetaValue = alphaBetaDict.FirstOrDefault(c => c.Key == tuple.Item4).Value;
                    double OrganDosePerFx = tuple.Item3.Dose / ((double)planSetup.NumberOfFractions * 100);
                    double OrganEQD2Fractional = OrganDosePerFx * (1 + OrganDosePerFx / alphaBetaValue) / (1 + 2 / alphaBetaValue); 
                    double OrganEQD2Total = OrganEQD2Fractional * (double)userPlanFxs;
                    Tuple<PlanSetup, Structure, string, double, string> EQD2Tuple = new Tuple<PlanSetup, Structure, string, double, string>(planSetup, tuple.Item1, tuple.Item2, OrganEQD2Total, tuple.Item4);
                    EQD2Tuples.Add(EQD2Tuple);


                }
            }



            return EQD2Tuples;


        }

        /// <summary>
        /// Use this for external beam EQD2 calcs
        /// </summary>
        /// <param name="PlanTuples"></param>
        /// <param name="planSetup"></param>
        /// <returns></returns>
        private List<Tuple<string, double, string>> CalcEQD2Values(List<Tuple<string, DoseValue, string>> PlanTuples, double EBRTFx)
        {

            IDictionary<string, double> alphaBetaDict = new Dictionary<string, double>();
            alphaBetaDict.Add("bladder", double.Parse(BladderAlphaBetaTextBox.Text));
            alphaBetaDict.Add("rectum", double.Parse(RectumAlphaBetaTextBox.Text));
            alphaBetaDict.Add("bowel", double.Parse(BowelAlphaBetaTextBox.Text));
            alphaBetaDict.Add("sigmoid", double.Parse(SigmoidAlphaBetaTextBox.Text));
            alphaBetaDict.Add("hr_ctv", double.Parse(HRCTVAlphaBetaTextBox.Text));
            alphaBetaDict.Add("ir_ctv", double.Parse(HRCTVAlphaBetaTextBox.Text));
            alphaBetaDict.Add("gtv", double.Parse(HRCTVAlphaBetaTextBox.Text));


            List<Tuple<string, double, string>> EQD2Tuples = new List<Tuple<string, double, string>>();



            foreach (var tuple in PlanTuples)
            {
                double alphaBetaValue = alphaBetaDict.FirstOrDefault(c => c.Key == tuple.Item3).Value;
                double OrganDosePerFx = tuple.Item2.Dose / (EBRTFx);

                var step1 = 1 + OrganDosePerFx / alphaBetaValue;
                double step2 = 1 + (double)2 / (double)alphaBetaValue;
                double OrganEQD2Fractional = OrganDosePerFx * (step1 / step2);
                double OrganEQD2Total = OrganEQD2Fractional * EBRTFx;
                Tuple<string, double, string> EQD2Tuple = new Tuple<string, double, string>(tuple.Item1, OrganEQD2Total, tuple.Item3);
                EQD2Tuples.Add(EQD2Tuple);


            }

            return EQD2Tuples;


        }


        private List<Tuple<string, DoseValue, string>> GetExternalDosesAsTuples(DoseValue EBRTDose, double EBRTFx)
        {

            List<Tuple<string, DoseValue, string>> Tuples = new List<Tuple<string, DoseValue, string>>();


            Tuple<string, DoseValue, string> ctvpackage = new Tuple<string, DoseValue, string>("D50%[cGy]", EBRTDose, "hr_ctv");
            Tuple<string, DoseValue, string> ctvpackage1 = new Tuple<string, DoseValue, string>("D90%[cGy]", EBRTDose, "hr_ctv");
            Tuple<string, DoseValue, string> ctvpackage2 = new Tuple<string, DoseValue, string>("D98%[cGy]", EBRTDose, "hr_ctv");
            Tuples.Add(ctvpackage);
            Tuples.Add(ctvpackage1);
            Tuples.Add(ctvpackage2);

            Tuple<string, DoseValue, string> irctvpackage = new Tuple< string, DoseValue, string>("D50%[cGy] IRCTV", EBRTDose, "ir_ctv");
            Tuple<string, DoseValue, string> irctvpackage1 = new Tuple<string, DoseValue, string>("D90%[cGy] IRCTV", EBRTDose, "ir_ctv");
            Tuple<string, DoseValue, string> irctvpackage2 = new Tuple<string, DoseValue, string>("D98%[cGy] IRCTV", EBRTDose, "ir_ctv");
            Tuples.Add(irctvpackage);
            Tuples.Add(irctvpackage1);
            Tuples.Add(irctvpackage2);

            Tuple<string, DoseValue, string> gtvpackage = new Tuple<string, DoseValue, string>("D50%[cGy] GTV", EBRTDose, "gtv");
            Tuple<string, DoseValue, string> gtvpackage1 = new Tuple<string, DoseValue, string>("D90%[cGy] GTV", EBRTDose, "gtv");
            Tuple<string, DoseValue, string> gtvpackage2 = new Tuple<string, DoseValue, string>("D98%[cGy] GTV", EBRTDose, "gtv");
            Tuples.Add(gtvpackage);
            Tuples.Add(gtvpackage1);
            Tuples.Add(gtvpackage2);



            Tuple<string, DoseValue, string> bladderpackage = new Tuple<string, DoseValue, string>("D0.1cc[cGy]", EBRTDose, "bladder");
            Tuple<string, DoseValue, string> bladderpackage1 = new Tuple<string, DoseValue, string>("D1cc[cGy]", EBRTDose, "bladder");
            Tuple<string, DoseValue, string> bladderpackage2 = new Tuple<string, DoseValue, string>("D2cc[cGy]", EBRTDose, "bladder");
            Tuples.Add(bladderpackage);
            Tuples.Add(bladderpackage1);
            Tuples.Add(bladderpackage2);

            Tuple<string, DoseValue, string> bowelpackage = new Tuple<string, DoseValue, string>("D0.1cc[cGy]", EBRTDose, "bowel");
            Tuple<string, DoseValue, string> bowelpackage1 = new Tuple<string, DoseValue, string>("D1cc[cGy]", EBRTDose, "bowel");
            Tuple<string, DoseValue, string> bowelpackage2 = new Tuple<string, DoseValue, string>("D2cc[cGy]", EBRTDose, "bowel");
            Tuples.Add(bowelpackage);
            Tuples.Add(bowelpackage1);
            Tuples.Add(bowelpackage2);

            Tuple<string, DoseValue, string> sigmoidpackage = new Tuple<string, DoseValue, string>("D0.1cc[cGy]", EBRTDose, "sigmoid");
            Tuple<string, DoseValue, string> sigmoidpackage1 = new Tuple<string, DoseValue, string>("D1cc[cGy]", EBRTDose, "sigmoid");
            Tuple<string, DoseValue, string> sigmoidpackage2 = new Tuple<string, DoseValue, string>("D2cc[cGy]", EBRTDose, "sigmoid");
            Tuples.Add(sigmoidpackage);
            Tuples.Add(sigmoidpackage1);
            Tuples.Add(sigmoidpackage2);

            Tuple<string, DoseValue, string> rectumpackage = new Tuple<string, DoseValue, string>("D0.1cc[cGy]", EBRTDose, "rectum");
            Tuple<string, DoseValue, string> rectumpackage1 = new Tuple<string, DoseValue, string>("D1cc[cGy]", EBRTDose, "rectum");
            Tuple<string, DoseValue, string> rectumpackage2 = new Tuple<string, DoseValue, string>("D2cc[cGy]", EBRTDose, "rectum");
            Tuples.Add(rectumpackage);
            Tuples.Add(rectumpackage1);
            Tuples.Add(rectumpackage2);


            return Tuples;


        }

        private CheckBox returnCurrentCheckBox(PlanSetup planSetup)
        {
            List<TextBox> UITextBoxes = new List<TextBox>
            {
                Plan1TextBox,
                Plan2TextBox,
                Plan3TextBox,
                Plan4TextBox,
                Plan5TextBox

            };

            Dictionary<TextBox, CheckBox> TextWithCheckBoxDict = new Dictionary<TextBox, CheckBox> { };
            TextWithCheckBoxDict.Add(Plan1TextBox, Plan1CheckBox);
            TextWithCheckBoxDict.Add(Plan2TextBox, Plan2CheckBox);
            TextWithCheckBoxDict.Add(Plan3TextBox, Plan3CheckBox);
            TextWithCheckBoxDict.Add(Plan4TextBox, Plan4CheckBox);
            TextWithCheckBoxDict.Add(Plan5TextBox, Plan5CheckBox);


            CheckBox currentCheckBox;
            TextWithCheckBoxDict.TryGetValue(TextWithCheckBoxDict.Keys.FirstOrDefault(c => c.Text == planSetup.Id), out currentCheckBox);

            return currentCheckBox;

        }

        private TextBox returnCurrentFractionTextBox(PlanSetup planSetup)
        {
            List<TextBox> UITextBoxes = new List<TextBox>
            {
                Plan1TextBox,
                Plan2TextBox,
                Plan3TextBox,
                Plan4TextBox,
                Plan5TextBox

            };

            Dictionary<TextBox, TextBox> TextWithCheckBoxDict = new Dictionary<TextBox, TextBox> { };
            TextWithCheckBoxDict.Add(Plan1TextBox, Plan1FxsTextBox);
            TextWithCheckBoxDict.Add(Plan2TextBox, Plan2FxsTextBox);
            TextWithCheckBoxDict.Add(Plan3TextBox, Plan3FxsTextBox);
            TextWithCheckBoxDict.Add(Plan4TextBox, Plan4FxsTextBox);
            TextWithCheckBoxDict.Add(Plan5TextBox, Plan5FxsTextBox);


            TextBox currentFxTextBox;
            TextWithCheckBoxDict.TryGetValue(TextWithCheckBoxDict.Keys.FirstOrDefault(c => c.Text == planSetup.Id), out currentFxTextBox);

            return currentFxTextBox;

        }

        private void FillCurrentFractionTextBox(PlanSetup planSetup, TextBox fxTextBox)
        {
            var EclipseFractions = planSetup.NumberOfFractions;

            fxTextBox.Text = EclipseFractions.ToString();

        }



        private void CalculateStatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            var PlansInScope = context1.PlansInScope;
            double NumberOfPLans = PlansInScope.Count();


            List<Tuple<string, double, string>> AllBrachyTuples = new List<Tuple<string, double, string>>();



            for (int i = 1; i < NumberOfPLans + 1; i++)
            {
                var currentPlan = PlansInScope.ElementAt(i - 1);
                CheckBox currentCheckBox = returnCurrentCheckBox(currentPlan);
                TextBox currentFxBox = returnCurrentFractionTextBox(currentPlan);

                if (currentFxBox.Text == "" & currentCheckBox.IsChecked == true)
                {
                    MessageBox.Show("Fxs input is empty for plan " + currentPlan.Id + ".\nThis plan will not be included " +
                        "in the EQD2 values shown in the report below.");
                }


                if (currentPlan != null & currentCheckBox.IsChecked == true & currentFxBox.Text != "")
                {
                    var PlanTuples = GetDVHStatistics(context1, PlansInScope.ElementAt(i - 1), i);


                    var EQD2Tuples = CalcEQD2Values(PlanTuples, PlansInScope.ElementAt(i - 1));



                    foreach (var tuple in EQD2Tuples)
                    {
                        string objective = tuple.Item3;
                        var dose = tuple.Item4;
                        string name = tuple.Item5;


                        Tuple<string, double, string> filteredTuple = new Tuple<string, double, string>(objective, dose, name);

                        AllBrachyTuples.Add(filteredTuple);


                    }


                }




            }




            int ExternalPlanCount = 0;
            //need to check for empty textboxes
            if (ExternalDose1.Text == "" || ExternalFx1.Text == "")
            {
                if (ExternalDose1.Text == "" & ExternalFx1.Text == "")
                {
                    MessageBox.Show("No External Beam Dose or fractions found.\n External Beam Dose will not be added to final EQD2Calc");
                }
                else if (ExternalDose1.Text == "")
                {
                    MessageBox.Show("External Beam Dose is Missing. External Beam Dose will not be included in the calculation.");
                }
                else
                {
                    MessageBox.Show("External Beam Fxs are Missing. External Beam Dose will not be included in the calculation.");
                }
                
            }
            else
            {
                ExternalPlanCount += 1;
            }

            if (ExternalDose2.Text != "" & ExternalFx2.Text != "")
            {
                ExternalPlanCount += 1;
            }
            if (ExternalDose3.Text != "" & ExternalFx3.Text != "")
            {
                ExternalPlanCount += 1;
            }

            List<List<Tuple<string, double, string>>> HolderForFinals = new List<List<Tuple<string, double, string>>>();

            List<double> ExternalEQD2 = new List<double>();

            List<double> RemoveLaterList = new List<double> ();

            if (ExternalPlanCount == 0)
            {
                List<Tuple<string, double, string>> AllTuples = new List<Tuple<string, double, string>>();
                AllTuples.AddRange(AllBrachyTuples);

                var FinalEQD2Tuples = AddModalityTuples(AllTuples);
                HolderForFinals.Add(FinalEQD2Tuples);

              

                
            }

            if (ExternalPlanCount == 1)
            {
                List<Tuple<string, double, string>> AllTuples = new List<Tuple<string, double, string>>();
                AllTuples.AddRange(AllBrachyTuples);

                if (ExternalDose1.Text != "" & ExternalFx1.Text != "")
                {
                    DoseValue EBRTDose1 = new DoseValue(double.Parse(ExternalDose1.Text), DoseValue.DoseUnit.Gy);
                    double EBRTFx1 = double.Parse(ExternalFx1.Text);

                  

                    var EBRTDoseTuples1 = GetExternalDosesAsTuples(EBRTDose1, EBRTFx1);

                    var EBRTEQD21 = CalcEQD2Values(EBRTDoseTuples1, EBRTFx1);


                    List<double> ValList1 = new List<double>();
                    foreach (var item in EBRTEQD21)
                    {
                        var doseVal = item.Item2;

                        ValList1.Add(doseVal);

                    }

                    var DistinctVals = ValList1.Distinct();

                    
                    RemoveLaterList.AddRange(DistinctVals);


                    AllTuples.AddRange(EBRTEQD21);


                }
                else
                {
                    if (ExternalDose1.Text == "")
                    {
                        MessageBox.Show("External Beam Dose is Missing");
                    }
                    if ( ExternalFx1.Text == "")
                    {
                        MessageBox.Show("External Beam Fx is Missing");
                    }
                }


                var FinalEQD2Tuples = AddModalityTuples(AllTuples);
                HolderForFinals.Add(FinalEQD2Tuples);

            }

            if (ExternalPlanCount == 2)
            {
                List<Tuple<string, double, string>> AllTuples = new List<Tuple<string, double, string>>();
                AllTuples.AddRange(AllBrachyTuples);

                List<TextBox> TextBoxList = new List<TextBox> { ExternalDose1, ExternalDose2, ExternalFx1, ExternalFx2 };

                if (ExternalDose1.Text != "" & ExternalFx1.Text != "" & ExternalDose2.Text != "" & ExternalFx2.Text != "")
                {
                    DoseValue EBRTDose1 = new DoseValue(double.Parse(ExternalDose1.Text), DoseValue.DoseUnit.Gy);
                    double EBRTFx1 = double.Parse(ExternalFx1.Text);

                    DoseValue EBRTDose2 = new DoseValue(double.Parse(ExternalDose2.Text), DoseValue.DoseUnit.Gy);
                    double EBRTFx2 = double.Parse(ExternalFx2.Text);

                    var EBRTDoseTuples1 = GetExternalDosesAsTuples(EBRTDose1, EBRTFx1);
                    var EBRTEQD21 = CalcEQD2Values(EBRTDoseTuples1, EBRTFx1);

                    var EBRTDoseTuples2 = GetExternalDosesAsTuples(EBRTDose2, EBRTFx2);
                    var EBRTEQD22 = CalcEQD2Values(EBRTDoseTuples2, EBRTFx2);

                    //What we need to do is create a new Tuple List which contain the sum of the Items 2 for EBRTEQD21, EBRTEQD22, EBRTEQD23
                    //oh yea we have an existing method for this

                    List<Tuple<string, double, string>> ConcattedList = new List<Tuple<string, double, string>>(EBRTEQD21.Concat(EBRTEQD22));
                    var summedExternalEQD2 = AddModalityTuples(ConcattedList);
                    

                    List<double> ValList1 = new List<double>();
                    foreach (var item in summedExternalEQD2)
                    {
                        var doseVal = item.Item2;

                        ValList1.Add(doseVal);

                    }

                    
                    var DistinctVals = ValList1.Distinct();


                    //ExternalEQD2.Add(fullBrachyEQD2);
                    RemoveLaterList.AddRange(DistinctVals);

                    
                    AllTuples.AddRange(EBRTEQD21);
                    AllTuples.AddRange(EBRTEQD22);
                }
                else
                {
                    TextBoxList.Where(c => c.Text == "").ToList();
                    foreach (var thing in TextBoxList)
                    {
                        MessageBox.Show(thing.Name + " box is missing a value");
                    }

                }


                var FinalEQD2Tuples = AddModalityTuples(AllTuples);
                HolderForFinals.Add(FinalEQD2Tuples);

            }

            if (ExternalPlanCount == 3)
            {
                List<Tuple<string, double, string>> AllTuples = new List<Tuple<string, double, string>>();
                AllTuples.AddRange(AllBrachyTuples);

                List<TextBox> TextBoxList = new List<TextBox> { ExternalDose1, ExternalDose2, ExternalFx1, ExternalFx2, ExternalDose3, ExternalFx3 };

                if (ExternalDose1.Text != "" & ExternalFx1.Text != "" & ExternalDose2.Text != "" & ExternalFx2.Text != "" & ExternalDose3.Text != "" & ExternalFx3.Text != "")
                {
                    DoseValue EBRTDose1 = new DoseValue(double.Parse(ExternalDose1.Text), DoseValue.DoseUnit.Gy);
                    double EBRTFx1 = double.Parse(ExternalFx1.Text);

                    DoseValue EBRTDose2 = new DoseValue(double.Parse(ExternalDose2.Text), DoseValue.DoseUnit.Gy);
                    double EBRTFx2 = double.Parse(ExternalFx2.Text);

                    DoseValue EBRTDose3 = new DoseValue(double.Parse(ExternalDose3.Text), DoseValue.DoseUnit.Gy);
                    double EBRTFx3 = double.Parse(ExternalFx3.Text);

                    var EBRTDoseTuples1 = GetExternalDosesAsTuples(EBRTDose1, EBRTFx1);
                    var EBRTEQD21 = CalcEQD2Values(EBRTDoseTuples1, EBRTFx1);

                    var EBRTDoseTuples2 = GetExternalDosesAsTuples(EBRTDose2, EBRTFx2);
                    var EBRTEQD22 = CalcEQD2Values(EBRTDoseTuples2, EBRTFx2);

                    var EBRTDoseTuples3 = GetExternalDosesAsTuples(EBRTDose3, EBRTFx3);
                    var EBRTEQD23 = CalcEQD2Values(EBRTDoseTuples3, EBRTFx3);



                    //What we need to do is create a new Tuple List which contain the sum of the Items 2 for EBRTEQD21, EBRTEQD22, EBRTEQD23
                    //oh yea we have an existing method for this



                    List<Tuple<string, double, string>> PreConcat = new List<Tuple<string, double, string>>(EBRTEQD21.Concat(EBRTEQD22));
                    List<Tuple<string, double, string>> ConcattedList = new List<Tuple<string, double, string>>(EBRTEQD23.Concat(PreConcat));
                    var summedExternalEQD2 = AddModalityTuples(ConcattedList);

                    List<double> ValList1 = new List<double>();
                    foreach (var item in summedExternalEQD2)
                    {
                        var doseVal = item.Item2;

                        ValList1.Add(doseVal);

                    }


                    var DistinctVals = ValList1.Distinct();



                    RemoveLaterList.AddRange(DistinctVals);


                    AllTuples.AddRange(EBRTEQD21);
                    AllTuples.AddRange(EBRTEQD22);
                    AllTuples.AddRange(EBRTEQD23);

                }
                else
                {
                    TextBoxList.Where(c => c.Text == "").ToList();
                    foreach (var thing in TextBoxList)
                    {
                        MessageBox.Show(thing.Name + " box is missing a value");
                    }

                }


                var FinalEQD2Tuples = AddModalityTuples(AllTuples);
                HolderForFinals.Add(FinalEQD2Tuples);


            }

            //remove any tuples that are equal to EBRT EQD2
            var FinalEQD2TuplesOuter = HolderForFinals.FirstOrDefault();
            List<Tuple<string, double, string>> FinalEQD2TuplesOuter1 = new List<Tuple<string, double, string>>();


            //fix
            List<Tuple<string, double, string>> RemoveList = new List<Tuple<string, double, string>>();
            foreach (var tupleItem in FinalEQD2TuplesOuter)
            {
                if (RemoveLaterList.Contains(tupleItem.Item2))
                {
                    RemoveList.Add(tupleItem);
                }
            }


            foreach (var thing in RemoveList)
            {
                FinalEQD2TuplesOuter.Remove(thing);
            }


            Dictionary<string, string> metricDict = new Dictionary<string, string>();
            metricDict.Add("D98%[cGy] GTV", "D98%[cGy]");
            metricDict.Add("D90%[cGy] GTV", "D90%[cGy]");
            metricDict.Add("D50%[cGy] GTV", "D50%[cGy]");
            metricDict.Add("D98%[cGy] IRCTV", "D98%[cGy]");
            metricDict.Add("D90%[cGy] IRCTV", "D90%[cGy]");
            metricDict.Add("D50%[cGy] IRCTV", "D50%[cGy]");


            foreach (var thing in FinalEQD2TuplesOuter)
            {

                var newItem1 = thing.Item1;
                var newItem3 = thing.Item3;


                double newItem2 = Math.Round(thing.Item2, 2);

                if (metricDict.Keys.Contains(thing.Item1))
                {
                    newItem1 = metricDict[thing.Item1];
                }

                Tuple<string, double, string> newTuple = new Tuple<string, double, string>( newItem1, newItem2, newItem3);

                FinalEQD2TuplesOuter1.Add(newTuple);


            }


            FinalEQD2TuplesOuter1.Reverse();


            this.ReportDataGrid.ItemsSource = FinalEQD2TuplesOuter1;


            ReportDataGrid.RowHeight = ReportDataGrid.ActualHeight / (FinalEQD2TuplesOuter1.Count + 1);


            //ReportDataGrid.SetBinding(DataGrid.WidthProperty, new Binding("Width") { Source = window1 });
            ReportDataGrid.SetBinding(DataGrid.HeightProperty, new Binding("Height") { Source = ReportDataGrid.RowHeight });
            ReportDataGrid.AutoGenerateColumns = true;

            

            ReportDataGrid.HeadersVisibility = DataGridHeadersVisibility.Column;
            ReportDataGrid.Columns[2].Header = "Structure";
            ReportDataGrid.Columns[0].Header = "Objective";
            ReportDataGrid.Columns[1].Header = "EQD2 Dose [Gy]";



            ReportDataGrid.Columns[2].DisplayIndex = 0;
            ReportDataGrid.Columns[0].DisplayIndex = 1;
            ReportDataGrid.Columns[1].DisplayIndex = 2;


            //ReportDataGrid.CellStyle = new Style(typeof(DataGridCell))
            //{
            //    Setters =
            //    {
            //        new Setter(DataGridCell.BackgroundProperty, System.Windows.Media.Brushes.Red)
            //    }
            //};



            this.ReportDataGrid.Items.Refresh();


            if (refreshCount == 0)
            {
                ColorToggleButton.IsChecked = true;
                refreshCount++;
            }


        }

        


        private List<Tuple<string, double, string>> AddModalityTuples(List<Tuple<string, double, string>> AllTuples)
        {
            var bladderTups = AllTuples.Where(c => c.Item3 == "bladder").ToList();
            List<Tuple<string,double,string>> bladderpoint1cc = bladderTups.Where(c => c.Item1 == "D0.1cc[cGy]").ToList();
            var bladder1cc = bladderTups.Where(c => c.Item1 == "D1cc[cGy]").ToList();
            var bladder2cc = bladderTups.Where(c => c.Item1 == "D2cc[cGy]").ToList();

            var rectumTups = AllTuples.Where(c => c.Item3 == "rectum").ToList();
            var rectumpoint1cc = rectumTups.Where(c => c.Item1 == "D0.1cc[cGy]").ToList();
            var rectum1cc = rectumTups.Where(c => c.Item1 == "D1cc[cGy]").ToList();
            var rectum2cc = rectumTups.Where(c => c.Item1 == "D2cc[cGy]").ToList();

            var sigmoidTups = AllTuples.Where(c => c.Item3 == "sigmoid").ToList();
            var sigmoidpoint1cc = sigmoidTups.Where(c => c.Item1 == "D0.1cc[cGy]").ToList();
            var sigmoid1cc = sigmoidTups.Where(c => c.Item1 == "D1cc[cGy]").ToList();
            var sigmoid2cc = sigmoidTups.Where(c => c.Item1 == "D2cc[cGy]").ToList();

            var bowelTups = AllTuples.Where(c => c.Item3 == "bowel").ToList();
            var bowelpoint1cc = bowelTups.Where(c => c.Item1 == "D0.1cc[cGy]").ToList();
            var bowel1cc = bowelTups.Where(c => c.Item1 == "D1cc[cGy]").ToList();
            var bowel2cc = bowelTups.Where(c => c.Item1 == "D2cc[cGy]").ToList();


            var hrctvTups = AllTuples.Where(c => c.Item3 == "hr_ctv").ToList();
            var hrctv50 = hrctvTups.Where(c => c.Item1 == "D50%[cGy]").ToList();
            var hrctv90 = hrctvTups.Where(c => c.Item1 == "D90%[cGy]").ToList();
            var hrctv98 = hrctvTups.Where(c => c.Item1 == "D98%[cGy]").ToList();

            var irctvTups = AllTuples.Where(c => c.Item3 == "ir_ctv").ToList();
            var irctv50 = irctvTups.Where(c => c.Item1 == "D50%[cGy] IRCTV").ToList();
            var irctv90 = irctvTups.Where(c => c.Item1 == "D90%[cGy] IRCTV").ToList();
            var irctv98 = irctvTups.Where(c => c.Item1 == "D98%[cGy] IRCTV").ToList();

            var gttvTups = AllTuples.Where(c => c.Item3 == "gtv").ToList();
            var gtv50 = gttvTups.Where(c => c.Item1 == "D50%[cGy] GTV").ToList();
            var gtv90 = gttvTups.Where(c => c.Item1 == "D90%[cGy] GTV").ToList();
            var gtv98 = gttvTups.Where(c => c.Item1 == "D98%[cGy] GTV").ToList();




            List<Tuple<string, double, string>> groupedTuples = new List<Tuple<string, double, string>>
            {
            };


            if (bladderpoint1cc.Any() == true)
            {
                var bladpoint1Totals = AddStructureValsTogether(bladderpoint1cc);
                groupedTuples.Add(bladpoint1Totals);
            }
            if (bladder1cc.Any() == true)
            {
                var blad1Totals = AddStructureValsTogether(bladder1cc);
                groupedTuples.Add(blad1Totals);
            }
            if (bladder2cc.Any() == true)
            {
                var blad2Totals = AddStructureValsTogether(bladder2cc);
                groupedTuples.Add(blad2Totals);
            }
            if (rectumpoint1cc.Any() == true)
            {
                var rectpoint1Totals = AddStructureValsTogether(rectumpoint1cc);
                groupedTuples.Add(rectpoint1Totals);
            }
            if (rectum1cc.Any() == true)
            {
                var rect1Totals = AddStructureValsTogether(rectum1cc);
                groupedTuples.Add(rect1Totals);
            }
            if (rectum2cc.Any() == true)
            {
                var rect2Totals = AddStructureValsTogether(rectum2cc);
                groupedTuples.Add(rect2Totals);
            }
            if (sigmoidpoint1cc.Any() == true)
            {
                var sigmoidpoint1Totals = AddStructureValsTogether(sigmoidpoint1cc);
                groupedTuples.Add(sigmoidpoint1Totals);
            }
            if (sigmoid1cc.Any() == true)
            {
                var sigmoid1Totals = AddStructureValsTogether(sigmoid1cc);
                groupedTuples.Add(sigmoid1Totals);
            }
            if (sigmoid2cc.Any() == true)
            {
                var sigmoid2Totals = AddStructureValsTogether(sigmoid2cc);
                groupedTuples.Add(sigmoid2Totals);
            }
            if (bowelpoint1cc.Any() == true)
            {
                var bowelpoint1Totals = AddStructureValsTogether(bowelpoint1cc);
                groupedTuples.Add(bowelpoint1Totals);
            }
            if (bowel1cc.Any() == true)
            {
                var bowel1Totals = AddStructureValsTogether(bowel1cc);
                groupedTuples.Add(bowel1Totals);
            }
            if (bowel2cc.Any() == true)
            {
                var bowel2Totals = AddStructureValsTogether(bowel2cc);
                groupedTuples.Add(bowel2Totals);
            }
            if (hrctv50.Any() == true)
            {
                var hrctv50Totals = AddStructureValsTogether(hrctv50);
                groupedTuples.Add(hrctv50Totals);
            }
            if (hrctv90.Any() == true)
            {
                var hrctv90Totals = AddStructureValsTogether(hrctv90);
                groupedTuples.Add(hrctv90Totals);
            }
            if (hrctv98.Any() == true)
            {
                var hrctv98Totals = AddStructureValsTogether(hrctv98);
                groupedTuples.Add(hrctv98Totals);
            }
            if (irctv50.Any() == true)
            {
                var irctv50Totals = AddStructureValsTogether(irctv50);
                groupedTuples.Add(irctv50Totals);
            }
            if (irctv90.Any() == true)
            {
                var irctv90Totals = AddStructureValsTogether(irctv90);
                groupedTuples.Add(irctv90Totals);
            }
            if (irctv98.Any() == true)
            {
                var irctv98Totals = AddStructureValsTogether(irctv98);
                groupedTuples.Add(irctv98Totals);
            }
            if (gtv50.Any() == true)
            {
                var gtv50Totals = AddStructureValsTogether(gtv50);
                groupedTuples.Add(gtv50Totals);

            }
            if (gtv90.Any() == true)
            {
                var gtv90Totals = AddStructureValsTogether(gtv90);
                groupedTuples.Add(gtv90Totals);
            }
            if (gtv98.Any() == true)
            {
                var gtv98Totals = AddStructureValsTogether(gtv98);
                groupedTuples.Add(gtv98Totals);

                
            }


            return groupedTuples;

        }
        private Tuple<string, double, string> AddStructureValsTogether(List<Tuple<string, double, string>> structureTuples)
        {

            List<Tuple<string, double, string>> returnList = new List<Tuple<string, double, string>>();

            if (structureTuples != null)
            {
                double EQD2Double = 0;
                foreach (var tuple in structureTuples)
                {
                    var currentDouble = tuple.Item2;
                    EQD2Double += currentDouble;
                }

                returnList.Add(new Tuple<string, double, string>(structureTuples.FirstOrDefault().Item1, EQD2Double, structureTuples.FirstOrDefault().Item3));


            }

            return returnList.FirstOrDefault();

        }

        private void ScreenshotButton_Click(object sender, RoutedEventArgs e)
        {

            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            //cmd.StandardInput.WriteLine("explorer ms-screenclip:");
            cmd.StandardInput.WriteLine("snippingtool.exe");


            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            Console.WriteLine(cmd.StandardOutput.ReadToEnd());


        }

        //old copy to clipboard methods below
        //public void CopyTableToClipboard(Dictionary<string,string> TableDict)
        //{
        //    // Create the data
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add();
        //    dt.Columns.Add();
        //    dt.Columns.Add();


            

        //    if (TableDict.TryGetValue("HRCTVD50", out string Val) & TableDict.TryGetValue("HRCTVD90", out string Val1) 
        //        & TableDict.TryGetValue("HRCTVD98", out string Val2))
        //    {
        //        dt.Rows.Add(Val, Val1, Val2);
        //    }

        //    if (TableDict.TryGetValue("Bladder0.1cc", out string BladVal) & TableDict.TryGetValue("Bladder1cc", out string BladVal1)
        //        & TableDict.TryGetValue("Bladder2cc", out string BladVal2))
        //    {
        //        dt.Rows.Add(BladVal, BladVal1, BladVal2);
        //    }

        //    if (TableDict.TryGetValue("Bowel0.1cc", out string BowelVal) & TableDict.TryGetValue("Bowel1cc", out string BowelVal1)
        //        & TableDict.TryGetValue("Bowel2cc", out string BowelVal2))
        //    {
        //        dt.Rows.Add(BowelVal, BowelVal1, BowelVal2);
        //    }

        //    if (TableDict.TryGetValue("Sigmoid0.1cc", out string SigmoidVal) & TableDict.TryGetValue("Sigmoid1cc", out string SigmoidVal1)
        //        & TableDict.TryGetValue("Sigmoid2cc", out string SigmoidVal2))
        //    {
        //        dt.Rows.Add(SigmoidVal, SigmoidVal1, SigmoidVal2);
        //    }

        //    if (TableDict.TryGetValue("Rectum0.1cc", out string RectumVal) & TableDict.TryGetValue("Rectum1cc", out string RectumVal1)
        //        & TableDict.TryGetValue("Rectum2cc", out string RectumVal2))
        //    {
        //        dt.Rows.Add(RectumVal, RectumVal1, RectumVal2);
        //    }

            




        //    // Create the string builder to store the formatted table
        //    StringBuilder sb = new StringBuilder();

        //    // Loop through the columns and add the column names to the string builder
        //    for (int i = 0; i < dt.Columns.Count; i++)
        //    {
        //        sb.Append(dt.Columns[i].ColumnName);
        //        if (i < dt.Columns.Count - 1)
        //        {
        //            sb.Append("\t");
        //        }
        //    }

        //    // Loop through the rows and append each row to the string builder
        //    sb.Append("\r\n");
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        for (int i = 0; i < dt.Columns.Count; i++)
        //        {
        //            sb.Append(dr[i].ToString());
        //            if (i < dt.Columns.Count - 1)
        //            {
        //                sb.Append("\t");
        //            }
        //        }
        //        sb.Append("\r\n");
        //    }

        //    // Copy the table to the clipboard
        //    Clipboard.SetText(sb.ToString());
        //}

        //public void CopyRowToClipboard(Dictionary<string, string> TableDict)
        //{
        //    // Create the data
        //    DataTable dt = new DataTable();
           

        //    if (TableDict.TryGetValue("HRCTVD50", out string Val) & TableDict.TryGetValue("HRCTVD90", out string Val1)
        //        & TableDict.TryGetValue("HRCTVD98", out string Val2))
        //    {
        //        dt.Columns.Add(Val);
        //        dt.Columns.Add(Val1);
        //        dt.Columns.Add(Val2);
        //    }

        //    if (TableDict.TryGetValue("Bladder0.1cc", out string BladVal) & TableDict.TryGetValue("Bladder1cc", out string BladVal1)
        //        & TableDict.TryGetValue("Bladder2cc", out string BladVal2))
        //    {
        //        dt.Columns.Add(BladVal);
        //        dt.Columns.Add(BladVal1);
        //        dt.Columns.Add(BladVal2);
        //    }

        //    if (TableDict.TryGetValue("Bowel0.1cc", out string BowelVal) & TableDict.TryGetValue("Bowel1cc", out string BowelVal1)
        //        & TableDict.TryGetValue("Bowel2cc", out string BowelVal2))
        //    {
        //        dt.Columns.Add(BowelVal);
        //        dt.Columns.Add(BowelVal1);
        //        dt.Columns.Add(BowelVal2);
        //    }


        //    if (TableDict.TryGetValue("Sigmoid0.1cc", out string SigmoidVal) & TableDict.TryGetValue("Sigmoid1cc", out string SigmoidVal1)
        //        & TableDict.TryGetValue("Sigmoid2cc", out string SigmoidVal2))
        //    {
        //        dt.Columns.Add(SigmoidVal);
        //        dt.Columns.Add(SigmoidVal1);
        //        dt.Columns.Add(SigmoidVal2);
        //    }

        //    if (TableDict.TryGetValue("Rectum0.1cc", out string RectumVal) & TableDict.TryGetValue("Rectum1cc", out string RectumVal1)
        //        & TableDict.TryGetValue("Rectum2cc", out string RectumVal2))
        //    {
        //        dt.Columns.Add(RectumVal);
        //        dt.Columns.Add(RectumVal1);
        //        dt.Columns.Add(RectumVal2);
        //    }





        //    // Create the string builder to store the formatted table
        //    StringBuilder sb = new StringBuilder();

        //    // Loop through the columns and add the column names to the string builder
        //    for (int i = 0; i < dt.Columns.Count; i++)
        //    {
        //        sb.Append(dt.Columns[i].ColumnName);
        //        if (i < dt.Columns.Count - 1)
        //        {
        //            sb.Append("\t");
        //        }
        //    }

        //    // Loop through the rows and append each row to the string builder
        //    sb.Append("\r\n");
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        for (int i = 0; i < dt.Columns.Count; i++)
        //        {
        //            sb.Append(dr[i].ToString());
        //            if (i < dt.Columns.Count - 1)
        //            {
        //                sb.Append("\t");
        //            }
        //        }
        //        sb.Append("\r\n");
        //    }

        //    // Copy the table to the clipboard
        //    Clipboard.SetText(sb.ToString());
        //}

        //private void CopyClipboard_Click(object sender, RoutedEventArgs e)
        //{
        //    if (clipboardTableDict != null)
        //    {

        //        if (this.ClipBoardOrientationBox.SelectedIndex == 0)
        //        {
        //            CopyTableToClipboard(clipboardTableDict);
        //        }

        //        if (this.ClipBoardOrientationBox.SelectedIndex == 1)
        //        {
        //            CopyRowToClipboard(clipboardTableDict);
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("No calculated statistics to copy.");
        //    }


        //}

        private void ColorToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (ReportDataGrid.Items.Count != 0 & refreshCount != 0)
            {
                this.ReportDataGrid.Items.Refresh();
              
            }

            refreshCount++;

        }

        private void ColorToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ReportDataGrid.Items.Count != 0)
            {
                this.ReportDataGrid.Items.Refresh();
            }
        }

        //private void ColorCodeReport()
        //{
            
        //    foreach (var thing in ReportDataGrid.Items)
        //    {
        //        var currentrow = thing.ToString();
        //        var rowTuple = ParseRowString(currentrow);
        //        var RowColor = PopulateOARConstraintClasses(rowTuple);
        //        var rowIndex = ReportDataGrid.Items.IndexOf(thing);
                
        //    }


        //}

        private Tuple<string, string, double> ParseRowString(string row)
        {

            string row1 = row.Replace('(', ' ');
            string row2 = row1.Replace(')', ' ');

            string[] stringArray = row2.Split(',');

            string metric = stringArray[0];
            string value = stringArray[1];
            string structure = stringArray[2];

            Tuple<string, string, double> tuple = new Tuple<string, string, double>(structure, metric, double.Parse(value));

            return tuple;
        }

        private System.Windows.Media.SolidColorBrush PopulateOARConstraintClasses(Tuple<string,string,double> tuple)
        {
            BladderDoseConstraints.IdealConstraint = 80;
            BladderDoseConstraints.MaximumConstraint = 90;
            BladderDoseConstraints.structure = "bladder";

            //checkString += tuple.Item1;

            RectumDoseConstraints.IdealConstraint = 65;
            RectumDoseConstraints.MaximumConstraint = 75;
            RectumDoseConstraints.structure = "rectum";


            SigmoidDoseConstraints.IdealConstraint = 70;
            SigmoidDoseConstraints.MaximumConstraint = 75;
            SigmoidDoseConstraints.structure = "sigmoid";


            BowelDoseConstraints.IdealConstraint = 70;
            BowelDoseConstraints.MaximumConstraint = 75;
            BowelDoseConstraints.structure = "bowel";

            HRCTVDoseConstraints.MaximumConstraint = 80;
            HRCTVDoseConstraints.structure = "hr_ctv";

            IRCTVDoseConstraints.MaximumConstraint = 60;
            IRCTVDoseConstraints.structure = "ir_ctv";

            GTVDoseConstraints.MaximumConstraint = 80;
            GTVDoseConstraints.structure = "gtv";

            if (tuple.Item1.Contains("bladder") & tuple.Item2.ToLower().Contains("2cc"))
            {
                BladderDoseConstraints.Dose2cc = tuple.Item3;
                var rowColor = BladderDoseConstraints.ColorCode(BladderDoseConstraints.Dose2cc);
                return rowColor;
            }
            else if (tuple.Item1.Contains("rectum") & tuple.Item2.ToLower().Contains("2cc"))
            {
                RectumDoseConstraints.Dose2cc = tuple.Item3;
                var rowColor = RectumDoseConstraints.ColorCode(RectumDoseConstraints.Dose2cc);
                return rowColor;

            }
            else if (tuple.Item1.Contains("sigmoid") & tuple.Item2.ToLower().Contains("2cc"))
            {
                SigmoidDoseConstraints.Dose2cc = tuple.Item3;
                var rowColor = SigmoidDoseConstraints.ColorCode(SigmoidDoseConstraints.Dose2cc);
                return rowColor;

            }
            else if (tuple.Item1.Contains("bowel") & tuple.Item2.ToLower().Contains("2cc"))
            {
                BowelDoseConstraints.Dose2cc = tuple.Item3;
                var rowColor = BowelDoseConstraints.ColorCode(BowelDoseConstraints.Dose2cc);
                return rowColor;
            }
            else if (tuple.Item1.Contains("hr_ctv") & tuple.Item2.ToLower().Contains("d90"))
            {
                HRCTVDoseConstraints.D90 = tuple.Item3;
                var rowColor = HRCTVDoseConstraints.ColorCode(HRCTVDoseConstraints.D90);
                return rowColor;
            }
            else if (tuple.Item1.Contains("ir_ctv") & tuple.Item2.ToLower().Contains("d90"))
            {
                IRCTVDoseConstraints.D90 = tuple.Item3;
                var rowColor = IRCTVDoseConstraints.ColorCode(IRCTVDoseConstraints.D90);
                return rowColor;
            }
            else if (tuple.Item1.Contains("gtv") & tuple.Item2.ToLower().Contains("d98"))
            {
                GTVDoseConstraints.D90 = tuple.Item3;
                var rowColor = GTVDoseConstraints.ColorCode(GTVDoseConstraints.D90);
                return rowColor;
            }
            else
            {
                return null;
            }


        }

        private void ReportDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            
            var rowString = e.Row.Item.ToString();
            var rowTuple = ParseRowString(rowString);

            //MessageBox.Show(String.Format("{0}{1}{2}", rowTuple.Item1, rowTuple.Item2, rowTuple.Item3));

            if ((rowTuple.Item2.Contains("2cc")|| rowTuple.Item2.ToLower().Contains("d90") || rowTuple.Item2.ToLower().Contains("d98")) & ColorToggleButton.IsChecked == true)
            {
                SolidColorBrush brush = PopulateOARConstraintClasses(rowTuple);

                //if (BladderDoseConstraints.structure != null)
                //{
                //    checkString += BladderDoseConstraints.structure;
                //}

                e.Row.Background = brush;

            }
        }

        private void Legend_Button_Click(object sender, RoutedEventArgs e)
        {
            UserControl ASTROPopout = new UserControlASTROLegend();
            Window window2 = new Window();
            window2.Content = ASTROPopout;
            window2.Height = 800;
            window2.Width = 825;

            window2.Show();

        }
    }
}



public class OARDoseConstraints
{
    public string structure;
    public double Dose2cc;
    public double IdealConstraint;
    public double MaximumConstraint;

    public SolidColorBrush ColorCode(double Dose2cc)
    {
        if (Dose2cc>= MaximumConstraint)
        {
            return System.Windows.Media.Brushes.Pink;
        }
        else if (Dose2cc >= IdealConstraint & Dose2cc <= MaximumConstraint)
        {
            return System.Windows.Media.Brushes.Yellow;
        }
        else
        {
            return System.Windows.Media.Brushes.LightGreen;
        }
        
    }

}

public class TargetDoseConstraints
{
    public string structure;
    public double D90;
    public double MaximumConstraint;

    public SolidColorBrush ColorCode(double Dose2cc)
    {
        if (D90 < MaximumConstraint)
        {
            return System.Windows.Media.Brushes.Pink;
        }
        else
        {
            return System.Windows.Media.Brushes.LightGreen;
        }

    }
}
//public class IRCTVDoseConstraints
//{
//    public string structure;
//    public double D90;
//    public double MaximumConstraint;

//    public SolidColorBrush ColorCode(double Dose2cc)
//    {
//        if (Dose2cc >= MaximumConstraint)
//        {
//            return System.Windows.Media.Brushes.Pink;
//        }
//        else
//        {
//            return System.Windows.Media.Brushes.LightGreen;
//        }

//    }
//}


