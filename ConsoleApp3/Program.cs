using Azure.Core;
using Azure.Identity;
using Azure.Identity.BrokeredAuthentication;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    internal class Program
    {
        public const string PowerShellClientId = "1950a258-227b-4e31-a9cf-717495945fc2";
        
        /// <summary>
        /// Uses Azure Identity + broker in interactive then silent mode.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task Main (string[] args)
        {
            try
            {
                
                TokenRequestContext requestContext = new TokenRequestContext(
                    new[] { "https://management.core.windows.net//.default" });

                TokenCachePersistenceOptions tokenCachePersistenceOptions = new TokenCachePersistenceOptions()
                {
                    UnsafeAllowUnencryptedStorage = true
                };

                InteractiveBrowserCredentialBrokerOptions options = 
                    new InteractiveBrowserCredentialBrokerOptions(GetConsoleWindow())
                {
                    ClientId = PowerShellClientId,
                    TenantId = "organizations",
                    AuthorityHost = new Uri("https://login.windows-ppe.net/"),
                    RedirectUri = new Uri($"ms-appx-web://microsoft.aad.brokerplugin/{PowerShellClientId}"),
                    TokenCachePersistenceOptions = tokenCachePersistenceOptions,                                       
                };

                InteractiveBrowserCredential interactiveBrowserCredential = new InteractiveBrowserCredential(options);
                AuthenticationRecord result = await interactiveBrowserCredential.AuthenticateAsync(requestContext);

                //Console.ReadLine();

                TokenCachePersistenceOptions tokenCachePersistenceOptions2 = new TokenCachePersistenceOptions()
                {
                    UnsafeAllowUnencryptedStorage = true
                };
                SharedTokenCacheCredentialOptions sharedOptions = new SharedTokenCacheCredentialOptions(
                    tokenCachePersistenceOptions2)
                {
                    EnableGuestTenantAuthentication = true,
                    ClientId = PowerShellClientId,
                    Username = result.Username,
                    AuthorityHost = new Uri("https://login.windows-ppe.net/"),
                    TenantId = result.TenantId
                };
                SharedTokenCacheCredential sharedTokenCacheCredential = new SharedTokenCacheCredential();
                var result2 = await sharedTokenCacheCredential.GetTokenAsync(requestContext);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR");
                Console.WriteLine(e);
                Console.ResetColor();
                
                Console.ReadLine();                
            }
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
    }
}
