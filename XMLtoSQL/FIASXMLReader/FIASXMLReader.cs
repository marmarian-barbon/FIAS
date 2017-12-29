namespace FIASXMLReader
{
    using System;
    using System.Data;
    using System.Xml;

    public class FIASXMLReader : System.Data.IDataReader
    {
        private XmlReader _xmlReader;
        private Func<XmlReader, bool> _predicate;
        private Func<XmlReader, object>[] _converter;

        public FIASXMLReader(XmlReader xmlReader, Func<XmlReader, bool> predicate, Func<XmlReader, object>[] converter)
        {
            this._xmlReader = xmlReader;
            this._predicate = predicate;
            this._converter = converter;
        }

        public object this[int i] => this._xmlReader[i];

        public object this[string name] => this._xmlReader[name];

        public int Depth => this._xmlReader.Depth;

        public bool IsClosed => false;

        public int RecordsAffected => new Random().Next();

        public int FieldCount => 4;

        public void Close()
        {
            this._xmlReader.Close();
        }

        public void Dispose()
        {
            this.Close();
        }

        public bool GetBoolean(int i)
        {
            var value = this._xmlReader[i];
            return (value == "1");
        }

        public byte GetByte(int i)
        {
            var value = this._xmlReader[i];
            var result = (byte)0;
            if (byte.TryParse(value, out result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return 0;
        }

        public char GetChar(int i)
        {
            var value = this._xmlReader[i];
            if (value.Length == 1)
            {
                return value[0];
            }
            else
            {
                return (char)0;
            }
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return 0;
        }

        public IDataReader GetData(int i)
        {
            return new FIASXMLReader(this._xmlReader, this._predicate, new Func<XmlReader, object>[] { this._converter[i] });
        }

        public string GetDataTypeName(int i)
        {
            return "String";
        }

        public DateTime GetDateTime(int i)
        {
            var value = this._xmlReader[i];
            var result = DateTime.MinValue;
            DateTime.TryParse(value, out result);
            return result;
        }

        public decimal GetDecimal(int i)
        {
            var result = decimal.MinValue;
            decimal.TryParse(this._xmlReader[i], out result);
            return result;
        }

        public double GetDouble(int i)
        {
            var result = double.MinValue;
            double.TryParse(this._xmlReader[i], out result);
            return result;
        }

        public Type GetFieldType(int i)
        {
            return null;
        }

        public float GetFloat(int i)
        {
            var result = float.MinValue;
            float.TryParse(this._xmlReader[i], out result);
            return result;
        }

        public Guid GetGuid(int i)
        {
            var result = Guid.Empty;
            Guid.TryParse(this._xmlReader[i], out result);
            return result;
        }

        public short GetInt16(int i)
        {
            var result = short.MinValue;
            short.TryParse(this._xmlReader[i], out result);
            return result;
        }

        public int GetInt32(int i)
        {
            var result = int.MinValue;
            int.TryParse(this._xmlReader[i], out result);
            return result;
        }

        public long GetInt64(int i)
        {
            var result = long.MinValue;
            long.TryParse(this._xmlReader[i], out result);
            return result;
        }

        public string GetName(int i)
        {
            this._xmlReader.MoveToAttribute(i);
            return this._xmlReader.Name + (this._xmlReader.MoveToElement() ? "" : "");
        }

        public int GetOrdinal(string name)
        {
            for (var i=0; i < this._xmlReader.AttributeCount; i++)
            {
                this._xmlReader.MoveToAttribute(i);
                if (this._xmlReader.Name == name)
                {
                    return i + (this._xmlReader.MoveToElement() ? 0 : 0);
                }
            }

            return -1;
        }

        public DataTable GetSchemaTable()
        {
            return null;
        }

        public string GetString(int i)
        {
            return this._xmlReader[i];
        }

        public object GetValue(int i)
        {
            return this._converter[i](this._xmlReader);
        }

        public int GetValues(object[] values)
        {
            for (var i=0; i < values.Length; i++)
            {
                values[i] = this._converter[i](this._xmlReader);
            }

            return values.Length;
        }

        public bool IsDBNull(int i)
        {
            return this._xmlReader[i] == null;
        }

        public bool NextResult()
        {
            return this.Read();
        }

        public bool Read()
        {
            while (this._xmlReader.Read() && !this._predicate(this._xmlReader)) { }
            return !this._xmlReader.EOF;
        }

        IDataReader IDataRecord.GetData(int i)
        {
            return null;
        }

        DataTable IDataReader.GetSchemaTable()
        {
            return null;
        }
    }
}
