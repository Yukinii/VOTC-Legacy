using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
/*
    This file is part of VOTC.

    VOTC is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    VOTC is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with VOTC.  If not, see <http://www.gnu.org/licenses/>.
*/
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