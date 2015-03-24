using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;

namespace VOTCClient.Core.External.Azure
{
    public static class ImageUpload
    {
        public static async Task Upload(string file, string fileName)
        {
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=diegutesten;AccountKey=dRYAiI5vGD1OurMR0wbOfI45K2dFXXQ9Vz59uzLGe6v6aptCfdfL/UNid2xX89Q6g/dU206tVp1FNbTOk5smEg==");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("votcstore");
            var blockBlob = container.GetBlockBlobReference(fileName);
            var data = File.ReadAllBytes(file);
            await blockBlob.UploadFromByteArrayAsync(data, 0, data.Length);
        }
    }
}