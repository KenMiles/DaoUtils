using System;
using System.Collections.Generic;
using System.Text;

namespace DaoUtilsCore.code
{
    interface ICoding
    {
        string DecryptString(string cipherText);
        string EncryptStr(string plainText);
    }
}
