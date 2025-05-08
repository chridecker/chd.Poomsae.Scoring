using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Contracts.Interfaces
{
    public interface ISignInManager
    {

        event EventHandler<Exception> LoginFailed;
        event EventHandler<(string,string)> LoginSucceded;
        void SignIn();
        void InvokeLoginFailed(Exception ex);
        void InvokeLoginSuccess(string email, string name);
    }
}
