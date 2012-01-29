using System;

namespace SiouxTube
{
	/// <summary>
	/// Package for server information
	/// </summary>
	public class ServerAddress
	{
		public ServerAddress(string host, int port, bool ssl, string username, string password)
		{
			this.Host = host;
			this.Port = port;
            this.Ssl  = ssl;
			this.Username = username;
			this.Password = password;
		}
		
		public string Host { get; private set; }
		public int Port { get; private set; }
        public bool Ssl { get; private set; }
		public string Username { get; private set; }
		public string Password { get; private set; }
	}
}

