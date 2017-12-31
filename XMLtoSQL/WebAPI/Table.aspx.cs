namespace WebAPI
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    public partial class Table : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Request.Params.Keys[0] != "reg")
            {
                return;
            }

            var conditions = new List<string>();
            var regionNumber = this.Request.Params[0].ToString();
            if (regionNumber != string.Empty)
            {
                conditions.Add($"[Region].[REGIONCODE] = {regionNumber}");
            }

            var tableNames = new string[] { "Rayon", "Town", "Street" };
            var outputNames = new string[] { "RayonName", "TownName", "StreetName" };
            var tableCount = tableNames.Length;
            var names = new string[tableCount];
            var officials = new bool[tableCount];
            var selects = new string[tableCount];
            for (var i = 0; i < tableCount; i++)
            {
                officials[i] = bool.Parse(this.Request.Params[1 + (i * 2)].ToString());
                selects[i] = $"[{tableNames[i]}].[OFFNAME] AS {outputNames[i]}";
                names[i] = this.Request.Params[2 + (i * 2)].ToString();
                if (names[i] != string.Empty)
                {
                    if (officials[i])
                    {
                        conditions.Add($"[{tableNames[i]}].[OFFNAME] = N'{names[i]}'");
                    }
                    else
                    {
                        conditions.Add($"[{tableNames[i]}].[FORMALNAME] = N'{names[i]}'");
                    }
                }
            }

            var useHouse = bool.Parse(this.Request.Params[7].ToString());
            if (useHouse)
            {
                this.lblHouseNumber.Enabled = true;
                this.lblPostalCode.Enabled = true;
                var houseNum = this.Request.Params[8].ToString();
                if (houseNum != string.Empty)
                {
                    conditions.Add($"[House].[HOUSENUM] = N'{houseNum}'");
                }

                var postal = this.Request.Params[9].ToString();
                if (postal != string.Empty)
                {
                    conditions.Add($"[House].[POSTALCODE] = N'{postal}'");
                }
            }
            else
            {
                this.lblHouseNumber.Enabled = false;
                this.lblPostalCode.Enabled = false;
            }

            var mainQuerry = "SELECT " +
                $"[Region].[REGIONCODE] AS RegionCode, " +
                $"[Region].[OFFNAME] AS RegionName, " +
                $"{selects[0]}, " +
                $"{selects[1]}, " +
                $"{selects[2]}";
            if (useHouse)
            {
                mainQuerry += $", " +
                    $"[House].[HOUSENUM] AS HouseNumber, " +
                    $"[House].[POSTALCODE] AS PostalIndex";
            }

            mainQuerry += " FROM [Region]";
            for (var i = 0; i < tableCount; i++)
            {
                mainQuerry += $" INNER JOIN " +
                    $"{tableNames[i]}";
            }

            if (useHouse)
            {
                mainQuerry += " INNER JOIN " +
                    "HOUSE" +
                    " ON [House].[AOGUID] = [Street].[AOGUID]";
            }

            for (var i = tableCount - 1; i > 0; i--)
            {
                mainQuerry += $" ON [{tableNames[i]}].[PARENTGUID] = [{tableNames[i - 1]}].[AOGUID]";
            }

            mainQuerry += " ON [Rayon].[PARENTGUID] = [Region].[AOGUID]";

            if (conditions.Count > 0)
            {
                mainQuerry += $" WHERE ({conditions[0]})";
                for (var i = 1; i < conditions.Count; i++)
                {
                    mainQuerry += $" AND ({conditions[i]})";
                }
            }

            var connection = new SqlConnection();
            connection.ConnectionString = @"Data Source=DESKTOP-17UB0HN\SQLEXPRESS;Initial Catalog=RestrictedFIAS;Persist Security Info=True;User ID=sa;Password=superadmin;Context Connection=False";
            connection.Open();
            var command = new SqlCommand(mainQuerry, connection);
            var result = command.ExecuteReader();
            Cache[Request.UserHostAddress] = result;
            Response.Redirect("Table.aspx");
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            var result = (SqlDataReader)Cache[Request.UserHostAddress];
            if (result.Read())
            {
                this.lblRegionCode.Text = result[0].ToString();
                this.lblRegion.Text = result[1].ToString();
                this.lblRayon.Text = result[2].ToString();
                this.lblTown.Text = result[3].ToString();
                this.lblStreet.Text = result[4].ToString();
                if (result.FieldCount >5)
                {
                    this.lblHouseNumber.Text = result[5].ToString();
                    this.lblPostalCode.Text = result[6].ToString();
                }
            }
            else
            {
                this.Button2.Text = "Результаты кончились";
                this.Button2.Enabled = false;
            }
        }
    }
}