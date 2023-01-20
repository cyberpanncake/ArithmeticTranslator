namespace Interpretation.Interpretation.Data
{
    /// <summary>
    /// Ячейка, хранящая данные типа целое число
    /// </summary>
    class IntData : NumericData
    {
        /// <summary>
        /// Значение данных (целое число)
        /// </summary>
        public int Value { get; }

        public IntData()
        {
            Value = 0;
        }

        public IntData(int value)
        {
            Value = value;
        }

        override
        public string ToString()
        {
            return $"{Value}";
        }
    }
}