using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Linq.Expressions;
using System.Text;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Security;
using System.IdentityModel.Tokens.Jwt;
using System.IdentityModel.Tokens;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
//using System.​Security.​Principal;
using Newtonsoft.Json.Linq;
using System.Linq;
using Grpc.Core;
using Grpc.Net.Client;
using System.IO;

namespace JWT_gRPC_console
{

    class Program
    {
      //  static string seckey = "YXlzdXBvaWhrdmZzZmtvYXZtb3plaHZqeGlrcGZ1d2c=";
      public  static void Main(string[] args)
        {
           gRPc_connect();

           
           // var  stringToken = GenerateToken_alt();
           //  ValidateToken(stringToken);


        }
               


        private static string GenerateToken(string seckey, string apikey)
        {
            
            

            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(seckey));

            /*
            // Create JWT credentials
            var encryptingCredentials = new EncryptingCredentials(securityKey,
                Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256
                );
            */
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);



            // Create JWT header

            IDictionary<string, string> D  = new Dictionary<string, string>

            {
                ["Typ"] = "JWT",
                ["Alg"] = "HS256",
                ["Kid"] =  apikey
            };


            var header = new JwtHeader(credentials,D);
            

            

            /////////////////////////////////////////////////

            var payload = new JwtPayload(
                
                
                claims: new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, "Filbert"),
                new Claim(JwtRegisteredClaimNames.Jti, System.Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sid, System.Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, "tinkoff_mobile_bank_api"),
                new Claim(JwtRegisteredClaimNames.Nbf, DateTime.UtcNow.ToString()),
                new Claim(JwtRegisteredClaimNames.Aud, "tinkoff.cloud.stt"),
                new Claim(JwtRegisteredClaimNames.Exp, DateTime.UtcNow.AddDays(1).ToString()),
                 
        }
                );


            




            var secToken = new JwtSecurityToken(
                header, payload);
            //signingCredentials: credentials,


            secToken.Header.Add("kid", "bnZ6cWRvcWJncWNud2dqcGxtZ21ndXVvZXdjaWpueHk=m.rogencovfilbert");





                

            var handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(secToken);
        }






        /*
        private static string GenerateToken_alt(string seckey, string apikey)
        {

            JObject json_hdr = JObject.Parse(@"{
                ""typ"" : ""JWT"",
                ""alg"" : ""HS256"",
                ""kid"" : ""bnZ6cWRvcWJncWNud2dqcGxtZ21ndXVvZXdjaWpueHk=m.rogencovfilbert""
}");



            JObject json_payLoad = JObject.Parse(
                "{ \"iss\": \"tinkoff_mobile_bank_api\",  \"sub\": \"user12345\", \"aud\": \"tinkoff.cloud.stt\", \"exp\":\"" + 
                DateTime.UtcNow.AddDays(1).ToString() + "\"}"


                );


            var unsignedToken = Base64UrlEncoder.Encode(json_hdr.ToString()) + '.' + Base64UrlEncoder.Encode(json_payLoad.ToString());


            string signature = HMACHASH(unsignedToken, seckey);

        



            var token = Base64UrlEncoder.Encode(json_hdr.ToString()) + '.' + Base64UrlEncoder.Encode(json_payLoad.ToString()) + '.' + 
            
               Base64UrlEncoder.Encode(signature);


            return token;
        }

        */





        private static bool ValidateToken(string authToken, string seckey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters(seckey);

            SecurityToken validatedToken;
            System.Security.Principal.IPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
            return true;
        }

        private static TokenValidationParameters GetValidationParameters(string seckey)
        {
            return new TokenValidationParameters()
            {
                ValidateLifetime = false, // Because there is no expiration in the generated token
                ValidateAudience = false, // Because there is no audiance in the generated token
                ValidateIssuer = false,   // Because there is no issuer in the generated token
                ValidIssuer = "Sample",
                ValidAudience = "Sample",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(seckey)) // The same key as the one that generate the token
            };
        }

        static string HMACHASH(string str, string key)
        {
            byte[] bkey = Encoding.UTF8.GetBytes(key);
            using (var hmac = new HMACSHA256(bkey))
            {
                byte[] bstr = Encoding.UTF8.GetBytes(str);
                var bhash = hmac.ComputeHash(bstr);
                // Base 64 Encode
                //                var encodedbhash = Convert.ToBase64String(bhash);



                //                var bh = byteToHex(bhash);
                // return BitConverter.ToString(bhash).Replace("-", string.Empty).ToLower();

                return  Encoding.UTF8.GetString(bhash, 0, bhash.Length);
                // return bytesToString(bhash);


            }
        }


        public static string bytesToString(byte[] byteArray)
        {
            StringBuilder builder = new StringBuilder();
                  for (int i = 0; i < byteArray.Length; i++)
                   {
       builder.Append(byteArray[i].ToString("x2"));
                 }
            return builder.ToString(); 
        }


        public static string byteToHex(byte[] byteArray)
        {
            StringBuilder result = new StringBuilder();
            foreach (byte b in byteArray)
            {
                result.AppendFormat("{0:x2}", b);
            }
            return result.ToString();
        }




        static void gRPc_connect()
        {



            // создаем канал для обмена сообщениями с сервером
            // параметр - адрес сервера gRPC





            // создаем клиента

            //  var client = new Greeter.GreeterClient(channel);
            // var mp3 = new MP3File(@"C:\Users\IlinKS\source\repos\JWT_gRPC_console\JWT_gRPC_console\2020.10.12_21.15.41_79214430425_incoming_mixed_79098420960.mp3","testik");
            string path = @"C:\Users\IlinKS\source\repos\JWT_gRPC_console\JWT_gRPC_console\2020.10.12_21.15.41_79214430425_incoming_mixed_79098420960.mp3";
            var mp3 = TagLib.File.Create(path);

            var vkc = new VoiceKitClient("bnZ6cWRvcWJncWNud2dqcGxtZ21ndXVvZXdjaWpueHk=m.rogencovfilbert", "YXlzdXBvaWhrdmZzZmtvYXZtb3plaHZqeGlrcGZ1d2c=");
            
            // первый запрос
            StreamingRecognitionConfig streaming_config = new StreamingRecognitionConfig();
            streaming_config.Config = new RecognitionConfig();
            streaming_config.Config.Encoding = AudioEncoding.MpegAudio;
            streaming_config.Config.SampleRateHertz = (uint)mp3.Properties.AudioSampleRate ;
            streaming_config.Config.NumChannels = (uint)mp3.Properties.AudioChannels;

            mp3.Dispose();
           using FileStream fstream =  File.Open(path, FileMode.Open);

             vkc.StreamingRecognize(streaming_config, fstream).GetAwaiter().GetResult();
            

            
            // var z =  c.StreamingRecognize(srr);







            /// Далее Реализуем генератор запросов:
            /// открвыем аудио файл и шлем частями






            /*
            Channel channel = new Channel("127.0.0.1:30051", ChannelCredentials.Insecure);

            var client = new Greeter.GreeterClient(channel);
            String user = "you";

            var reply = client.SayHello(new HelloRequest { Name = user });
            Console.WriteLine("Greeting: " + reply.Message);

            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            */

        }


       

      

        public class VoiceKitClient
        {
            SpeechToText.SpeechToTextClient _clientSTT;
            string _authSTT;

             public VoiceKitClient(string apiKey, string secretKey)

            {
                _authSTT = GenerateToken(secretKey, apiKey);

                //using var channel = GrpcChannel.ForAddress("https://localhost:5001");


                GrpcChannelOptions gco = new GrpcChannelOptions();
                


                //var cred = new SslCredentials();


                gco.Credentials = new SslCredentials();


                // var channelSTT = new Channel("stt.tinkoff.ru:443", cred);


                var channelSTT =  GrpcChannel.ForAddress("https://stt.tinkoff.ru",gco);
               // var channelSTT = new Grpc.Core.Channel("stt.tinkoff.ru:443", cred);

                _clientSTT = new SpeechToText.SpeechToTextClient(channelSTT);

            }
            private Metadata GetMetadataSTT()
            {
                Metadata header = new Metadata();
                header.Add("Authorization", $"Bearer { _authSTT }");
                return header;
            }

            public async Task StreamingRecognize(StreamingRecognitionConfig config, Stream audioStream)
            {
                var streamingSTT = _clientSTT.StreamingRecognize(GetMetadataSTT());
                var requestWithConfig = new StreamingRecognizeRequest
                {
                    StreamingConfig = config
                };
                await streamingSTT.RequestStream.WriteAsync(requestWithConfig);

                Task PrintResponsesTask = Task.Run(async () =>
                {
                    while (await streamingSTT.ResponseStream.MoveNext())
                    {
                        foreach (var result in streamingSTT.ResponseStream.Current.Results)
                            foreach (var alternative in result.RecognitionResult.Alternatives)
                                System.Console.WriteLine(alternative.Transcript);
                    }
                });

                var buffer = new byte[2 * 1024];
                int bytesRead;
                while ((bytesRead = audioStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    await streamingSTT.RequestStream.WriteAsync(
                        new StreamingRecognizeRequest
                        {
                            AudioContent = Google.Protobuf
                            .ByteString.CopyFrom(buffer, 0, bytesRead),
                        });
                }

                await streamingSTT.RequestStream.CompleteAsync();
                await PrintResponsesTask;
            }



        }



    }







}
