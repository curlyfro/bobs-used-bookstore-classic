namespace Bookstore.Domain.ReferenceData
{
    public class CreateReferenceDataItemDto
    {
        public CreateReferenceDataItemDto(ReferenceDataType referenceDataType, string text)
        {
            this.ReferenceDataType = referenceDataType;
            this.Text = text;
        }

        public ReferenceDataType ReferenceDataType { get; }
        public string Text { get; }
    }

    public class UpdateReferenceDataItemDto
    {
        public UpdateReferenceDataItemDto(int id, ReferenceDataType referenceDataType, string text)
        {
            this.Id = id;
            this.ReferenceDataType = referenceDataType;
            this.Text = text;
        }

        public int Id { get; }
        public ReferenceDataType ReferenceDataType { get; }
        public string Text { get; }
    }
}
