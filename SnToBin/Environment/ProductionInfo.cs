using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows;
namespace Production.ProductionTest
{
    static class ProductionInfo
    {
        public enum SystemType
        {
            Offline=0,
            GSMMES = 1,
            iMES = 2,
        }

        private static string configPath = ConfigInfo.ConfigPath;            //配置文件路径
        //manufature Info
        private static string customerName;             //客户名称
        private static string productModel;             //产品型号
        private static string planCode;                 //计划单号
    
        private static string procedure;                   //工序
        private static string station;                     //工位

        public static SystemType Type
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                Win32API.GetPrivateProfileString("System", "Type", "", stringBuilder, 256, configPath);
                return (SystemType)Enum.Parse(typeof(SystemType), stringBuilder.ToString());
            }
        }

        public static int TimeOut
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                Win32API.GetPrivateProfileString("Time", "TimeOut", "", stringBuilder, 256, configPath);
                return int.Parse(stringBuilder.ToString());
            }
        }

        public static string CustomerName
        {
            get
            {
                return customerName;
            }

            set
            {
                customerName = value;
                if (!string.IsNullOrEmpty(value))
                {
                    Win32API.WritePrivateProfileString("ProductionInfo", "CustomerName", customerName, configPath);
                }
            }
        }

        public static string ProductModel
        {
            get
            {
                return productModel;
            }

            set
            {
                productModel = value;
                if (!string.IsNullOrEmpty(value))
                {
                    Win32API.WritePrivateProfileString("ProductionInfo", "ProductModel", productModel, configPath);
                }
            }
        }

        public static string PlanCode
        {
            get
            {
                return planCode;
            }

            set
            {
                planCode = value;
                if (!string.IsNullOrEmpty(value))
                {
                    Win32API.WritePrivateProfileString("ProductionInfo", "PlanCode", planCode, configPath);
                }
            }
        }

        public static string Procedure
        {
            get
            {
                return procedure;
            }

            set
            {
                procedure = value;
            }
        }

        public static string Station
        {
            get
            {
                return station;
            }

            set
            {
                station = value;
            }
        }



        public static void ReadConfig()
        {
            StringBuilder stringBuilder = new StringBuilder();
            configPath = ConfigInfo.ConfigPath;

            //产品型号
            Win32API.GetPrivateProfileString("ProductionInfo", "ProductModel", "", stringBuilder, 256, configPath);
            productModel = stringBuilder.ToString().Trim();
            Win32API.GetPrivateProfileString("ProductionInfo", "CustomerName", "", stringBuilder, 256, configPath);
            customerName = stringBuilder.ToString().Trim();
            Win32API.GetPrivateProfileString("ProductionInfo", "PlanCode", "", stringBuilder, 256, configPath);
            planCode = stringBuilder.ToString().Trim();

            Win32API.GetPrivateProfileString("ProductionInfo", "Procedure", "", stringBuilder, 256, configPath);
            procedure = stringBuilder.ToString().Trim();
            Win32API.GetPrivateProfileString("ProductionInfo", "Station", "", stringBuilder, 256, configPath);
            station = stringBuilder.ToString().Trim();
        }

    }
}
