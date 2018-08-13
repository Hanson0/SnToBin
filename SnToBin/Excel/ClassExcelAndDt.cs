using NPOI.HSSF.UserModel;
//using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.OleDb;
using Production.Excel;
using System.IO;
using System.Windows.Forms;

namespace JiaHao.ExcelHelp
{
    class ClassExcelAndDt
    {
        public DataTable ExcelToDS(string Path, string order)
        {
            string tableName="";
            bool IsContainOrder = false;
            string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + Path + ";" + "Extended Properties=Excel 8.0;";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();

            DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {
                tableName = schemaTable.Rows[i][2].ToString();//"TABLE NAME"

                if (tableName.Contains(order))
                {
                    IsContainOrder = true;
                    break;
                }
                
            }
            if (IsContainOrder==false)
            {
                return null;
            }
            string strExcel = "";
            strExcel = string.Format("select * from [{0}]", tableName);

            //if (isStandardFormat)
            //{
            //    strExcel = "select * from [sheet1$]";//获取整张表
            //    //strExcel = "select * ";//获取整张表
            //}
            //else
            //{
            //    strExcel = "select * from [sheet$]";//获取整张表
            //}

            OleDbDataAdapter myCommand = new OleDbDataAdapter(strExcel, strConn);
            DataTable dt = new DataTable();
            myCommand.Fill(dt);

            conn.Close();
            return dt;

        }

        public int InsertRow(string Path, string strTableName,string sn,string imei,string eid,string iccid)
        {
            int ret = -1;
            string tableName = "";
            bool IsContainOrder = false;
            string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + Path + ";" + "Extended Properties=Excel 8.0;";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();

            DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {
                tableName = schemaTable.Rows[i][2].ToString();//"TABLE NAME"

                if (tableName.Contains(strTableName))
                {
                    IsContainOrder = true;
                    break;
                }

            }
            if (IsContainOrder == false)
            {
                return ret;
            }
            string strExcel = "";
            strExcel = string.Format("insert into [{0}] (SN,IMEI,EID,ICCID) values ({1},{2},{3},{4})", tableName, sn, imei, eid, iccid);

            //if (isStandardFormat)
            //{
            //    strExcel = "select * from [sheet1$]";//获取整张表
            //    //strExcel = "select * ";//获取整张表
            //}
            //else
            //{
            //    strExcel = "select * from [sheet$]";//获取整张表
            //}

            OleDbDataAdapter myCommand = new OleDbDataAdapter(strExcel, strConn);
            DataTable dt = new DataTable();
            myCommand.Fill(dt);

            conn.Close();

            ret = 0;
            return ret;
        }

    }

    class ExcelHelper
    {
        //path
        private static string folderExcel;                          //Excel文件夹路径

        //EXCEL文件信息
        protected static string excelNamePrefix;                       //文件名起始
        protected static string excelNameSuffix;                       //文件名结束
        private static string excelFullName;                            //根据箱号的文件名
        private static string excelSNFullName;                            //根据箱号的文件名
        
        protected static List<string> excelColumnList;              //Excel列名集合

        private static ExcelHelper excelHelper;
        //private static List<TrayInfo> trayInfoList;

        public class TrayInfoInExcel
        {
            public int num;
            public string mac;
            public string sn;
            //public string eid;
            //public string iccid;
            public string cuatomerName;
        }

        private ExcelHelper()
        {
            ExcelInfo.ReadConfig();
            folderExcel = ExcelInfo.FolderExcel;
            excelNamePrefix = ExcelInfo.ExcelNamePrefix;
            excelNameSuffix = ExcelInfo.ExcelNameSuffix;

            excelColumnList = new List<string>() { "序号", "MAC", "SN","客户名称" };
            //trayInfoList = new List<TrayInfo>() { new TrayInfo(){ num = 1,sn = "111",imei="222",
            //    eid = "333",iccid = "444",cuatomerName = "中移物联"} };
        }



        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static ExcelHelper GetExcelHelperInstance()
        {
            if (excelHelper == null)
            {
                excelHelper = new ExcelHelper();
            }

            excelFullName = folderExcel + excelNamePrefix + "本机过站记录表" + excelNameSuffix;

            return excelHelper;
        }

        public static ExcelHelper GetExcelHelperInstance(bool isok)
        {
            if (isok)
            {
                
            }
            if (excelHelper == null)
            {
                excelHelper = new ExcelHelper();
            }

            excelSNFullName = folderExcel + excelNamePrefix + "序列号" + ".xls";

            return excelHelper;
        }



