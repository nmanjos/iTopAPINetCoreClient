using iTopAPIClient.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iTopAPIClient.Services
{
    //Interface for the Identity Service
    public interface IIdentityService
    {
        //Registration Method
        Task<AuthenticationResult> RegisterAsync(string email, string password);

        Task<AuthenticationResult> LoginAsync(string email, string password);
        
    }
}
