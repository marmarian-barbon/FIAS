using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FIASXMLReader;

namespace ContinueHouseInsert
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var connection = new System.Data.SqlClient.SqlConnection())
            {
                var stringBuilder = new SqlConnectionStringBuilder();
                var data = @".\SQLEXPRESS";
                var dataBase = "RestrictedFIAS";
                var login = "sa";
                var password = "superadmin";
                while (connection.State != System.Data.ConnectionState.Open)
                {
                    try
                    {
                        stringBuilder.Authentication = SqlAuthenticationMethod.SqlPassword;
                        Console.Write("Источник данных:");
                        stringBuilder.DataSource = data;
                        Console.Write("База данных:");
                        stringBuilder.InitialCatalog = dataBase;
                        Console.Write("Имя пользователя:");
                        stringBuilder.UserID = login;
                        Console.Write("Пароль:");
                        stringBuilder.Password = password;
                        Console.Clear();
                        stringBuilder.TrustServerCertificate = true;
                        connection.ConnectionString = stringBuilder.ConnectionString;
                        connection.Open();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.ToString());
                        Console.ReadLine();
                    }
                }


                var command = new SqlCommand("SELECT * FROM [dbo].[Street]", connection);
                var result = command.ExecuteReader();
                var streetGUIDs = new List<Guid>();
                while (result.Read())
                {
                    streetGUIDs.Add((Guid)result[0]);
                }

                var parentGUIDs = new SortedSet<Guid>(streetGUIDs);
                Console.WriteLine(parentGUIDs.Count);

                var watch = new System.Diagnostics.Stopwatch();
                var newGUIDs = new List<Guid>();
                var tempParentGUID = new Guid();
                var tempPostalIndex = (string)null;
                var tempHouseNumber = (string)null;

                var tablesCount = 5;
                var tablesNames = new string[]
                {
                    "Region",
                    "Rayon",
                    "Town",
                    "Street",
                    "House"
                };

                var objectsPath = @"F:\Browser\fias_xml\AS_ADDROBJ_20171214_4af453b1-c874-4a55-935d-91aa70546984.XML";
                var housesPath = @"F:\Browser\fias_xml\AS_HOUSE_20171214_ad9f8dea-850f-4f70-9f54-1e05d9ecde76.XML";

                var soursePaths = new string[tablesCount];
                for (var i = 0; i < soursePaths.Length - 1; i++)
                {
                    soursePaths[i] = objectsPath;
                }
                var currentDate = DateTime.Today;

                soursePaths[soursePaths.Length - 1] = housesPath;
                var predicates = new Func<XmlReader, bool>[]
                {
                    reader =>
                    {
                        if (reader.GetAttribute("ACTSTATUS") != "1")
                        {
                            return false;
                        }

                        return reader.GetAttribute("AOLEVEL") == "1";
                    },
                    reader =>
                    {
                        if (reader.GetAttribute("ACTSTATUS") != "1")
                        {
                            return false;
                        }

                        var level = byte.Parse(reader.GetAttribute("AOLEVEL"));
                        var parentGUID = reader.GetAttribute("PARENTGUID");
                        if ((level != 3 && level != 35) || parentGUID == null)
                        {
                            return false;
                        }

                        tempParentGUID = Guid.Parse(parentGUID);
                        return parentGUIDs.Contains(tempParentGUID);
                    },
                    reader =>
                    {
                        if (reader.GetAttribute("ACTSTATUS") != "1")
                        {
                            return false;
                        }

                        var level = byte.Parse(reader.GetAttribute("AOLEVEL"));
                        var parentGUID = reader.GetAttribute("PARENTGUID");
                        if ((level != 4 && level != 6) || parentGUID == null)
                        {
                            return false;
                        }

                        tempParentGUID = Guid.Parse(parentGUID);
                        return parentGUIDs.Contains(tempParentGUID);
                    },
                    reader =>
                    {
                        if (reader.GetAttribute("ACTSTATUS") != "1")
                        {
                            return false;
                        }

                        var level = byte.Parse(reader.GetAttribute("AOLEVEL"));
                        var parentGUID = reader.GetAttribute("PARENTGUID");
                        if ((level != 7 && level != 75) || parentGUID == null)
                        {
                            return false;
                        }

                        tempParentGUID = Guid.Parse(parentGUID);
                        return parentGUIDs.Contains(tempParentGUID);
                    },
                    reader =>
                    {
                        tempPostalIndex = reader.GetAttribute("POSTALCODE");
                        if (tempPostalIndex ==null)
                        {
                            return false;
                        }

                        /*if (reader.GetAttribute("STATSTATUS") != "0")
                        {
                            return false;
                        }*/

                        if (DateTime.Parse(reader.GetAttribute("ENDDATE")) <= currentDate)
                        {
                            return false;
                        }

                        if (DateTime.Parse(reader.GetAttribute("STARTDATE")) > currentDate)
                        {
                            return false;
                        }



                        tempHouseNumber = reader.GetAttribute("HOUSENUM");
                        if (tempHouseNumber == null)
                        {
                            return false;
                        }

                        tempParentGUID = Guid.Parse(reader.GetAttribute("AOGUID"));
                        return parentGUIDs.Contains(tempParentGUID);
                    }
                };

                var parsers = new Func<XmlReader, object>[][]
                {
                    new Func<XmlReader, object>[]
                    {
                        reader =>
                        {
                            var regionGUID = Guid.Parse(reader.GetAttribute("AOGUID"));
                            newGUIDs.Add(regionGUID);
                            return regionGUID;
                        },
                        reader => sbyte.Parse(reader.GetAttribute("REGIONCODE")),
                        reader => reader.GetAttribute("OFFNAME"),
                        reader => reader.GetAttribute("FORMALNAME")
                    },
                    new Func<XmlReader, object>[]
                    {
                        reader =>
                        {
                            var rayonGUID = Guid.Parse(reader.GetAttribute("AOGUID"));
                            newGUIDs.Add(rayonGUID);
                            return rayonGUID;
                        },
                        reader => tempParentGUID,
                        reader => reader.GetAttribute("OFFNAME"),
                        reader => reader.GetAttribute("FORMALNAME")
                    },
                    new Func<XmlReader, object>[]
                    {
                        reader =>
                        {
                            var townGUID = Guid.Parse(reader.GetAttribute("AOGUID"));
                            newGUIDs.Add(townGUID);
                            return townGUID;
                        },
                        reader => tempParentGUID,
                        reader => reader.GetAttribute("OFFNAME"),
                        reader => reader.GetAttribute("FORMALNAME")
                    },
                    new Func<XmlReader, object>[]
                    {
                        reader =>
                        {
                            var streetGUID = Guid.Parse(reader.GetAttribute("AOGUID"));
                            newGUIDs.Add(streetGUID);
                            return streetGUID;
                        },
                        reader => tempParentGUID,
                        reader => reader.GetAttribute("OFFNAME"),
                        reader => reader.GetAttribute("FORMALNAME")
                    },
                    new Func<XmlReader, object>[]
                    {
                        reader =>
                        {
                            return Guid.Parse(reader.GetAttribute("HOUSEGUID"));
                        },
                        reader => tempParentGUID,
                        reader => tempPostalIndex,
                        reader => tempHouseNumber
                    }
                };

                for (var tableNumber = 4; tableNumber < tablesCount; tableNumber++)
                {
                    using (var writer = new SqlBulkCopy(connection))
                    {
                        writer.BatchSize = 1000;
                        writer.DestinationTableName = tablesNames[tableNumber];
                        watch.Restart();
                        var xmlReader = XmlReader.Create(soursePaths[tableNumber]);

                        var commandForHouses = new SqlCommand("SELECT * FROM [dbo].[House]", connection);
                        var resultForHouses = commandForHouses.ExecuteReader();
                        var houseGUIDsAlready = new List<Guid>();
                        while (resultForHouses.Read())
                        {
                            houseGUIDsAlready.Add((Guid)resultForHouses[0]);
                        }

                        Console.WriteLine("Added");

                        var sortedHouseGUIDsAlready = new SortedSet<Guid>(houseGUIDsAlready);
                        Console.WriteLine("Sorted");
                        while (xmlReader.Read())
                        {
                            if (predicates[4](xmlReader))
                            {
                                if (!sortedHouseGUIDsAlready.Contains(Guid.Parse(xmlReader.GetAttribute("HOUSEGUID"))))
                                {
                                    Console.WriteLine("Finally!");
                                    break;
                                }
                            }
                        }

                        var reader = (new FIASXMLReader.FIASXMLReader(xmlReader, predicates[tableNumber], parsers[tableNumber]));
                        writer.WriteToServer(reader);
                        watch.Stop();
                        Console.WriteLine("Таблица [{0}] заполнена, времени затрачено - {1}", tablesNames[tableNumber], watch.ElapsedMilliseconds);
                    }
                }
            }
        }
    }
}
