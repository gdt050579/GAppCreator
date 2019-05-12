using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace GAppCreator
{
    public class GZipPackage
    {
        private const byte VERSION = 0;
        class PackInfo
        {
            public string FileName;
            public string Name;
            public int Size;
        };
        [XmlType("File"), XmlRoot("File")]
        public class FileHash
        {
            [XmlAttribute()]
            public string Name;
            [XmlAttribute()]
            public string MD5;
        };
        List<PackInfo> lst = new List<PackInfo>();
        private byte[] tempBuffer;
        private int tempBufferPoz;

        private byte[] EncryptBytes(byte[] key, byte[] plaintext)
        {
            using (var cipher = new RijndaelManaged { Key = key })
            {
                using (var encryptor = cipher.CreateEncryptor())
                {
                    var ciphertext = encryptor.TransformFinalBlock(plaintext, 0, plaintext.Length);

                    // IV is prepended to ciphertext
                    return cipher.IV.Concat(ciphertext).ToArray();
                }
            }
        }

        private byte[] DecryptBytes(byte[] key, byte[] packed)
        {
            using (var cipher = new RijndaelManaged { Key = key })
            {
                int ivSize = cipher.BlockSize / 8;

                cipher.IV = packed.Take(ivSize).ToArray();

                using (var encryptor = cipher.CreateDecryptor())
                {
                    return encryptor.TransformFinalBlock(packed, ivSize, packed.Length - ivSize);
                }
            }
        }

        private byte[] AddMac(byte[] key, byte[] data)
        {
            using (var hmac = new HMACSHA256(key))
            {
                var macBytes = hmac.ComputeHash(data);

                // HMAC is appended to data
                return data.Concat(macBytes).ToArray();
            }
        }

        private bool BadMac(byte[] found, byte[] computed)
        {
            int mismatch = 0;

            // Aim for consistent timing regardless of inputs
            for (int i = 0; i < found.Length; i++)
            {
                mismatch += found[i] == computed[i] ? 0 : 1;
            }

            return mismatch != 0;
        }

        private byte[] RemoveMac(byte[] key, byte[] data)
        {
            using (var hmac = new HMACSHA256(key))
            {
                int macSize = hmac.HashSize / 8;

                var packed = data.Take(data.Length - macSize).ToArray();

                var foundMac = data.Skip(packed.Length).ToArray();

                var computedMac = hmac.ComputeHash(packed);

                if (this.BadMac(foundMac, computedMac))
                {
                    throw new Exception("Bad MAC");
                }

                return packed;
            }
        }

        private List<byte[]> DeriveTwoKeys(string password)
        {
            var salt = new byte[] { 2, 3, 5, 7, 9, 11, 13, 17 };

            var kdf = new Rfc2898DeriveBytes(password, salt, 10000);

            var bytes = kdf.GetBytes(32); // Two keys 128 bits each

            return new List<byte[]> { bytes.Take(16).ToArray(), bytes.Skip(16).ToArray() };
        }

        private byte[] EncryptBuffer(string password, byte[] message)
        {
            var keys = this.DeriveTwoKeys(password);

            var packed = this.EncryptBytes(keys[0], message);

            return this.AddMac(keys[1], packed);
        }

        private byte[] DecryptBuffer(string password, byte[] secret)
        {
            var keys = this.DeriveTwoKeys(password);

            var packed = this.RemoveMac(keys[1], secret);

            return this.DecryptBytes(keys[0], packed);
        }


        private void AddByte(byte value)
        {
            tempBuffer[tempBufferPoz] = value;
            tempBufferPoz++;            
        }
        private void AddInt(int value)
        {
            tempBuffer[tempBufferPoz] = (byte)(value & 0xFF);
            tempBufferPoz++;
            tempBuffer[tempBufferPoz] = (byte)((value >> 8) & 0xFF);
            tempBufferPoz++;
            tempBuffer[tempBufferPoz] = (byte)((value >> 16) & 0xFF);
            tempBufferPoz++;
        }
        private void AddString(string text)
        {
            AddInt(text.Length);
            foreach (char ch in text)
            {
                tempBuffer[tempBufferPoz] = (byte)ch;
                tempBufferPoz++;                
            }
        }
        private void AddBuffer(byte[] buffer)
        {
            //foreach (byte b in buffer)
            //    list.Add(b);
            //list.AddRange(buffer);
            AddInt(buffer.Length);
            for (int tr = 0; tr < buffer.Length; tr++)
            {
                tempBuffer[tempBufferPoz] = buffer[tr];
                tempBufferPoz++;
            }
        }
        private int GetInt()
        {
            if (tempBufferPoz + 3 > tempBuffer.Length)
                return -1;
            int res = (int)tempBuffer[tempBufferPoz] | (((int)tempBuffer[tempBufferPoz+1]) << 8) | (((int)tempBuffer[tempBufferPoz+2]) << 16);
            tempBufferPoz += 3;
            return res;
        }
        private string GetString()
        {
            int sz = GetInt();
            if (sz < 0)
                return null;
            string ss = "";
            for (int tr = 0; tr < sz; tr++)
            {
                if (tempBufferPoz >= tempBuffer.Length)
                    return null;
                ss += (char)tempBuffer[tempBufferPoz];
                tempBufferPoz++;
            }
            return ss;
        }
        private byte[] GetBuffer()
        {
            int sz = GetInt();
            if (sz <= 0)
                return null;
            if (sz + tempBufferPoz > tempBuffer.Length)
                return null;
            byte[] res = new byte[sz];
            for (int tr=0;tr<sz;tr++)
            {
                res[tr] = tempBuffer[tempBufferPoz];
                tempBufferPoz++;
            }
            return res;
        }

        public void Clear()
        {
            lst.Clear();
            tempBuffer = null;
            tempBufferPoz = 0;
        }

        public bool Add(string filePath, string packageName,ErrorsContainer ec)
        {
            try
            {
                FileInfo f = new FileInfo(filePath);
                PackInfo pi = new PackInfo();
                pi.FileName = filePath;
                pi.Size = (int)f.Length;
                pi.Name = packageName;
                lst.Add(pi);
                return true;
            }
            catch (Exception e)
            {
                if (ec != null)
                    ec.AddException("Unable to add " + filePath, e);
                return false;
            }
        }
        public bool AddFolder(string folder, string packageFolder, string extension, ErrorsContainer ec)
        {
            string[] files = Directory.GetFiles(folder);
            if (extension!=null)
                extension = "."+extension.ToLower();
            foreach (string fname in files)
            {
                if (extension != null)
                    if (fname.ToLower().EndsWith(extension) == false)
                        continue;
                if (Add(fname, Path.Combine(packageFolder, Path.GetFileName(fname)), ec) == false)
                    return false;
            }
            return true;
        }
        public byte[] Compress(string password, ErrorsContainer ec)
        {
            int sz = 8;
            try
            {
                foreach (PackInfo pi in lst)
                {
                    sz += pi.Size;
                    sz += 4;
                    sz += pi.Name.Length;
                    sz += 1;
                }
                tempBuffer = new byte[sz + 2048];
                tempBufferPoz = 0;

                // MAGIC
                AddByte((byte)'P'); AddByte((byte)'G'); AddByte((byte)'A'); AddByte((byte)'C');
                // VERSION
                AddByte(VERSION);
                // NR_FILES
                AddInt(lst.Count);
                int count = 0;
                foreach (PackInfo pi in lst)
                {
                    AddString(pi.Name);
                    byte[] b = Disk.ReadFile(pi.FileName, ec);
                    if (b.Length!=pi.Size)
                    {
                        if (ec != null)
                            ec.AddError("Incorect size for: " + pi.FileName + "\r\nExpecting: " + pi.Size.ToString() + " -> receiving: " + b.Length.ToString());
                        return null;
                    }
                    if (b == null)
                    {
                        if (ec != null)
                            ec.AddError("Unable to add file: " + pi.FileName);
                        return null;
                    }
                    if (b.Length + tempBufferPoz > tempBuffer.Length)
                    {
                        if (ec != null)
                            ec.AddError("Unable to add file (incorect size): " + pi.FileName+"\r\nAdded so far: "+count.ToString());
                        return null;
                    }
                    count++;
                    AddBuffer(b);
                }
                // all good - comprim            
                MemoryStream m = new MemoryStream();
                GZipStream gz = new GZipStream(m, CompressionMode.Compress, true);
                gz.Write(tempBuffer, 0, tempBufferPoz);
                gz.Close();
                int bufSize = (int)m.Position;
                m.Close();
                byte[] bb = new byte[bufSize];
                byte[] mb = m.GetBuffer();
                for (int tr = 0; tr < bufSize; tr++)
                    bb[tr] = mb[tr];
                return EncryptBuffer(password, bb);
            }
            catch (Exception e)
            {
                if (ec != null)
                    ec.AddException("Unable to encrypt/compress data", e);
                return null;
            }
        }
        public bool Compress(string filename, string password, ErrorsContainer ec)
        {
            byte[] res = Compress(password, ec);
            if (res == null)
                return false;
            if (Disk.SaveFile(filename, res, ec) == false)
                return false;
            return true;
        }
        private byte[] Ungzip(byte[] zipped)
        {
            using (GZipStream stream = new GZipStream(new MemoryStream(zipped), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    } while (count > 0);
                    return memory.ToArray();
                }
            }
        }
        public bool Uncompress(string password, byte[] compressBuffer, string folder,ErrorsContainer ec)
        {
            try
            {
                byte[] bbgzip = DecryptBuffer(password, compressBuffer);
                if (bbgzip == null)
                {
                    if (ec != null)
                        ec.AddError("Unable to decrypt " + compressBuffer.Length.ToString() + " bytes");
                    return false;
                }
                byte[] bb = Ungzip(bbgzip);
                if (bb == null)
                {
                    if (ec != null)
                        ec.AddError("Unable to unzip " + bbgzip.Length.ToString() + " bytes");
                    return false;
                }
                // all is gool - am bufferul original
                if (bb.Length < 8)
                {
                    if (ec != null)
                        ec.AddError("Invalid zipped buffer - should be at least 8 bytes");
                    return false;
                }
                if ((bb[0] != 'P') || (bb[1] != 'G') || (bb[2] != 'A') || (bb[3] != 'C'))
                {
                    if (ec != null)
                        ec.AddError("Invalid magic header.");
                    return false;
                }
                if (bb[4] != VERSION)
                {
                    if (ec != null)
                        ec.AddError("Invalid version (expecting version " + VERSION.ToString() + " - found version " + bb[4].ToString());
                    return false;
                }
                Clear();
                tempBuffer = bb;
                tempBufferPoz = 5;
                int nrFiles = GetInt();
                if (nrFiles < 0)
                {
                    if (ec != null)
                        ec.AddError("Invalid number of files in compressed buffer: " + nrFiles.ToString());
                    return false;
                }
                for (int tr = 0; tr < nrFiles; tr++)
                {
                    string name = GetString();
                    if (name == null)
                    {
                        if (ec != null)
                            ec.AddError("Invalid string/name (corrupted archive) !");
                        return false;
                    }
                    byte[] b = GetBuffer();
                    if (b == null)
                    {
                        if (ec != null)
                            ec.AddError("Incomplet or corupted data for " + name);
                        return false;
                    }
                    string fname = Path.Combine(folder, name);
                    if (Disk.CreateFolder(Path.GetDirectoryName(fname), ec) == false)
                        return false;
                    if (Disk.SaveFile(fname, b, ec) == false)
                        return false;
                }
                // all good
                return true;
            }
            catch (Exception e)
            {
                if (ec != null)
                    ec.AddException("Unable to decompress/unpack data", e);
                return false;
            }
        }
        public bool Uncompress(string password, string fileName, string folder, ErrorsContainer ec)
        {
            byte[] b = Disk.ReadFile(fileName,ec);
            if (b==null)
            {
                if (ec!=null)
                    ec.AddError("Unable to read from file: "+fileName);
                return false;
            }
            return Uncompress(password, b, folder,ec);
        }
        private string ComputeMD5(string fileName, ErrorsContainer ec)
        {
            try
            {
                MD5 m = MD5.Create();
                FileStream fs = File.OpenRead(fileName);
                byte[] b = m.ComputeHash(fs);
                fs.Close();
                return BitConverter.ToString(b).Replace("-", "");
            }
            catch (Exception e)
            {
                if (ec != null)
                    ec.AddException("Can not compute MD5 for " + fileName, e);
                return null;
            }
        }
        public int GetFilesCount()
        {
            return lst.Count();
        }
        public bool ExportHashList(string fileName,ErrorsContainer ec)
        {
            List<FileHash> fi = new List<FileHash>();
            foreach (PackInfo pi in lst)
            {
                FileHash fis = new FileHash();
                fis.Name = pi.Name;
                fis.MD5 = ComputeMD5(pi.FileName, ec);
                if (fis.MD5 == null)
                    return false;
                fi.Add(fis);
            }
            // am facut lista - o salvez
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<FileHash>));
                TextWriter textWriter = new StreamWriter(fileName);
                serializer.Serialize(textWriter, fi);
                textWriter.Close();
                return true;
            }
            catch (Exception e)
            {
                if (ec!=null)
                    ec.AddException("Unable to hash list to "+fileName, e);
                return false;
            }
        }
        public bool FilterFileList(string xmlFileHash, ErrorsContainer ec)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<FileHash>));
                TextReader textReader = new StreamReader(xmlFileHash);
                List<FileHash> sol = (List<FileHash>)serializer.Deserialize(textReader);
                textReader.Close();
                if (sol == null)
                {
                    if (ec != null)
                        ec.AddError("Unable to deserialize : "+xmlFileHash);
                    return false;
                }
                Dictionary<string, string> d = new Dictionary<string, string>();
                foreach (FileHash fh in sol)
                    d[fh.Name] = fh.MD5;
                // citesc MD5-urile de la fisierele mele
                List<PackInfo> toRemove = new List<PackInfo>();
                foreach (PackInfo pi in lst)
                {
                    string md5 = ComputeMD5(pi.FileName, ec);
                    if (md5==null)
                        return false;
                    if ((d.ContainsKey(pi.Name)) && (d[pi.Name].Equals(md5)))
                        toRemove.Add(pi);
                }
                foreach (PackInfo pi in toRemove)
                    lst.Remove(pi);
                return true;
            }
            catch (Exception e)
            {
                if (ec != null)
                    ec.AddException("Unable to hash list to " + xmlFileHash, e);
                return false;
            }
        }
        public bool CopyTo(string folder, ErrorsContainer ec)
        {
            foreach (PackInfo pi in lst)
            {
                string dest = Path.Combine(folder,pi.Name);
                if (Disk.CreateFolder(Path.GetDirectoryName(dest), ec) == false)
                    return false;
                if (Disk.Copy(pi.FileName, dest, ec) == false)
                    return false;
            }
            return true;
        }
    }
}
