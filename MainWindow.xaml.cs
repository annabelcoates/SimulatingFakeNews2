using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting.Hosting;
using SciChart;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.Annotations;
using SciChart.Charting.Visuals.Axes;
using SciChart.Charting.ChartModifiers;

namespace ModelAttemptWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Simulation simulation;

        Random random = new Random();
        public DispatcherTimer Clock { get; set; } = new DispatcherTimer();
        private DispatcherTimer MinClock {get;set;}=new DispatcherTimer();
        Facebook facebook;
        private string smallWorldPath = @"C:\Users\Anni\Documents\Uni\Computer Science\Proj\CSVs and text files\FacebookUK\small_world_graph.csv";


        // define fixed settings 
        public int fixedN;
        public int fixedK;
        public int fixedNFake;
        public int fixedNTrue;
        public int dpNumber;

        public List<int> values;
        
        public MainWindow()
        {
            this.fixedN = 1000;
            this.fixedK = 100;
            this.fixedNFake = 100;
            this.fixedNTrue = 200;
            //this.values = new List<int> { 1, 2, 4, 6, 8, 10, 12 };
            this.values = new List<int> {20};
            this.dpNumber = 0;

            this.UKDistributionSimulation("FTFWithShareProbs20", fixedN, fixedK, fixedNFake, fixedNTrue, values[0]);
        }

        private void SetClockFunctions()
        {
            Clock.Interval = TimeSpan.FromMilliseconds(150f / simulation.runSpeed);
            Clock.Tick += StandardFBUpdate;

            InitializeComponent();
            Clock.Start();
            MinClock.Start();
        }
        private void StandardFBUpdate(object sender, EventArgs e)
        {
            this.facebook.TimeSlotPasses(simulation.time);

            if (simulation.time == 1000)
            {
                // prevent timer based functions firing during the next simulation being made
                this.Clock = new DispatcherTimer();
                this.MinClock = new DispatcherTimer();

                SimulationEnd(this.simulation);
            }
            else if (simulation.time % 100 == 0)
            {
             //   this.AddDistributedNews(0, 1, this.facebook);
            }
            simulation.time++;

        }

     
        private void UKDistributionSimulation(string name,int n,int k=100,int nFake=20,int nTrue=20,int feedTimeFrame=100)
        {
           

            //this.Activate();
            this.simulation = new Simulation(name, 10, feedTimeFrame);
            this.simulation.DistributionPopulate(n);
            this.facebook = new Facebook("FacebookUK",feedTimeFrame);

            // Give facebook a small initial population
            int defaultFollows = n/2;
            this.facebook.PopulateFromPeople(n,k, simulation.humanPopulation);
            this.facebook.CreateMutualFollowsFromGraph(smallWorldPath);
            this.facebook.CreateFollowsBasedOnPersonality(defaultFollows);

            // Create some news to be shared
            AddDistributedNews(nFake, nTrue,this.facebook);
            SetClockFunctions();
        }
        
        private void AddDistributedNews(int nFake,int nTrue, OSN osn, double meanEFake=0.75, double meanETrue=0.5, double meanBFake=0.25,double meanBTrue = 0.75)
        {
            double std = 0.1;
            int nPostsPerTrue = 1;
            for (int i = 0; i < nFake; i++)
            {
                double e = simulation.NormalDistribution(meanEFake, std);
                double b = simulation.NormalDistribution(meanBFake, std);
                osn.CreateNewsRandomPoster("FakeNews", false, simulation.time, e, b);
            }
            for (int j =nFake; j< nFake+nTrue; j++)
            {
                double e = simulation.NormalDistribution(meanETrue, std);
                double b = simulation.NormalDistribution(meanBTrue, std);
                osn.CreateNewsRandomPoster("TrueNews", true, simulation.time, e, b,nPostsPerTrue);
            }
        }
      
        private void UpdateSimulationTime(object sender, EventArgs e)
        {
            this.simulation.time++;
        }



        private List<string[]> LoadCsvFile(string filePath)
        {
            var reader = new StreamReader(File.OpenRead(filePath));
            List<string[]> searchList = new List<string[]>();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine(); // ignore the line of labels
                if (line != ",source,target")
                {
                    char[] seperator = new char[] { ',' };
                    string[] lineList = line.Split(seperator);
                    searchList.Add(lineList);
                }
            }
            return searchList;

        }


        // Sci Chart Stuff
        public void CreateTestSciChart()
        {
            // Create the chart surface
            var sciChartSurface = new SciChartSurface();

            // Create the X and Y Axis
            var xAxis = new NumericAxis() { AxisTitle = "Number of Samples (per series)" };
            var yAxis = new NumericAxis() { AxisTitle = "Value" };

            sciChartSurface.XAxis = xAxis;
            sciChartSurface.YAxis = yAxis;

            // Specify Interactivity Modifiers
            sciChartSurface.ChartModifier = new ModifierGroup(new RubberBandXyZoomModifier(), new ZoomExtentsModifier());
            // Add annotation hints to the user
            var textAnnotation = new TextAnnotation()
            {
                Text = "Hello World!",
                X1 = 5.0,
                Y1 = 5.0
            };
            sciChartSurface.Annotations.Add(textAnnotation);
            this.InitializeComponent();
        }
  
       
        private void SimulationEnd(Simulation simulation)
        {


            string generalPath = @"C:\Users\Anni\Documents\Uni\Computer Science\Proj\CSVs and text files\"+simulation.versionName+"_"+simulation.runNumber+@"\";
           Directory.CreateDirectory(generalPath);

            facebook.SaveFollowCSV(generalPath);

            File.WriteAllLines(generalPath + "nSharedFakeNews.csv", this.facebook.nSharedFakeNewsList.Select(x => string.Join(",", x)));

            File.WriteAllLines(generalPath + "newsInfo.csv", this.facebook.newsList.Select(x => string.Join(",", x.believability, x.emotionalLevel)));


            var csv = new StringBuilder();
            var csv2 = new StringBuilder();
            var csvNShared = new StringBuilder();
            var csvNViewed = new StringBuilder();
            var csvSharers = new StringBuilder();
            var csvViewers = new StringBuilder();
            // List<double> populationAverages = simulation.CalculateAverages();
            //var firstLine = string.Format("{0},{1},{2},{3},{4},{5},{6}", populationAverages[0], populationAverages[1], populationAverages[2], populationAverages[3], populationAverages[4], populationAverages[5], populationAverages[6]);
            // csv.AppendLine(firstLine);
            File.WriteAllLines(generalPath + "fakeShareProbs.csv", facebook.fakeShareProbs.Select(x => string.Join(",", x)));
            File.WriteAllLines(generalPath + "trueShareProbs.csv", facebook.trueShareProbs.Select(x => string.Join(",", x)));
            foreach (News news in facebook.newsList)
            {
                // the number that shared with respect to time
                //var singleString = string.Join(",", _values.ToArray() );
                Console.WriteLine(string.Join(",", news.nSharedList.ToArray()));
                csvNShared.Append(string.Join(",",news.nSharedList.ToArray())+"\n");
                csvNViewed.Append(string.Join(",",news.nViewedList.ToArray())+ "\n"); ;


                // Write a list of everyone who has shared each news article     
                csvSharers.Append(string.Join(",", news.sharers.Select(x => string.Join(",", x.ID))) + "\n");
                csvViewers.Append(string.Join(",", news.viewers.Select(x => string.Join(",", x.ID))) + "\n");
                


               // List<double> personalityAverages = news.CalculateSharerAverages();
               // List<double> viewerAverages = news.CalculateViewerAverages();

               // var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6}", personalityAverages[0], personalityAverages[1], personalityAverages[2], personalityAverages[3], personalityAverages[4], personalityAverages[5], personalityAverages[6]);
               // csv.AppendLine(newLine);

              //  var newLine2 = string.Format("{0},{1},{3},{4},{5},{6}", viewerAverages[0], viewerAverages[1], viewerAverages[2], viewerAverages[3], viewerAverages[4], viewerAverages[5], viewerAverages[6]);
              //  csv2.AppendLine(newLine2);

            }
            File.WriteAllText(generalPath + "nSharesAll.csv", csvNShared.ToString());
            File.WriteAllText(generalPath + "nViewsAll.csv", csvNViewed.ToString());
            File.WriteAllText(generalPath + "sharersAll.csv", csvSharers.ToString());
            File.WriteAllText(generalPath + "viewersAll.csv", csvViewers.ToString());
           // File.WriteAllText(generalPath + "sharerPersonalityAverages.csv", csv.ToString());
           // File.WriteAllText(generalPath + "viewerPersonalityAverages.csv", csv2.ToString());


            CreateNSharesCSV(generalPath);
            // now undo clock functions

            MakeNextSimulation(simulation);
            
        }

        private void MakeNextSimulation(Simulation currentSimulation)
        {

            if (currentSimulation.runNumber == currentSimulation.nRuns) // if all the runs of one data point have been done
            {
                this.dpNumber++;
                
                if (this.dpNumber<values.Count)
                {
                    int newNPostsPerTrue = values[this.dpNumber];
                    string newName = currentSimulation.versionName.Remove(currentSimulation.versionName.Length - 2) + newNPostsPerTrue.ToString();
                    this.UKDistributionSimulation(newName, fixedN, fixedK, fixedNFake, fixedNTrue, newNPostsPerTrue);
                }
                else // if all the desired setting values have been simulated
                {
                    this.Close();
                }
            }
            else
            {
                this.UKDistributionSimulation(currentSimulation.versionName, fixedN, fixedK, fixedNFake, fixedNTrue, currentSimulation.feedTimeFrame);
                this.simulation.runNumber = currentSimulation.runNumber + 1;
            }
            

        }
    
        public void CreateNSharesCSV(string generalPath)
        {
            var csv = new StringBuilder();
            csv.AppendLine("ID,nFollowers,o,c,e,a,n,Online Literacy,Political Leaning,nFakeShares,nTrueShares"); // column headings
            foreach (Account account in facebook.accountList)
            {
                Console.WriteLine("OL in write:" + account.person.onlineLiteracy);
                var line = String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", account.ID, account.followers.Count, account.person.o, account.person.c, account.person.e, account.person.a, account.person.n, account.person.onlineLiteracy, account.person.politicalLeaning, account.person.nFakeShares, account.person.nTrueShares);// o,c,e,a,n,OL,PL nFakeShares, nTrueShares
                csv.AppendLine(line);
            }
            File.WriteAllText(generalPath+"NsharesPopulation.csv", csv.ToString());

        }
    }
}
