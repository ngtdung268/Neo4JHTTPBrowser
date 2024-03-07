using Neo4JHTTPBrowser.DTOs;
using Neo4JHTTPBrowser.Helpers;
using Neo4JHTTPBrowser.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Neo4JHTTPBrowser
{
    internal sealed class Neo4JApiService
    {
        private readonly RestClient client;

        private AsyncRetryPolicy<RestResponse> waitAndRetryPolicy;
        private const int RetryCount = 6;
        private static readonly long MaxDelayTicks = TimeSpan.FromSeconds(30).Ticks;
        private static readonly TimeSpan MedianFirstRetryDelay = TimeSpan.FromSeconds(1);
        private static readonly HttpStatusCode[] RetryOnStatusCodes = new[]
        {
            HttpStatusCode.BadGateway,
            HttpStatusCode.ServiceUnavailable,
            HttpStatusCode.GatewayTimeout
        };

        private static readonly Neo4JApiService instance = new Neo4JApiService();

        private Neo4JApiService()
        {
            client = CreateClient();
        }

        public static Neo4JApiService Instance => instance;

        public Task<QueryResponseDTO> QueryAsync(QueryRequestDTO payload)
        {
            var request = CreatePostRequest("db/neo4j/tx/commit", payload);
            return ExecuteWithRetryAsync<QueryResponseDTO>(request);
        }

        private static RestClient CreateClient()
        {
            var options = new RestClientOptions(Settings.Default.Neo4JBaseUrl)
            {
                RemoteCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true,
                ThrowOnAnyError = true,
                UserAgent = $"{Assembly.GetEntryAssembly().GetName().Name} ({Assembly.GetEntryAssembly().GetName().Version})"
            };

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DefaultValueHandling = DefaultValueHandling.Include,
                TypeNameHandling = TypeNameHandling.None,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            };

            return new RestClient(
                options,
                configureSerialization: s => s.UseNewtonsoftJson(jsonSerializerSettings)
            );
        }

        private static RestRequest CreateGetRequest(string path, Dictionary<string, object> queryParams = null)
        {
            var request = new RestRequest(path, Method.Get);

            if (queryParams != null && queryParams.Any())
            {
                foreach (var param in queryParams)
                {
                    request.AddParameter(Parameter.CreateParameter(param.Key, param.Value, ParameterType.QueryString));
                }
            }

            return request;
        }

        private static RestRequest CreatePostRequest(string path, object body = null)
        {
            var request = new RestRequest(path, Method.Post);
            request.AddHeader("Content-Type", "application/json");

            if (body != null)
            {
                request.AddJsonBody(JsonConvert.SerializeObject(body));
            }

            return request;
        }

        private async Task<T> ExecuteAsync<T>(RestRequest request) where T : class
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var response = await client.ExecuteAsync<T>(request);

            Trace(client, request, response);

            return response?.Data;
        }

        // https://gist.github.com/pedrovasconcellos/cf2b8dcde14313e19a891408c3404337
        private async Task<T> ExecuteWithRetryAsync<T>(RestRequest request) where T : class
        {
            var retryPolicy = GetWaitAndRetryPolicy();

            var restResponse = await retryPolicy.ExecuteAsync(async () => await client.ExecuteAsync<T>(request));
            if (restResponse == null)
            {
                return null;
            }

            try
            {
                var response = restResponse as RestResponse<T>;
                return response?.Data;
            }
            finally
            {
                Trace(client, request, restResponse);
            }
        }

        private AsyncRetryPolicy<RestResponse> GetWaitAndRetryPolicy()
        {
            if (waitAndRetryPolicy == null)
            {
                var sleepDurations = Backoff
                    .DecorrelatedJitterBackoffV2(MedianFirstRetryDelay, RetryCount)
                    .Select(s => TimeSpan.FromTicks(Math.Min(s.Ticks, MaxDelayTicks)));

                waitAndRetryPolicy = Policy
                    .HandleResult<RestResponse>(r => RetryOnStatusCodes.Contains(r.StatusCode))
                    .WaitAndRetryAsync(
                        sleepDurations,
                        (response, timeSpan, retryCount, context) =>
                        {
                            LogHelper.Error($"Failed to request {response.Result.ResponseUri}, will retry after {timeSpan} (Attempts={retryCount}, HttpStatusCode={response.Result.StatusCode}, Response={response.Result.Content})");
                        });
            }

            return waitAndRetryPolicy;
        }

        private void Trace(RestClient client, RestRequest request, RestResponse response)
        {
            if (client == null || request == null || response == null)
            {
                return;
            }

            LogHelper.Info($"Request: {request.Method} {client.BuildUri(request).AbsoluteUri}");

#if DEBUG
            var requestToLog = new
            {
                Parameters = request.Parameters
                    .Where(p => p.Name != "Authorization")
                    .Select(p => new
                    {
                        p.Name,
                        p.Value,
                        Type = p.Type.ToString()
                    }),
            };
            LogHelper.Debug(JsonConvert.SerializeObject(requestToLog));

            var responseToLog = new
            {
                response.StatusCode,
                response.ErrorMessage,
                response.Content,
                Headers = response.Headers
                    .Where(p => p.Name != "Authorization")
                    .Select(p => new
                    {
                        p.Name,
                        p.Value,
                        Type = p.Type.ToString(),
                        p.ContentType,
                        p.Encode
                    }),
            };

            LogHelper.Debug(JsonConvert.SerializeObject(responseToLog));
#endif
        }
    }
}
