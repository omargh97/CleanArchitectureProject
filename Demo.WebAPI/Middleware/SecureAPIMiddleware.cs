using Demo.WebAPI.Infrastructure.Security;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Demo.WebAPI.Middleware
{
    public class SecureAPIMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SecurityConfig _securityConfig;
        private readonly ILogger _logger;

        public SecureAPIMiddleware(RequestDelegate next, SecurityConfig securityConfig, ILoggerFactory loggerFactory)
        {
            _next = next;
            _securityConfig = securityConfig;
            _logger = loggerFactory.CreateLogger<SecureAPIMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            object Action;
            object Controller;

            context.Request.RouteValues.TryGetValue("action", out Action);
            context.Request.RouteValues.TryGetValue("controller", out Controller);

            if (_securityConfig.EnbableAPISecurity == true && !_securityConfig.ActionDontNeedSecure(Action?.ToString(), Controller?.ToString()))
            {

                context.Request.ContentType = "application/text";

                var request = context.Request.Body;
                var response = context.Response.Body;
                var originRequest = string.Empty;
                var originResponse = string.Empty;
                try
                {

                    //if (context.Request.QueryString.HasValue)
                    //{
                    //    string decryptedString = DecodeServerName(context.Request.QueryString.Value.Substring(1));
                    //    context.Request.QueryString = new QueryString($"?{decryptedString}");
                    //}

                    using (var newRequest = new MemoryStream())
                    {
                        // replace request flow
                        context.Request.Body = newRequest;

                        using (var newResponse = new MemoryStream())
                        {
                            // replace the response stream
                            context.Response.Body = newResponse;

                            using (var reader = new StreamReader(request))
                            {
                                // read the contents of the original request stream
                                originRequest = await reader.ReadToEndAsync();
                                if (string.IsNullOrEmpty(originRequest)) await _next.Invoke(context);
                                originRequest = DecodeServerName(originRequest);

                                // log request after decryption
                                _logger.LogInformation($"REQUEST METHOD: {context.Request.Method} - REQUEST BODY: {originRequest.Replace("\n", "").Replace("\r", "")} - REQUEST URL: {UriHelper.GetDisplayUrl(context.Request)}");

                            }

                            using (var writer = new StreamWriter(newRequest))
                            {
                                await writer.WriteAsync(originRequest);
                                await writer.FlushAsync();
                                newRequest.Position = 0;
                                context.Request.Body = newRequest;
                                context.Request.ContentType = "application/json";
                                await _next(context);
                            }

                            using (var reader = new StreamReader(newResponse))
                            {
                                newResponse.Seek(0, SeekOrigin.Begin);
                                originResponse = await reader.ReadToEndAsync();
                                if (!string.IsNullOrWhiteSpace(originResponse))
                                {
                                    // log response befor encryption
                                    _logger.LogInformation($"RESONSE STATUS CODE: {context.Response.StatusCode}");

                                    originResponse = EncodeServerName(originResponse);
                                }

                                using (var writer = new StreamWriter(response))
                                {
                                    context.Response.ContentType = "application/text";
                                    context.Response.ContentLength = GenerateStreamFromString(originResponse).Length;
                                    await writer.WriteAsync(originResponse);
                                    await writer.FlushAsync();
                                }
                            }
                        }
                    }
                }
                catch (CryptographicException cex)
                {
                    _logger.LogCritical("error ",cex);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogCritical("error ",ex);
                    throw;
                }
                finally
                {
                    context.Request.Body = request;
                    context.Response.Body = response;
                }
            }
            else
            {
                await _next(context);
            }            
        }

        public string DecodeServerName(string encodedServername)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encodedServername));
        }

        public string EncodeServerName(string serverName)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(serverName));
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
