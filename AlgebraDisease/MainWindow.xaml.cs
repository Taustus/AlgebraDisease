using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace AlgebraDisease
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            const int heavyFormStart = 400, lightFormStart = 600, all = 14000;

            const double lightFormChance = 0.14, heavyFormChance = 0.11,

                chanceToGetWellInLightForm = 0.08,

                chanceToGetHeavyFormInLightForm = 0.18,

                chanceToGetWellInHeavyForm = 0.06,

                chanceToGetLightInHeavyForm = 0.09;

            var wellPeople = new List<Human>();

            for (int i = 0; i < all; i++)
            {
                wellPeople.Add(new Human());
            }

            var sickInLighFormPeople = new List<Human>();
            var sickInHeavyFormPeople = new List<Human>();

            ChangePeopleState(ref wellPeople, ref sickInLighFormPeople, lightFormStart, DiseaseForm.Light);
            ChangePeopleState(ref wellPeople, ref sickInHeavyFormPeople, heavyFormStart, DiseaseForm.Heavy);

            chart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<double> { wellPeople.Count },
                    Title = "Healthy",
                },
                new LineSeries
                {
                    Values = new ChartValues<double> { sickInLighFormPeople.Count },
                    Title = "Heavy disease form",
                },
                new LineSeries
                {
                    Values = new ChartValues<double> { sickInHeavyFormPeople.Count },
                    Title = "Light disease form",
                }
            };
            chart.LegendLocation = LegendLocation.Top;

            SetMinimumAndMaximumLabels(sickInLighFormPeople, sickInHeavyFormPeople);

            Task.Run(() =>
            {
                int repeatedValuesCounter = 0;
                var r = new Random();
                // Repeat until all values ​​are repeated 3 times
                while (repeatedValuesCounter < 2)
                {
                    // Calculate amount of people to get sick
                    int
                    peopleToGetSickInLightForm = (int)Math.Ceiling(wellPeople.Count * lightFormChance),
                    peopleToGetSickInHeavyForm = (int)Math.Ceiling(wellPeople.Count * heavyFormChance),
                    peopleToGetWellInLightForm = (int)Math.Ceiling(sickInLighFormPeople.Count * chanceToGetWellInLightForm),
                    peopleToGetWellInHeavyForm = (int)Math.Ceiling(sickInHeavyFormPeople.Count * chanceToGetWellInHeavyForm),
                    peopleToGetHeavyFormInLightForm = (int)Math.Ceiling(sickInLighFormPeople.Count * chanceToGetHeavyFormInLightForm),
                    peopleToGetLightFormInHeavyForm = (int)Math.Ceiling(sickInHeavyFormPeople.Count * chanceToGetLightInHeavyForm);
                    // Change people's state
                    ChangePeopleState(ref wellPeople, ref sickInLighFormPeople, peopleToGetSickInLightForm, DiseaseForm.Light);
                    ChangePeopleState(ref wellPeople, ref sickInHeavyFormPeople, peopleToGetSickInHeavyForm, DiseaseForm.Heavy);
                    ChangePeopleState(ref wellPeople, ref sickInLighFormPeople, peopleToGetWellInLightForm, DiseaseForm.Healthy);
                    ChangePeopleState(ref wellPeople, ref sickInHeavyFormPeople, peopleToGetWellInHeavyForm, DiseaseForm.Healthy);
                    ChangePeopleState(ref sickInLighFormPeople, ref sickInHeavyFormPeople, peopleToGetHeavyFormInLightForm, DiseaseForm.Heavy, true);
                    ChangePeopleState(ref sickInHeavyFormPeople, ref sickInLighFormPeople, peopleToGetLightFormInHeavyForm, DiseaseForm.Light, true);
                    // If values are repeated increase counter
                    if (wellPeople.Count == int.Parse(chart.Series[0].Values[chart.Series[0].Values.Count - 1].ToString()) &&
                       sickInLighFormPeople.Count == int.Parse(chart.Series[1].Values[chart.Series[1].Values.Count - 1].ToString()) &&
                       sickInHeavyFormPeople.Count == int.Parse(chart.Series[2].Values[chart.Series[2].Values.Count - 1].ToString()))
                        repeatedValuesCounter++;
                    // Add values to chart series
                    chart.Series[0].Values.Add((double)wellPeople.Count);
                    chart.Series[1].Values.Add((double)sickInLighFormPeople.Count);
                    chart.Series[2].Values.Add((double)sickInHeavyFormPeople.Count);
                    // Add min/max values to labels
                    SetMinimumAndMaximumLabels(sickInLighFormPeople, sickInHeavyFormPeople);
                    // Sleep a bit
                    Thread.Sleep(50);
                }
            });
        }

        void SetMinimumAndMaximumLabels(List<Human> sickInLighFormPeople, List<Human> sickInHeavyFormPeople)
        {
            Dispatcher.Invoke(() =>
            {
                int.TryParse(LightMinLabel.Content.ToString(), out int minLight);
                int.TryParse(LightMaxLabel.Content.ToString(), out int maxLight);
                int.TryParse(HeavyMinLabel.Content.ToString(), out int minHeavy);
                int.TryParse(HeavyMaxLabel.Content.ToString(), out int maxHeavy);

                LightMinLabel.Content = minLight == 0 || minLight > sickInLighFormPeople.Count ? sickInLighFormPeople.Count : minLight;
                LightMaxLabel.Content = maxLight == 0 || maxLight < sickInLighFormPeople.Count ? sickInLighFormPeople.Count : maxLight;
                HeavyMinLabel.Content = minHeavy == 0 || minHeavy > sickInHeavyFormPeople.Count ? sickInHeavyFormPeople.Count : minHeavy;
                HeavyMaxLabel.Content = maxHeavy == 0 || maxHeavy < sickInHeavyFormPeople.Count ? sickInHeavyFormPeople.Count : maxHeavy;
            });
        }

        void ChangePeopleState(ref List<Human> wellPeople, ref List<Human> sickPeople, int count, DiseaseForm form, bool allSick = false)
        {
            var r = new Random();
            if (form != DiseaseForm.Healthy)
            {
                count += sickPeople.Count;

                while (sickPeople.Count != count && wellPeople.Count != 0)
                {
                    int index = r.Next(wellPeople.Count);

                    if (wellPeople[index].CurrentState == DiseaseForm.Healthy)
                    {
                        wellPeople[index].GetSickerForOneLevel();
                        if (wellPeople[index].CurrentState != form)
                        {
                            wellPeople[index].GetSickerForOneLevel();
                        }

                        sickPeople.Add(wellPeople[index]);

                        wellPeople.RemoveAt(index);
                    }
                    else if (allSick)
                    {
                        if (form == DiseaseForm.Heavy)
                        {
                            wellPeople[index].GetSickerForOneLevel();

                            sickPeople.Add(wellPeople[index]);

                            wellPeople.RemoveAt(index);
                        }
                        else
                        {
                            wellPeople[index].GetWellForOneLevel();

                            sickPeople.Add(wellPeople[index]);

                            wellPeople.RemoveAt(index);
                        }
                    }
                }
            }
            else
            {
                count += wellPeople.Count;

                while (wellPeople.Count != count && sickPeople.Count != 0)
                {
                    int index = r.Next(sickPeople.Count);

                    sickPeople[index].GetWellForOneLevel();
                    if (sickPeople[index].CurrentState != form)
                    {
                        sickPeople[index].GetWellForOneLevel();
                    }

                    wellPeople.Add(sickPeople[index]);

                    sickPeople.RemoveAt(index);
                }
            }
        }

    }
}
