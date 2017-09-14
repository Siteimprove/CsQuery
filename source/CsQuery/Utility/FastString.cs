using System.Text;

namespace CsQuery.Utility
{
    /// <summary>
    /// A string-class optimized for appending lots of text. Can support null as the
    /// initial string, which is used by some nodes (<seealso cref="IDomComment"/>).
    /// </summary>
    public class FastString
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        private string _value;
        private bool _isDirty;

        public FastString()
            : this(null)
        {
        }

        public FastString(string value)
        {
            Value = value;
        }

        public string Value
        {
            get
            {
                if (_isDirty)
                {
                    _value = _stringBuilder.ToString();
                    _isDirty = false;
                }
                return _value;
            }
            set
            {
                if (value == null)
                {
                    _value = null;
                    _isDirty = false;
                }
                else
                {
                    _stringBuilder.Clear();
                    AppendValue(value);
                }
            }
        }

        public void AppendValue(string text)
        {
            _stringBuilder.Append(text);
            _isDirty = true;
        }
    }
}
