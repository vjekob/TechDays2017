using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.IO;
using TechDays2017;
using System.Drawing.Imaging;

namespace ImageManipulatorAzure
{
    public static class ScaleAndCrop
    {
        [FunctionName("ScaleAndCrop")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var data = await req.Content.ReadAsStreamAsync();
            using (var outStream = new MemoryStream())
            {
                // Do stuff here
                var scaledBitmap = ImageManipulator.ScaleTo(data, 300, 300);
                using (var tempStream1 = new MemoryStream())
                {
                    scaledBitmap.Save(tempStream1, ImageFormat.Png);
                    var croppedBitmap = ImageManipulator.Crop(tempStream1, 300, 300);
                    using (var tempStream2 = new MemoryStream())
                    {
                        croppedBitmap.Save(tempStream2, ImageFormat.Png);

                        //var proc = new ImageProcessor.ImageFactory();
                        //proc.Load(tempStream2.ToArray()).RoundedCorners(150).Save(outStream);

                        var result = new HttpResponseMessage(HttpStatusCode.OK);
                        //result.Content = new ByteArrayContent(outStream.ToArray());
                        result.Content = new ByteArrayContent(tempStream2.ToArray());
                        result.Content.Headers.Add("Content-Type", "application/octet-stream");
                        return result;
                    }
                }
            }
        }
    }
}
