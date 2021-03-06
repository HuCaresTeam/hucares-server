﻿using System;
using System.Collections.Generic;
using System.Linq;
using HucaresServer.Storage.Models;
using HucaresServer.Storage.Properties;
using HucaresServer.Utils;

namespace HucaresServer.Storage.Helpers
{
    public class MissingPlateHelper : IMissingPlateHelper
    {
        private IDbContextFactory _dbContextFactory;

        public MissingPlateHelper(IDbContextFactory dbContextFactory = null)
        {
            _dbContextFactory = dbContextFactory ?? new DbContextFactory();
        }

        public MissingLicensePlate InsertPlateRecord(string plateNumber, DateTime searchStartDatetime)
        {
            if (!plateNumber.IsValidPlateNumber())
                throw new ArgumentException(Resources.Error_PlateNumberFomatInvalid);

            var missingPlateObj = new MissingLicensePlate()
            {
                PlateNumber = plateNumber,
                SearchStartDateTime = searchStartDatetime,
                Status = LicensePlateFoundStatus.Searching   
            };

            using (var ctx = _dbContextFactory.BuildHucaresContext())
            {
                if (ctx.MissingLicensePlates.Any(m => m.PlateNumber == plateNumber &&
                     m.Status == LicensePlateFoundStatus.Searching))
                {
                    throw new Exception(Resources.Error_MissingPlateExists);
                }

                ctx.MissingLicensePlates.Add(missingPlateObj);
                ctx.SaveChanges();
            }

            return missingPlateObj;
        }

        public IEnumerable<MissingLicensePlate> GetAllPlateRecords()
        {
            using (var ctx = _dbContextFactory.BuildHucaresContext())
            {
                var query = ctx.MissingLicensePlates
                    .Select(c => c)
                    .OrderBy(c => c.SearchStartDateTime);
                
                return query.ToList();
            }
        }

        public IEnumerable<MissingLicensePlate> GetPlateRecordByPlateNumber(string plateNumber)
        {
            if (!plateNumber.IsValidPlateNumber())
                throw new ArgumentException(Resources.Error_PlateNumberFomatInvalid);

            using (var ctx = _dbContextFactory.BuildHucaresContext())
            {
                return ctx.MissingLicensePlates
                    .Where(c => c.PlateNumber == plateNumber)
                    .OrderBy(c => c.SearchStartDateTime)
                    .ToList();
            }
        }

        public MissingLicensePlate UpdatePlateRecord(int plateId, string plateNumber, DateTime searchStartDatetime)
        {
            if (!plateNumber.IsValidPlateNumber())
                throw new ArgumentException(Resources.Error_PlateNumberFomatInvalid);

            using (var ctx = _dbContextFactory.BuildHucaresContext())
            {
                var recordToUpdate = ctx.MissingLicensePlates.FirstOrDefault(c => c.Id == plateId) ?? 
                                     throw new ArgumentException(string.Format(Resources.Error_BadIdProvided, plateId));

                if (ctx.MissingLicensePlates.Any(m => m.PlateNumber == plateNumber &&
                    m.Status == LicensePlateFoundStatus.Searching &&
                    m.Id != plateId))
                {
                    throw new Exception(Resources.Error_MissingPlateExists);
                }

                recordToUpdate.PlateNumber = plateNumber;
                recordToUpdate.SearchStartDateTime = searchStartDatetime;
                ctx.SaveChanges();

                return recordToUpdate;
            }
        }

        public MissingLicensePlate MarkFoundPlate(int plateId, DateTime requestDateTime, LicensePlateFoundStatus status)
        {
            using (var ctx = _dbContextFactory.BuildHucaresContext())
            {
                var recordToUpdate = ctx.MissingLicensePlates.FirstOrDefault(c => c.Id == plateId) ?? 
                                     throw new ArgumentException(string.Format(Resources.Error_BadIdProvided, plateId));

                recordToUpdate.Status = status;
                recordToUpdate.SearchEndDateTime = requestDateTime;
                ctx.SaveChanges();

                return recordToUpdate;
            }
        }
        
        public MissingLicensePlate DeletePlateById(int plateId)
        {
            using (var ctx = _dbContextFactory.BuildHucaresContext())
            {
                var recordToDelete = ctx.MissingLicensePlates.FirstOrDefault(c => c.Id == plateId) ?? 
                                     throw new ArgumentException(string.Format(Resources.Error_BadIdProvided, plateId));

                ctx.MissingLicensePlates.Remove(recordToDelete);
                ctx.SaveChanges();

                return recordToDelete;
            }
        }

        public MissingLicensePlate DeletePlateByNumber(string plateNumber)
        {
            if (!plateNumber.IsValidPlateNumber())
                throw new ArgumentException(Resources.Error_PlateNumberFomatInvalid);

            using (var ctx = _dbContextFactory.BuildHucaresContext())
            {
                var recordToDelete = ctx.MissingLicensePlates.FirstOrDefault(c => c.PlateNumber == plateNumber) ?? 
                                     throw new ArgumentException(string.Format(Resources.Error_BadIdProvided, plateNumber));

                ctx.MissingLicensePlates.Remove(recordToDelete);
                ctx.SaveChanges();

                return recordToDelete;
            }
        }

        public void DeleteAll()
        {
            using (var ctx = _dbContextFactory.BuildHucaresContext())
            {
                var recordsToDelete = ctx.MissingLicensePlates.Select(c => c);
                ctx.MissingLicensePlates.RemoveRange(recordsToDelete);
                ctx.SaveChanges();
            }
        }
    }
}
