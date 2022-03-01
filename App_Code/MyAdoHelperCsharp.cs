using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.IO;

public class MyAdoHelperCsharp
{

    /// <summary>
    /// Summary description for MyAdoHelper
    /// פעולות עזר לשימוש במסד נתונים  מסוג 
    /// SQL SERVER
    ///  App_Data המסד ממוקם בתקיה 
    /// </summary>
    public MyAdoHelperCsharp()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static string AppDir()
    {
        string x = Directory.GetCurrentDirectory();
        string b = x.Substring(0, x.Length - 10);
        return b;
    }


    public static SqlConnection ConnectToDb(string fileName)
    {
        string path;
        //CHECK full path or from App_Code
        if (fileName.Contains(":\\"))//if contain it full path
        {
            path = fileName;
        }
        else // if in app_Data
        {
            path = AppDir() + @"\App_Data\";
            path += fileName;
        }


        //string path = HttpContext.Current.Server.MapPath("App_Data/" + fileName);//מאתר את מיקום מסד הנתונים מהשורש ועד התקייה בה ממוקם המסד
        string connString = @"Data Source=(localdb)\v11.0;AttachDbFilename=" +
                             path +
                             ";Integrated Security=True;";
        SqlConnection conn = new SqlConnection(connString);
        return conn;

    }



    /// <summary>
    /// To Execute update / insert / delete queries
    ///  הפעולה מקבלת שם קובץ ומשפט לביצוע ומבצעת את הפעולה על המסד
    /// </summary>

    public static void DoQuery(string fileName, string sql)//הפעולה מקבלת שם מסד נתונים ומחרוזת מחיקה/ הוספה/ עדכון
    //ומבצעת את הפקודה על המסד הפיזי
    {

        SqlConnection conn = ConnectToDb(fileName);
        conn.Open();
        SqlCommand com = new SqlCommand(sql, conn);
        com.ExecuteNonQuery();
        com.Dispose();
        conn.Close();
        SqlConnection.ClearPool(conn);

    }

    public static void DoQueryWithImage(string fileName, string sql, byte[] img)//הפעולה מקבלת שם מסד נתונים ומחרוזת מחיקה/ הוספה/ עדכון
    //ומבצעת את הפקודה על המסד הפיזי
    {

        SqlConnection conn = ConnectToDb(fileName);
        conn.Open();
        SqlCommand com = new SqlCommand(sql, conn);
        com.Parameters.Add(new SqlParameter("@img", img));
        com.ExecuteNonQuery();
        com.Dispose();
        conn.Close();
        SqlConnection.ClearPool(conn);

    }

    /// <summary>
    /// To Execute update / insert / delete queries
    ///  הפעולה מקבלת שם קובץ ומשפט לביצוע ומחזירה את מספר השורות שהושפעו מביצוע הפעולה
    /// </summary>
    public static int RowsAffected(string fileName, string sql)//הפעולה מקבלת מסלול מסד נתונים ופקודת עדכון
    //ומבצעת את הפקודה על המסד הפיזי
    {

        SqlConnection conn = ConnectToDb(fileName);
        conn.Open();
        SqlCommand com = new SqlCommand(sql, conn);
        int rowsA = com.ExecuteNonQuery();
        conn.Close();
        SqlConnection.ClearPool(conn);
        return rowsA;

    }

    /// <summary>
    /// הפעולה מקבלת שם קובץ ומשפט לחיפוש ערך - מחזירה אמת אם הערך נמצא ושקר אחרת
    /// </summary>
    public static bool IsExist(string fileName, string sql)//הפעולה מקבלת שם קובץ ומשפט בחירת נתון ומחזירה אמת אם הנתונים קיימים ושקר אחרת
    {

        SqlConnection conn = ConnectToDb(fileName);
        conn.Open();
        SqlCommand com = new SqlCommand(sql, conn);
        SqlDataReader data = com.ExecuteReader();
        bool found;
        found = (bool)data.Read();// אם יש נתונים לקריאה יושם אמת אחרת שקר - הערך קיים במסד הנתונים
        conn.Close();
        SqlConnection.ClearPool(conn);
        return found;

    }


    public static DataTable ExecuteDataTable(string fileName, string sql)
    {
        SqlConnection conn = ConnectToDb(fileName);
        conn.Open();
        SqlDataAdapter tableAdapter = new SqlDataAdapter(sql, conn);
        DataTable dt = new DataTable();
        tableAdapter.Fill(dt);
        conn.Close();
        SqlConnection.ClearPool(conn);
        return dt;
    }

    public static string[] returnIFonlyoneLine(string fileName, string sql)
    {

        DataTable dt = MyAdoHelperCsharp.ExecuteDataTable(fileName, sql);
        DataRow row = dt.Rows[0];//זה הרשומה הידועה
        string[] result = new string[row.ItemArray.Length];
        int i = 0;
        foreach (object myItemArray in row.ItemArray)
        {
            result[i] = myItemArray.ToString();
            i++;
        }

        return result;
    }
    public static string[,] returnIFMultiLine(string fileName, string sql)
    {
        DataTable dt = MyAdoHelperCsharp.ExecuteDataTable(fileName, sql);

        string[,] result = new string[dt.Rows.Count, dt.Columns.Count];
        int i = 0, j = 0;
        foreach (DataRow row in dt.Rows)
        {

            foreach (object myItemArray in row.ItemArray)
            {
                result[i, j] = myItemArray.ToString();
                j++;
            }
            i++;
            j = 0;
        }
        return result;

    }


    /* public static void ExecuteNonQuery(string fileName, string sql)
     {
         SqlConnection conn = ConnectToDb(fileName);
         conn.Open();
         SqlCommand command = new SqlCommand(sql, conn);
         command.ExecuteNonQuery();
         conn.Close();
     }זה כבר יש למעלה

     public static string printDataTable(string fileName, string sql)//הפעולה מקבלת שם קובץ ומשפט בחירת נתון ומחזירה אמת אם הנתונים קיימים ושקר אחרת
     {
        
       
         DataTable dt = ExecuteDataTable(fileName, sql);
       
         string printStr = "<table border='1'>";
        
         foreach (DataRow row in dt.Rows)
         {
             printStr += "<tr>";
             foreach (object myItemArray in row.ItemArray)
             {
                 printStr += "<td>" + myItemArray.ToString() +"</td>";
             }
             printStr += "</tr>";
         }
         printStr += "</table>";
        
         return printStr;
     }*/
    //זה רק ל HTML

}


























