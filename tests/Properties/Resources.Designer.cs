﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace tests.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("tests.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] agrix {
            get {
                object obj = ResourceManager.GetObject("agrix", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to firewalls:
        ///  - name: ssh
        ///    rules:
        ///      - protocol: tcp
        ///        source: 0.0.0.0/0
        ///        port: 22
        ///
        ///      - protocol: tcp
        ///        source: 0.0.0.0/0
        ///        port: 3389
        ///
        ///  - name: myapp
        ///    rules:
        ///      - protocol: udp
        ///        source: 172.0.24.1/20
        ///        ports: 8000 - 8100
        ///
        ///      - protocol: tcp
        ///        source: cloudflare
        ///        port: 80.
        /// </summary>
        internal static string FirewallsConfig {
            get {
                return ResourceManager.GetString("FirewallsConfig", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to scripts:
        ///  - name: test
        ///    type: tony
        ///    content: this is a test script.
        /// </summary>
        internal static string InvalidScriptTypeConfig {
            get {
                return ResourceManager.GetString("InvalidScriptTypeConfig", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to platform: vultr
        ///servers:
        ///  - label: test.
        /// </summary>
        internal static string InvalidServerConfig {
            get {
                return ResourceManager.GetString("InvalidServerConfig", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to servers:
        ///  - os:
        ///      iso: alpine.iso
        ///    plan:
        ///      cpu: 2
        ///      memory: 4096
        ///      type: SSD
        ///    region: Chicago
        ///    userdata:
        ///      my-array:
        ///        - 1
        ///        - 2.
        /// </summary>
        internal static string JSONUserDataConfig {
            get {
                return ResourceManager.GetString("JSONUserDataConfig", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to scripts:
        ///  - name: test
        ///    type: boot
        ///    content: this is a test script
        ///
        ///  - name: bash-script
        ///    type: boot
        ///    content: |
        ///      #!/usr/bin/env bash
        ///      echo hello.
        /// </summary>
        internal static string ScriptsConfig {
            get {
                return ResourceManager.GetString("ScriptsConfig", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to platform: vultr
        ///servers:.
        /// </summary>
        internal static string SimpleConfig {
            get {
                return ResourceManager.GetString("SimpleConfig", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to platform: test
        ///servers:
        ///  - os:
        ///      name: Fedora 32 x64
        ///    plan:
        ///      cpu: 2
        ///      memory: 4096
        ///      type: SSD
        ///    region: Atlanta.
        /// </summary>
        internal static string TestPlatformConfig {
            get {
                return ResourceManager.GetString("TestPlatformConfig", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to servers:
        ///  - os:
        ///      iso: alpine.iso
        ///    plan:
        ///      cpu: 2
        ///      memory: 4096
        ///      type: SSD
        ///    region: Chicago
        ///    userdata: test data.
        /// </summary>
        internal static string UserDataConfig {
            get {
                return ResourceManager.GetString("UserDataConfig", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to platform: vultr
        ///servers:
        ///  - os:
        ///      iso: alpine.iso
        ///    plan:
        ///      cpu: 2
        ///      memory: 4096
        ///      type: SSD
        ///    region: Chicago
        ///    userdata:
        ///      my-array:
        ///        - 1
        ///        - 2.
        /// </summary>
        internal static string ValidAlpineConfig {
            get {
                return ResourceManager.GetString("ValidAlpineConfig", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to platform: vultr
        ///servers:
        ///  - os:
        ///      name: Fedora 32 x64
        ///    plan:
        ///      cpu: 2
        ///      memory: 4096
        ///      type: SSD
        ///    region: Atlanta.
        /// </summary>
        internal static string ValidFedoraConfig {
            get {
                return ResourceManager.GetString("ValidFedoraConfig", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {&quot;127&quot;:{&quot;OSID&quot;:127,&quot;name&quot;:&quot;CentOS 6 x64&quot;,&quot;arch&quot;:&quot;x64&quot;,&quot;family&quot;:&quot;centos&quot;,&quot;windows&quot;:false},&quot;147&quot;:{&quot;OSID&quot;:147,&quot;name&quot;:&quot;CentOS 6 i386&quot;,&quot;arch&quot;:&quot;i386&quot;,&quot;family&quot;:&quot;centos&quot;,&quot;windows&quot;:false},&quot;167&quot;:{&quot;OSID&quot;:167,&quot;name&quot;:&quot;CentOS 7 x64&quot;,&quot;arch&quot;:&quot;x64&quot;,&quot;family&quot;:&quot;centos&quot;,&quot;windows&quot;:false},&quot;381&quot;:{&quot;OSID&quot;:381,&quot;name&quot;:&quot;CentOS 7 SELinux x64&quot;,&quot;arch&quot;:&quot;x64&quot;,&quot;family&quot;:&quot;centos&quot;,&quot;windows&quot;:false},&quot;362&quot;:{&quot;OSID&quot;:362,&quot;name&quot;:&quot;CentOS 8 x64&quot;,&quot;arch&quot;:&quot;x64&quot;,&quot;family&quot;:&quot;centos&quot;,&quot;windows&quot;:false},&quot;215&quot;:{&quot;OSID&quot;:215,&quot;name&quot;:&quot;Ubuntu 16.04 x64&quot;,&quot;arch&quot;:&quot;x64&quot;,&qu....
        /// </summary>
        internal static string VultrOSList {
            get {
                return ResourceManager.GetString("VultrOSList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {&quot;201&quot;:{&quot;VPSPLANID&quot;:&quot;201&quot;,&quot;name&quot;:&quot;1024 MB RAM,25 GB SSD,1.00 TB BW&quot;,&quot;vcpu_count&quot;:&quot;1&quot;,&quot;ram&quot;:&quot;1024&quot;,&quot;disk&quot;:&quot;25&quot;,&quot;bandwidth&quot;:&quot;1.00&quot;,&quot;bandwidth_gb&quot;:&quot;1024&quot;,&quot;price_per_month&quot;:&quot;5.00&quot;,&quot;plan_type&quot;:&quot;SSD&quot;,&quot;windows&quot;:false,&quot;available_locations&quot;:[1,2,3,4,5,6,7,8,9,12,19,22,24,25,34,39,40]},&quot;202&quot;:{&quot;VPSPLANID&quot;:&quot;202&quot;,&quot;name&quot;:&quot;2048 MB RAM,55 GB SSD,2.00 TB BW&quot;,&quot;vcpu_count&quot;:&quot;1&quot;,&quot;ram&quot;:&quot;2048&quot;,&quot;disk&quot;:&quot;55&quot;,&quot;bandwidth&quot;:&quot;2.00&quot;,&quot;bandwidth_gb&quot;:&quot;2048&quot;,&quot;price_per_month&quot;:&quot;10.00&quot;,&quot;plan_type&quot;:&quot;SSD&quot;,&quot;windows&quot;:false,&quot;available_locations&quot;:[1, [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string VultrPlansList {
            get {
                return ResourceManager.GetString("VultrPlansList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {&quot;6&quot;:{&quot;DCID&quot;:&quot;6&quot;,&quot;name&quot;:&quot;Atlanta&quot;,&quot;country&quot;:&quot;US&quot;,&quot;continent&quot;:&quot;North America&quot;,&quot;state&quot;:&quot;GA&quot;,&quot;ddos_protection&quot;:false,&quot;block_storage&quot;:false,&quot;regioncode&quot;:&quot;ATL&quot;,&quot;availability&quot;:[201,202,203,204,205,206,400,401,402,29,93,94,95,96,97,98,100]},&quot;2&quot;:{&quot;DCID&quot;:&quot;2&quot;,&quot;name&quot;:&quot;Chicago&quot;,&quot;country&quot;:&quot;US&quot;,&quot;continent&quot;:&quot;North America&quot;,&quot;state&quot;:&quot;IL&quot;,&quot;ddos_protection&quot;:true,&quot;block_storage&quot;:false,&quot;regioncode&quot;:&quot;ORD&quot;,&quot;availability&quot;:[201,202,203,204,205,206,400,401,402,29,93,94,95,96,97,98,100]},&quot;3&quot;:{&quot;DCID&quot;:&quot;3&quot;,&quot;name&quot;:&quot;Dallas&quot;,&quot;country&quot;:&quot;US&quot; [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string VultrRegionsList {
            get {
                return ResourceManager.GetString("VultrRegionsList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///    &quot;576965&quot;: {
        ///        &quot;SUBID&quot;: &quot;576965&quot;,
        ///        &quot;os&quot;: &quot;CentOS 6 x64&quot;,
        ///        &quot;ram&quot;: &quot;4096 MB&quot;,
        ///        &quot;disk&quot;: &quot;Virtual 60 GB&quot;,
        ///        &quot;main_ip&quot;: &quot;123.123.123.123&quot;,
        ///        &quot;vcpu_count&quot;: &quot;2&quot;,
        ///        &quot;location&quot;: &quot;New Jersey&quot;,
        ///        &quot;DCID&quot;: &quot;1&quot;,
        ///        &quot;default_password&quot;: &quot;nreqnusibni&quot;,
        ///        &quot;date_created&quot;: &quot;2013-12-19 14:45:41&quot;,
        ///        &quot;pending_charges&quot;: &quot;46.67&quot;,
        ///        &quot;status&quot;: &quot;active&quot;,
        ///        &quot;cost_per_month&quot;: &quot;10.05&quot;,
        ///        &quot;current_bandwidth_gb&quot;: 131.512,
        ///        &quot;all [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string VultrServerList {
            get {
                return ResourceManager.GetString("VultrServerList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///    &quot;3&quot;: {
        ///        &quot;SCRIPTID&quot;: &quot;3&quot;,
        ///        &quot;date_created&quot;: &quot;2014-05-21 15:27:18&quot;,
        ///        &quot;date_modified&quot;: &quot;2014-05-21 15:27:18&quot;,
        ///        &quot;name&quot;: &quot;hello-boot&quot;,
        ///        &quot;type&quot;: &quot;boot&quot;,
        ///        &quot;script&quot;: &quot;#!/bin/bash echo Hello World &gt; /root/hello&quot;
        ///    },
        ///    &quot;5&quot;: {
        ///        &quot;SCRIPTID&quot;: &quot;5&quot;,
        ///        &quot;date_created&quot;: &quot;2014-08-22 15:27:18&quot;,
        ///        &quot;date_modified&quot;: &quot;2014-09-22 15:27:18&quot;,
        ///        &quot;name&quot;: &quot;hello-pxe&quot;,
        ///        &quot;type&quot;: &quot;pxe&quot;,
        ///        &quot;script&quot;: &quot;#!ipxe\necho Hello World\nshell&quot;
        ///     [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string VultrStartupScripts {
            get {
                return ResourceManager.GetString("VultrStartupScripts", resourceCulture);
            }
        }
    }
}
