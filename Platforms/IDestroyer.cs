namespace agrix.Platforms
{
    /// <summary>
    /// Describes an interface for destroying infrastructure.
    /// </summary>
    /// <typeparam name="T">The type of infrastructure to provision.</typeparam>
    internal interface IDestroyer<in T>
    {
        /// <summary>
        /// Destroys the given infrastructure configuration.
        /// </summary>
        /// <param name="infrastructure">The configuration to reference during
        /// destruction.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public void Destroy(T infrastructure, bool dryrun = false);
    }
}
