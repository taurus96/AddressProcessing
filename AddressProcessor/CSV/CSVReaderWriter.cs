using System;
using System.IO;
using System.Text;

namespace AddressProcessing.CSV
{
    /*
        2) Refactor this class into clean, elegant, rock-solid & well performing code, without over-engineering.
           Assume this code is in production and backwards compatibility must be maintained.
    */

    public class CSVReaderWriter
    {
        private StreamReader _readerStream = null;
        private StreamWriter _writerStream = null;
        private FileInfo _fileInfo = null;

        // string manipulation variables
        private const int FIRST_COLUMN = 0;
        private const int SECOND_COLUMN = 1;
        private string[] columns;
        private char[] separator = { '\t' };

        [Flags]
        public enum Mode { Read = 1, Write = 2 };

        public void Open(string fileName, Mode mode)
        {
            try
            {
                if (!File.Exists(fileName)) {
                    throw new Exception("File does not exists at the given path. Path details - " + fileName);
                }
                switch (mode)
                {
                    case Mode.Read:
                        _readerStream = File.OpenText(fileName);
                        break;
                    case Mode.Write:
                        _fileInfo = new FileInfo(fileName);
                        _writerStream = _fileInfo.CreateText();
                        break;
                    default:
                        throw new Exception("Unknown file mode for " + fileName);
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("Null arguments exception occurred. Details - " + e.InnerException);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("User not permitted to access the file - " + e.InnerException);
            }
        }

        public void Write(params string[] columns)
        {
            int colLength = columns.Length;
            StringBuilder outPut = new StringBuilder();

            for (int i = 0; i < colLength; i++)
            {
                outPut.Append(columns[i]);
                if ((colLength - 1) != i)
                {
                    outPut.Append("\t");
                }
            }
            WriteLine(outPut.ToString(), _fileInfo);
        }
        
        public bool Read(string column1, string column2)
        {
            string line = ReadLine(_fileInfo);
            string col1 = column1;
            string col2 = column2;
            return ProcessLine(out col1, out col2, line);
        }

        public bool Read(out string column1, out string column2)
        {
            string line = ReadLine(_fileInfo);
            if (line == null)
            {
                column1 = null;
                column2 = null;
                return false;
            }
            return ProcessLine(out column1, out column2, line);
        }

        private bool ProcessLine(out string column1, out string column2, string line)
        {
            columns = line.Split(separator);
            if (columns.Length == 0)
            {
                column1 = null;
                column2 = null;
                return false;
            }
            else
            {
                column1 = columns[FIRST_COLUMN];
                column2 = columns[SECOND_COLUMN];
                Array.Clear(columns, 0, columns.Length);
                return true;
            }
        }

        private void WriteLine(string line, FileInfo fileInfo)
        {
            using (StreamWriter _writerStream = new StreamWriter(fileInfo.FullName))
            {
                _writerStream.WriteLine(line);
            }
        }

        private string ReadLine(FileInfo fileInfo)
        {
            using (StreamReader sr = fileInfo.OpenText())
            {
                return _readerStream.ReadLine();
            }
        }
        
        public void Close()
        {
            if (_writerStream != null)
            {
                ((IDisposable)_writerStream).Dispose();
            }

            if (_readerStream != null)
            {
                ((IDisposable)_readerStream).Dispose();
            }
        }
    }
}
