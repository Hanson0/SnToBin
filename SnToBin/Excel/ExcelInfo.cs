//using Production.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Windows;

namespace Production.Excel
{
    class ExcelInfo
    {
        private static string fileConfig;          //配置文件路径
        private static string folderExcel;         //Excel存放路径
        private static bool flagExcelExportEnable;  //导出箱号使能

        private static string excelNamePrefix;     //Excel名称前缀
        private static string excelNameSuffix;     //Excel名称后缀


        public static string FolderExcel
        {
            get
            {
                return folderExcel;
            }

            set
            {
                folderExcel = value;
            }
        }

        public static string ExcelNamePrefix
        {
            get
            {
                return excelNamePrefix;
            }

            set
            {
                excelNamePrefix = value;
            }
        }

        public static string ExcelNameSuffix
        {
            get
            {
                return excelNameSuffix;
            }

            set
            {
                excelNameSuffix = value;
            }
        }


        /// <summary>
        /// 读配置文件
        /// </summary>
        public static void ReadConfig()
        {
            fileConfig = ConfigInfo.ConfigPath;
            StringBuilder stringBuilder = new StringBuilder(256);

            Win32API.GetPrivateProfileString("Excel", "ExcelPath", "", stringBuilder, 256, fileConfig);
            folderExcel = stringBuilder.ToString().Trim();
            if (!Directory.Exists(folderExcel))
            {
                Directory.CreateDirectory(folderExcel);
            }

            //Excel
            Win32API.GetPrivateProfileString("Excel", "ExcelNamePrefix", "", stringBuilder, 256, fileConfig);
            excelNamePrefix = stringBuilder.ToString();
            Win32API.GetPrivateProfileString("Excel", "ExcelNameSuffix", "", stringBuilder, 256, fileConfig);
            excelNameSuffix = stringBuilder.ToString();
        }

    }
}
