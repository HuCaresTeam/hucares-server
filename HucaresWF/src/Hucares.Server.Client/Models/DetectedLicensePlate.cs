﻿using System;

namespace Hucares.Server.Client.Models
{
    public class DetectedLicensePlate : IEquatable<DetectedLicensePlate>
    {
        /// <summary>
        /// The primary key of the DetectedLicensePlates table.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The string representation of the detected license plate number.
        /// </summary>
        public string PlateNumber { get; set; }

        /// <summary>
        /// The DateTime of when the image used for the detection was taken.
        /// </summary>
        /// <remarks>
        /// Saved as "datetime2" in SQL
        /// </remarks>
        public DateTime DetectedDateTime { get; set; }

        /// <summary>
        /// Foreign key relation for the CameraInfo table.
        /// <see cref="CameraInfo.Id"/>
        /// </summary>
        /// <remarks>
        /// Used to get location of detection as well as information about the camera.
        /// </remarks>
        public int CamId { get; set; }

        /// <summary>
        /// URL of the publicly hosted image from which the detection was done.
        /// </summary>
        public string ImgUrl { get; set; }

        /// <summary>
        /// Confidence range [0, 100]%. 
        /// Determines how confident the system is that the parsed license plate number is correct.
        /// </summary>
        public double Confidence { get; set; }
        
        public bool Equals(DetectedLicensePlate other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (ReferenceEquals(null, other))
                return false;
            return (Id == other.Id
                    && PlateNumber == other.PlateNumber
                    && DetectedDateTime == other.DetectedDateTime
                    && CamId == other.CamId
                    && ImgUrl == other.ImgUrl
                    && Confidence == other.Confidence);
        }
    }
}
