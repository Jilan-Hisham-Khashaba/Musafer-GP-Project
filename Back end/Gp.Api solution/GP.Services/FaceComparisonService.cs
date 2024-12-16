using Emgu.CV;
using GP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Emgu.CV;
using OpenCvSharp;
using Emgu.CV.Reg;

namespace GP.Services
{
    public class FaceComparisonService
    {
        private readonly HttpClient _httpClient;

        public FaceComparisonService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        //public async Task<FaceComparison> CompareFaces(byte[] image1, byte[] image2)
        //{
        //    var formData = new MultipartFormDataContent();
        //    formData.Add(new ByteArrayContent(image1), "image1", "image1.jpg");
        //    formData.Add(new ByteArrayContent(image2), "image2", "image2.jpg");

        //    var response = await _httpClient.PostAsync("http://localhost:5000/compare_faces", formData);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var contentString = await response.Content.ReadAsStringAsync();
        //        var result = JsonSerializer.Deserialize<FaceComparison>(contentString);
        //        return result;
        //    }
        //    else
        //    {
        //        throw new HttpRequestException($"Face comparison request failed with status code {response.StatusCode}");
        //    }
        //}



        public async Task<FaceComparison> CompareFaces(byte[] image1, byte[] image2, bool useCamera = true)
        {
            var formData = new MultipartFormDataContent();

            formData.Add(new ByteArrayContent(image1), "image1", "image1.jpg");

            if (!useCamera)
            {
                // Add image2 as a file (if not using camera)
                formData.Add(new ByteArrayContent(image2), "image2", "image2.jpg");
            }

            // Send the multipart form data to the Flask API
            var response = await _httpClient.PostAsync("http://localhost:5000/compare_faces", formData);

            if (response.IsSuccessStatusCode)
            {
                var contentString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<FaceComparison>(contentString);
                return result;
            }
            else
            {
                throw new HttpRequestException($"Face comparison request failed with status code {response.StatusCode}");
            }
        }

        public async Task<byte[]> CaptureFrameFromCameraAsync()
        {
            using (var capture = new OpenCvSharp.VideoCapture(0))
            {
                OpenCvSharp.Mat frame = new OpenCvSharp.Mat();
                capture.Read(frame);

                // Save the Mat to a temporary file directly as JPEG
                string tempFilePath = Path.GetTempFileName();
                Cv2.ImWrite(tempFilePath, frame, new int[] { (int)ImwriteFlags.PngCompression, 100 });

                // Read the temporary file as byte array
                byte[] byteArray = File.ReadAllBytes(tempFilePath);

                // Delete the temporary file
                File.Delete(tempFilePath);

                return byteArray;
            }
        }

        private byte[] ConvertMatToByteArray(OpenCvSharp.Mat mat)
        {
            // Save the Mat to a temporary file
            string tempFilePath = Path.GetTempFileName();
            Cv2.ImWrite(tempFilePath, mat, new int[] { (int)ImwriteFlags.PngCompression, 9 });



            // Read the temporary file as byte array
            byte[] byteArray = File.ReadAllBytes(tempFilePath);

            // Delete the temporary file
            File.Delete(tempFilePath);

            return byteArray;
        }
    }

}





