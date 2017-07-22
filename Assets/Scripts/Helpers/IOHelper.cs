using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

namespace CEUtilities.Helpers
{
    public static class IOHelper
    {
        #region Sub Classes / Structs

        /// <summary>
        /// Custom StringWriter with specific encoding
        /// </summary>
        public sealed class StringWriterWithEncoding : StringWriter
        {
            private readonly Encoding encoding;

            public StringWriterWithEncoding(Encoding encoding)
            {
                this.encoding = encoding;
            }

            public override Encoding Encoding
            {
                get { return encoding; }
            }
        }

        #endregion

        #region Exposed fields

        #endregion Exposed fields

        #region Internal fields

        #endregion Internal fields

        #region Custom Events

        #endregion Custom Events

        #region Properties

        #endregion Properties

        #region Events methods

        #endregion Events methods

        #region Public Methods

        /// <summary>
        /// Checks file if it's being written.
        /// </summary>
        /// <param name="file">Filepath</param>
        public static bool IsBeingWritten(string file)
        {
            FileInfo fileInfo = new FileInfo(file);
            FileStream stream = null;

            try
            {
                stream = fileInfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return false;
        }

        /// <summary>
        /// Checks if file is not opened or being used by another process
        /// </summary>
        /// <param name="file">Filepath</param>
        public static bool CheckReadyFile(string file, int inactivitySecRequired = 2)
        {
            if (IsBeingWritten(file))
                return false;

            // Checks the file modifications times
            FileInfo fileInfo = new FileInfo(file);
            DateTime lastModifiedDate = fileInfo.LastWriteTime;
            DateTime actualDate = DateTime.Now;

            TimeSpan difference = actualDate - lastModifiedDate;
            if (TimeSpan.Compare(difference, new TimeSpan(0, 0, inactivitySecRequired)) < 0)
                return false;

            return true;
        }

        /// <summary>
        /// Exclude certain chars from a string
        /// </summary>
        /// <param name="text">String which contains the chars</param>
        /// <param name="toExclude">Excluding chars</param>
        public static string ExceptChars(string text, char[] toExclude)
        {
            StringBuilder sBuilder = new StringBuilder();
            for (int loop = 0; loop < text.Length; loop++)
            {
                char c = text[loop];
                if (Array.IndexOf(toExclude, c) == -1)
                    sBuilder.Append(c);
            }

            return sBuilder.ToString();
        }

        /// <summary>
        /// Exclude a certain char from a string
        /// </summary>
        /// <param name="text">String which contains the char</param>
        /// <param name="toExclude">Excluding char</param>
        public static string ExceptChars(string text, char toExclude)
        {
            StringBuilder sBuilder = new StringBuilder();
            for (int loop = 0; loop < text.Length; loop++)
            {
                char c = text[loop];
                if (c != toExclude)
                    sBuilder.Append(c);
            }

            return sBuilder.ToString();
        }

        /// <summary>
        /// Fill a data bytes array with certain default data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fillwith"></param>
        /// <param name="dataSize"></param>
        public static void DataSet(ref byte[] data, byte fillwith, UInt32 dataSize)
        {
            for (int i = 0; i < dataSize; i++)
                data[i] = fillwith;

            return;
        }

        /// <summary>
        /// Writes a text or value into a filestream
        /// </summary>
        public static void AddText(FileStream fileStream, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);

            fileStream.Write(info, 0, info.Length);
            fileStream.Flush();

            return;
        }

        /// <summary>
        /// Generate a GUID boundary
        /// </summary>
        public static string GenerateBoundary()
        {
            string sBoundary = Guid.NewGuid().ToString();

            sBoundary = sBoundary.Replace("-", string.Empty);
            sBoundary = sBoundary.Substring(0, 16);

            return sBoundary;
        }
        

        /// <summary>
        /// Convert a string into an array of bytes
        /// </summary>
        public static byte[] BytesFromBase64String(String text)
        {
            return Convert.FromBase64String(text);
        }

        /// <summary>
        /// Creates a string from an array of bytes
        /// </summary>
        public static string BytesToBase64String(byte[] data)
        {
            return Convert.ToBase64String(data);
        }


