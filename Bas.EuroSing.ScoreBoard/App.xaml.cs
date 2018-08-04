using Bas.EuroSing.ScoreBoard.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bas.EuroSing.ScoreBoard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 1 &&
                e.Args[0].Equals("tocsv", StringComparison.InvariantCultureIgnoreCase))
            {
                var db = new ScoreBoardDbContext();


                using (var csvFile = File.CreateText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Eurosing\\points.csv")))
                {
                    csvFile.WriteLine("sep=;");

                    var headerLineStringBuilder = new StringBuilder("Voting country");
                    foreach (var country in db.Countries.OrderBy(c => c.Name))
                    {
                        headerLineStringBuilder.Append($";{country.Name}");
                    }

                    csvFile.WriteLine(headerLineStringBuilder.ToString());

                    foreach (var givingCountry in db.Countries.OrderBy(c => c.Name))
                    {
                        var countryLineStringBuilder = new StringBuilder(givingCountry.Name);

                        foreach (var receivingCountry in db.Countries.OrderBy(c => c.Name))
                        {
                            var vote = db.Votes.SingleOrDefault(v => v.FromCountry.Id == givingCountry.Id &&
                                                                     v.ToCountry.Id == receivingCountry.Id);

                            countryLineStringBuilder.Append($";{vote?.NumPoints}");
                        }

                        csvFile.WriteLine(countryLineStringBuilder.ToString());
                    }

                    csvFile.Close();
                }
            }
        }
    }
}
