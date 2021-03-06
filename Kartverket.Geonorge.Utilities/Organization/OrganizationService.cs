﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Kartverket.Geonorge.Utilities.Logging;

namespace Kartverket.Geonorge.Utilities.Organization
{
    public class OrganizationService : IOrganizationService
    {
        private static readonly ILog Logger = LogProvider.For<OrganizationService>(); 

        private readonly string _registryUrl;
        private readonly IHttpClientFactory _factory;

        public OrganizationService(string registryUrl, IHttpClientFactory factory)
        {
            _registryUrl = registryUrl;
            _factory = factory;
        }

        public async Task<Organization> GetOrganizationByName(string name)
        {
            Logger.Debug(string.Format("Looking up organization by name: {0}", name));
            
            HttpClient client = _factory.GetHttpClient();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync(_registryUrl + "api/organisasjon/navn/" + name).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                Organization organization = await response.Content.ReadAsAsync<Organization>().ConfigureAwait(false);

                Logger.Debug(string.Format("Organization [{0}] found. [Number={1}], [LogoUrl={2}], [LogoLargeUrl={3}], [ShortName={4}]", organization.Name, organization.Number, organization.LogoUrl, organization.LogoLargeUrl, organization.ShortName));
                
                return organization;
            }
            
            Logger.Debug(string.Format("Organization [{0}] not found. Http response code: {1}", name, response.StatusCode));
            
            return null;
        }
    }

    
}
