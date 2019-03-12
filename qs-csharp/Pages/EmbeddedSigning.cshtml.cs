using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;


namespace qs_csharp.Pages
{
    public class EmbeddedSigning : PageModel
    {
        // Constants need to be set:
        private const string accessToken = "eyJ0eXAiOiJNVCIsImFsZyI6IlJTMjU2Iiwia2lkIjoiNjgxODVmZjEtNGU1MS00Y2U5LWFmMWMtNjg5ODEyMjAzMzE3In0.AQoAAAABAAUABwCA5k5qZKbWSAgAgCZyeKem1kgCAAvmCyXou1tFoib-l9kBPGoVAAEAAAAYAAEAAAAFAAAADQAkAAAAZjBmMjdmMGUtODU3ZC00YTcxLWE0ZGEtMzJjZWNhZTNhOTc4EgABAAAACwAAAGludGVyYWN0aXZlMAAAULZpZKbWSDcARWK2UOFk7kKfwmjL9SDEEA.YVy_uVXY01bSPXa05vpqHKHdsiJEIluKZOpLfQxxQrOj2dylPRVUIjtUSbfFpD63XnjlPQQ01AxJ1WH-LdDymIlJ48iFzQ-KwvWtj-_KZy5w8fGr5_zOL_ZXLkYWjnTFrd8P8iVHm-InBUpqKn6igz-1gV_YZ9PHMYd_MCfxyu89QPf-C8JuqefQ3hKjD4AlQJWvt4pwLEq0OURkuPkrnDpG9KRMxEaZUADnpiyNXgDViwyAqX_0T9ywjUrLAKuZD-nJge3_S9BPqw0OjTF8_ax8ty9-vlCdRJuz4ddlw10jhpgafXRXrvouljeMrs8p_9EHnhj25vW-JDcnemnVzg";
        private const string accountId = "8076693";
        private const string signerName = "Karim Jalil";
        private const string signerEmail = "karim.jalil@accuv.com";
        private const string docName = "NDA.pdf";

        // Additional constants
        private const string signerClientId = "1000";
        private const string basePath = "https://demo.docusign.net/restapi";

        // Change the port number in the Properties / launchSettings.json file:
        //     "iisExpress": {
        //        "applicationUrl": "http://localhost:5050",
        private const string returnUrl = "http://localhost:5050/DSReturn";

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Embedded Signing Ceremony
            // 1. Create envelope request obj
            // 2. Use the SDK to create and send the envelope
            // 3. Create Envelope Recipient View request obj
            // 4. Use the SDK to obtain a Recipient View URL
            // 5. Redirect the user's browser to the URL

            // 1. Create envelope request object
            //    Start with the different components of the request
            //    Create the document object
            Document document = new Document
            { DocumentBase64 = Convert.ToBase64String(ReadContent(docName)),
              Name = "Non-Disclosure Agreement", FileExtension = "pdf", DocumentId = "1"
            };
            Document[] documents = new Document[] { document };

            // Create the signer recipient object 
            Signer signer = new Signer
            { Email = signerEmail, Name = signerName, ClientUserId = signerClientId,
              RecipientId = "1", RoutingOrder = "1"
            };

            // Create the sign here tab (signing field on the document)
            SignHere signHereTab = new SignHere
            { DocumentId = "1", PageNumber = "4", RecipientId = "1",
              TabLabel = "Sign Here Tab", XPosition = "173", YPosition = "410"
            };
            SignHere[] signHereTabs = new SignHere[] { signHereTab };

            // Add the sign here tab array to the signer object.
            signer.Tabs = new Tabs { SignHereTabs = new List<SignHere>(signHereTabs) };
            // Create array of signer objects
            Signer[] signers = new Signer[] { signer };
            // Create recipients object
            Recipients recipients = new Recipients { Signers = new List<Signer>(signers) };
            // Bring the objects together in the EnvelopeDefinition
            EnvelopeDefinition envelopeDefinition = new EnvelopeDefinition
            { EmailSubject = "Non-Disclosure Agreement",
              Documents = new List<Document>( documents ),
              Recipients = recipients,
              Status = "sent"
            };

            // 2. Use the SDK to create and send the envelope
            ApiClient apiClient = new ApiClient(basePath);
            apiClient.Configuration.AddDefaultHeader("Authorization", "Bearer " + accessToken);
            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient.Configuration);
            EnvelopeSummary results = envelopesApi.CreateEnvelope(accountId, envelopeDefinition);

            // 3. Create Envelope Recipient View request obj
            string envelopeId = results.EnvelopeId;
            RecipientViewRequest viewOptions = new RecipientViewRequest
           {
              ReturnUrl = returnUrl, ClientUserId = signerClientId,
              AuthenticationMethod = "none",
              UserName = signerName, Email = signerEmail
            };

            // 4. Use the SDK to obtain a Recipient View URL
            ViewUrl viewUrl = envelopesApi.CreateRecipientView(accountId, envelopeId, viewOptions);

            // 5. Redirect the user's browser to the URL
            return Redirect(viewUrl.Url);
        }

        /// <summary>
        /// This method read bytes content from the project's Resources directory
        /// </summary>
        /// <param name="fileName">resource path</param>
        /// <returns>return bytes array</returns>
        internal static byte[] ReadContent(string fileName)
        {
            byte[] buff = null;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Resources", fileName);
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    long numBytes = new FileInfo(path).Length;
                    buff = br.ReadBytes((int)numBytes);
                }
            }
            return buff;
        }
    }
}
