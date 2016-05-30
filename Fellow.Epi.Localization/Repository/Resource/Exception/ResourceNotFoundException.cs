using System;

namespace Fellow.Epi.Localization.Repository.Resource.Exception
{
	public class ResourceNotFoundException : System.Exception
	{
		public ResourceNotFoundException(string key)
			: base(String.Format("Resource with key {0} was not found", key))
		{
			
		}
	}
}
