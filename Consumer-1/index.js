#!/usr/bin/env node
var amqp = require('amqplib/callback_api');
amqp.connect('amqp://localhost', function(error0, connection) {
  if (error0) {
    throw error0;
  }
  connection.createChannel(function(error1, channel) {
    if (error1) {
      throw error1;
    }
    var queue = 'pedidos';
    channel.assertQueue(queue, {
      durable: false
    });

    var queueAcc = 'pedidosAcc';
    channel.assertQueue(queueAcc, {
      durable: false
    });

    var queueNAcc = 'pedidosNAcc';
    channel.assertQueue(queueNAcc, {
      durable: false
    });

    channel.prefetch(1);
    channel.consume(queue, function(msg) {
        console.log(" [x] Received %s", msg.content.toString());

        setTimeout(() => {
            var n = Math.floor(Math.random() * (10 - 1) + 1);

            if(n % 2 == 0){
              console.log(" [x] Accepted");
              channel.ack(msg);
              channel.sendToQueue(queueAcc, Buffer.from(JSON.stringify(msg)));
            }
            else{
              console.log(" [x] Rejected");
              
              channel.ack(msg);
              channel.sendToQueue(queueNAcc, Buffer.from(JSON.stringify(msg)));
            }
        }, 2000);
    }, {
        noAck: false
    });
  });
});