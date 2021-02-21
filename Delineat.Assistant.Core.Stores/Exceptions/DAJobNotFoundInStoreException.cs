namespace Delineat.Assistant.Core.Stores.Exceptions
{
    public class DAJobNotFoundInStoreException : DAStoresException
    {
        public DAJobNotFoundInStoreException(string jobId) : base($"Impossibile trovare la commessa con id '{jobId}'")
        {

        }
    }
}
