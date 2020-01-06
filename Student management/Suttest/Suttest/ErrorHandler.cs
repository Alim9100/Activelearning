using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class ErrorHandler
    {
        public static string ErrorMessage { get; private set; }
        public static string ErrorMessageDetial { get; private set; }
        public static int Errorcode { get; private set; }
        public static int GetError(object ex)
        {
            Errorcode = 100;

            ErrorMessage = " خطا ";
           // ErrorMessageDetial = ex.Message;
            if (ex.GetType() == typeof(FormatException))
            {
                ErrorMessage = " خطای فرمت ";
                Errorcode = 101;
            }
            if (ex.GetType() == typeof(DivideByZeroException))
            {
                ErrorMessage = " خطای تقسبم بر صفر ";
                Errorcode = 102;
            }
            

            if (ex.GetType() == typeof(System.Security.Cryptography.CryptographicException))
            {
                ErrorMessage = " خطا در بازیابی رشته ";
                Errorcode = 103;
            }
            
            if (ex.GetType() == typeof(SqlException))
            {
                SqlException exSql = ex as SqlException;
                GetSqlServerError(exSql);
                Errorcode = 104;
            }
            if(ex.GetType()==typeof(SocketException))
            {
                SocketException socketex = ex as SocketException;
                ErrorMessage = "خطای ارتباط";
                Errorcode = 105;
            }

            Console.WriteLine(ErrorMessage);
            return Errorcode;
        }

        private static void GetSqlServerError(SqlException exSql)
        {
            ErrorMessage = " خطای پایگاه داده ";
            switch (exSql.Number)
            {
                case 2627: { ErrorMessage = "اطلاعات تکراری است"; break; }
                case 229: { ErrorMessage = " شما مجوز دسترسی به این داده را ندارید "; break; };
                case 2: { ErrorMessage = "پایگاه داده قابل دسترسی نیست"; break; };
                case 547: { ErrorMessage = " به دلیل اینکه اطلاعات در قسمت های دیگر استفاده شده است قابل تغییر نیست "; break; }
            }
        }
    }
}
