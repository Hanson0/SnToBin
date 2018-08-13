using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MyControls
{
    /// <summary>
    /// 读写INI文件的类。
    /// </summary>
    public class INIHelper
    {
        public string path = "";

        #region
        // 读写INI文件相关。
        [DllImport ( "kernel32.dll" , EntryPoint = "WritePrivateProfileString" , CharSet = CharSet.Ansi )]
        public static extern bool WritePrivateProfileString ( string section , string key , string val , string filePath );
        [DllImport ( "kernel32.dll" , EntryPoint = "GetPrivateProfileString" , CharSet = CharSet.Ansi )]
        public static extern int GetPrivateProfileString ( string section , string key , string def , StringBuilder retVal , int size , string filePath );

        [DllImport ( "kernel32.dll" , EntryPoint = "GetPrivateProfileSectionNames" , CharSet = CharSet.Ansi )]
        public static extern int GetPrivateProfileSectionNames ( IntPtr lpszReturnBuffer , int nSize , string filePath );

        [DllImport ( "KERNEL32.DLL " , EntryPoint = "GetPrivateProfileSection" , CharSet = CharSet.Ansi )]
        public static extern int GetPrivateProfileSection ( string lpAppName , byte[ ] lpReturnedString , int nSize , string filePath );
        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="filename"></param>
        public INIHelper ( string filename )
        {
            path = filename;
        }

        /// <summary>
        /// 向INI写入数据。
        /// </summary>
        /// <PARAM name="Section">节点名。</PARAM>
        /// <PARAM name="Key">键名。</PARAM>
        /// <PARAM name="Value">值名。</PARAM>
        public bool Write ( string Section , string Key , string Value )
        {
            return WritePrivateProfileString ( Section , Key , Value , path );
        }
        /// <summary>
        /// 向INI写入数据。
        /// </summary>
        /// <PARAM name="Section">节点名。</PARAM>
        /// <PARAM name="Key">键名。</PARAM>
        /// <PARAM name="Value">值名。</PARAM>
        public static bool Write ( string Section , string Key , string Value , string path )
        {
            return WritePrivateProfileString ( Section , Key , Value , path );
        }


        /// <summary>
        /// 读取INI数据。
        /// </summary>
        /// <PARAM name="Section">节点名。</PARAM>
        /// <PARAM name="Key">键名。</PARAM>
        /// <PARAM name="Path">值名。</PARAM>
        /// <returns>相应的值。</returns>
        public string Read ( string Section , string Key )
        {
            StringBuilder temp = new StringBuilder ( 255 );
            int i = GetPrivateProfileString ( Section , Key , "" , temp , 255 , path );
            return temp.ToString ( );
        }
        public Int32 GetInt32 ( string Section , string Key )
        {
            try
            {
                return Convert.ToInt32 ( Read ( Section , Key ) );
            }
            catch
            {
                return 0;
            }
        }
        public static Int32 GetInt32 ( string Section , string Key , string path )
        {
            try
            {
                return Convert.ToInt32 ( Read ( Section , Key , path ) );
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 读取INI数据。
        /// </summary>
        /// <PARAM name="Section">节点名。</PARAM>
        /// <PARAM name="Key">键名。</PARAM>
        /// <PARAM name="Path">值名。</PARAM>
        /// <returns>相应的值。</returns>
        public static string Read ( string Section , string Key , string path )
        {
            StringBuilder temp = new StringBuilder ( 255 );
            int i = GetPrivateProfileString ( Section , Key , "" , temp , 255 , path );
            return temp.ToString ( );
        }

        /// <summary>
        /// 读取一个ini里面所有的节
        /// </summary>
        /// <param name="sections"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public int GetAllSectionNames ( out string[ ] sections , string path )
        {
            int MAX_BUFFER = 32767;
            IntPtr pReturnedString = Marshal.AllocCoTaskMem ( MAX_BUFFER );
            int bytesReturned = GetPrivateProfileSectionNames ( pReturnedString , MAX_BUFFER , path );
            if ( bytesReturned == 0 )
            {
                sections = null;
                return -1;
            }
            string local = Marshal.PtrToStringAnsi ( pReturnedString , ( int ) bytesReturned ).ToString ( );
            Marshal.FreeCoTaskMem ( pReturnedString );
            //use of Substring below removes terminating null for split
            sections = local.Substring ( 0 , local.Length - 1 ).Split ( '\0' );
            return 0;
        }

        /// <summary>
        /// 得到某个节点下面所有的key和value组合
        /// </summary>
        /// <param name="section"></param>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public int GetAllKeyValues ( string section , out string[ ] keys , out string[ ] values , string path )
        {
            byte[] b = new byte[65535];

            GetPrivateProfileSection ( section , b , b.Length , path );
            string s = System.Text.Encoding.Default.GetString ( b );
            string[] tmp = s.Split ( ( char ) 0 );
            List<string> result = new List<string> ( );
            foreach ( string r in tmp )
            {
                if ( r != string.Empty )
                    result.Add ( r );
            }
            keys = new string[result.Count];
            values = new string[result.Count];
            for ( int i = 0; i < result.Count; i++ )
            {
                string[] item = result[i].ToString ( ).Split ( new char[ ] { '=' } );
                if ( item.Length == 2 )
                {
                    keys[i] = item[0].Trim ( );
                    values[i] = item[1].Trim ( );
                }
                else if ( item.Length == 1 )
                {
                    keys[i] = item[0].Trim ( );
                    values[i] = "";
                }
                else if ( item.Length == 0 )
                {
                    keys[i] = "";
                    values[i] = "";
                }
            }

            return 0;
        }



        //INI文件由节、键、值组成。 注解使用分号表示（;）。在分号后面的文字，直到该行结尾都全部为注解。
        //; comment textINI文件的数据格式的例子（配置文件的内容）　
        //[Section1 Name]
        //KeyName1=value1
        //KeyName2=value2
        //...
        //[Section2 Name]
        //KeyName21=value21
        //KeyName22=value22
    }

}