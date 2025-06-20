namespace Bookstore.Domain.ReferenceData
{
    public class ReferenceDataItem : Entity
    {
        private ReferenceDataItem() { }

        public ReferenceDataItem(ReferenceDataType referenceDataType, string text)
        {
            DataType = referenceDataType;
            Text = text;
        }

        public ReferenceDataType DataType { get; set; }

        public string Text { get; set; }
    }
}
