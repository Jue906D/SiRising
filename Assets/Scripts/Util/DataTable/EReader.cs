using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Data;
using Excel;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;


public class BuildExcelWindow : EditorWindow
{
    [MenuItem("SimpleTables/Manager Window", priority = 100)]
    public static void ShowReadExcelWindow()
    {
        BuildExcelWindow window = GetWindow<BuildExcelWindow>(true);
        window.Show();
        window.minSize = new Vector2(300, 100);
    }

    //Excel读取路径，绝对路径，放在Assets同级路径
    private static string excelReadAbsolutePath;
    public ConfigSO config;

    private void Awake()
    {
        titleContent.text = "Excel配置表读取";
        excelReadAbsolutePath = Application.dataPath.Replace("Assets", "Excel") + "/Occuption.xlsx";
        config = Resources.Load<ConfigSO>("ScriptableObject/Config");
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }


    private void OnGUI()
    {
        GUILayout.Space(10);

        //展示路径
        GUILayout.BeginHorizontal(GUILayout.Height(40));
        if (GUILayout.Button("刷新数据"))
        {
            
            //读取Excel
            FileStream stream = File.Open(excelReadAbsolutePath, FileMode.Open, FileAccess.Read);
            //解析Excel
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            Debug.Log("开始解析Excel : " + excelReader.Name);

            DataTable token = excelReader.AsDataSet().Tables[0]; //

            int index = 0;

            for (int i = 3; i <= 21; i++) //逐行
            {
                index = Convert.ToInt32(token.Rows[i][0].ToString());

                if (!int.TryParse(token.Rows[i][2].ToString(), out config.Fire[index]))
                {
                    config.Fire[index] = 0;
                }
                if (!int.TryParse(token.Rows[i][3].ToString(), out config.Water[index]))
                {
                    config.Water[index] = 0;
                }
                if (!int.TryParse(token.Rows[i][4].ToString(), out config.Grass[index]))
                {
                    config.Grass[index] = 0;
                }
                if (!int.TryParse(token.Rows[i][5].ToString(), out config.Earth[index]))
                {
                    config.Earth[index] = 0;
                }
            }

            DataTable attri = excelReader.AsDataSet().Tables[1]; //
            index = 0;

            for (int i = 2; i <= 20; i++) //逐行
            {
                index = Convert.ToInt32(attri.Rows[i][0].ToString());

                if (!int.TryParse(attri.Rows[i][7].ToString(), out config.Attack[index]))
                {
                    config.Attack[index] = 0;
                }
                if (!int.TryParse(attri.Rows[i][8].ToString(), out config.Health[index]))
                {
                    config.Health[index] = 0;
                }
                if (!int.TryParse(attri.Rows[i][9].ToString(), out config.Recharge[index]))
                {
                    config.Recharge[index] = 0;
                }
                if (!int.TryParse(attri.Rows[i][10].ToString(), out config.AttackSpeed[index]))
                {
                    config.AttackSpeed[index] = 0;
                }

                if (!int.TryParse(attri.Rows[i][17].ToString(), out config.AttackMode[index])) //attack mode
                {
                    config.AttackMode[index] = 0;
                }
            }

            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.Label("Excel位置");
        GUILayout.BeginHorizontal("Box", GUILayout.Height(25));
        excelReadAbsolutePath = EditorGUILayout.TextField(excelReadAbsolutePath);
        if (GUILayout.Button("选择", GUILayout.Width(100)))
        {
            string tmp = excelReadAbsolutePath;
            tmp = EditorUtility.OpenFilePanel("选择Excel位置", tmp, "");
            if (!string.IsNullOrEmpty(tmp))
            {
                excelReadAbsolutePath = tmp;
            }
        }

        GUILayout.EndHorizontal();

    }
}

