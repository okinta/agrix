using agrix.Configuration;
using Vultr.API;

namespace agrix.Platforms.Vultr.Destroyers
{
    internal class VultrScriptDestroyer : VultrDestroyer<Script>
    {
        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="client">The VultrClient to use to destroy scripts.</param>
        public VultrScriptDestroyer(VultrClient client) : base(client)
        {
        }

        /// <summary>
        /// Destroys an existing Vultr script.
        /// </summary>
        /// <param name="script">The configuration to reference to destroy the
        /// script.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public override void Destroy(Script script, bool dryrun = false)
        {
            throw new System.NotImplementedException();
        }
    }
}
