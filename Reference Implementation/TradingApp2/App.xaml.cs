using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OANDARestLibrary;
using TradingApp2.Common;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TradingApp2.Data;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Split App template is documented at http://go.microsoft.com/fwlink/?LinkId=234228

namespace TradingApp2
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
			Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                //Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }
			
            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(ItemsPage), "AllGroups"))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();
	        CentralDispatcher.Dispatcher = Window.Current.Dispatcher;

			// Can't do anything useful until/unless we login
			while (Credentials.GetDefaultCredentials() == null && !await PerformOauth())
			{
			}
			
			AccountDataSource.DefaultDataSource = new AccountDataSource(Credentials.GetDefaultCredentials().DefaultAccountId);
	        rootFrame.Navigate(typeof (ItemsPage), "AllGroups");
        }

		async Task<bool> PerformOauth()
		{
			const string redirect = "https://api-sandbox.oanda.com/index.html";
			const string clientId = "g6E5XbvB28Ph9Kin";
			const string scopes = "read+trade+marketdata+stream";
			// create a random state string
			var rand = new Random();
			string state = "fnuajuiwqnzpofmAHAAJfNMLKAOW9mvm9r201jmpmfpa" + rand.Next() + rand.Next() + rand.Next();

			//String GoogleURL = "https://accounts.google.com/o/oauth2/auth?client_id=" + Uri.EscapeDataString(GoogleClientID.Text) + "&redirect_uri=" + Uri.EscapeDataString(GoogleCallbackUrl.Text) + "&response_type=code&scope=" + Uri.EscapeDataString("http://picasaweb.google.com/data");
			string requestURL = "https://api-fxpractice.oanda.com/v1/oauth2/authorize?client_id=" +
								Uri.EscapeDataString(clientId) + "&redirect_uri=" + Uri.EscapeDataString(redirect) +
								"&state=" + Uri.EscapeDataString(state) + "&response_type=token&scope=" + Uri.EscapeDataString(scopes);

			Uri startUri = new Uri(requestURL);
			// When using the desktop flow, the success code is displayed in the html title of this end uri
			
			Uri EndUri = new Uri(redirect + "#");


			WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
													WebAuthenticationOptions.None,
													startUri,
													EndUri);
			if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
			{
				Regex stateRegex = new Regex("state=([^&]*)");
				Regex tokenRegex = new Regex("access_token=([^&]*)");
				string returnedState = stateRegex.Match(WebAuthenticationResult.ResponseData).Groups[1].ToString();
				string returnedToken = tokenRegex.Match(WebAuthenticationResult.ResponseData).Groups[1].ToString();
				if (returnedState != state)
				{	// freak out or something
					throw new Exception("Unexpected state returned during authentication");
				}
				Credentials.SetCredentials(EEnvironment.Practice, returnedToken);
				
				// Set the default account
				var accounts = await Rest.GetAccountListAsync();
				Credentials.SetCredentials(EEnvironment.Practice, returnedToken, accounts[0].accountId);

				return true;
			}
			else if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
			{
				// Note: Should do something useful with this error
				// "HTTP Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseErrorDetail.ToString();
				return false;
			}
			else
			{
				// Note: Should do something useful with this error
				// return "Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseStatus.ToString();
				return false;
			}
		}

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }
    }
}