        /// <summary>
        /// 在第0行填列名
        /// </summary>
        /// <param name="sheet"></param>
        private void SetColumn(ref ISheet sheet, ICellStyle style)
        {
            IRow sheetRow = sheet.CreateRow(0);
            sheetRow.RowStyle = style;
            for (int i = 0; i < excelColumnList.Count; i++)
            {
                ICell cell = sheetRow.CreateCell(i);                    //创建Excel的ICell单元格实例
                cell.SetCellValue(excelColumnList[i]);                  //赋值
                cell.CellStyle = style;                                 //单元格格式
                //sheet.AutoSizeColumn(0);                              //自适应列宽              
            }
        }



        /// <summary>
        /// 从第1行填值
        /// </summary>
        /// <param name="sheet"></param>
        private void SetValueByRow(ref ISheet sheet, TrayInfoInExcel trayInfoInExcel, ICellStyle style)
        {
            int nextRowNum = sheet.LastRowNum + 1;
            IRow sheetRow = sheet.CreateRow(nextRowNum);

            int column = 0;
            ICell cell = sheetRow.CreateCell(column);               //创建Excel的ICell单元格实例
            cell.SetCellValue(nextRowNum);
            cell.CellStyle = style;                                 //单元格格式

            //column++;
            //cell = sheetRow.CreateCell(column);                     //创建Excel的ICell单元格实例
            //cell.SetCellValue(trayInfoInExcel.sn);                  //赋值 sn
            //cell.CellStyle = style;                                 //单元格格式

            column++;
            cell = sheetRow.CreateCell(column);                     //创建Excel的ICell单元格实例
            cell.SetCellValue(trayInfoInExcel.mac);                //赋值 imei
            cell.CellStyle = style;                                 //单元格格式

            column++;
            cell = sheetRow.CreateCell(column);                     //创建Excel的ICell单元格实例
            //cell.SetCellValue(casingInfoList[i].station);           //赋值 eid
            cell.SetCellValue(trayInfoInExcel.sn);
            cell.CellStyle = style;                                 //单元格格式

            //column++;
            //cell = sheetRow.CreateCell(column);                     //创建Excel的ICell单元格实例
            //cell.SetCellValue(trayInfoInExcel.iccid);               //赋值 iccid
            //cell.CellStyle = style;                                 //单元格格式

            column++;
            cell = sheetRow.CreateCell(column);                     //创建Excel的ICell单元格实例
            cell.SetCellValue(trayInfoInExcel.cuatomerName);
            //cell.SetCellValue(casingInfoList[i].factory);         //赋值 客户名称
            cell.CellStyle = style;                                 //单元格格式

        }


        /// <summary>
        /// 设置sheet自动对齐
        /// </summary>
        /// <param name="sheet"></param>
        private void SetSheetAutoSize(ref ISheet sheet)
        {
            for (int i = 0; i < excelColumnList.Count; i++)
            {
                sheet.AutoSizeColumn(i);
            }
        }



