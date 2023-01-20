namespace Interpretation.Interpretation.Data
{
    /// <summary>
    /// Ячейка, хранящая данные типа вещественное число
    /// </summary>
    class FloatData : NumericData
    {
        /// <summary>
        /// Значение данных (вещественное число)
        /// </summary>
        public float Value { get; }

        public FloatData()
        {
            Value = 0;
        }

        public FloatData(float value)
        {
            Value = value;
        }

        override
        public string ToString()
        {
            string s = $"{Value}".Replace(",", ".");
            return s.Contains(".") ? s : s + ".0";
        }
    }
}