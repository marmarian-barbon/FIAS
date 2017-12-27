using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TryParse
{
    class Program
    {
        static void Main(string[] args)
        {
            var doc = new XmlDocument();
            var path1 = @"F:\Browser\fias_xml\AS_CENTERST_20171214_aaebe8b4-4476-4772-8f54-5a017b471cde.XML";
            var path2 = @"F:\Browser\fias_xml\AS_HOUSE_20171214_ad9f8dea-850f-4f70-9f54-1e05d9ecde76.XML";
            var path3 = @"F:\Browser\fias_xml\AS_ADDROBJ_20171214_4af453b1-c874-4a55-935d-91aa70546984.XML";
            var path4 = @"F:\Browser\fias_xml\AS_SOCRBASE_20171214_f95c3d3c-fbbd-4dc6-99e5-a35fb4505b82.XML";

            var connectionString = new SqlConnectionStringBuilder
            {
                Authentication = SqlAuthenticationMethod.SqlPassword,
                DataSource = @".\SQLEXPRESS",
                InitialCatalog = "RestrictedFIAS",
                UserID = "sa",
                Password = "superadmin",
                TrustServerCertificate = true
            };

            var connection = new SqlConnection(connectionString.ConnectionString);
            connection.Open();
            Console.WriteLine(connection.State);
            connection.Close();
            /*using (var reader = XmlReader.Create(path3))
            {
                var attributes = new List<List<string[]>>();
                while (reader.Read())
                {
                    if (reader.GetAttribute("LEVEL") == "1" && reader.GetAttribute("ACTSTATUS") == "1")
                    {
                        Console.WriteLine(reader.GetAttribute("HOUSENUM"));
                        var record = new List<string[]>();
                        for (var i = 0; i < reader.AttributeCount; i++)
                        {
                            var attribute = new string[2];
                            reader.MoveToAttribute(i);
                            attribute[0] = reader.Name;
                            attribute[1] = reader.Value;
                            record.Add(attribute);
                        }

                        Console.WriteLine(attributes.Count);
                        attributes.Add(record);
                    }
                }

                using (var file = new StreamWriter(@"F:\Regions.txt"))
                {
                    foreach (var record in attributes)
                    {
                        for (var i = 0; i < record.Count; i++)
                        {
                            file.WriteLine(record[i][0] + " = " + record[i][1]);
                        }
                    }
                }
            }*/
        }
    }
}
