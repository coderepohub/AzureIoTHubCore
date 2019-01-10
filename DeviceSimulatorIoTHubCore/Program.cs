using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;


namespace DeviceSimulatorIoTHubCore
{
    class Program
    {
        //Device ConnectionString (Device which has been created in Azure IoT)
        private readonly static string _deviceConnectionString = "HostName=iotcorehub.azure-devices.net;DeviceId=FirstDevice;SharedAccessKey=/JbdCr04Y0XZOdlr/WZ3DlZSugsTzyGonqrlxKrCHOE=";
        private static DeviceClient _deviceClient;
        static void Main(string[] args)
        {
            Console.WriteLine( "Applicatiopn to send Deevice2Cloud messages"  );
            _deviceClient =  DeviceClient.CreateFromConnectionString(_deviceConnectionString,TransportType.Mqtt);
            SendDevice2CloudMessageAsync();
            Console.ReadLine();
        }

        //Asynchronous Method to send random data from the Device To Cloud 
        private static async void SendDevice2CloudMessageAsync()
        {
            int data1 = 10;
            int data2 = 20;

            Random random = new Random();
            while(true)
            {
                int updatedData1 = data1 + random.Next(1,50);
                int updatedData2 = data2 + random.Next(15,70);

                //Create a JSON structure to send over IoT
                var datatoSend = new 
                {
                    firstValue = updatedData1,
                    secondValue = updatedData2
                };

                var messageJsonString = JsonConvert.SerializeObject(datatoSend);
                var message = new Message(Encoding.ASCII.GetBytes(messageJsonString));

                //Add some extra properties
                //NOTE : This could be used as filter properties , where IoT will filter the message
                //based on this property without looking inside the message body
                message.Properties.Add("isDataPointExceeded",updatedData1>33?"true":"false");

                //Finally send the message to cloud (IoT)
                await _deviceClient.SendEventAsync(message);
                Console.WriteLine($"{DateTime.Now} > Sending Message {messageJsonString}");

                //Wait for 2 second before sending another message
                await Task.Delay(2000);
            }
        }
    }
}
