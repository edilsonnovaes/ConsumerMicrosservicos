using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace Receive
{
    class Program
    {
        public static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //MailTrap Padrão.
                    var client = new SmtpClient("smtp.mailtrap.io", 2525)
                    {
                        Credentials = new NetworkCredential("7bc45c51a4af68", "a143562b30daa7"),
                        EnableSsl = true
                    };

                    #region Inicio Acc
                    channel.QueueDeclare(queue: "pedidosAcc",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);    
                

                    var consumerAcc = new EventingBasicConsumer(channel);

                    consumerAcc.Received += (Model, ea) => {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        
                        // sendMail(client, message, "pedidosAcc");
                        client.Send("from@example.com", "to@example.com", "Aceito", message);
                        Console.WriteLine(" [x] Sent - {0}", message);
                    };
                    channel.BasicConsume(queue: "pedidosAcc", autoAck: true, consumer: consumerAcc);

                    channel.QueueDeclare(queue: "pedidosNAcc",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);
                    #endregion

                    #region Inicio NAcc
                    var consumerNAcc = new EventingBasicConsumer(channel);

                    consumerNAcc.Received += (Model, ea) => {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        
                        // sendMail(client, message, "pedidosNAcc");
                        client.Send("from@example.com", "to@example.com", "Não Aceito", message);
                        Console.WriteLine(" [x] Sent - {0}", message);
                    };
                    channel.BasicConsume(queue: "pedidosNAcc", autoAck: true, consumer: consumerNAcc);
                    #endregion
                    

                    Console.WriteLine(" Fim da execução.");
                    Console.ReadLine();
                }
            }
        }
    }
}
