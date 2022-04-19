using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using Microsoft.Identity.Client;
using System.Windows.Interop;
using Microsoft.Graph;
using System.Net.Http.Headers;
using Application = Microsoft.Graph.Application;
using LibGit2Sharp;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Windows.Storage.Streams;

namespace IdAceCodeEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        //Set the API Endpoint to Graph 'me' endpoint. 
        // To change from Microsoft public cloud to a national cloud, use another value of graphAPIEndpoint.
        // Reference with Graph endpoints here: https://docs.microsoft.com/graph/deployments#microsoft-graph-and-graph-explorer-service-root-endpoints
        string graphAPIEndpoint = "https://graph.microsoft.com/v1.0/applications";

        //Set the scope for API call to user.read
        string[] scopes = new string[] { "user.read", "Application.ReadWrite.All" };

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            Repository.Clone("https://github.com/Azure-Samples/ms-identity-javascript-angular-tutorial.git", @"C:\Users\pramkum\source\repos\MicrosoftIdentityPlatformEditor\Repo");

            // ProcessStartInfo startInfo = new ProcessStartInfo();
            // startInfo.FileName = @"git.exe";
            // startInfo.Arguments = "clone https://github.com/Azure-Samples/ms-identity-javascript-angular-tutorial.git";
            // startInfo.RedirectStandardOutput = true;
            // startInfo.RedirectStandardError = true;
            // startInfo.UseShellExecute = false;
            //// startInfo.CreateNoWindow = true;
            // System.Diagnostics.Process process = new System.Diagnostics.Process();
            // process.StartInfo = startInfo;
            // process.Start();

            //AuthenticationResult authResult = null;
            //var app = App.PublicClientApp;

            //IAccount firstAccount;
            //var accounts = await app.GetAccountsAsync();
            //firstAccount = accounts.FirstOrDefault();
            //try
            //{
            //    authResult = await app.AcquireTokenSilent(scopes, firstAccount)
            //        .ExecuteAsync();
            //}
            //catch (MsalUiRequiredException ex)
            //{
            //    // A MsalUiRequiredException happened on AcquireTokenSilent. 
            //    // This indicates you need to call AcquireTokenInteractive to acquire a token
            //    System.Diagnostics.Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

            //    try
            //    {
            //        authResult = await app.AcquireTokenInteractive(scopes)
            //            .WithAccount(firstAccount)
            //            .WithParentActivityOrWindow(new WindowInteropHelper(this).Handle) // optional, used to center the browser on the window
            //            .WithPrompt(Microsoft.Identity.Client.Prompt.SelectAccount)
            //            .ExecuteAsync();
            //    }
            //    catch (MsalException msalex)
            //    {
            //        MessageBox.Show($"Error Acquiring Token:{System.Environment.NewLine}{msalex}");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}");
            //    return;
            //}

            //if (authResult != null)
            //{
            //    MessageBox.Show(await GetHttpContentWithToken(graphAPIEndpoint, authResult.AccessToken));
            //}
        }
        public async Task<Application> CreateAppRegistration(string token, string appName, string[] redirectUri, AppType type, bool isHybrid = false)
        {
            var httpClient = new System.Net.Http.HttpClient();
            try
            {
                var graphClient = new GraphServiceClient("https://graph.microsoft.com/v1.0", new DelegateAuthenticationProvider(async (requestMessage) =>
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }));

                Application application = null;
                switch (type)
                {
                    case AppType.Web:
                        application = new Application
                        {
                            DisplayName = appName,
                            Web = new WebApplication
                            {
                                RedirectUris = redirectUri,
                                ImplicitGrantSettings = new ImplicitGrantSettings { EnableIdTokenIssuance = isHybrid }
                            }

                        };
                        break;
                    case AppType.Spa:
                        application = new Application
                        {
                            DisplayName = appName,
                            Spa = new SpaApplication
                            {
                                RedirectUris = redirectUri,
                            }

                        };
                        break;
                    case AppType.Desktop:
                        break;
                    default:
                        break;
                }


                var content = await graphClient.Applications
                    .Request()
                    .AddAsync(application);
                return content;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var accounts = await App.PublicClientApp.GetAccountsAsync();
            if (accounts.Any())
            {
                try
                {
                    await App.PublicClientApp.RemoveAsync(accounts.FirstOrDefault());
                    MessageBox.Show("User signed out");
                }
                catch (MsalException ex)
                {
                    MessageBox.Show($"Error signing-out user: {ex.Message}");
                }
            }
        }
        private async Task<bool> CloneRepo(string GitPath, string localPath)
        {
            if (System.IO.Directory.Exists(localPath))
            {
                ForceDeleteDirectory(localPath);
            }
            Repository.Clone(GitPath, localPath);
            return true;
        }
        private async Task<bool> UpdateJsonwithvalues(string jsonFileName, string domain, string tenantId, string clientID)
        {
            string jsonString = System.IO.File.ReadAllText(jsonFileName);


            JObject? jObject = JsonConvert.DeserializeObject<JObject>(jsonString);

            if (jObject?["AzureAd"] is JObject p)
            {
                p["Domain"] = domain;
                p["TenantId"] = tenantId;
                p["ClientId"] = clientID;
            }

            string newJson = JsonConvert.SerializeObject(jObject);


            var writerOptions = new JsonWriterOptions
            {
                Indented = true
            };

            var documentOptions = new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip
            };

            using FileStream fs = System.IO.File.Create(jsonFileName);
            using var writer = new Utf8JsonWriter(fs, options: writerOptions);
            using JsonDocument document = JsonDocument.Parse(newJson, documentOptions);

            JsonElement root = document.RootElement;

            if (root.ValueKind == JsonValueKind.Object)
            {
                writer.WriteStartObject();
            }
            foreach (JsonProperty property in root.EnumerateObject())
            {


                property.WriteTo(writer);
            }

            writer.WriteEndObject();

            writer.Flush();
            return true;
        }

        private async void Button_Configure(object sender, RoutedEventArgs e)
        {
            string gitpath = "https://github.com/Azure-Samples/active-directory-aspnetcore-webapp-openidconnect-v2.git";
            string localPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "LocalRep", "ASP.NETCORE");
            var authResult = await AuthenticateWithAzureAd();

            var tenantName = new System.Net.Mail.MailAddress(authResult.Account.Username);

            var app = await CreateAppRegistration(authResult.AccessToken, "1-WebApp-OIDC", new string[] { "https://localhost:44321/", "https://localhost:44321/signin-oidc" }, AppType.Web, true);
            var testResult = await CloneRepo(gitpath, localPath);

            string jsonFileName = System.IO.Path.Combine(localPath, @"1-WebApp-OIDC\1-1-MyOrg\appsettings.json");
            var testResultValue = await UpdateJsonwithvalues(jsonFileName, tenantName.Host, authResult.TenantId, app.AppId);


            Prerequisites window = new Prerequisites("DOTNETCORE",
                "https://github.com/Azure-Samples/active-directory-aspnetcore-webapp-openidconnect-v2/tree/master/1-WebApp-OIDC/1-1-MyOrg",
                System.IO.Path.Combine(localPath, @"1-WebApp-OIDC\1-1-MyOrg"),
                "https://docs.microsoft.com/en-us/azure/active-directory/develop/web-app-quickstart?pivots=devlang-aspnet-core");
            window.ShowDialog();
        }

        private async void Button_Configure_Angular(object sender, RoutedEventArgs e)
        {
            string gitpath = "https://github.com/Azure-Samples/ms-identity-javascript-angular-tutorial.git";
            string localPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "LocalRep", "Angular");
            var authResult = await AuthenticateWithAzureAd();

            var tenantName = new System.Net.Mail.MailAddress(authResult.Account.Username);

            var app = await CreateAppRegistration(authResult.AccessToken, "1-sign-in", new string[] { "https://localhost:4200/" }, AppType.Spa);
            var testResult = await CloneRepo(gitpath, localPath);

            string jsonFileName = System.IO.Path.Combine(localPath, @"1-Authentication\1-sign-in\SPA\src\app\auth-config.ts");
            FindAndReplace(jsonFileName, app.AppId, authResult.TenantId, "https://localhost:4200/");

            Prerequisites window = new Prerequisites("ANGULAR",
                "https://github.com/Azure-Samples/ms-identity-javascript-angular-tutorial/blob/main/1-Authentication/1-sign-in/README.md",
                 System.IO.Path.Combine(localPath, @"1-Authentication\1-sign-in"),
                 "https://docs.microsoft.com/en-us/azure/active-directory/develop/single-page-app-quickstart?pivots=devlang-angular");
            window.ShowDialog();
        }
        public void FindAndReplace(string fileName, string appId, string tenantId, string redirectURi)
        {
            string text = System.IO.File.ReadAllText(fileName);
            text = text.Replace("Enter_the_Application_Id_Here", appId);
            text = text.Replace("Enter_the_Tenant_Info_Here", tenantId);
            text = text.Replace("'/'", "'" + redirectURi + "'");
            System.IO.File.WriteAllText(fileName, text);
        }
        public void ForceDeleteDirectory(string path)
        {
            var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };

            foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                info.Attributes = FileAttributes.Normal;
            }

            directory.Delete(true);
        }
        private async Task<AuthenticationResult> AuthenticateWithAzureAd()
        {
            try
            {

                AuthenticationResult authResult = null;
                var app = App.PublicClientApp;

                IAccount firstAccount;
                var accounts = await app.GetAccountsAsync();
                firstAccount = accounts.FirstOrDefault();
                //try
                //{
                //    authResult = await app.AcquireTokenSilent(scopes, firstAccount)
                //        .ExecuteAsync();
                //}
                //catch (MsalUiRequiredException ex)
                //{
                //    // A MsalUiRequiredException happened on AcquireTokenSilent. 
                //    // This indicates you need to call AcquireTokenInteractive to acquire a token
                //    System.Diagnostics.Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

                try
                {
                    authResult = await app.AcquireTokenInteractive(scopes)
                        .WithAccount(firstAccount)
                        .WithParentActivityOrWindow(new WindowInteropHelper(this).Handle) // optional, used to center the browser on the window
                        .WithPrompt(Microsoft.Identity.Client.Prompt.SelectAccount)
                        .ExecuteAsync();
                }
                catch (MsalException msalex)
                {
                    MessageBox.Show($"Error Acquiring Token:{System.Environment.NewLine}{msalex}");
                }
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show($"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}");
                //    return;
                //}

                return authResult;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }
    public enum AppType
    {
        Web,
        Spa,
        Desktop

    }
}
