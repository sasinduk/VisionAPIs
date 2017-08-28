using Microsoft.ProjectOxford.Face;
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

namespace IdentifyImage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly IFaceServiceClient faceServiceClient = new FaceServiceClient("<your face API key>", "https://westus.api.cognitive.microsoft.com/face/v1.0");
        


        private const string personGroupId = "ABC_Marketing_Division";

        public MainWindow()
        {
            InitializeComponent();
            TestMethodAsync();
        }

        public async void TestMethodAsync()
        {
            try
            {
                string testImageFile = @"D:\test.jpg";

                using (Stream s = File.OpenRead(testImageFile))
                {
                    var faces = await faceServiceClient.DetectAsync(s);
                    var faceIds = faces.Select(face => face.FaceId).ToArray();

                    var results = await faceServiceClient.IdentifyAsync(personGroupId, faceIds);
                    foreach (var identifyResult in results)
                    {
                        Console.WriteLine("Result of face: {0}", identifyResult.FaceId);
                        if (identifyResult.Candidates.Length == 0)
                        {
                            Console.WriteLine("No one identified" );
                        }
                        else
                        {
                            // Get top 1 among all candidates returned
                            var candidateId = identifyResult.Candidates[0].PersonId;
                            var person = await faceServiceClient.GetPersonAsync(personGroupId, candidateId);
                            Console.WriteLine("Identified as {0} & Confident Level {1}", person.Name, identifyResult.Candidates[0].Confidence.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {

                throw;
            }
            
        }
    }
}
