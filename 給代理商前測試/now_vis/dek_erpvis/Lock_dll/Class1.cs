using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lock_dll
{
    public class Class1
    {
        public static string Class1_Name()
        {
            return dekSecure.dekEncDec.Encrypt("abc", True_Password.Decode());
        }
    }
    public class Lock_DLL
    {
        public static string Lock_DLL_Name()
        {

            return dekSecure.dekEncDec.Encrypt("dek54886961", True_Password.Decode()); ;
        }
    }
    public class Protect_DLL
    {
        public static string Protect_DLL_Name()
        {
            return dekSecure.dekEncDec.Encrypt("deta89886066", True_Password.Decode()); ;
        }
    }
    public class Damage_DLL
    {
        public static string Damage_DLL_Name()
        {
            return dekSecure.dekEncDec.Encrypt("dek89886066", True_Password.Decode()); ;
        }
    }
    public class Desire_DLL
    {
        public static string Desire_DLL_Name()
        {
           
            return dekSecure.dekEncDec.Encrypt("deta54886961", True_Password.Decode()); ;
        }
    }
    public class True_Password
    {
        public static string Decode()
        {

            return "83037743";
        }
        public static string True_Password_Name()
        {
            
            return "deta89886066";
        }

    }
}
