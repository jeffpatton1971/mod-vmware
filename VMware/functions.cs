namespace Vmware
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using VMware.Vim;
    public class functions
    {
        /// <summary>
        /// This function returns a string that represents a proper vmware sdk url
        /// https://server.domain/sdk
        /// </summary>
        /// <param name="viServer"></param>
        /// <returns>A string containing a valid URI</returns>
        public static string ValidateServer(string viServer)
        {
            //
            // Validate viServer for appropriate values
            //
            viServer = viServer.Trim().ToLower();
            if (viServer.Contains("://") == false)
            {
                //
                // Assuming if there is no :// someone entered flat server name
                //
                viServer = "https://" + viServer;
            }
            //
            // Convert the string into a URI for further testing
            //
            Uri uriVServer = new Uri(viServer);
            //
            // Check to see what protocol we're using
            //
            string urlScheme = uriVServer.Scheme;
            switch (urlScheme)
            {
                case "https":
                    break;
                default:
                    viServer = viServer.Replace(uriVServer.Scheme + "://", "https://");
                    break;
            }
            //
            // Check to see what that path is
            //
            if (uriVServer.AbsolutePath == "/")
            {
                viServer = viServer + "/sdk";
            }
            else if (uriVServer.AbsolutePath != "/sdk")
            {
                //
                // Some other path is listed
                //
                viServer = viServer.Replace(uriVServer.AbsolutePath, "/sdk");
            }
            return viServer;
        }
        /// <summary>
        /// This function is a wrapper for the FindEntityViews method of the vimclient. It
        /// allows you to easily return a list of objects based on a filter. The filter
        /// is a NameValueCollection object that contains a property name (string) and a
        /// property value (string).
        /// </summary>
        /// <typeparam name="T">This is the type of object we want to get</typeparam>
        /// <param name="vimClient">This is our client connection</param>
        /// <param name="beginEntity">This is a ManagedObjectReference to start at</param>
        /// <param name="filter">A NameValueCollection object that contains our filter</param>
        /// <param name="properties">An array of properties to return, all are returned if null</param>
        /// <returns>Returns the appropriate objects that you asked for.</returns>
        public static List<T> GetEntities<T>(VimClient vimClient, ManagedObjectReference beginEntity, NameValueCollection filter, string[] properties)
        {
            List<T> things = new List<T>();
            List<EntityViewBase> vBase = vimClient.FindEntityViews(typeof(T), beginEntity, filter, properties);

            foreach (EntityViewBase eBase in vBase)
            {
                T thing = (T)(object)eBase;
                things.Add(thing);
            }
            return things;
        }
        /// <summary>
        /// This function is a wrapper for the FindEntityView method of the vimclient. It
        /// allows you to easily return an object based on a filter. The filter is a 
        /// NameValueCollection object that contains a property name (string) and a property value (string).
        /// </summary>
        /// <typeparam name="T">This is the type of object we want to get</typeparam>
        /// <param name="vimClient">This is our client connection</param>
        /// <param name="beginEntity">This is a ManagedObjectReference to start at</param>
        /// <param name="filter">A NameValueCollection object that contains our filter</param>
        /// <param name="properties">An array of properties to return, all are returned if null</param>
        /// <returns>Returns the appropriate objectsthat you asked for.</returns>
        public static T GetEntity<T>(VimClient vimClient, ManagedObjectReference beginEntity, NameValueCollection filter, string[] properties)
        {
            EntityViewBase vBase = vimClient.FindEntityView(typeof(T), beginEntity, filter, properties);
            T thing = (T)(object)vBase;
            return thing;
        }
        /// <summary>
        /// This function is a wrapper for the GetView method of the vimclient. It returns 
        /// and object when passed in a proper ManagedObjectReference. It will return an
        /// array of properties if asked, otherwise all properties are returned.
        /// </summary>
        /// <typeparam name="T">This is the type of object we want to get</typeparam>
        /// <param name="vimClient">This is our client connection</param>
        /// <param name="moRef">This is the ManagedObjectReference of the object to return</param>
        /// <param name="properties">An array of properties to return, all are returned if null</param>
        /// <returns>Returns the appropriate objectsthat you asked for.</returns>
        public static T GetObject<T>(VimClient vimClient, ManagedObjectReference moRef, string[] properties)
        {
            ViewBase vBase = vimClient.GetView(moRef, properties);
            T thisObject = (T)(object)vBase;
            return thisObject;
        }
        /// <summary>
        /// This function returns a valid VimClient connection object. This is used throughout
        /// to establish a connection to the SDK server in order to query and perform tasks.
        /// </summary>
        /// <param name="viServer">A properly validated SDK URI</param>
        /// <param name="viUser">A user with permissions on the server</param>
        /// <param name="viPassword">The password of the user with permissions on the server</param>
        /// <returns>A VimClient object</returns>
        public static VimClient ConnectServer(string viServer, string viUser, string viPassword)
        {
            //
            // Establish a connetion to the provided sdk server
            //
            VimClient vimClient = new VimClient();
            ServiceContent vimServiceContent = new ServiceContent();
            UserSession vimSession = new UserSession();
            //
            // Connect over https to the /sdk page
            //
            try
            {
                vimClient.Connect(viServer);
                vimSession = vimClient.Login(viUser, viPassword);
                vimServiceContent = vimClient.ServiceContent;

                return vimClient;
            }
            catch
            {
                //
                // VMware Exception occurred
                //
                //txtErrors.Text = "A server fault of type " + ex.MethodFault.GetType().Name + " with message '" + ex.Message + "' occured while performing requested operation.";
                //Error_Panel.Visible = true;
                return null;
            }
        }
    }
}