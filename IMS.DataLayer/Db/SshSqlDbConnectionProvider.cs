using IMS.DataLayer.Interfaces;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Db
{
    public class SshSqlDbConnectionProvider : IDbConnectionProvider
    { private static SshClient _sshClient;
        private static uint _localPort;
        private IConfiguration _configuration;
        public SshSqlDbConnectionProvider(IConfiguration configuration)
        {
            _configuration = configuration;
         
        }
        public async Task<MySqlConnection> GetConnection(string databaseName)
        {  try
             {
                if (!IsSshClientConnected())
                    (_sshClient, _localPort) =  ConnectSsh(
                                                       _configuration["Ssh:Server"],
                                                       _configuration["Ssh:UserName"],
                                                       _configuration["Ssh:Password"]
                                                     );

                MySqlConnectionStringBuilder csb = new MySqlConnectionStringBuilder
                {
                    Server = _configuration["Sql:Server"],
                    Database=databaseName,

                    Port = _localPort,
                    UserID = _configuration["Sql:UserName"],
                    Password = _configuration["Sql:Password"],
                };

                return new MySqlConnection(csb.ConnectionString);
            }
            catch(Exception e)
            {
                throw e;
            }
               
            
        }

        private static bool IsSshClientConnected()
        {    if (_sshClient != null )
                return _sshClient.IsConnected;
            return false;
        }

        public static (SshClient SshClient, uint Port) ConnectSsh(string sshHostName, string sshUserName, string sshPassword = null,
    string sshKeyFile = null, string sshPassPhrase = null, int sshPort = 22, string databaseServer = "localhost", int databasePort = 3306)
        {
            // check arguments
            if (string.IsNullOrEmpty(sshHostName))
                throw new ArgumentException($"{nameof(sshHostName)} must be specified.", nameof(sshHostName));
            if (string.IsNullOrEmpty(sshHostName))
                throw new ArgumentException($"{nameof(sshUserName)} must be specified.", nameof(sshUserName));
            if (string.IsNullOrEmpty(sshPassword) && string.IsNullOrEmpty(sshKeyFile))
                throw new ArgumentException($"One of {nameof(sshPassword)} and {nameof(sshKeyFile)} must be specified.");
            if (string.IsNullOrEmpty(databaseServer))
                throw new ArgumentException($"{nameof(databaseServer)} must be specified.", nameof(databaseServer));

            // define the authentication methods to use (in order)
            var authenticationMethods = new List<AuthenticationMethod>();
            if (!string.IsNullOrEmpty(sshKeyFile))
            {
                authenticationMethods.Add(new PrivateKeyAuthenticationMethod(sshUserName,
                    new PrivateKeyFile(sshKeyFile, string.IsNullOrEmpty(sshPassPhrase) ? null : sshPassPhrase)));
            }
            if (!string.IsNullOrEmpty(sshPassword))
            {
                authenticationMethods.Add(new PasswordAuthenticationMethod(sshUserName, sshPassword));
            }

            // connect to the SSH server
            var sshClient = new SshClient(new ConnectionInfo(sshHostName, sshPort, sshUserName, authenticationMethods.ToArray()));
            sshClient.Connect();

            // forward a local port to the database server and port, using the SSH server
            var forwardedPort = new ForwardedPortLocal("127.0.0.1", databaseServer, (uint)databasePort);
            sshClient.AddForwardedPort(forwardedPort);
            forwardedPort.Start();

            return (sshClient, forwardedPort.BoundPort);
        }
    }
}
