using E_Raamatud.Model;
using Microsoft.Maui.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace E_Raamatud.View
{
    public partial class LibraryPage : ContentPage
    {
        public LibraryPage()
        {
            InitializeComponent();
            BindingContext = new ViewModel.LibraryViewModel();
        }

        private async void OnItemTapped(object sender, EventArgs e)
        {
            var frame = sender as Frame;
            var raamat = frame?.BindingContext as Raamat;

            if (raamat == null)
                return;

            try
            {
                string path = Path.Combine(FileSystem.AppDataDirectory, "Resources", "Raw", raamat.Tekstifail);

                if (!File.Exists(path))
                {
                    await DisplayAlert("Viga", "Raamatu tekstifaili ei leitud.", "OK");
                    return;
                }

                string content = await File.ReadAllTextAsync(path);
                await Navigation.PushAsync(new BookReaderPage(raamat.Pealkiri, content));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Viga raamatu avamisel: {ex.Message}");
                await DisplayAlert("Viga", "Raamatu avamisel tekkis probleem.", "OK");
            }
        }
    }
}

