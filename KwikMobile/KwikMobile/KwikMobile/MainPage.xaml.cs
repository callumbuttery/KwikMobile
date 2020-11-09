using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KwikMobile
{
    public partial class MainPage : ContentPage
    {

        //connection to firebase server
        public static FirebaseClient firebaseClient = new FirebaseClient("https://kwikmedical-96ff6.firebaseio.com/");
        public MainPage()
        {

            InitializeComponent();

            //set default value for picker
            AmbulanceChoice.SelectedItem = "Ambulance 1";
        }


        private void Button_Clicked(object sender, EventArgs e)
        {
            //validate user has selected an ambulance
            if (AmbulanceChoice.SelectedItem.ToString() != null)
            {
                string selectedAmbulance = AmbulanceChoice.SelectedItem.ToString();

                //redirect to second screen
                Application.Current.MainPage = new NavigationPage(new Job(selectedAmbulance));
            }
            else
            {
                DisplayAlert("Error", "Please select an ambulance", "Retry");
            }

        }
    }
}
