using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bodoconsult.Finance.Quotations.Helpers;

/// <summary>
/// Fore Http2 usage for the HttpClient instance
/// </summary>
public class ForceHttp2Handler : DelegatingHandler
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="innerHandler">HTTP message handler</param>
    public ForceHttp2Handler(HttpMessageHandler innerHandler)
        : base(innerHandler)
    { }

    /// <summary>Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.</summary>
    /// <param name="request">The HTTP request message to send to the server.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> was <see langword="null" />.</exception>
    /// <exception cref="T:System.OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <returns>The task object representing the asynchronous operation.</returns>
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Version = HttpVersion.Version20;
        return base.SendAsync(request, cancellationToken);
    }
}