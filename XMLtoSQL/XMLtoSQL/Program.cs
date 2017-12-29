namespace XMLtoSQL
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Xml;
    using System.Configuration;

    class Program
    {
        static void Main()
        {
            var settings = (ConfigurationManager.GetSection("applicationSettings/XMLtoSQL.Properties.Settings") as ClientSettingsSection).Settings;
            var objectsPath = settings.Get("Объекты").Value.ValueXml.InnerText;
            var housesPath = settings.Get("Дома").Value.ValueXml.InnerText;
            using (var connection = new SqlConnection())
            {
                var connectionString = ConfigurationManager.ConnectionStrings["XMLtoSQL.Properties.Settings.СтрокаСоединенияСSQL"].ConnectionString;
                connection.ConnectionString = connectionString;
                try
                {
                    connection.Open();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.ToString());
                    return;
                }

                Console.WriteLine("Connected!");
                var watch = new Stopwatch();
                var newGUIDs = new List<Guid>();
                var parentGUIDs = (SortedSet<Guid>)(null);
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

                for (var tableNumber = 0; tableNumber < tablesCount; tableNumber++)
                {
                    using (var writer = new SqlBulkCopy(connection))
                    {
                        writer.BatchSize = 1000;
                        writer.DestinationTableName = tablesNames[tableNumber];
                        watch.Restart();
                        var reader = (new FIASXMLReader.FIASXMLReader(XmlReader.Create(soursePaths[tableNumber]), predicates[tableNumber], parsers[tableNumber]));
                        writer.WriteToServer(reader);
                        watch.Stop();
                        Console.WriteLine("Таблица [{0}] заполнена, времени затрачено - {1}", tablesNames[tableNumber], watch.ElapsedMilliseconds);
                    }

                    parentGUIDs = new SortedSet<Guid>(newGUIDs);
                    newGUIDs.Clear();
                }
            }
        }
    }
}

