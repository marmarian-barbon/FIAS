using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebAPI
{
    public partial class Table : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
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
                        conditions.Add($"[{tableNames[i]}].[OFFNAME] = {names[i]}");
                    }
                    else
                    {
                        conditions.Add($"[{tableNames[i]}].[FORMALNAME] = {names[i]}");
                    }
                }
            }

            var useHouse = bool.Parse(this.Request.Params[7].ToString());
            if (useHouse)
            {
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
                for (var i=1; i < conditions.Count; i++)
                {
                    mainQuerry += $" AND ({conditions[i]})";
                }
            }

            this.Label1.Text = mainQuerry;
        }
    }
}