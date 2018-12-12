﻿using HucaresServer.Storage.Helpers;
using System;
using System.Web.Http;
using static HucaresServer.Models.CameraInfoDataModels;
using HucaresServer.Utils;
using HucaresServer.Properties;
using HucaresServer.DataAcquisition;
using System.IO;

namespace HucaresServer.Controllers
{
    /// <summary>
    /// Responsible for managing the DetectedPlateController database table via http requests.
    /// </summary>
    /// <seealso cref="IDetectedPlateHelper"/>
    public class DetectedPlateController : ApiController
    {
        public IDetectedPlateHelper DetectedPlateHelper { get; set; } = new DetectedPlateHelper();
        public IImageManipulator ImageManipulator { get; set; } = new LocalImageManipulator();

        [HttpGet]
        [Route("api/dlp/all")]
        public IHttpActionResult GetAllDetectedMissingPlates()
        {
            return Json(DetectedPlateHelper.GetAllDlps());
        }

        [HttpGet]
        [Route("api/dlp/plate/{plateNumber}")]
        public IHttpActionResult GetAllDetectedPlatesByPlateNumber(string plateNumber, DateTime? startDateTime = null,
            DateTime? endDateTime = null)
        {
            if (!plateNumber.IsValidPlateNumber())
                throw new ArgumentException(Resources.Error_PlateNumberFomatInvalid);

            return Json(DetectedPlateHelper.GetAllActiveDetectedPlatesByPlateNumber(plateNumber, startDateTime, endDateTime));
        }

        [HttpGet]
        [Route("api/dlp/cam/{cameraId}")]
        public IHttpActionResult GetAllDetectedPlatesByCamera(int cameraId, DateTime? startDateTime = null, 
            DateTime? endDateTime = null)
        {
            return Json(DetectedPlateHelper.GetAllDetectedPlatesByCamera(cameraId, startDateTime, endDateTime));
        }

        // DEMONSTRATION PURPOSES ONLY
        [HttpDelete]
        [Route("api/dlp/all")]
        public IHttpActionResult DeleteDLPs()
        {
            var dlpList = DetectedPlateHelper.GetAllDlps();

            //delete images
            foreach (var dlp in dlpList)
            {
                var pathArray = dlp.ImgUrl.Split('/');
                var fileName = pathArray[pathArray.Length - 1];
                var dateTime = DateTime.Parse(pathArray[pathArray.Length - 2]); 
                var folderLocation = ImageManipulator.GenerateFolderLocationPath(dateTime);
                var filePath = Path.Combine(folderLocation, fileName) + ".jpg";

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            //delete records
            DetectedPlateHelper.DeleteAll();
            return Ok();
        }
    }
}
