using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ.Management.Client;
using EasyNetQ.Management.Client.Model;
namespace Esmart.Framework.RabbitMq
{
  public  class MQMonitor
    {
      public void ManagerClientOpr()
      {
          #region selectall
          var initial = new ManagementClient("http://localhost", "guest", "guest");

          // first create a new virtual host
          var vhost = initial.CreateVirtualHost("my_virtual_host");

          // next create a user for that virutal host
          var user = initial.CreateUser(new UserInfo("mike", "topSecret"));

          // give the new user all permissions on the virtual host
          initial.CreatePermission(new PermissionInfo(user, vhost));

          // now log in again as the new user
          var management = new ManagementClient("http://localhost", user.Name, "topSecret");

          // test that everything's OK
          management.IsAlive(vhost);

          // create an exchange
          var exchange = management.CreateExchange(new ExchangeInfo("my_exchagne", "direct"), vhost);

          // create a queue
          var queue = management.CreateQueue(new QueueInfo("my_queue"), vhost);

          // bind the exchange to the queue
          management.CreateBinding(exchange, queue, new BindingInfo("my_routing_key"));

          // publish a test message
          management.Publish(exchange, new PublishInfo("my_routing_key", "Hello World!"));

          // get any messages on the queue
          var messages = management.GetMessagesFromQueue(queue, new GetMessagesCriteria(1, false));

          foreach (var message in messages)
          {
              Console.Out.WriteLine("message.payload = {0}", message.Payload);
          }
          #endregion

          #region Connecting
          // Create a new instance of EasyNetQ.Management.Client.ManagementClient:
          var client = new ManagementClient("http://localhost", "my_user_name", "my_password");

          //This doesn't actually connect to RabbitMQ, an HTTP request (or two) is made for each method that you call on the managementClient instance.
          //If you want to connect with a port number other than the default (15672), use the optional portNumber parameter:
          var client1 = new ManagementClient("url", "username", "password", portNumber: 8080);

          //If you are using Mono, you must set the optional runningOnMono parameter to true:
          var client2 = new ManagementClient("url", "username", "password", runningOnMono: true);

          //If you need to do extra configuration of the HttpWebRequest (to configure a proxy for example) use the configureRequest action optional parameter:
          var client3 = new ManagementClient("url", "username", "password", configureRequest: request =>
                  request.Headers.Add("x-not-used", "some_value"));
          #endregion
          #region Overview
          var overview = initial.GetOverview();

          Console.Out.WriteLine("overview.management_version = {0}", overview.ManagementVersion);
          foreach (var exchangeType in overview.ExchangeTypes)
          {
              Console.Out.WriteLine("exchangeType.name = {0}", exchangeType.Name);
          }
          foreach (var listener in overview.Listeners)
          {
              Console.Out.WriteLine("listener.ip_address = {0}", listener.IpAddress);
          }

          Console.Out.WriteLine("overview.queue_totals = {0}", overview.QueueTotals.Messages);

          foreach (var context in overview.Contexts)
          {
              Console.Out.WriteLine("context.description = {0}", context.Description);
          }
          #endregion
          #region Virtual Hosts
          var vhosts = initial.GetVHosts();

          foreach (var vhost11 in vhosts)
          {
              Console.Out.WriteLine("vhost.name = {0}", vhost11.Name);
          }
          #endregion
          #region Users
          var users = initial.GetUsers();

          foreach (var user1 in users)
          {
              Console.Out.WriteLine("user.name = {0}", user1.Name);
          }
          #endregion
          #region Permissions
          var permissions = initial.GetPermissions();

          foreach (var permission in permissions)
          {
              Console.Out.WriteLine("permission.user = {0}", permission.User);
              Console.Out.WriteLine("permission.vhost = {0}", permission.Vhost);
              Console.Out.WriteLine("permission.configure = {0}", permission.Configure);
              Console.Out.WriteLine("permission.read = {0}", permission.Read);
              Console.Out.WriteLine("permission.write = {0}", permission.Read);
          }
          #endregion
          #region Connections
          var connections = initial.GetConnections();

          foreach (var connection in connections)
          {
              Console.Out.WriteLine("connection.name = {0}", connection.Name);
              Console.WriteLine("user:\t{0}", connection.ClientProperties.User);
              Console.WriteLine("application:\t{0}", connection.ClientProperties.Application);
              Console.WriteLine("client_api:\t{0}", connection.ClientProperties.ClientApi);
              Console.WriteLine("application_location:\t{0}", connection.ClientProperties.ApplicationLocation);
              Console.WriteLine("connected:\t{0}", connection.ClientProperties.Connected);
              Console.WriteLine("easynetq_version:\t{0}", connection.ClientProperties.EasynetqVersion);
              Console.WriteLine("machine_name:\t{0}", connection.ClientProperties.MachineName);
          }
          #endregion
          #region Channels
          var channels = initial.GetChannels();

          foreach (var channel in channels)
          {
              Console.Out.WriteLine("channel.name = {0}", channel.Name);
              Console.Out.WriteLine("channel.user = {0}", channel.User);
              Console.Out.WriteLine("channel.prefetch_count = {0}", channel.PrefetchCount);
          }
          #endregion
          #region Exchanges
          var exchanges = initial.GetExchanges();

          foreach (Exchange exchange1 in exchanges)
          {
              Console.Out.WriteLine("exchange.name = {0}", exchange1.Name);
          }
          #endregion
          #region Queues
          var queues = initial.GetQueues();

          foreach (Queue queue11 in queues)
          {
              Console.Out.WriteLine("queue.name = {0}", queue11.Name);
          }
          var vhost12 = initial.GetVhost("/");
          var queueInfo = new QueueInfo("testQueue");
          var queue12 = initial.CreateQueue(queueInfo, vhost);
          initial.DeleteQueue(queue);
          var bindings = initial.GetBindingsForQueue(queue);
          initial.Purge(queue);
          #endregion
          #region Bindings
          var bindings1 = initial.GetBindings();

          foreach (var binding in bindings1)
          {
              Console.Out.WriteLine("binding.destination = {0}", binding.Destination);
              Console.Out.WriteLine("binding.source = {0}", binding.Source);
              Console.Out.WriteLine("binding.properties_key = {0}", binding.PropertiesKey);
          }
          var bindings12 = initial.GetBindings(exchange, queue);



          var vhost13 = initial.GetVhost("/");
          var queue13 = initial.GetQueue("my_queue", vhost);
          var exchange13 = initial.GetExchange("my_exchange", vhost);

          var bindingInfo = new BindingInfo("my.routing.key");

          initial.CreateBinding(exchange, queue, bindingInfo);

          var vhost14 = initial.GetVhost("/");
          var queue14 = initial.GetQueue("testQueue", vhost);
          var exchange14 = initial.GetExchange("testExchange", vhost);

          var bindings14 = initial.GetBindings(exchange, queue);

          foreach (var binding in bindings)
          {
              initial.DeleteBinding(binding);
          }
          #endregion
      }

    }
}
