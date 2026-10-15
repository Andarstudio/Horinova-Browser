using CefSharp;
using CefSharp.Handler;

namespace Horinova
{
    public class BlockRequestHandler : RequestHandler
    {
        protected override IResourceRequestHandler GetResourceRequestHandler(
            IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request,
            bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            return new BlockResourceHandler();
        }
    }

    public class BlockResourceHandler : ResourceRequestHandler
    {
        protected override CefReturnValue OnBeforeResourceLoad(
            IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            if (request.ResourceType != ResourceType.MainFrame && FilterEngine.ShouldBlock(request.Url))
            {
                BlockStats.Hit();
                return CefReturnValue.Cancel;
            }
            return CefReturnValue.Continue;
        }
    }
}
