﻿using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using static DocuSign.eSign.Api.EnvelopesApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace qs_csharp.Pages
{
    public class ListEnvelopesModel : PageModel
    {
        // Constants need to be set:
        private const string accessToken = "eyJ0eXAiOiJNVCIsImFsZyI6IlJTMjU2Iiwia2lkIjoiNjgxODVmZjEtNGU1MS00Y2U5LWFmMWMtNjg5ODEyMjAzMzE3In0.AQoAAAABAAUABwCA5k5qZKbWSAgAgCZyeKem1kgCAAvmCyXou1tFoib-l9kBPGoVAAEAAAAYAAEAAAAFAAAADQAkAAAAZjBmMjdmMGUtODU3ZC00YTcxLWE0ZGEtMzJjZWNhZTNhOTc4EgABAAAACwAAAGludGVyYWN0aXZlMAAAULZpZKbWSDcARWK2UOFk7kKfwmjL9SDEEA.YVy_uVXY01bSPXa05vpqHKHdsiJEIluKZOpLfQxxQrOj2dylPRVUIjtUSbfFpD63XnjlPQQ01AxJ1WH-LdDymIlJ48iFzQ-KwvWtj-_KZy5w8fGr5_zOL_ZXLkYWjnTFrd8P8iVHm-InBUpqKn6igz-1gV_YZ9PHMYd_MCfxyu89QPf-C8JuqefQ3hKjD4AlQJWvt4pwLEq0OURkuPkrnDpG9KRMxEaZUADnpiyNXgDViwyAqX_0T9ywjUrLAKuZD-nJge3_S9BPqw0OjTF8_ax8ty9-vlCdRJuz4ddlw10jhpgafXRXrvouljeMrs8p_9EHnhj25vW-JDcnemnVzg";
        private const string accountId = "8076693";
        private const int envelopesAgeDays = -10;

        // Additional constants
        private const string basePath = "https://demo.docusign.net/restapi";

        public void OnGet()
        {
            // List the user's envelopes created in the last 10 days
            // 1. Create request options
            // 2. Use the SDK to list the envelopes

            // 1. Create request options
            ListStatusChangesOptions options = new ListStatusChangesOptions();
            DateTime date = DateTime.Now.AddDays(envelopesAgeDays);
            options.fromDate = date.ToString("yyyy/MM/dd");

            // 2. Use the SDK to list the envelopes
            ApiClient apiClient = new ApiClient(basePath);
            apiClient.Configuration.AddDefaultHeader("Authorization", "Bearer " + accessToken);
            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient.Configuration);
            EnvelopesInformation results = envelopesApi.ListStatusChanges(accountId, options);

            // Prettyprint the results
            string json = JsonConvert.SerializeObject(results);
            string jsonFormatted = JValue.Parse(json).ToString(Formatting.Indented);
            ViewData["results"] = jsonFormatted;

            return;
        }
    }
}
