using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ess;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ETA.Integrator.Server.Services.Consumer
{
    public class InvoiceSignerService
    {
        public string BinCode = "";
        public InvoiceSignerService(string binCode)
        {
            this.BinCode = binCode;
        }
        public string SignDocument(string SourceDocumentJson)
        {
            string cades = "";
            JObject? request = JsonConvert.DeserializeObject<JObject>(SourceDocumentJson, new JsonSerializerSettings()
            {
                FloatFormatHandling = FloatFormatHandling.String,
                FloatParseHandling = FloatParseHandling.Decimal,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = DateParseHandling.None,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy() {
                        ProcessDictionaryKeys = true,
                        OverrideSpecifiedNames = true
                    }
                }
            });
            if (request == null) throw new Exception("request cannot be null");
            //Start serialize
            String canonicalString = SerializeJSON(request);
            var requestString = request.ToString();
            var documentVersion = request["documentTypeVersion"];
            if (documentVersion == null) throw new Exception("document type version doesn't exist");
            // retrieve cades
            if (documentVersion.Value<string>() == "0.9")
            {
                cades = "ANY";
            }
            else
            {
                cades = SignWithCMS(canonicalString);
            }

            JObject signaturesObject = new JObject(
                                    new JProperty("signatureType", "I"),
                                    new JProperty("value", cades));
            JArray signaturesArray = [signaturesObject];
            request.Add("signatures", signaturesArray);
        

            return JsonConvert.SerializeObject(request);
        }
        public string SignWithCMS(String serializedText)
        {
            byte[] data = Encoding.UTF8.GetBytes(serializedText);
            var DllLibPath = @"C:\Windows\System32\eps2003csp11.dll" ;
            Pkcs11InteropFactories factories = new Pkcs11InteropFactories();
            using (IPkcs11Library pkcs11Library = factories.Pkcs11LibraryFactory.LoadPkcs11Library(factories, DllLibPath, AppType.MultiThreaded))
            {
                ISlot? slot = pkcs11Library.GetSlotList(SlotsType.WithTokenPresent).FirstOrDefault();

                if (slot is null)
                {
                    throw new Exception("No slots found");
                }

                ITokenInfo tokenInfo = slot.GetTokenInfo();

                ISlotInfo slotInfo = slot.GetSlotInfo();

                using (var session = slot.OpenSession(SessionType.ReadWrite))
                {
                    session.Login(CKU.CKU_USER, Encoding.UTF8.GetBytes(BinCode));

                    var certificateSearchAttributes = new List<IObjectAttribute>()
                {
                    session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CLASS, CKO.CKO_CERTIFICATE),
                    session.Factories.ObjectAttributeFactory.Create(CKA.CKA_TOKEN, true),
                    session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CERTIFICATE_TYPE, CKC.CKC_X_509)
                };

                    IObjectHandle? certificate = session.FindAllObjects(certificateSearchAttributes).FirstOrDefault();

                    if (certificate is null)
                    {
                        throw new Exception( "Certificate not found");
                    }

                    X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                    store.Open(OpenFlags.MaxAllowed);

                    // find cert by thumbprint
                    var foundCerts = store.Certificates.Find(X509FindType.FindByIssuerName, "Egypt Trust CA G6", false);

                    //var foundCerts = store.Certificates.Find(X509FindType.FindBySerialNumber, "2b1cdda84ace68813284519b5fb540c2", true);

                    if (foundCerts.Count == 0)
                    {
                        throw new Exception("no device detected");
                    }

                    var certForSigning = foundCerts[0];
                    store.Close();

                    ContentInfo content = new ContentInfo(new Oid("1.2.840.113549.1.7.5"), data);
                    SignedCms cms = new SignedCms(content, true);

                    EssCertIDv2 bouncyCertificate = new EssCertIDv2(
                        new Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier(new DerObjectIdentifier("1.2.840.113549.1.9.16.2.47")),
                        HashBytes(certForSigning.RawData)
                    );

                    SigningCertificateV2 signerCertificateV2 = new SigningCertificateV2(new EssCertIDv2[] { bouncyCertificate });
                    CmsSigner signer = new CmsSigner(certForSigning);

                    signer.DigestAlgorithm = new Oid("2.16.840.1.101.3.4.2.1");
                    signer.SignedAttributes.Add(new Pkcs9SigningTime(DateTime.UtcNow));
                    signer.SignedAttributes.Add(new AsnEncodedData(new Oid("1.2.840.113549.1.9.16.2.47"), signerCertificateV2.GetEncoded()));
                    cms.ComputeSignature(signer);
                    var output = cms.Encode();

                    return Convert.ToBase64String(output);
                }
            }
        }
        private byte[] HashBytes(byte[] input)
        {
            using (SHA256 sha = SHA256.Create())
            {
                var output = sha.ComputeHash(input);
                return output;
            }
        }
        public string SerializeJSON(JObject request)
        {
            return SerializeJSONToken(request);
        }

        private string SerializeJSONToken(JToken request)
        {
            string serialized = "";
            if (request.Parent is null)
            {
                SerializeJSONToken(request.First);
            }
            else
            {
                if (request.Type == JTokenType.Property)
                {
                    string name = ((JProperty)request).Name.ToUpper();
                    serialized += "\"" + name + "\"";
                    foreach (var property in request)
                    {
                        if (property.Type == JTokenType.Object)
                        {
                            serialized += SerializeJSONToken(property);
                        }
                        if (property.Type == JTokenType.Boolean || property.Type == JTokenType.Integer || property.Type == JTokenType.Float || property.Type == JTokenType.Date)
                        {
                            serialized += "\"" + property.Value<string>() + "\"";
                        }
                        if (property.Type == JTokenType.String)
                        {
                            serialized += JsonConvert.ToString(property.Value<string>());
                        }
                        if (property.Type == JTokenType.Array)
                        {
                            foreach (var item in property.Children())
                            {
                                serialized += "\"" + ((JProperty)request).Name.ToUpper() + "\"";
                                serialized += SerializeJSONToken(item);
                            }
                        }
                    }
                }
                // Added to fix "References"
                if (request.Type == JTokenType.String)
                {
                    serialized += JsonConvert.ToString(request.Value<string>());
                }
            }
            if (request.Type == JTokenType.Object)
            {
                foreach (var property in request.Children())
                {

                    if (property.Type == JTokenType.Object || property.Type == JTokenType.Property)
                    {
                        serialized += SerializeJSONToken(property);
                    }
                }
            }

            return serialized;
        }
    }
}
