﻿using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Kartverket.Geonorge.Utilities.Organization;
using NUnit.Framework;

namespace Kartverket.Geonorge.Utilities.Tests.Organization
{
    class OrganizationServiceTest
    {
        [Test]
        public void ShouldReturnOrganizationWhenFoundByName()
        {
            const string registryUrl = "http://dummy";
            const string content = @"{Number:""123456789"", Name:""Kartverket"", LogoUrl:""http://example.com/logo.png""}";

            var httpClient = CreateHttpClient(HttpStatusCode.OK, content);

            var service = new OrganizationService(registryUrl, httpClient);
            Task<Utilities.Organization.Organization> task = service.GetOrganizationByName("Kartverket");
            Utilities.Organization.Organization organization = task.Result;

            organization.Should().NotBeNull();
            organization.Number.Should().Be("123456789");
            organization.Name.Should().Be("Kartverket");
            organization.LogoUrl.Should().Be("http://example.com/logo.png");
        }

        [Test]
        public void ShouldReturnNullWhenNoContentFound()
        {
            const string registryUrl = "http://dummy";
            const string content = "";

            var httpClient = CreateHttpClient(HttpStatusCode.NotFound, content);

            var service = new OrganizationService(registryUrl, httpClient);
            Task<Utilities.Organization.Organization> task = service.GetOrganizationByName("Kartverket");
            Utilities.Organization.Organization organization = task.Result;

            organization.Should().BeNull();
        }

        private HttpClient CreateHttpClient(HttpStatusCode httpStatusCode, string content)
        {
            var response = new HttpResponseMessage(httpStatusCode);
            response.Content = new StringContent(content, Encoding.UTF8, "application/json");
            var httpClient = new HttpClient(new FakeHttpHandler
            {
                Response = response,
                InnerHandler = new HttpClientHandler()
            });
            return httpClient;
        }
    }

    public class FakeHttpHandler : DelegatingHandler
    {
        public HttpResponseMessage Response { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                               CancellationToken cancellationToken)
        {
            if (Response == null)
            {
                return base.SendAsync(request, cancellationToken);
            }

            return Task.Factory.StartNew(() => Response);
        }
    }
}

