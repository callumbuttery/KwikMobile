using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KwikMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Job : ContentPage
    {
        //used to store patient data
        string globalNHSID = "";
        string globalAddress = "";
        string globalCondition = "";
        string hospital = "";
        string condition = "";
        bool active = false;
        string medic = "";
        string timeArrived = "";
        string timeFinished = "";
        string helpGiven = "";
        string caseNo = "";



        //initialise firebase server connection
        public static FirebaseClient firebaseClient = new FirebaseClient("https://kwikmedical-96ff6.firebaseio.com/");

        //list to store jobs found on server
        List<JobsList> ListOfJobs = new List<JobsList>();

        //global selected ambulance
        string Ambulance = "";
        public Job(string selectedAmbulance)
        {
            InitializeComponent();
            Ambulance = selectedAmbulance;
            GetAvailableJobs(selectedAmbulance);
        }

        async void GetAvailableJobs(string selectedAmbulance)
        {

            //get jobs
            ListOfJobs = (await firebaseClient
                .Child("Dispatches")
                //gets jobs that match our ambulance number
                .OnceAsync<JobsList>()).Where(a => a.Object.ambulance == selectedAmbulance).Where(b => b.Object.active == true).Select(item => new JobsList
                {
                    ambulance = item.Object.ambulance,
                    active = item.Object.active,
                    caseNumber = item.Object.caseNumber,
                    hosptial = item.Object.hosptial,
                    condition = item.Object.condition,
                    nhsID = item.Object.nhsID,
                    patientAddress = item.Object.patientAddress
                }).ToList();

            var orderedList = ListOfJobs.OrderByDescending(x => x.ambulance).ToList();



            JobsList.ItemsSource = orderedList;

            


        }

        //update job as complete
        public async void Button_Clicked(object sender, EventArgs e)
        {
            
            //Find away to pull case number from each data cell to update on backend
            Button btn = (Button)sender;
            var buttonID = btn.ClassId;
            

            //find record to delete on firebase
            var updateRecord = (await firebaseClient
                .Child("Dispatches")
                .OnceAsync<JobsList>()).Where(a => a.Object.caseNumber == buttonID).FirstOrDefault();

            caseNo = updateRecord.Object.caseNumber;
            Ambulance = updateRecord.Object.ambulance;
            hospital = updateRecord.Object.hosptial;
            globalNHSID = updateRecord.Object.nhsID;
            condition = updateRecord.Object.condition;
            globalAddress = updateRecord.Object.patientAddress;
            //rest of the vars are assigned during text box changed methods at bottom




            //delete record
            await firebaseClient.Child("Dispatches").Child(updateRecord.Key).DeleteAsync();

            //add updated record
            //post new emergency to server
            var postAmbulance = await firebaseClient
                .Child("Dispatches")
                .PostAsync(new JobsList() { nhsID = globalNHSID, caseNumber = caseNo, patientAddress = globalAddress, ambulance = Ambulance, hosptial = hospital, condition = globalCondition, active = false, medicName = medic, helpGiven = helpGiven, timeOfArrival = timeArrived, timeOfFinish = timeFinished});

        }

        private void MedicChanged(object sender, TextChangedEventArgs e)
        {
            medic = e.NewTextValue;
        }

        private void HelpChanged(object sender, TextChangedEventArgs e)
        {
            helpGiven = e.NewTextValue;
        }

        private void ArrivalChange(object sender, TextChangedEventArgs e)
        {
            timeArrived = e.NewTextValue;
        }

        private void FinishedChange(object sender, TextChangedEventArgs e)
        {
            timeFinished = e.NewTextValue;
        }

        //used to refresh jobs on screen
        private void refresh(object sender, EventArgs e)
        {
            GetAvailableJobs(Ambulance);
        }
    }
}