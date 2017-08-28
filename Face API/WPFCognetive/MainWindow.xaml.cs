using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace WPFCognetive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TestMethodAsync();
        }


        private readonly IFaceServiceClient faceServiceClient = new FaceServiceClient("<your face API key>");

        private const string personGroupId = "ABC_Marketing_Division";



        public async void TestMethodAsync()
        {


            try
            {

                await faceServiceClient.CreatePersonGroupAsync(personGroupId, personGroupId);
                // Define Anna
                CreatePersonResult friend1 = await faceServiceClient.CreatePersonAsync(
                    // Id of the person group that the person belonged to
                    personGroupId,
                    // Name of the person
                    "Anna Simmons"
                );

                // Directory contains image files of the person
                string friend1ImageDir = @"D:\Pictures\Anna";


                foreach (string imagePath in Directory.GetFiles(friend1ImageDir, "*.jpg"))
                {
                    using (Stream s = File.OpenRead(imagePath))
                    {
                        // Detect faces in the image and add to Anna
                        await faceServiceClient.AddPersonFaceAsync(
                            personGroupId, friend1.PersonId, s);
                    }
                }

                // Do the same for Bill and Clare
                await faceServiceClient.TrainPersonGroupAsync(personGroupId);

                TrainingStatus trainingStatus = null;
                while (true)
                {
                    trainingStatus = await faceServiceClient.GetPersonGroupTrainingStatusAsync(personGroupId);

                    if (!(trainingStatus.Status.Equals("running")))
                    {
                        break;
                    }

                    await Task.Delay(1000);
                }
            }
            catch (Exception e)
            {

            }



        }
    }
}
