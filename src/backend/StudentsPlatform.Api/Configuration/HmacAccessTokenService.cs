using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using StudentsPlatform.Api.Common.Security;
using StudentsPlatform.Application.Auth.Login;
using StudentsPlatform.Application.Common.Abstractions;

namespace StudentsPlatform.Api.Configuration;

public sealed class HmacAccessTokenService(IOptions<AuthOptions> options) : IAccessTokenService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public string CreateToken(UserSessionIdentity identity)
    {
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(GetExpirationMinutes());
        var payload = new AccessTokenPayload(
            identity.Id.ToString("D"),
            identity.FullName,
            identity.Email,
            identity.Role,
            expiresAt.ToUnixTimeSeconds());

        var payloadBytes = JsonSerializer.SerializeToUtf8Bytes(payload, JsonOptions);
        var payloadSegment = WebEncoders.Base64UrlEncode(payloadBytes);
        var signatureSegment = WebEncoders.Base64UrlEncode(Sign(payloadSegment));

        return $"{payloadSegment}.{signatureSegment}";
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var segments = token.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length != 2)
        {
            return null;
        }

        var actualSignature = DecodeSegment(segments[1]);
        if (actualSignature is null)
        {
            return null;
        }

        var expectedSignature = Sign(segments[0]);
        if (!CryptographicOperations.FixedTimeEquals(actualSignature, expectedSignature))
        {
            return null;
        }

        var payloadBytes = DecodeSegment(segments[0]);
        if (payloadBytes is null)
        {
            return null;
        }

        var payload = JsonSerializer.Deserialize<AccessTokenPayload>(payloadBytes, JsonOptions);
        if (payload is null || payload.Exp <= DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        {
            return null;
        }

        if (!Guid.TryParse(payload.Sub, out var userId))
        {
            return null;
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString("D")),
            new(ClaimTypes.Name, payload.Name),
            new(ClaimTypes.Email, payload.Email),
            new(ClaimTypes.Role, payload.Role)
        };

        var identity = new ClaimsIdentity(claims, BearerTokenAuthenticationHandler.SchemeName);
        return new ClaimsPrincipal(identity);
    }

    private byte[] Sign(string payloadSegment)
    {
        var signingKeyBytes = Encoding.UTF8.GetBytes(GetSigningKey());
        using var algorithm = new HMACSHA256(signingKeyBytes);
        return algorithm.ComputeHash(Encoding.UTF8.GetBytes(payloadSegment));
    }

    private string GetSigningKey()
    {
        var signingKey = options.Value.SigningKey?.Trim();
        if (string.IsNullOrWhiteSpace(signingKey))
        {
            throw new InvalidOperationException("Auth signing key is required.");
        }

        return signingKey;
    }

    private int GetExpirationMinutes()
    {
        return options.Value.TokenExpirationMinutes > 0
            ? options.Value.TokenExpirationMinutes
            : 480;
    }

    private static byte[]? DecodeSegment(string segment)
    {
        try
        {
            return WebEncoders.Base64UrlDecode(segment);
        }
        catch (FormatException)
        {
            return null;
        }
    }

    private sealed record AccessTokenPayload(
        string Sub,
        string Name,
        string Email,
        string Role,
        long Exp);
}
