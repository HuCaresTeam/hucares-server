﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using SqliteManipulation.Models;

namespace SqliteManipulation
{
    public abstract class SqliteBaseManipulator
    {
        private const string connectionString = @"Data Source=SqliteData\HucaresMock.sqlite;Version=3;";

        public IEnumerable<Target> GetData<Target>(string query) where Target : new()
        {
            using (var sqliteConn = new SQLiteConnection(connectionString))
            {
                sqliteConn.Open();
                var command = new SQLiteCommand(query, sqliteConn);
                var dataReader = command.ExecuteReader();
                return dataReader.MapToObjectEnumerable<Target>();
            }
        }
    }
}