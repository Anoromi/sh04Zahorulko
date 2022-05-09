using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace sh04Zahorulko
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Repository? repository;
        public App()
        {
            Task.Run(async () =>
            {
                repository = await Repository.Get();
                Debug.WriteLine("Filling");
                await TestValuesProvider.Fill(repository);
            });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Task.Run(async () =>
            {
                if (repository is not null)
                {
                    await repository.Save();
                }
            });
        }
    }
}
