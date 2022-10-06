using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Demo.WebUi.Providers
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {

        private readonly ILocalStorageService _localStorage;
        private readonly JwtSecurityTokenHandler _tokenGandker;

        public ApiAuthenticationStateProvider(ILocalStorageService localStorage, JwtSecurityTokenHandler tokenGandker)
        {
            _localStorage = localStorage;
            _tokenGandker = tokenGandker;
        }
        public async Task LoggedIn()
        {
            var savedEncryptToken = await _localStorage.GetItemAsync<string>("authToken");
            var (principal, tokenContent) = DecodeJwtToken(savedEncryptToken);
            var authstate  = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaims(tokenContent), "jwt"))));
            NotifyAuthenticationStateChanged(authstate);
        }

        public void LoggedOut()
        {
            var authstate = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            NotifyAuthenticationStateChanged(authstate);
        }


        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var savedEncryptToken = await _localStorage.GetItemAsync<string>("authToken");

                if (string.IsNullOrEmpty(savedEncryptToken))
                {
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                var (principal, tokenContent) = DecodeJwtToken(savedEncryptToken);

                var expiry = tokenContent.ValidTo;

                if (expiry > DateTime.Now)
                {
                    await _localStorage.ClearAsync();
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaims(tokenContent), "jwt")));

            } 
            catch (Exception ex)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            }
        }

 
        private IList<Claim> ParseClaims(JwtSecurityToken tokenContent)
        {
            List<Claim> claims = tokenContent.Claims.ToList();
            claims.Add(new Claim(ClaimTypes.Name, tokenContent.Subject));
            return claims;
        }

        public (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SecurityTokenException("Invalid token");
            }
            try
            {
                ClaimsPrincipal principal = new JwtSecurityTokenHandler()
                                .ValidateToken(token,
                                    new TokenValidationParameters
                                    {
                                        ValidateIssuer = true,
                                        ValidIssuer = "o.tounsi",
                                        ValidateIssuerSigningKey = true,
                                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("8f44947258af9c048ff1d71727cffd350321acb1aa57f5961eedd03bb61bb9f196be971ee27569a8247fb60e7dc920a86cfaa88f0f6ee1e61000b4cf2de97158")),
                                        ValidAudience = "https://O.tounsi.com",
                                        ValidateAudience = true,
                                        ValidateLifetime = true,
                                        ClockSkew = TimeSpan.FromMinutes(1)
                                    },
                                    out var validatedToken);
                return (principal, validatedToken as JwtSecurityToken);
            }
            catch(Exception ex)
            {
                return (null, null);
            }

        }

    }
}
