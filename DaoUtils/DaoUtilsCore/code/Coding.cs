using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using DaoUtilsCore.code;
using DaoUtilsCore.core;

namespace DaoUtils.code
{
    internal enum EncryptionDirection { Encrypt, Decrypt }

    internal delegate void WithCryptoTransform(ICryptoTransform transform);
    internal class Coding: ICoding
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public Coding()
            : this(new CodeKey
            {
                InitializationVectorBase64 = "HRT+dLbcLw5P7kN+fMkttQ==",
                KeyBase64 = "5AHYzSj29JNNwe7h5b+ofzNDeuyB/Oo5e1BpQtL55EY="
            })
        {

        }

        public Coding(string codeKeyString)
            : this(KeyEncrypter.ExtractKey(codeKeyString))
        {
        }

        internal Coding(CodeKey codeKey)
        {
            _key = codeKey.Key;
            _iv = codeKey.InitializationVector;
        }

        private ICryptoTransform Transform(AesCryptoServiceProvider provider, EncryptionDirection direction)
        {
            switch (direction)
            {
                case EncryptionDirection.Encrypt:
                    return provider.CreateEncryptor(_key, _iv);
                case EncryptionDirection.Decrypt:
                    return provider.CreateDecryptor(_key, _iv);
            }
            throw new DaoUtilsException($"Unknown Encryption Direction = {direction}");
        }

        private void DoEncryptionTransform(EncryptionDirection encryptionDirection, WithCryptoTransform process)
        {
            using (var aesAlg = new AesCryptoServiceProvider() { IV = _iv, Key = _key })
            {
                aesAlg.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform transform = Transform(aesAlg, encryptionDirection))
                {
                    process(transform);
                }
            }
        }

        private readonly RNGCryptoServiceProvider _rand = new RNGCryptoServiceProvider();
        private byte[] CreateRandomBytes(int length)
        {
            byte[] randBytesBuffer = new byte[Math.Max(1, length)];
            _rand.GetBytes(randBytesBuffer);
            return randBytesBuffer;
        }

        private byte[] IntToArray(int intValue, byte[] randomiseArray = null)
        {
            byte[] result = new byte[4];
            for (var idx = 0; idx < 4; idx++)
            {
                var val = (byte)(intValue >> ((3 - idx) * 8));
                result[idx] =  randomiseArray == null ? val : (byte)(val ^ randomiseArray[idx]);
                 
            }
            return result;
        }

        private int IntFromArray(byte[] intArray, int offset, byte[] randomiseArray = null)
        {
            int result = 0;
            for (var idx = 0; idx < 4; idx++)
            {
                byte val = randomiseArray == null ? intArray[offset + idx] : (byte)(intArray[offset + idx] ^ randomiseArray[idx]);
                result += (val) << ((3 - idx) * 8);
            }
            return result;
        }

        private byte[] RandomiseArray(byte[] salt)
        {
            return new[] {salt[7], salt[5], salt[3], salt[1]};
        }

        private byte[] AppendSalt(byte[] data)
        {
            /*Random Salt has Two purposes 
             *   - ensure same password gets different output each time - so you can't find password by brute forcing encrypt password and compare
             *   - obscure short password lengths - which can help 
             *   
             * Also need to store lengths of salt and data - obscured them, but not sure needed
             */
            if (data == null) return AppendSalt(new byte[0]);
            var dataLength = data.Length;
            byte saltLen = (byte)(Math.Max(51 - data.Length, 15));
            byte[] salt = CreateRandomBytes(saltLen);
            salt[salt[1] % 6 + 2] = (byte)(saltLen ^ salt[8]);
            IntToArray(dataLength, RandomiseArray(salt)).CopyTo(salt, 9);
            return salt.Concat(data).ToArray();
        }

        private string Encrypt(byte[] plainData)
        {
            byte[] plain = AppendSalt(plainData);
            byte[] encrypted = null;
            DoEncryptionTransform(EncryptionDirection.Encrypt, encryptor =>
            {
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(plain, 0, plain.Length);
                        encrypted = msEncrypt.ToArray();
                    }
                    encrypted = msEncrypt.ToArray();
                }
            });
            return Convert.ToBase64String(encrypted);
        }

        public string EncryptStr(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText)) throw new ArgumentNullException(nameof(plainText));
            return Encrypt(Encoding.UTF8.GetBytes(plainText));
        }

        private int DataLength(Stream dataStream)
        {
            byte[] salt = new byte[15];
            dataStream.Read(salt, 0, 15);
            var remainingSaltLen = (salt[salt[1] % 6 + 2] ^ salt[8]) - 15;
            dataStream.Read(new byte[remainingSaltLen], 0, remainingSaltLen);
            int len = IntFromArray(salt, 9, RandomiseArray(salt));
            return len;
        }

        public byte[] Decrypt(string cipherText)
        {
            byte[] data = null;
            if (string.IsNullOrWhiteSpace(cipherText)) throw new ArgumentNullException(nameof(cipherText));
            DoEncryptionTransform(EncryptionDirection.Decrypt, decryptor =>
            {
                using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        data = new byte[DataLength(csDecrypt)];
                        csDecrypt.Read(data, 0, data.Length);
                    }
                }
            });
            return data;
        }

        public string DecryptString(string cipherText)
        {
            return Encoding.UTF8.GetString(Decrypt(cipherText));
        }

        private static Coding _keyEncrypter;
        private static Coding KeyEncrypter
        {
            get
            {
                return _keyEncrypter = _keyEncrypter ?? new Coding(new CodeKey()
                {
                    KeyBase64 = "5AHYzSj29JNNwe7h5b+ofzNDeuyB/Oo5e1BpQtL55EY=",
                    InitializationVectorBase64 = "HRT+dLbcLw5P7kN+fMkttQ=="
                });
            }
        }

        private readonly byte[] _randomise = {23, 76, 45, 100};
        public string GenerateCodeKeyString()
        {
            using (var aesAlg = new AesCryptoServiceProvider())
            {
                var data = IntToArray(aesAlg.Key.Length, _randomise).Concat(aesAlg.IV).Concat(aesAlg.Key);
                return KeyEncrypter.Encrypt(data.ToArray());
            }
        }

        internal CodeKey ExtractKey(string codeKeyString)
        {
            byte[] plain = Decrypt(codeKeyString);
            int ivStart = 4;
            int ivEnd = plain.Length - IntFromArray(plain, 0, _randomise) - ivStart;
            return new CodeKey()
            {
                InitializationVector = plain.Skip(ivStart).Take(ivEnd).ToArray(),
                Key = plain.Skip(ivEnd + ivStart).ToArray()
                
            };
        }
    }
}

