﻿using System;
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
using JWT.Builder;
using JWT.Algorithms;
using NAudio.Wave;



namespace JWT_gRPC_console
{

    class Program
    {
        //  static string seckey = "YXlzdXBvaWhrdmZzZmtvYXZtb3plaHZqeGlrcGZ1d2c=";
        static string text = string.Empty;

      public  static void Main(string[] args)
        {
            if (args.Length < 1)
                return;
            else                             gRPc_connect(args[0]);

           
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


            var header = new System.IdentityModel.Tokens.Jwt.JwtHeader(credentials,D);
            

            

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
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())

        }
                );


            




            var secToken = new JwtSecurityToken(
                header, payload);
            //signingCredentials: credentials,


            secToken.Header.Add("kid", "bnZ6cWRvcWJncWNud2dqcGxtZ21ndXVvZXdjaWpueHk=m.rogencovfilbert");





                

            var handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(secToken);
        }



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




        static void gRPc_connect(string path)
        {

            // создаем канал для обмена сообщениями с сервером
            // параметр - адрес сервера gRPC





            // создаем клиента

            //  var client = new Greeter.GreeterClient(channel);
            // var mp3 = new MP3File(@"C:\Users\IlinKS\source\repos\JWT_gRPC_console\JWT_gRPC_console\2020.10.12_21.15.41_79214430425_incoming_mixed_79098420960.mp3","testik");
            // string path = @"C:\Users\IlinKS\source\repos\JWT_gRPC_console\JWT_gRPC_console\2020.10.12_21.15.41_79214430425_incoming_mixed_79098420960.mp3";
            // string path = @"c:\Users\ilinks\source\repos\JWT_gRPC_console\JWT_gRPC_console\6650322009_89136872782_2020-01-24_08_24_12.wav";

            var audio = TagLib.File.Create(path);

            var vkc = new VoiceKitClient("bnZ6cWRvcWJncWNud2dqcGxtZ21ndXVvZXdjaWpueHk=m.rogencovfilbert", "YXlzdXBvaWhrdmZzZmtvYXZtb3plaHZqeGlrcGZ1d2c=");
            
            // первый запрос
            StreamingRecognitionConfig streaming_config = new StreamingRecognitionConfig();
            streaming_config.Config = new RecognitionConfig();
            
            streaming_config.Config.SampleRateHertz = (uint)audio.Properties.AudioSampleRate ;
            streaming_config.Config.NumChannels = (uint)audio.Properties.AudioChannels;

            audio.Dispose();

//           using FileStream fstream =  File.Open(path, FileMode.Open);

            switch (Path.GetExtension(path))
            {
                case ".mp3":
                    streaming_config.Config.Encoding = AudioEncoding.MpegAudio;
                    vkc.StreamingRecognize(streaming_config, File.Open(path, FileMode.Open)).GetAwaiter().GetResult();
                    break;
                case ".wav":
                    streaming_config.Config.Encoding = AudioEncoding.Linear16;
                    vkc.StreamingRecognizeWAV(streaming_config, WavToPcmConvert(path)).GetAwaiter().GetResult();
                    break;

            }



            return ;

            }


       

      

        public class VoiceKitClient
        {
            SpeechToText.SpeechToTextClient _clientSTT;
            Auth _authSTT;

             public VoiceKitClient(string apiKey, string secretKey)

            {
                // _authSTT = GenerateToken(secretKey, apiKey);
                _authSTT = new Auth(apiKey, secretKey, "tinkoff.cloud.stt");



                GrpcChannelOptions gco = new GrpcChannelOptions();
                


                //var cred = new SslCredentials();


                gco.Credentials = new SslCredentials();


                // var channelSTT = new Channel("stt.tinkoff.ru:443", cred);


                var channelSTT =  GrpcChannel.ForAddress("https://stt.tinkoff.ru:443",gco);
               // var channelSTT = new Grpc.Core.Channel("stt.tinkoff.ru:443", cred);

                _clientSTT = new SpeechToText.SpeechToTextClient(channelSTT);

            }
            private Metadata GetMetadataSTT()
            {
                Metadata header = new Metadata();
                header.Add("Authorization", $"Bearer { _authSTT.Token }");
                return header;
            }

            public async Task StreamingRecognize(StreamingRecognitionConfig config, Stream audioStream)
            {
                var streamingSTT = _clientSTT.StreamingRecognize(GetMetadataSTT());
                var requestWithConfig = new StreamingRecognizeRequest
                {
                    StreamingConfig = config
                };
                await streamingSTT.RequestStream.WriteAsync(requestWithConfig); //передаем параметры аудио

                Task PrintResponsesTask = Task.Run(async () =>                 // Пишем ответ в консоль
                {
                    while (await streamingSTT.ResponseStream.MoveNext())
                    {
                        foreach (var result in streamingSTT.ResponseStream.Current.Results)
                            foreach (var alternative in result.RecognitionResult.Alternatives)
                            {
                                System.Console.WriteLine(alternative.Transcript);
                                text += alternative.Transcript;
                            }
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
            public async Task StreamingRecognizeWAV(StreamingRecognitionConfig config, Stream audioStream)
            {
                var streamingSTT = _clientSTT.StreamingRecognize(GetMetadataSTT());
                var requestWithConfig = new StreamingRecognizeRequest
                {
                    StreamingConfig = config
                };
                await streamingSTT.RequestStream.WriteAsync(requestWithConfig); //передаем параметры аудио

                Task PrintResponsesTask = Task.Run(async () =>                 // Пишем ответ в консоль
                {
                    while (await streamingSTT.ResponseStream.MoveNext())
                    {
                        foreach (var result in streamingSTT.ResponseStream.Current.Results)
                            foreach (var alternative in result.RecognitionResult.Alternatives)
                            {
                                System.Console.WriteLine(alternative.Transcript);
                                text += alternative.Transcript;
                            }
                    }
                });

                var buffer = new byte[config.Config.SampleRateHertz/10];
                int bytesRead;
             //   audioStream.Position = 44;
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


        public class Auth
        {
            string _apiKey;
            string _secretKey;
            string _endpoint;
            DateTimeOffset _expTime;
            string _jwt;

            public string Token
            {
                get
                {
                    if (_expTime == null || _expTime < DateTimeOffset.UtcNow)
                    {
                        CreateJWT();
                    }
                    return _jwt;
                }
            }

            public Auth(string apiKey, string secretKey, string endpoint)
            {
                _apiKey = apiKey;
                _secretKey = secretKey;
                _endpoint = endpoint;

                CreateJWT();
            }

            private void CreateJWT()
            {
                _expTime = DateTimeOffset.UtcNow.AddMinutes(5);

                _jwt = new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(Convert.FromBase64String(_secretKey))
                .AddClaim("aud", _endpoint)
                .AddClaim("exp", _expTime.ToUnixTimeSeconds())
                .AddHeader(HeaderName.KeyId, _apiKey)
                .Encode();
            }
            
        }
        public static Stream WavToPcmConvert(string filePath)
        {
            //   string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            MemoryStream mMemoryStream = new MemoryStream();
           // Stream stream = null;

            using (var reader = new WaveFileReader(filePath))
            {
                using (var converter = WaveFormatConversionStream.CreatePcmStream(reader))
                {
                    //  WaveFileWriter.CreateWaveFile(@"C:\TEMP\wav\out.wav", converter);
                    WaveFileWriter.WriteWavFileToStream(mMemoryStream, converter);
                    return mMemoryStream;
                }
            }
        }
    }







}
