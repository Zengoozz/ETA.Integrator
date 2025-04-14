namespace ETA.Integrator.Server.Models.Consumer.Response
{
    public class ConsumerDocumentTypesResponseModel
    {
        public List<DocumentType> result { get; set; } = new List<DocumentType>();
    }
    public class DocumentType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ActiveFrom { get; set; }
        public DateTime? ActiveTo { get; set; }
        public string DocumentTypeGroup { get; set; } = string.Empty;
        public string DocumentTypeBase { get; set; } = string.Empty;
        public List<DocumentTypeVersion> DocumentTypeVersions { get; set; } = new List<DocumentTypeVersion>();
    }
    public class DocumentTypeVersion
    {
        public int Id { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string VersionNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime ActiveFrom { get; set; }
        public DateTime? ActiveTo { get; set; }
    }
}
