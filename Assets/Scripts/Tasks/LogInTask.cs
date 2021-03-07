using Networking.Http;
using Networking.Http.Requests;

namespace Tasks
{
    public class LogInTask : BaseTask
    {
        private readonly string _username;
        private readonly string _password;
        private readonly bool _rememberUsername;
    
        public LogInTask(string username, string password, bool rememberUsername)
        {
            _username = username;
            _password = password;
            _rememberUsername = rememberUsername;
        }

        public override void StartAsync()
        {
            var logInRequest = new LogInRequest(OnRequestComplete, _username, _password);
            WebRequestSender.SendRequestAsync(logInRequest);
        }

        private void OnRequestComplete(string response, bool success)
        {
            if (success)
            {
                Account.Login(_username, _rememberUsername);
            }
            IsComplete = true;
        }
    }
}