        /// <summary>
        /// 创建Excel并设置列标题
        /// </summary>
        /// <returns></returns>
        private int CreatExcel()
        {
            int ret = -1;

            HSSFWorkbook workbook = new HSSFWorkbook();                         //实例化Excel工作薄类
            ISheet sheet = workbook.CreateSheet("Sheet1");                      //创建Excel的Sheet实例
            workbook.CreateSheet("Sheet2");
            workbook.CreateSheet("Sheet3");

            ICellStyle cellstyle = workbook.CreateCellStyle();                  //设置垂直居中
            cellstyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            SetColumn(ref sheet, cellstyle);

            try
            {
                using (FileStream fileStream = File.OpenWrite(excelFullName))   //实例化文件流
                {
                    workbook.Write(fileStream);                                 //文件流写入工作薄实例
                    workbook.Close();
                    ret = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return ret;
        }

        public int ExportExcelOneByOne(TrayInfoInExcel trayInfoInExcel)
        {
            int ret = -1;

            //获取excel文件信息
            FileInfo file = new FileInfo(excelFullName);
            //检测文件是否存在
            if (!File.Exists(excelFullName))
            {
                if (CreatExcel() != 0)
                {
                    return ret;
                }
            }
            else
            {
                file.IsReadOnly = false;
            }


            HSSFWorkbook workbook = null;
            try
            {
                using (FileStream fs1 = File.OpenRead(excelFullName))
                {
                    workbook = new HSSFWorkbook(fs1);                         //实例化Excel工作薄类
                    ISheet sheet = workbook.GetSheet("Sheet1");

                    ICellStyle cellstyle = workbook.CreateCellStyle();                  //设置垂直居中
                    cellstyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    //SetValueByRow中自动设序号 +1
                    //int rowCount = sheet.LastRowNum;
                    //trayInfoInExcel.num = rowCount;
                    //设定值
                    SetValueByRow(ref sheet, trayInfoInExcel, cellstyle);
                    SetSheetAutoSize(ref sheet);
                    fs1.Close();

                    using (FileStream fs2 = File.OpenWrite(excelFullName))
                    {
                        workbook.Write(fs2);
                        workbook.Close();
                    }
                    ret = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (workbook != null)
                {
                    workbook.Close();
                }
                file.IsReadOnly = false;
            }

            return ret;
        }

        public int SearchImei(string imei)
        {
            int ret = 0;
            //获取excel文件信息
            FileInfo file = new FileInfo(excelFullName);
            //检测文件是否存在
            if (!File.Exists(excelFullName))
            {
                if (CreatExcel() != 0)
                {
                    return ret;
                }
            }
            else
            {
                file.IsReadOnly = false;
            }


            HSSFWorkbook workbook = null;
            try
            {
                using (FileStream fs1 = File.OpenRead(excelFullName))
                {
                    workbook = new HSSFWorkbook(fs1);                         //实例化Excel工作薄类
                    fs1.Close();
                    ISheet sheet = workbook.GetSheet("Sheet1");

                    //ICellStyle cellstyle = workbook.CreateCellStyle();                  //设置垂直居中
                    //cellstyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    //设序号
                    int coloumNum=sheet.GetRow(0).PhysicalNumberOfCells;
                    int rowCount = sheet.LastRowNum;

                    //string strCell = sheet.GetRow(0).GetCell(0).ToString();


                    for (int k = 1; k < rowCount+1; k++)
                    {
                        //for (var j = 0; j < coloumNum; j++)
                        //{
                            string strCell = sheet.GetRow(k).GetCell(1).ToString();
                        //sheet.RemoveRow()
                            if (strCell == imei)
                            {
                                ret = -1;
                                break;
                            }
                        //}
                    }

                    //设定值

                    //using (FileStream fs2 = File.OpenWrite(excelFullName))
                    //{
                    //    workbook.Write(fs2);
                    //    workbook.Close();
                    //}
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                ret = -1;
            }
            finally
            {
                if (workbook != null)
                {
                    workbook.Close();
                }
            }
            return ret;

        }

        public static int rowCount;
        public string GetSn()
        {
            int ret = 0;
            string strCell = "";
            HSSFWorkbook workbook = null;
            try
            {
                using (FileStream fs1 = File.OpenRead(excelSNFullName))
                {
                    //XSSFWorkbook
                    workbook = new HSSFWorkbook(fs1);                         //实例化Excel工作薄类
                    fs1.Close();
                    ISheet sheet = workbook.GetSheet("Sheet1");

                    //ICellStyle cellstyle = workbook.CreateCellStyle();                  //设置垂直居中
                    //cellstyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    //设序号
                    int coloumNum = sheet.GetRow(0).PhysicalNumberOfCells;
                    rowCount = sheet.LastRowNum;

                    strCell = sheet.GetRow(rowCount).GetCell(0).ToString();


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                strCell ="";
            }
            finally
            {
                if (workbook != null)
                {
                    workbook.Close();
                }

            }
            return strCell;

        }

        public string ReMoveRow()
        {
            int ret = 0;
            string strCell = "";
            HSSFWorkbook workbook = null;
            try
            {
                using (FileStream fs1 = new FileStream(excelSNFullName, FileMode.Open, FileAccess.Write))
                {
                    workbook = new HSSFWorkbook(fs1);                         //实例化Excel工作薄类
                    fs1.Close();
                    ISheet sheet = workbook.GetSheet("Sheet1");

                    //ICellStyle cellstyle = workbook.CreateCellStyle();                  //设置垂直居中
                    //cellstyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    //设序号
                    //int coloumNum = sheet.GetRow(0).PhysicalNumberOfCells;
                    //rowCount = sheet.LastRowNum;

                    //strCell = sheet.GetRow(rowCount).GetCell(0).ToString();
                    sheet.ShiftRows(1, sheet.LastRowNum, -1);

                    

                    workbook.Write(fs1);
                    fs1.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                strCell = "";
            }
            finally
            {
                if (workbook != null)
                {
                    workbook.Close();
                }

            }
            return strCell;

        }
        /// <summary>
        /// 当发生上报服务器信息异常时，删掉最后EXCEl一行
        /// </summary>
        /// <returns></returns>
        public int DeleteLastRow()
        {
            int ret = -1;
            HSSFWorkbook workbook = null;

            //获取excel文件信息
            FileInfo file = new FileInfo(excelSNFullName);
            file.IsReadOnly = false;
            try
            {
                using (FileStream fs1 = File.OpenRead(excelSNFullName))
                {
                    workbook = new HSSFWorkbook(fs1);                         //实例化Excel工作薄类
                    ISheet sheet = workbook.GetSheet("Sheet1");
                    sheet.RemoveRow(sheet.GetRow(sheet.LastRowNum));
                    fs1.Close();

                    using (FileStream fs2 = File.OpenWrite(excelSNFullName))
                    {
                        workbook.Write(fs2);
                    }
                    ret = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                workbook.Close();
                file.IsReadOnly = false;
            }

            return ret;
        }
    }

}
