using System;

namespace DaoUtilsCore.code
{
    internal class CodeKey
    {
        public byte[] Key { get; set; }
        public byte[] InitializationVector { get; set; }

        public string KeyBase64
        {
            get { return Convert.ToBase64String(Key); }
            set { Key = Convert.FromBase64String(value); }
        }
        public string InitializationVectorBase64
        {
            get { return Convert.ToBase64String(InitializationVector); }
            set { InitializationVector = Convert.FromBase64String(value); }
        }
    }

}
