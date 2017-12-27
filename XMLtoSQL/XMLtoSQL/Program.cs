using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XMLtoSQL
{
    internal class FIASXMLReader : System.Data.IDataReader
    {
        private XmlReader _xmlReader;
        private Func<XmlReader, bool> _predicate;
        private Func<XmlReader, object>[] _converter;

        public FIASXMLReader(XmlReader xmlReader, Func<XmlReader, bool> predicate, Func<XmlReader, object>[] converter)
        {
            this._xmlReader = xmlReader;
            this._predicate = predicate;
            this._converter = converter;
        }

        public object this[int i] => throw new NotImplementedException();

        public object this[string name] => throw new NotImplementedException();

        public int Depth => throw new NotImplementedException();

        public bool IsClosed => throw new NotImplementedException();

        public int RecordsAffected => throw new NotImplementedException();

        public int FieldCount => 4;

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            this._xmlReader.Close();
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public string GetName(int i)
        {
            throw new NotImplementedException();
        }

        public int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public object GetValue(int i)
        {
            return this._converter[i](this._xmlReader);
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public bool Read()
        {
            while (this._xmlReader.Read())
            {
                if (this._xmlReader.GetAttribute("ACTSTATUS") == "1" && this._predicate(this._xmlReader))
                {
                    break;
                }
            }

            return !this._xmlReader.EOF;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            /*if (args.Length != 2)
            {
                Console.WriteLine("Программа на вход принимает 2 аргумента:");
                Console.WriteLine("Путь к файлу AS_ADDROBJ_*.XML");
                Console.WriteLine("Путь к файлу AS_HOUSE_*.XML");
            }
            */

            /*var objectsXML = args[0];
            var housesXML = args[1];*/

            var objectsPath = @"F:\Browser\fias_xml\AS_ADDROBJ_20171214_4af453b1-c874-4a55-935d-91aa70546984.XML";
            var housesPath = @"F:\Browser\fias_xml\AS_HOUSE_20171214_ad9f8dea-850f-4f70-9f54-1e05d9ecde76.XML";

            using (var connection = new SqlConnection())
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

                var watch = new Stopwatch();
                var newGUIDs = new List<Guid>();
                var parentGUIDs = (SortedSet<Guid>)(null);
                var tempParentGUID = new Guid();

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

                soursePaths[soursePaths.Length - 1] = housesPath;
                var predicates = new Func<XmlReader, bool>[]
                {
                    reader => reader.GetAttribute("AOLEVEL") == "1",
                    reader =>
                    {
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
                        reader => reader.GetAttribute("OFFNAME"),
                        reader => reader.GetAttribute("FORMALNAME")
                    }
                };

                for (var tableNumber = 0; tableNumber < tablesCount; tableNumber++)
                {
                    using (var writer = new SqlBulkCopy(connection))
                    {
                        writer.BatchSize = 1000;
                        writer.DestinationTableName = tablesNames[tableNumber];
                        watch.Restart();
                        var reader = (new FIASXMLReader(XmlReader.Create(soursePaths[tableNumber]), predicates[tableNumber], parsers[tableNumber]));
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

