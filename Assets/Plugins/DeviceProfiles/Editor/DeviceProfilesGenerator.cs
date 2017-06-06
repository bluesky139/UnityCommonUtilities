using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public class DeviceProfilesGenerator
{
    [MenuItem("Common/Device Profiles/Generate", priority = 80)]
    public static void Generate()
    {
        Debug.Log("Generating device profiles json...");
		var path = common.EditorEnv.dstUnityProjectPlugins + "/DeviceProfiles/Editor/DeviceProfiles.xlsx";
		FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        XSSFWorkbook workbook = new XSSFWorkbook(stream);
        ISheet sheet = workbook.GetSheetAt(0);
        IRow row;

        JSONClass json = new JSONClass();
        JSONArray jCommonArray = new JSONArray();
        try
        {
            row = sheet.GetRow(39);
            if (row.Cells[0].StringCellValue != "Quality")
            {
                throw new Exception("First cell of sheet " + sheet.SheetName + " is " + row.Cells[0].StringCellValue);
            }

            List<ICell> cells = row.Cells;
            for (int i = 1; i < cells.Count; ++i)
            {
                string key = cells[i].ToString();
                JSONClass jCommon = new JSONClass();
                jCommon.Add("target", new JSONData(key));
                for (int j = 0; j < 3; ++j)
                {
                    row = sheet.GetRow(40 + j);
                    string value = row.Cells[i].ToString();
                    jCommon.Add("" + j, new JSONData(value));
                }
                jCommonArray.Add(jCommon);
            }
        }
        catch (Exception e)
        {
            throw new Exception("Can't read first row of sheet " + sheet.SheetName + ", " + e.Message);
        }
        json.Add("commons", jCommonArray);

        try
        {
            row = sheet.GetRow(45);
            if (row.Cells[0].StringCellValue != "MatchReg")
            {
                throw new Exception("First cell of sheet " + sheet.SheetName + " is " + row.Cells[0].StringCellValue);
            }
        }
        catch (Exception e)
        {
            throw new Exception("Can't read first row of sheet " + sheet.SheetName + ", " + e.Message);
        }

        JSONArray jRuleArray = new JSONArray();
        JSONArray jSpecArray = new JSONArray();
        List<string> usedSystemInfo = new List<string>();
        bool isSpec = false;
        Dictionary<int, string> col2common = new Dictionary<int, string>();
        for (int i = 46; i <= sheet.LastRowNum; i++)
        {
            try
            {
                row = sheet.GetRow(i);
                if (row == null || row.FirstCellNum != 0)
                {
                    continue;
                }
                if (row.Cells[0].ToString() == "[Below are specific devices]")
                {
                    isSpec = true;
                    for (int j = 5; j < row.LastCellNum; ++j)
                    {
                        col2common.Add(j, row.GetCell(j).ToString());
                    }
                    continue;
                }
                if (row.Cells[0].ToString().StartsWith("[Below are "))
                {
                    continue;
                }

                string reg     = row.Cells[0].ToString();
                string target  = row.Cells[1].ToString();
                string compare = row.Cells[2].ToString();
                string value   = row.Cells[3].ToString();
                string result_ = row.Cells[4].ToString();
                JSONClass node = new JSONClass();
                node.Add("reg",    new JSONData(reg));
                node.Add("target", new JSONData(target));
                node.Add("compare",new JSONData(compare));
                node.Add("value",  new JSONData(value));
                int result = 0;
                switch (result_)
                {
                case "Fastest":
                    result = 0;
                    break;
                case "Good":
                    result = 1;
                    break;
                case "Fantastic":
                    result = 2;
                    break;
                default:
                    throw new Exception("Unknown result " + result_);
                }
                node.Add("result", new JSONData(result));

                for (int j = 5; j < row.LastCellNum; ++j)
                {
                    ICell cell = row.GetCell(j);
                    if (cell == null || string.IsNullOrEmpty(cell.ToString().Replace(" ", "")))
                        continue;
                    node.Add(col2common[j], cell.ToString());
                }

                if (isSpec)
                    jSpecArray.Add(node);
                else
                    jRuleArray.Add(node);

                if (!usedSystemInfo.Contains(target))
                    usedSystemInfo.Add(target);
            }
            catch (Exception e)
            {
				throw new Exception("Error at row " + (i + 1) + ", " + e.Message);
			}
        }
        json.Add("rules", jRuleArray);
        json.Add("specs", jSpecArray);

		var bytes = JSON.CreateBytesFrom(json, compressed: true);
		File.WriteAllBytes(common.EditorEnv.dstUnityProjectPlugins + "/DeviceProfiles/Resources/DeviceProfiles.bytes", bytes);

		Debug.Log("Device profiles json is created.");


        // Generate used SystemInfo methods and other methods to be referenced, otherwise Unity will strip them on il2cpp build.
        string gen = "using UnityEngine;\n\n";
        gen += "class DeviceProfiles_Gen\n";
        gen += "{\n";
        gen += "\tvoid a()\n";
        gen += "\t{\n";

        int k = 0;
        foreach (string used in usedSystemInfo)
        {
            gen += "\t\tvar a" + k + " = SystemInfo." + used + ";\n";
            ++k;
        }
        gen += "\n";

        k = 0;
        foreach (JSONClass jCommon in jCommonArray)
        {
            string target = jCommon["target"].Value;
            gen += "\t\t" + target + " = " + target + ";\n";
            ++k;
        }
        gen += "\n";

        gen += "\t}\n";
        gen += "}\n";
		var outpath = common.EditorEnv.dstUnityProjectAssets + "/DeviceProfiles_Gen.cs";
		File.WriteAllText(outpath, gen);
		AssetDatabase.Refresh();
    }
}
