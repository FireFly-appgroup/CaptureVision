﻿using CaptureVision.DAL.Connection;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using Capture = CaptureVision.DAL.Models.Capture;

namespace CaptureVision.BLL.Services
{
    public class Queries : IQueries
    {
        private MySqlConnection _conn;
        private MySqlCommand _cmd;
        private List<Capture> _captures = new List<Capture>();
        private readonly int _limit = 10;
        private string _query = String.Empty;
        private Capture _predictCapture;

        public Queries()
        {
            _conn = null;
            _conn = DBMySQLUtils.GetDBConnection(new DAL.DBSettings());
        }

        public void InsertPicturesToDB(string Picture, string FileName, string Result)
        {
            try
            {
                _conn.Open();
                string query =
                    $"INSERT INTO `Capture` (`CaptureImage`, `FileName`, `Result`) VALUES ('{Picture}', '{FileName}', '{Result}');";
                _cmd = new MySqlCommand() { Connection = _conn, CommandText = query };
                _cmd.ExecuteNonQuery();
                _conn.Close();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public List<Capture> GetPicturesFromDB()
        {
            try
            {
                _conn.Open();

                if (_limit > 0)
                    _query = $"SELECT * FROM `Capture` WHERE ID != 1 LIMIT {_limit};";
                else
                    _query = $"SELECT * FROM `Capture` WHERE ID != 1;";

                _cmd = new MySqlCommand() { Connection = _conn, CommandText = _query };
                _cmd.ExecuteNonQuery();
        
                Capture capture;
                using (DbDataReader reader = _cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            capture = new Capture
                            {
                                CaptureImage = reader.GetString(reader.GetOrdinal("CaptureImage")),
                                Result = reader.GetString(reader.GetOrdinal("Result"))
                            };

                            _captures.Add(capture);
                        }
                    }
                }

                _conn.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return _captures;
        }

        public Capture GetPictureForPredict()
        {
            try
            {
                _conn.Open();

                _query = $"SELECT * FROM `Capture` WHERE ID == 1;";

                _cmd = new MySqlCommand() { Connection = _conn, CommandText = _query };
                _cmd.ExecuteNonQuery();

                
                using (DbDataReader reader = _cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            _predictCapture = new Capture
                            {
                                CaptureImage = reader.GetString(reader.GetOrdinal("CaptureImage")),
                                Result = reader.GetString(reader.GetOrdinal("Result"))
                            };
                        }
                    }
                }

                _conn.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return _predictCapture;
        }
    }
}
