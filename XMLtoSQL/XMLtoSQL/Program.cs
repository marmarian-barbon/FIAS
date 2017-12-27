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

                var regionGUIDs = new List<string>();

                using (var writer = new SqlBulkCopy(connection))
                {
                    writer.BatchSize = 1000;
                    var regionPredicate = (Func<XmlReader, bool>)((reader) =>
                    {
                        return reader.GetAttribute("AOLEVEL") == "1";
                    });
                    var regionParser = new Func<XmlReader, object>[]
                    {
                        reader =>
                        {
                            var regionGUID = reader.GetAttribute("AOGUID");
                            regionGUIDs.Add(regionGUID);
                            return Guid.Parse(regionGUID);
                        },
                        reader => sbyte.Parse(reader.GetAttribute("REGIONCODE")),
                        reader => reader.GetAttribute("OFFNAME"),
                        reader => reader.GetAttribute("FORMALNAME")
                    };
                    watch.Restart();
                    writer.DestinationTableName = "Region";
                    writer.WriteToServer(new FIASXMLReader(XmlReader.Create(objectsPath),regionPredicate, regionParser));
                    watch.Stop();
                    Console.WriteLine("Регионы записаны, времени затрачено - {0}", watch.ElapsedMilliseconds);
                }

                var regionGUIDsSorted = new SortedSet<string>(regionGUIDs);
                var rayonGUIDs = new List<string>();
                using (var writer = new SqlBulkCopy(connection))
                {
                    writer.BatchSize = 1000;
                    var rayonPredicate = (Func<XmlReader, bool>)((reader) =>
                    {
                        var level = byte.Parse(reader.GetAttribute("AOLEVEL"));
                        return (level == 3 || level == 35) && regionGUIDs.Contains(reader.GetAttribute("PARENTGUID"));
                    });
                    var rayonParser = new Func<XmlReader, object>[]
                    {
                    reader =>
                    {
                        var rayonGUID = reader.GetAttribute("AOGUID");
                        rayonGUIDs.Add(rayonGUID);
                        return Guid.Parse(rayonGUID);
                    },
                    reader => Guid.Parse(reader.GetAttribute("PARENTGUID")),
                    reader => reader.GetAttribute("OFFNAME"),
                    reader => reader.GetAttribute("FORMALNAME")
                    };
                    watch.Restart();
                    writer.DestinationTableName = "Rayon";
                    writer.WriteToServer(new FIASXMLReader(XmlReader.Create(objectsPath), rayonPredicate, rayonParser));
                    watch.Stop();
                    Console.WriteLine("Районы записаны, времени затрачено - {0}", watch.ElapsedMilliseconds);
                }

                regionGUIDs.Clear();
                var townGUIDs = new List<string>();
                using (var writer = new SqlBulkCopy(connection))
                {
                    writer.BatchSize = 1000;
                    var townPredicate = (Func<XmlReader, bool>)((reader) =>
                    {
                        var level = byte.Parse(reader.GetAttribute("AOLEVEL"));
                        return (level == 4 || level == 6) && rayonGUIDs.Contains(reader.GetAttribute("PARENTGUID"));
                    });
                    var townParser = new Func<XmlReader, object>[]
                    {
                        reader =>
                        {
                            var townGUID = reader.GetAttribute("AOGUID");
                            townGUIDs.Add(townGUID);
                            return Guid.Parse(townGUID);
                        },
                        reader => Guid.Parse(reader.GetAttribute("PARENTGUID")),
                        reader => reader.GetAttribute("OFFNAME"),
                        reader => reader.GetAttribute("FORMALNAME")
                    };
                    watch.Restart();
                    writer.DestinationTableName = "Town";
                    writer.WriteToServer(new FIASXMLReader(XmlReader.Create(objectsPath), townPredicate, townParser));
                    watch.Stop();
                    Console.WriteLine("Города записаны, времени затрачено - {0}", watch.ElapsedMilliseconds);
                }

                rayonGUIDs.Clear();
                var streetGUIDs = new List<string>();
                using (var writer = new SqlBulkCopy(connection))
                {
                    writer.BatchSize = 1000;
                    var streetPredicate = (Func<XmlReader, bool>)((reader) =>
                    {
                        var level = byte.Parse(reader.GetAttribute("AOLEVEL"));
                        return (level == 7 || level == 75) && townGUIDs.Contains(reader.GetAttribute("PARENTGUID"));
                    });
                    var streetParser = new Func<XmlReader, object>[]
                    {
                        reader =>
                        {
                            var streetGUID = reader.GetAttribute("AOGUID");
                            streetGUIDs.Add(streetGUID);
                            return Guid.Parse(streetGUID);
                        },
                        reader => Guid.Parse(reader.GetAttribute("PARENTGUID")),
                        reader => reader.GetAttribute("OFFNAME"),
                        reader => reader.GetAttribute("FORMALNAME")
                    };
                    watch.Restart();
                    writer.DestinationTableName = "Street";
                    writer.WriteToServer(new FIASXMLReader(XmlReader.Create(objectsPath), streetPredicate, streetParser));
                    watch.Stop();
                    Console.WriteLine("Улицы записаны, времени затрачено - {0}", watch.ElapsedMilliseconds);
                }

                townGUIDs.Clear();
            }
        }
    }
}
