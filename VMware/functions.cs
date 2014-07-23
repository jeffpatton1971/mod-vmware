namespace mod_vmware
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Net;
    using VMware.Vim;
    using System.Linq;
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
        /// <param name="Credential">A System.Net Credential object to store user/pass</param>
        /// <returns>A VimClient object</returns>
        public static VimClient ConnectServer(string viServer, NetworkCredential Credential)
        {
            //
            // Establish a connetion to the provided sdk server
            //
            VimClient vimClient = new VimClient();
            ServiceContent vimServiceContent = new ServiceContent();
            UserSession vimSession = new UserSession();
            //
            // Get the user/pass from the credential object
            //
            string viUser = Credential.UserName;
            string viPassword = Credential.Password;
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
        public class SimpleObject
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
        public class SimpleClone
        {
            public string Name { get; set; }
            public string ClusterMoRef { get; set; }
            public string SourceVmMoRef { get; set; }
            public string DatastoreMoRef { get; set; }
            public string PortGroupMoRef { get; set; }
            public string OsCustomizationSpec { get; set; }
            public string ResourcePoolMoRef { get; set; }
            public int Processor { get; set; }
            public long Memory { get; set; }
            public string winSuffix { get; set; }
            public string linSuffix { get; set; }
            public string[] Nameserver { get; set; }
            public string IpAddress { get; set; }
            public string Netmask { get; set; }
            public string[] Gateway { get; set; }
        }
        /// <summary>
        /// Return a hashtable of Virtual Machine name and Moref for use in the web app
        /// </summary>
        /// <param name="Credential">A NetworkCredential object that contains the username 
        /// and password with rights to the server</param>
        /// <param name="Server">The name of the vSphere server to connect to</param>
        /// <param name="Value">something to filter on</param>
        /// <returns>A hashtable that contains the name and string moref</returns>
        public static List<SimpleObject> GetVm(NetworkCredential Credential, string Server, string MoRefString, string Value)
        {
            VimClient vimClient = ConnectServer(ValidateServer(Server), Credential);
            NameValueCollection Filter = new NameValueCollection();
            if (Value != null)
            {
                Filter.Add("name", Value);
            }
            else
            {
                Filter = null;
            }
            ManagedObjectReference beginEntity = null;
            if (MoRefString != null)
            {
                beginEntity = new ManagedObjectReference(MoRefString);
            }
            List<VirtualMachine> lstVirtualMachines = GetEntities<VirtualMachine>(vimClient, beginEntity, Filter, null);
            lstVirtualMachines = lstVirtualMachines.OrderBy(thisVm => thisVm.Name).ToList();
            List<SimpleObject> VirtualMachines = new List<SimpleObject>();
            foreach (VirtualMachine itmVm in lstVirtualMachines)
            {
                VirtualMachines.Add(new SimpleObject { Name = itmVm.Name, Value = itmVm.MoRef.ToString() });
            }
            vimClient.Disconnect();
            return VirtualMachines;
        }
        public static List<SimpleObject> GetCluster(NetworkCredential Credential, string Server, string Value)
        {
            VimClient vimClient = ConnectServer(ValidateServer(Server), Credential);
            NameValueCollection Filter = new NameValueCollection();
            Filter.Add("name", Value);
            List<ClusterComputeResource> lstClusters = GetEntities<ClusterComputeResource>(vimClient, null, Filter, null);
            lstClusters = lstClusters.OrderBy(thisCluster => thisCluster.Name).ToList();
            List<SimpleObject> Clusters = new List<SimpleObject>();
            foreach (ClusterComputeResource itmClster in lstClusters)
            {
                Clusters.Add(new SimpleObject { Name = itmClster.Name, Value = itmClster.MoRef.ToString() });
            }
            vimClient.Disconnect();
            return Clusters;
        }
        public static Hashtable GetOsCustomization(NetworkCredential Credential, string Server)
        {
            VimClient vimClient = ConnectServer(ValidateServer(Server), Credential);
            CustomizationSpecManager specManager = GetObject<CustomizationSpecManager>(vimClient, vimClient.ServiceContent.CustomizationSpecManager, null);
            Hashtable Customizations = new Hashtable();
            foreach (CustomizationSpecInfo specInfo in specManager.Info)
            {
                Customizations.Add(specInfo.Name, specInfo.Name + "." + specInfo.Type);
            }
            vimClient.Disconnect();
            return Customizations;
        }
        public static List<SimpleObject> GetDatastore(NetworkCredential Credential, string Server, string MoRefString, string Value)
        {
            VimClient vimClient = ConnectServer(ValidateServer(Server), Credential);
            NameValueCollection Filter = new NameValueCollection();
            ManagedObjectReference beginEntity = null;
            if (MoRefString != null)
            {
                beginEntity = new ManagedObjectReference(MoRefString);
            }
            ClusterComputeResource SelectedCluster = GetObject<ClusterComputeResource>(vimClient, beginEntity, null);
            List<Datastore> lstDatastores = new List<Datastore>();
            foreach (ManagedObjectReference dsMoRef in SelectedCluster.Datastore)
            {
                ViewBase dsView = vimClient.GetView(dsMoRef, null);
                Datastore clusterDS = (Datastore)dsView;
                lstDatastores.Add(clusterDS);
            }
            List<SimpleObject> Datastores = new List<SimpleObject>();
            lstDatastores = lstDatastores.OrderByDescending(thisStore => thisStore.Info.FreeSpace).ToList();
            foreach (Datastore itmDatastore in lstDatastores)
            {
                Datastores.Add(new SimpleObject { Name = itmDatastore.Name, Value = itmDatastore.MoRef.ToString() });
            }
            vimClient.Disconnect();
            return Datastores;
        }
        public static Hashtable GetDatacenter(NetworkCredential Credential, string Server, string MoRefString)
        {
            VimClient vimClient = ConnectServer(ValidateServer(Server), Credential);
            NameValueCollection Filter = new NameValueCollection();
            ManagedObjectReference beginEntity = null;
            if (MoRefString != null)
            {
                beginEntity = new ManagedObjectReference(MoRefString);
            }
            ClusterComputeResource SelectedCluster = GetObject<ClusterComputeResource>(vimClient, beginEntity, null);
            Filter.Add("hostfolder", SelectedCluster.Parent.Value);
            Datacenter itmDatacenter = GetEntity<Datacenter>(vimClient, null, Filter, null);
            Hashtable DC = new Hashtable();
            DC.Add(itmDatacenter.Name, itmDatacenter.MoRef);
            vimClient.Disconnect();
            return DC;
        }
        public static Hashtable GetPortGroup(NetworkCredential Credential, string Server, string MoRefString, string Value)
        {
            VimClient vimClient = ConnectServer(ValidateServer(Server), Credential);
            NameValueCollection Filter = new NameValueCollection();
            ManagedObjectReference beginEntity = null;
            if (MoRefString != null)
            {
                beginEntity = new ManagedObjectReference(MoRefString);
            }
            ClusterComputeResource SelectedCluster = GetObject<ClusterComputeResource>(vimClient, beginEntity, null);
            Filter.Add("hostfolder", SelectedCluster.Parent.Value);
            Datacenter itmDatacenter = GetEntity<Datacenter>(vimClient, null, Filter, null);
            Filter.Remove("hostfolder");
            if (Value != null)
            {
                Filter.Add("name", Value);
            }
            else
            {
                Filter = null;
            }
            List<DistributedVirtualPortgroup> lstPortGroups = GetEntities<DistributedVirtualPortgroup>(vimClient, itmDatacenter.MoRef, Filter, null);
            Hashtable PortGroup = new Hashtable();
            foreach (DistributedVirtualPortgroup itmPortGroup in lstPortGroups)
            {
                PortGroup.Add(itmPortGroup.Name, itmPortGroup.MoRef);
            }
            vimClient.Disconnect();
            return PortGroup;
        }
        public static Hashtable GetResourcePool(NetworkCredential Credential, string Server, string MoRefString)
        {
            VimClient vimClient = ConnectServer(ValidateServer(Server), Credential);
            NameValueCollection Filter = new NameValueCollection();
            ManagedObjectReference beginEntity = null;
            if (MoRefString != null)
            {
                beginEntity = new ManagedObjectReference(MoRefString);
            }
            Filter.Add("parent", beginEntity.Value);
            List<ResourcePool> lstResourcePools = GetEntities<ResourcePool>(vimClient, null, Filter, null);
            Hashtable ResourcePools = new Hashtable();
            foreach (ResourcePool itmResourcePool in lstResourcePools)
            {
                ResourcePools.Add(itmResourcePool.Name, itmResourcePool.MoRef.ToString());
            }
            vimClient.Disconnect();
            return ResourcePools;
        }
        public static void CloneVM(NetworkCredential Credential, string Server, SimpleClone NewVm)
        {
            //
            // Assume all data in newvm has been validated, just do the clone
            //
            VimClient vimClient = ConnectServer(ValidateServer(Server), Credential);
            NameValueCollection Filter = new NameValueCollection();
            //
            // Get the cluster
            //
            ManagedObjectReference ClusterMoRef = new ManagedObjectReference(NewVm.ClusterMoRef);
            ClusterComputeResource Cluster = GetObject<ClusterComputeResource>(vimClient, ClusterMoRef, null);
            //
            // Get the datacenter
            //
            Filter.Add("hostFolder", Cluster.Parent.Value);
            Datacenter Datacenter = GetEntity<Datacenter>(vimClient, null, Filter, null);
            Filter.Remove("hostFolder");
            //
            // Get the hosts
            //
            ManagedObjectReference[] hostMoRefs = Cluster.Host;
            //
            // Randomly select a given host
            //
            Random rand = new Random();
            HostSystem Host = GetObject<HostSystem>(vimClient, hostMoRefs[rand.Next(0, hostMoRefs.Count())], null);
            //
            // Get the SourceVM
            //
            ManagedObjectReference SourceVmMoRef = new ManagedObjectReference(NewVm.SourceVmMoRef);
            VirtualMachine SourceVm = GetObject<VirtualMachine>(vimClient, SourceVmMoRef, null);
            //
            // Get the datastore
            //
            ManagedObjectReference DatastoreMoRef = new ManagedObjectReference(NewVm.DatastoreMoRef);
            Datastore Datastore = GetObject<Datastore>(vimClient, DatastoreMoRef, null);
            //
            // Get the PortGroup
            //
            ManagedObjectReference PortGroupMoRef = new ManagedObjectReference(NewVm.PortGroupMoRef);
            DistributedVirtualPortgroup PortGroup = GetObject<DistributedVirtualPortgroup>(vimClient, PortGroupMoRef, null);
            //
            // Get the CustomizationSpec
            //
            char[] splitChar = new char[] { '.' };
            string[] thisSpec = NewVm.OsCustomizationSpec.Split(splitChar);
            string specName = thisSpec[0];
            string specType = thisSpec[1];
            CustomizationSpecManager specManager = GetObject<CustomizationSpecManager>(vimClient, vimClient.ServiceContent.CustomizationSpecManager, null);
            CustomizationSpecItem specItem = specManager.GetCustomizationSpec(specName);
            //
            // Create a new VirtualMachineCloneSpec
            //
            VirtualMachineCloneSpec vmCloneSpec = new VirtualMachineCloneSpec();
            vmCloneSpec.Location = new VirtualMachineRelocateSpec();
            //
            // Assign Datastore
            //
            vmCloneSpec.Location.Datastore = Datastore.MoRef;
            //
            // Assign Host
            //
            vmCloneSpec.Location.Host = Host.MoRef;
            //
            // Get the ResourcePool
            //
            ManagedObjectReference ResourcePoolMoRef = new ManagedObjectReference(NewVm.ResourcePoolMoRef);
            ResourcePool ResourcePool = GetObject<ResourcePool>(vimClient, ResourcePoolMoRef, null);
            //
            // Assign ResourcePool
            //
            vmCloneSpec.Location.Pool = ResourcePool.MoRef;
            //
            // Add selected clonespec to this CloneSpec
            //
            vmCloneSpec.Customization = specItem.Spec;
            //
            // Handle hostname for Windows or Linux
            //
            if (specType == "Windows")
            {
                //
                // Create a Windows Sysprep object
                //
                CustomizationSysprep winIdent = (CustomizationSysprep)specItem.Spec.Identity;
                CustomizationFixedName hostname = new CustomizationFixedName();
                hostname.Name = NewVm.Name;
                winIdent.UserData.ComputerName = hostname;
                //
                // Store identity in the CloneSpec
                //
                vmCloneSpec.Customization.Identity = winIdent;
            }
            if (specType == "Linux")
            {
                //
                // Create a linux Sysprep object
                //
                CustomizationLinuxPrep linIdent = (CustomizationLinuxPrep)specItem.Spec.Identity;
                CustomizationFixedName hostname = new CustomizationFixedName();
                hostname.Name = NewVm.Name;
                linIdent.HostName = hostname;
                linIdent.Domain = NewVm.linSuffix;
                //
                // Store identity in the CloneSpec
                //
                vmCloneSpec.Customization.Identity = linIdent;
            }
            //
            // Create a ConfigSpec
            //
            vmCloneSpec.Config = new VirtualMachineConfigSpec();
            //
            // Number of CPUs
            //
            vmCloneSpec.Config.NumCPUs = NewVm.Processor;
            //
            // Allocate memory
            //
            vmCloneSpec.Config.MemoryMB = NewVm.Memory;
            //
            // Configure the first network card
            //
            vmCloneSpec.Customization.NicSettingMap = new CustomizationAdapterMapping[1];
            vmCloneSpec.Customization.NicSettingMap[0] = new CustomizationAdapterMapping();
            //
            // Set nameserver
            //
            vmCloneSpec.Customization.GlobalIPSettings = new CustomizationGlobalIPSettings();
            //
            // Convert the single dns entry to an array
            //
            vmCloneSpec.Customization.GlobalIPSettings.DnsServerList = NewVm.Nameserver;
            //
            // Create a network device
            //
            VirtualDevice NetworkCard = new VirtualDevice();
            foreach (VirtualDevice vDevice in SourceVm.Config.Hardware.Device)
            {
                //
                // Get the network card
                //
                if (vDevice.DeviceInfo.Label.Contains("Network"))
                {
                    NetworkCard = vDevice;
                }
            }
            //
            // Create a DeviceSpec
            //
            vmCloneSpec.Config.DeviceChange = new VirtualDeviceConfigSpec[1];
            vmCloneSpec.Config.DeviceChange[0] = new VirtualDeviceConfigSpec();
            vmCloneSpec.Config.DeviceChange[0].Operation = VirtualDeviceConfigSpecOperation.edit;
            vmCloneSpec.Config.DeviceChange[0].Device = NetworkCard;
            //
            // Define Network settings
            // Assign IP Address
            //
            CustomizationFixedIp ipAddress = new CustomizationFixedIp();
            ipAddress.IpAddress = NewVm.IpAddress;
            //
            // Assign Netmask
            //
            vmCloneSpec.Customization.NicSettingMap[0].Adapter.Ip = ipAddress;
            vmCloneSpec.Customization.NicSettingMap[0].Adapter.SubnetMask = NewVm.Netmask;
            //
            // Assign gateway
            //
            vmCloneSpec.Customization.NicSettingMap[0].Adapter.Gateway = NewVm.Gateway;
            //
            // Create network backing information
            //
            VirtualEthernetCardDistributedVirtualPortBackingInfo NetworkBacking = new VirtualEthernetCardDistributedVirtualPortBackingInfo();
            NetworkBacking.Port = new DistributedVirtualSwitchPortConnection();
            //
            // Connect to the virtual switch
            //
            VmwareDistributedVirtualSwitch VirtualSwitch = GetObject<VmwareDistributedVirtualSwitch>(vimClient, PortGroup.Config.DistributedVirtualSwitch, null);
            //
            // Connect the VM to the correct switch
            //
            NetworkBacking.Port.SwitchUuid = VirtualSwitch.Uuid;
            //
            // Connect the vm to the correct portgroup
            //
            NetworkBacking.Port.PortgroupKey = PortGroup.MoRef.Value;
            vmCloneSpec.Config.DeviceChange[0].Device.Backing = NetworkBacking;
            //
            // Enable the network card at boot
            //
            vmCloneSpec.Config.DeviceChange[0].Device.Connectable = new VirtualDeviceConnectInfo();
            vmCloneSpec.Config.DeviceChange[0].Device.Connectable.StartConnected = true;
            vmCloneSpec.Config.DeviceChange[0].Device.Connectable.AllowGuestControl = true;
            vmCloneSpec.Config.DeviceChange[0].Device.Connectable.Connected = true;
            //
            // Get the VM folder from the datacenter
            // Perform the clone
            //
            ManagedObjectReference CloneTaskMoRef = new ManagedObjectReference();
            CloneTaskMoRef = SourceVm.CloneVM_Task(Datacenter.VmFolder, NewVm.Name, vmCloneSpec);
            Task CloneTask = new Task(vimClient, CloneTaskMoRef);
            //
            // Disconnect
            //
            vimClient.Disconnect();
        }
    }
}