        /// <summary>
        /// Split every character from a string to create an array of chars
        /// </summary>
        public static char[] StringToChars(string data)
        {
            char[] result = new char[data.Length];

            for (int loop = 0; loop < data.Length; loop++)
                result[loop] = data[loop];

            return result;
        }

        /// <summary>
        /// Append every char to create an string
        /// </summary>
        public static string CharsToString(char[] data)
        {
            var sb = new StringBuilder(data.Length);
            sb.Append(data);
            return sb.ToString();
        }


        /// <summary>
        /// Creates an array of chars from an array of bytes
        /// </summary>
        public static char[] BytesToChars(byte[] data)
        {
            List<char> result = new List<char>();

            foreach (byte dataByte in data)
                result.Add((char)dataByte);

            return result.ToArray();
        }

        /// <summary>
        /// Creates an array of bytes from an array of chars
        /// </summary>
        public static byte[] CharsToBytes(char[] data)
        {
            List<byte> result = new List<byte>();

            foreach (char ch in data)
                result.Add((byte)ch);

            return result.ToArray();
        }


        /// <summary>
        /// Gets an array of bytes from the stream
        /// </summary>
        public static byte[] StreamToBytes(Stream stream)
        {
            return ((MemoryStream)stream).ToArray();
        }

        /// <summary>
        /// Creates an string from a stream
        /// </summary>
        public static string StreamToString(Stream stream)
        {
            var sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }

        /// <summary>
        /// Creates an string from a stream
        /// </summary>
        public static string StreamToASCIIString(Stream stream)
        {
            return Encoding.ASCII.GetString(((MemoryStream)stream).ToArray());
        }

        /// <summary>
        /// Creates an string from a stream
        /// </summary>
        public static string Base64StreamToString(Stream stream)
        {
            return Convert.ToBase64String(((MemoryStream)stream).ToArray());
        }


        /// <summary>
        /// Extract the list of directories from a path
        /// </summary>
        public static string[] ParsePath(string path)
        {
            char[] separators = { '/', '\\' };
            string[] folders = path.Split(separators);
            List<string> result = new List<string>();

            foreach (string folder in folders)
            {
                if (folder.Length > 0)
                    result.Add(folder);
            }

            return result.Count == 0 ? null : result.ToArray();
        }

        /// <summary>
        /// DEPRECATED. Extract the filename from the filepath
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFilenameFromPath(string filePath)
        {
            char[] separators = { '/', '\\' };
            string[] nodes = filePath.Split(separators);

            return nodes[nodes.Length - 1];
        }

        /// <summary>
        /// Creates an XMLDocProperty
        /// </summary>
        public static string XMLDocProperty(string propName, string propValue, string boundary)
        {
            StringBuilder sBuilder = new StringBuilder();

            sBuilder.AppendLine("--" + boundary);
            sBuilder.AppendLine("Content-Disposition: form-data; name=\"" + propName + "\"");
            sBuilder.AppendLine("");
            sBuilder.AppendLine(propValue);

            return sBuilder.ToString();
        }

        /// <summary>
        /// Creates an XML file containing a file
        /// </summary>
        public static byte[] XMLDocFile(string filepath, string boundary)
        {
            //Variable local
            List<byte> result = new List<byte>();

            //Crea el encabezado
            string head = string.Empty;
            head += ("--" + boundary + "\r\n");
            head += ("Content-Disposition: form-data; name=\"file1\"; filename=\"" + GetFilenameFromPath(filepath) + "\"" + "\r\n");
            head += ("Content-Type: application/octet-stream" + "\r\n");
            head += ("Content-Transfer-Encoding: binary" + "\r\n");
            head += "\r\n";
            result.AddRange(BytesFromBase64String(head));

            //Abre el archivo en solo lectura
            FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                byte[] readedBuffer = new byte[bytesRead];
                Array.Copy(buffer, readedBuffer, bytesRead);
                result.AddRange(readedBuffer);
            }

            //Cierra el bloque
            string end = string.Empty;
            head += "\r\n";
            head += ("--" + boundary + "--");
            result.AddRange(BytesFromBase64String(end));

            return result.ToArray();
        }

        #endregion Public Methods

        #region Non Public Methods

        #endregion Non Public Methods
    }
}